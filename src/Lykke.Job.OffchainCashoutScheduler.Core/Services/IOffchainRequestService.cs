using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.Offchain;

namespace Lykke.Job.OffchainCashoutScheduler.Core.Services
{
    public interface IOffchainRequestService
    {
        Task CreateOffchainRequest(string transactionId, string clientId, string assetId, decimal amount, string orderId, OffchainTransferType type);

        Task NotifyUser(string clientId);

        Task CreateOffchainRequestAndNotify(string transactionId, string clientId, string assetId, decimal amount, string orderId, OffchainTransferType type);

        Task<IEnumerable<IOffchainRequest>> GetAllCurrentRequests();

        Task Complete(string requestId);

        Task CreateHubCashoutRequests(string asset, string clientId);
    }
}
