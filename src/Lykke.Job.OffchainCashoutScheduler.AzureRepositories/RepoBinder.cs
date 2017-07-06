using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
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
            ioc.RegisterInstance(CreateTradersRepository(settings.Db.ClientPersonalInfoConnString, log))
                .As<IClientAccountsRepository>();

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

            ioc.RegisterInstance<IHubCashoutSettingsRepository>(
                new HubCashoutHubCashoutSettingsRepository(
                    new AzureTableStorage<SettingsEntity>(settings.Db.OffchainConnString, "OffchainHubCashoutSettings", log)));
        }

        private static ClientsRepository CreateTradersRepository(string connstring, ILog log)
        {
            const string tableName = "Traders";
            const string tableNameForRelations = "ClientPartnerRelations";
            return new ClientsRepository(
                new AzureTableStorage<ClientAccountEntity>(connstring, tableName, log),
                new AzureTableStorage<ClientPartnerRelationEntity>(connstring, tableNameForRelations, log),
                new AzureTableStorage<AzureIndex>(connstring, tableName, log));
        }
    }
}
