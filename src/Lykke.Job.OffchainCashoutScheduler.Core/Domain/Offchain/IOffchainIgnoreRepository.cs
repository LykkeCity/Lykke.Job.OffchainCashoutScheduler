using System.Threading.Tasks;

namespace Lykke.Job.OffchainCashoutScheduler.Core.Domain.Offchain
{
    public interface IOffchainIgnore
    {
        string ClientId { get; }
    }

    public interface IOffchainIgnoreRepository
    {
        Task<bool> IsIgnored(string client);
    }
}
