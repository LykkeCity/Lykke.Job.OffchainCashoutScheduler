using System;
using System.Threading.Tasks;

namespace Lykke.Job.OffchainCashoutScheduler.Core.Services
{
    public interface IClientAccount
    {
        string NotificationsId { get; }
    }

    public class ClientAccount : IClientAccount
    {
        public string NotificationsId { get; set; }
    }

    public interface IClientAccounts
    {
        Task<IClientAccount> GetByIdAsync(string id);        
    }
}
