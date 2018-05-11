using System;
using Autofac;
using Lykke.Job.OffchainCashoutScheduler.BitcoinApi;
using Lykke.Job.OffchainCashoutScheduler.Core;
using Lykke.Job.OffchainCashoutScheduler.Core.Services;
using Lykke.Job.OffchainCashoutScheduler.Services.ClientAccountApi;

namespace Lykke.Job.OffchainCashoutScheduler.Services
{
    public static class SrvBinder
    {
        public static void BindServices(this ContainerBuilder ioc, AppSettings settings)
        {
            ioc.RegisterType<OffchainRequestService>().As<IOffchainRequestService>();

            ioc.Register<IAppNotifications>(x => new SrvAppNotifications(settings.AppNotifications.HubConnString, settings.AppNotifications.HubName)).SingleInstance();

            ioc.Register<IBitcoinApi>(x => new BitcoinApi.BitcoinApi(new Uri(settings.OffchainCashoutSchedulerJob.BitcoinApiUrl)));

            ioc.Register<IClientAccounts>(x => new ClientAccounts(settings.ClientAccountClient.ServiceUrl));
        }
    }
}
