using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.ClientPersonalInfo;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.Offchain;
using Lykke.Job.OffchainCashoutScheduler.Core.Services;
using Lykke.Service.ClientAccount.Client;

namespace Lykke.Job.OffchainCashoutScheduler.Services
{
    public class OffchainRequestService : IOffchainRequestService
    {
        private readonly IOffchainRequestRepository _offchainRequestRepository;
        private readonly IOffchainTransferRepository _offchainTransferRepository;
        private readonly IClientSettingsRepository _clientSettingsRepository;
        private readonly IClientAccountClient _clientAccountClient;
        private readonly IAppNotifications _appNotifications;

        public OffchainRequestService(
            IOffchainRequestRepository offchainRequestRepository,
            IOffchainTransferRepository offchainTransferRepository, 
            IClientSettingsRepository clientSettingsRepository,
            IClientAccountClient clientAccountClient, 
            IAppNotifications appNotifications)
        {
            _offchainRequestRepository = offchainRequestRepository;
            _offchainTransferRepository = offchainTransferRepository;
            _clientSettingsRepository = clientSettingsRepository;
            _clientAccountClient = clientAccountClient ?? throw new ArgumentNullException(nameof(clientAccountClient));
            _appNotifications = appNotifications;
        }

        public async Task CreateOffchainRequest(string transactionId, string clientId, string assetId, decimal amount, string orderId, OffchainTransferType type)
        {
            var transfer = await _offchainTransferRepository.CreateTransfer(transactionId, clientId, assetId, amount, type, null, orderId);

            await _offchainRequestRepository.CreateRequest(transfer.Id, clientId, assetId, RequestType.RequestTransfer, type);
        }

        public async Task NotifyUser(string clientId)
        {
            var pushSettings = await _clientSettingsRepository.GetSettings<PushNotificationsSettings>(clientId);
            if (pushSettings.Enabled)
            {
                var clientAcc = await _clientAccountClient.GetByIdAsync(clientId);

                await _appNotifications.SendDataNotificationToAllDevicesAsync(new[] { clientAcc.NotificationsId }, NotificationType.OffchainRequest, "Wallet");
            }
        }

        public async Task CreateOffchainRequestAndNotify(string transactionId, string clientId, string assetId, decimal amount,
            string orderId, OffchainTransferType type)
        {
            await CreateOffchainRequest(transactionId, clientId, assetId, amount, orderId, type);
            await NotifyUser(clientId);
        }

        public async Task<IEnumerable<IOffchainRequest>> GetAllCurrentRequests()
        {
            return await _offchainRequestRepository.GetCurrentRequests();
        }

        public Task Complete(string requestId)
        {
            return _offchainRequestRepository.Complete(requestId);
        }

        public async Task CreateHubCashoutRequests(string asset, string clientId)
        {
            await CreateOffchainRequest(Guid.NewGuid().ToString(), clientId, asset, 0, null, OffchainTransferType.HubCashout);

            await NotifyUser(clientId);
        }
    }
}
