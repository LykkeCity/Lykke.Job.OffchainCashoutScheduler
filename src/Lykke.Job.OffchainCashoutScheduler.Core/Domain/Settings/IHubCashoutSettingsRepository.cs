using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.OffchainCashoutScheduler.Core.Domain.Settings
{
    public interface IHubCashoutSettingsRepository
    {
        Task<decimal> Get(string asset);
        Task Set(string key, decimal value);
    }
}
