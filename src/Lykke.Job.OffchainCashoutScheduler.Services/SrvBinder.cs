using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Lykke.Job.OffchainCashoutScheduler.BitcoinApi;
using Lykke.Job.OffchainCashoutScheduler.Core;
using Lykke.Job.OffchainCashoutScheduler.Core.Services;

namespace Lykke.Job.OffchainCashoutScheduler.Services
{
    public static class SrvBinder
    {
        public static void BindServices(this ContainerBuilder ioc, AppSettings settings)
        {
            ioc.RegisterType<OffchainRequestService>().As<IOffchainRequestService>();

            ioc.Register<IAppNotifications>(x => new SrvAppNotifications(settings.AppNotifications.HubConnString, settings.AppNotifications.HubName)).SingleInstance();

            ioc.Register<IBitcoinApi>(x => new BitcoinApi.BitcoinApi(new Uri(settings.OffchainCashoutSchedulerJob.BitcoinApiUrl)));
        }
    }
}
