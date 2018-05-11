using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Job.OffchainCashoutScheduler.AzureRepositories;
using Lykke.Job.OffchainCashoutScheduler.Core;
using Lykke.Job.OffchainCashoutScheduler.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.OffchainCashoutScheduler.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings _appSettings;
        private readonly ILog _log;

        public JobModule(AppSettings appSettings, ILog log)
        {
            _appSettings = appSettings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var jobSettings = _appSettings.OffchainCashoutSchedulerJob;
            builder.RegisterInstance(jobSettings)
                .SingleInstance();

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.BindAzure(jobSettings, _log);
            builder.BindServices(_appSettings);
        }
    }
}