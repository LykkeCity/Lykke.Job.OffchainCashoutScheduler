using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Job.OffchainCashoutScheduler.Core;
using Lykke.Job.OffchainCashoutScheduler.Core.Services;
using Lykke.JobTriggers.Triggers.Attributes;

namespace Lykke.Job.OffchainCashoutScheduler.TriggerHandlers
{
    public class NotificationFunction
    {
        private readonly IOffchainRequestService _offchainRequestService;
        private readonly ILog _logger;

        public NotificationFunction(IOffchainRequestService offchainRequestService, ILog logger)
        {
            _offchainRequestService = offchainRequestService;
            _logger = logger;
        }

        [TimerTrigger("06:00:00")]
        public async Task Process()
        {
            var clients = await GetClientsWithExistingRequests(Constants.BtcAssetId);

            foreach (var client in clients)
            {
                try
                {
                    await _offchainRequestService.NotifyUser(client);
                }
                catch (Exception ex)
                {
                    await _logger.WriteErrorAsync("NotificationFunction", "Process", client, ex);
                }
            }

        }

        private async Task<HashSet<string>> GetClientsWithExistingRequests(string asset)
        {
            var result = new HashSet<string>();

            var currentRequests = await _offchainRequestService.GetAllCurrentRequests();

            foreach (var request in currentRequests.Where(x => x.AssetId == asset &&
                                                               x.TransferType == Core.Domain.Offchain.OffchainTransferType.HubCashout))
            {
                result.Add(request.ClientId);
            }

            return result;
        }
    }
}
