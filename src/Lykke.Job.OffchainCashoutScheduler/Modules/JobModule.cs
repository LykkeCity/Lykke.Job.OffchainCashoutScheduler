using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.OffchainCashoutScheduler.AzureRepositories.ClientPersonalInfo;
using Lykke.Job.OffchainCashoutScheduler.AzureRepositories.Offchain;
using Lykke.Job.OffchainCashoutScheduler.AzureRepositories.Settings;
using Lykke.Job.OffchainCashoutScheduler.BitcoinApi;
using Lykke.Job.OffchainCashoutScheduler.Core;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.ClientPersonalInfo;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.Offchain;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.Settings;
using Lykke.Job.OffchainCashoutScheduler.Core.Services;
using Lykke.Job.OffchainCashoutScheduler.Services;
using Lykke.Job.OffchainCashoutScheduler.Services.ClientAccountApi;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lykke.Job.OffchainCashoutScheduler.Modules
{
    public class JobModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settingsManager;
        private readonly AppSettings _settings;
        private readonly ILog _log;

        public JobModule(IReloadingManager<AppSettings> settingsManager, ILog log)
        {
            _settingsManager = settingsManager;
            _settings = settingsManager.CurrentValue;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_settings.OffchainCashoutSchedulerJob)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            //builder.RegisterType<HealthService>()
            //    .As<IHealthService>()
            //    .SingleInstance()
            //    .WithParameter(TypedParameter.From(TimeSpan.FromSeconds(30)));

            // NOTE: You can implement your own poison queue notifier. See https://github.com/LykkeCity/JobTriggers/blob/master/readme.md
            // builder.Register<PoisionQueueNotifierImplementation>().As<IPoisionQueueNotifier>();

            BindAzure(builder);
            BindServices(builder);
        }

        private void BindAzure(ContainerBuilder builder)
        {
            builder.RegisterInstance<IClientSettingsRepository>(
                new ClientSettingsRepository(
                    AzureTableStorage<ClientSettingsEntity>.Create(_settingsManager.ConnectionString(i => i.OffchainCashoutSchedulerJob.Db.ClientPersonalInfoConnString), "TraderSettings", _log)));

            builder.RegisterInstance<IWalletCredentialsRepository>(
                new WalletCredentialsRepository(
                    AzureTableStorage<WalletCredentialsEntity>.Create(_settingsManager.ConnectionString(i => i.OffchainCashoutSchedulerJob.Db.ClientPersonalInfoConnString),
                        "WalletCredentials", _log)));

            builder.RegisterInstance<IOffchainRequestRepository>(
                new OffchainRequestRepository(
                    AzureTableStorage<OffchainRequestEntity>.Create(_settingsManager.ConnectionString(i => i.OffchainCashoutSchedulerJob.Db.ClientPersonalInfoConnString), "OffchainRequests", _log)));

            builder.RegisterInstance<IOffchainTransferRepository>(
                new OffchainTransferRepository(
                    AzureTableStorage<OffchainTransferEntity>.Create(_settingsManager.ConnectionString(i => i.OffchainCashoutSchedulerJob.Db.ClientPersonalInfoConnString), "OffchainTransfers", _log)));

            builder.RegisterInstance<IOffchainOrdersRepository>(
                new OffchainOrderRepository(
                    AzureTableStorage<OffchainOrder>.Create(_settingsManager.ConnectionString(i => i.OffchainCashoutSchedulerJob.Db.ClientPersonalInfoConnString), "OffchainOrders", _log)));

            builder.RegisterInstance<IOffchainIgnoreRepository>(
                new OffchainIgnoreRepository(
                    AzureTableStorage<OffchainIgnoreEntity>.Create(_settingsManager.ConnectionString(i => i.OffchainCashoutSchedulerJob.Db.ClientPersonalInfoConnString), "OffchainClientsIgnore", _log)));

            builder.RegisterInstance<IOffchainSettingsRepository>(
                new OffchainSettingsRepository(
                    AzureTableStorage<OffchainSettingEntity>.Create(_settingsManager.ConnectionString(i => i.OffchainCashoutSchedulerJob.Db.ClientPersonalInfoConnString), "OffchainSettings", _log)));
        }

        public void BindServices(ContainerBuilder builder)
        {
            builder.RegisterType<OffchainRequestService>().As<IOffchainRequestService>();

            builder.Register<IAppNotifications>(x => new SrvAppNotifications(_settings.AppNotifications.HubConnString, _settings.AppNotifications.HubName)).SingleInstance();

            builder.Register<IBitcoinApi>(x => new BitcoinApi.BitcoinApi(new Uri(_settings.OffchainCashoutSchedulerJob.BitcoinApiUrl)));

            builder.Register<IClientAccounts>(x => new ClientAccounts(_settings.ClientAccountClient.ServiceUrl));
        }

    }
}