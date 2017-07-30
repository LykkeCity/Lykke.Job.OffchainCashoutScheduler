﻿using Common.Log;
using Lykke.Job.OffchainCashoutScheduler.BitcoinApi;
using Lykke.Job.OffchainCashoutScheduler.BitcoinApi.Models;
using Lykke.Job.OffchainCashoutScheduler.Core;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.ClientPersonalInfo;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.Settings;
using Lykke.Job.OffchainCashoutScheduler.Core.Services;
using Lykke.JobTriggers.Triggers.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.OffchainCashoutScheduler.TriggerHandlers
{
    public class BroadcastCommitmentFunction
    {
        private const int HoursBeforeCommitmentBroadcasting = 24;
        private const int MaxBroadcastCount = 10;

        private readonly IWalletCredentialsRepository _walletCredentialsRepository;
        private readonly IOffchainRequestService _offchainRequestService;
        private readonly IBitcoinApi _bitcoinApi;
        private readonly ILog _logger;

        public BroadcastCommitmentFunction(IOffchainRequestService offchainRequestService, ILog logger, IBitcoinApi bitcoinApi, IWalletCredentialsRepository walletCredentialsRepository)
        {
            _offchainRequestService = offchainRequestService;
            _bitcoinApi = bitcoinApi;
            _logger = logger;
            _walletCredentialsRepository = walletCredentialsRepository;
        }

        [TimerTrigger("01:00:00")]
        public async Task Process()
        {
            var multisigsWithOldRequests = await PrepareExistingRequests();

            var channels = await GetChannels(Constants.BtcAssetId);

            var ordered = channels.Balances.Where(x => x.ClientAmount == 0).OrderByDescending(x => x.HubAmount);

            var broadcastedCount = 0;

            foreach (var item in ordered)
            {
                if (broadcastedCount >= MaxBroadcastCount)
                    return;

                if (multisigsWithOldRequests.ContainsKey(item.Multisig))
                {
                    try
                    {
                        await _logger.WriteInfoAsync(nameof(BroadcastCommitmentFunction), nameof(Process), $"Multisig: {item.Multisig}", "Start commitment broadcasting");

                        await BroadcastCommitment(item.Multisig, Constants.BtcAssetId);

                        broadcastedCount++;

                        await _offchainRequestService.Complete(multisigsWithOldRequests[item.Multisig]);

                        await _logger.WriteInfoAsync(nameof(BroadcastCommitmentFunction), nameof(Process), $"Multisig: {item.Multisig}", "Finish commitment broadcasting");
                    }
                    catch (Exception e)
                    {
                        await _logger.WriteErrorAsync(nameof(BroadcastCommitmentFunction), nameof(Process), $"Multisig: {item.Multisig}", e);
                    }
                }
            }
        }

        private async Task<Dictionary<string, string>> PrepareExistingRequests()
        {
            var result = new Dictionary<string, string>();

            var currentRequests = await _offchainRequestService.GetAllCurrentRequests();

            foreach (var request in currentRequests.Where(x => x.AssetId == Constants.BtcAssetId &&
                                                            x.TransferType == Core.Domain.Offchain.OffchainTransferType.HubCashout &&
                                                            (DateTime.UtcNow - x.CreateDt).TotalHours > HoursBeforeCommitmentBroadcasting))
            {
                var client = await _walletCredentialsRepository.GetAsync(request.ClientId);
                if (!result.ContainsKey(client?.MultiSig))
                    result[client?.MultiSig] = request.RequestId;
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

        private async Task<AssetBalanceInfoResponse> BroadcastCommitment(string multisig, string asset)
        {
            var response = await _bitcoinApi.ApiOffchainBroadcastcommitmentPostAsync(new BroadcastCommitmentModel { });

            var error = response as ApiException;

            if (error != null)
                throw new Exception($"Cannot get channels for {asset}, response: {error.Error?.Message}, code: {error.Error?.Code}");

            return response as AssetBalanceInfoResponse;
        }
    }
}