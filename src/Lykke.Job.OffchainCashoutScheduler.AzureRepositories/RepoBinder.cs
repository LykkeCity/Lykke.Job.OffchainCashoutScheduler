using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.OffchainCashoutScheduler.AzureRepositories.ClientPersonalInfo;
using Lykke.Job.OffchainCashoutScheduler.AzureRepositories.Offchain;
using Lykke.Job.OffchainCashoutScheduler.AzureRepositories.Settings;
using Lykke.Job.OffchainCashoutScheduler.Core;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.ClientPersonalInfo;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.Offchain;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.Settings;

namespace Lykke.Job.OffchainCashoutScheduler.AzureRepositories
{
    public static class RepoBinder
    {
        public static void BindAzure(this ContainerBuilder ioc, AppSettings.OffchainCashoutSchedulerSettings settings, ILog log)
        {
            ioc.RegisterInstance<IClientSettingsRepository>(
                new ClientSettingsRepository(
                    new AzureTableStorage<ClientSettingsEntity>(settings.Db.ClientPersonalInfoConnString,
                        "TraderSettings", log)));

            ioc.RegisterInstance<IWalletCredentialsRepository>(
                new WalletCredentialsRepository(
                    new AzureTableStorage<WalletCredentialsEntity>(settings.Db.ClientPersonalInfoConnString,
                        "WalletCredentials", log)));

            ioc.RegisterInstance<IOffchainRequestRepository>(
                new OffchainRequestRepository(
                    new AzureTableStorage<OffchainRequestEntity>(settings.Db.OffchainConnString, "OffchainRequests", log)));

            ioc.RegisterInstance<IOffchainTransferRepository>(
                new OffchainTransferRepository(
                    new AzureTableStorage<OffchainTransferEntity>(settings.Db.OffchainConnString, "OffchainTransfers", log)));

            ioc.RegisterInstance<IOffchainOrdersRepository>(
                new OffchainOrderRepository(
                    new AzureTableStorage<OffchainOrder>(settings.Db.OffchainConnString, "OffchainOrders", log)));

            ioc.RegisterInstance<IOffchainIgnoreRepository>(
                new OffchainIgnoreRepository(
                    new AzureTableStorage<OffchainIgnoreEntity>(settings.Db.OffchainConnString, "OffchainClientsIgnore", log)));

            ioc.RegisterInstance<IOffchainSettingsRepository>(
                new OffchainSettingsRepository(
                    new AzureTableStorage<OffchainSettingEntity>(settings.Db.OffchainConnString, "OffchainSettings", log)));
        }
        
    }
}
