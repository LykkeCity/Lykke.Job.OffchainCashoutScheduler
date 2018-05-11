using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class ManualHubCashoutFunction
    {
        private readonly ISlackNotificationsSender _slackNotificationsSender;
        private readonly IOffchainRequestService _offchainRequestService;
        private readonly IWalletCredentialsRepository _walletCredentialsRepository;
        private readonly IOffchainSettingsRepository _offchainSettingsRepository;
        private readonly IBitcoinApi _bitcoinApi;
        private readonly ILog _log;

        public ManualHubCashoutFunction(IOffchainSettingsRepository offchainSettingsRepository, ILog log, IOffchainRequestService offchainRequestService, ISlackNotificationsSender slackNotificationsSender, IWalletCredentialsRepository walletCredentialsRepository, IBitcoinApi bitcoinApi)
        {
            _offchainSettingsRepository = offchainSettingsRepository;
            _log = log;
            _offchainRequestService = offchainRequestService;
            _slackNotificationsSender = slackNotificationsSender;
            _walletCredentialsRepository = walletCredentialsRepository;
            _bitcoinApi = bitcoinApi;
        }

        [TimerTrigger("00:12:00")]
        public async Task Process()
        {
            var setting = await GetSetting();

            if (!setting.Enabled)
                return;
            
            await _log.WriteInfoAsync("ManualHubCashoutFunction", "Process", setting.ToJson(), "Start manual cashout");

            await GenerateRequestsForAsset(setting.Asset, setting.MinHubBalance, setting.LastActivity);

            await ResetSetting(setting);

            await _log.WriteInfoAsync("ManualHubCashoutFunction", "Process", null, "Finish manual cashout");
        }

        private async Task<ManualCashoutSettings> GetSetting()
        {
            var settings = await _offchainSettingsRepository.Get<string>(Constants.ManualHubCashoutSettingsKey);

            return settings.DeserializeJson(() => new ManualCashoutSettings());
        }

        private Task ResetSetting(ManualCashoutSettings setting)
        {
            setting.Enabled = false;

            return _offchainSettingsRepository.Set(Constants.ManualHubCashoutSettingsKey, setting.ToJson());
        }

        private async Task GenerateRequestsForAsset(string asset, decimal minAmount, DateTime? lastActivity = null)
        {
            var createdCount = 0;

            try
            {
                var channels = await GetChannels(asset);

                var existingRequests = await PrepareExistingRequests(asset);

                foreach (var channel in channels.Balances)
                {
                    // check balance
                    if (channel.HubAmount < minAmount || existingRequests.Contains(channel.Multisig))
                        continue;

                    //check last activity date
                    if (lastActivity != null && channel.UpdateDt > lastActivity)
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
                if (createdCount != 0)
                    await _slackNotificationsSender.SendAsync("Offchain", ":information_source:", $"New {createdCount} hub requests were created for {asset}. (min. {minAmount})");

                await _log.WriteInfoAsync(nameof(ManualHubCashoutFunction), nameof(GenerateRequestsForAsset), $"Asset: {asset}, requests created: {createdCount}", "Finished");
            }
        }

        private async Task<HashSet<string>> PrepareExistingRequests(string asset)
        {
            var result = new HashSet<string>();

            var currentRequests = await _offchainRequestService.GetAllCurrentRequests();

            foreach (var request in currentRequests.Where(x => x.AssetId == asset && x.TransferType == Core.Domain.Offchain.OffchainTransferType.HubCashout))
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
    }

    public class ManualCashoutSettings
    {
        public bool Enabled { get; set; }
        public string Asset { get; set; }
        public decimal MinHubBalance { get; set; }
        public DateTime? LastActivity { get; set; }
    }
}
