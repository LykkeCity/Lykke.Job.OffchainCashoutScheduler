using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.OffchainCashoutScheduler.BitcoinApi;
using Lykke.Job.OffchainCashoutScheduler.BitcoinApi.Models;
using Lykke.Job.OffchainCashoutScheduler.Core;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.ClientPersonalInfo;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.Settings;
using Lykke.Job.OffchainCashoutScheduler.Core.Services;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.SlackNotifications;

namespace Lykke.Job.OffchainCashoutScheduler.TriggerHandlers
{
    public class HubCashoutFunction
    {
        private readonly ISlackNotificationsSender _slackNotificationsSender;
        private readonly IOffchainRequestService _offchainRequestService;
        private readonly IWalletCredentialsRepository _walletCredentialsRepository;
        private readonly IOffchainSettingsRepository _offchainSettingsRepository;
        private readonly IBitcoinApi _bitcoinApi;
        private readonly ILog _logger;

        public HubCashoutFunction(ISlackNotificationsSender slackNotificationsSender, IOffchainRequestService offchainRequestService, IBitcoinApi bitcoinApi, IWalletCredentialsRepository walletCredentialsRepository, ILog logger, IOffchainSettingsRepository offchainSettingsRepository)
        {
            _slackNotificationsSender = slackNotificationsSender;
            _offchainRequestService = offchainRequestService;
            _bitcoinApi = bitcoinApi;
            _walletCredentialsRepository = walletCredentialsRepository;
            _logger = logger;
            _offchainSettingsRepository = offchainSettingsRepository;
        }

        [TimerTrigger("12:00:00")]
        public async Task TimeTriggeredHandler()
        {
            var settings = await GetSettings();

            await GenerateRequestsForAsset(Constants.BtcAssetId, settings[Constants.BtcAssetId]);

            await GenerateRequestsForAsset(Constants.LkkAssetId, settings[Constants.LkkAssetId]);
        }

        private async Task GenerateRequestsForAsset(string asset, decimal minAmount)
        {
            var createdCount = 0;

            await _logger.WriteInfoAsync(nameof(HubCashoutFunction), nameof(GenerateRequestsForAsset), asset, "Started");

            try
            {
                var channels = await GetChannels(asset);

                var existingRequests = await PrepareExistingRequests(asset);

                foreach (var channel in channels.Balances)
                {
                    if (channel.HubAmount < minAmount || existingRequests.Contains(channel.Multisig))
                        continue;
                    
                    var client = await _walletCredentialsRepository.GetClientIdByMultisig(channel.Multisig);

                    if (string.IsNullOrWhiteSpace(client))
                        continue;

                    await _offchainRequestService.CreateHubCashoutRequests(asset, client);

                    createdCount++;
                }
            }
            finally
            {
                if (createdCount == 0)
                    await _slackNotificationsSender.SendAsync("Offchain", ":information_source:", $"New {createdCount} hub requests were created for {asset}. (min. {minAmount})");

                await _logger.WriteInfoAsync(nameof(HubCashoutFunction), nameof(GenerateRequestsForAsset), $"Asset: {asset}, requests created: {createdCount}", "Finished");
            }
        }

        private async Task<HashSet<string>> PrepareExistingRequests(string asset)
        {
            var result = new HashSet<string>();

            var currentRequests = await _offchainRequestService.GetAllCurrentRequests();

            foreach (var request in currentRequests.Where(x => x.AssetId == asset))
            {
                var client = await _walletCredentialsRepository.GetAsync(request.ClientId);
                result.Add(client?.MultiSig);
            }

            return result;
        }

        private async Task<AssetBalanceInfoResponse> GetChannels(string asset)
        {
            var response = await _bitcoinApi.ApiOffchainAssetBalancesGetAsync(asset);

            var error = response as ApiException;

            if (error != null)
                throw new Exception($"Cannot get channels for {asset}, response: {error.Error?.Message}, code: {error.Error?.Code}");

            return response as AssetBalanceInfoResponse;
        }

        private async Task<Dictionary<string, decimal>> GetSettings()
        {
            var settings = await _offchainSettingsRepository.Get<string>(Constants.HubCashoutSettingsKey);

            var data = settings.DeserializeJson(() => new Dictionary<string, decimal>());

            if (!data.ContainsKey(Constants.BtcAssetId))
                data[Constants.BtcAssetId] = Constants.BtcDefaultCashout;

            if (!data.ContainsKey(Constants.LkkAssetId))
                data[Constants.LkkAssetId] = Constants.LkkDefaultCashout;

            return data;
        }
    }
}
