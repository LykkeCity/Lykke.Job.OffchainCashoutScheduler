﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.Offchain;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.OffchainCashoutScheduler.AzureRepositories.Offchain
{
    public class OffchainTransferEntity : BaseEntity, IOffchainTransfer
    {
        public string Id => RowKey;
        public string ClientId { get; set; }
        public string AssetId { get; set; }
        public decimal Amount { get; set; }
        public bool Completed { get; set; }
        public string OrderId { get; set; }
        public DateTime CreatedDt { get; set; }
        public string ExternalTransferId { get; set; }
        public OffchainTransferType Type { get; set; }
        public bool ChannelClosing { get; set; }
        public bool Onchain { get; set; }

        public class ByCommon
        {
            public static string GeneratePartitionKey()
            {
                return "OffchainTransfer";
            }

            public static OffchainTransferEntity Create(string id, string clientId, string assetId, decimal amount, OffchainTransferType type, string externalTransferId,
                string orderId = null, bool channelClosing = false, bool onchain = false)
            {
                return new OffchainTransferEntity
                {
                    PartitionKey = GeneratePartitionKey(),
                    RowKey = id,
                    AssetId = assetId,
                    Amount = amount,
                    ClientId = clientId,
                    OrderId = orderId,
                    CreatedDt = DateTime.UtcNow,
                    ExternalTransferId = externalTransferId,
                    Type = type,
                    ChannelClosing = channelClosing,
                    Onchain = onchain
                };
            }
        }

        public class ByClient
        {
            public static OffchainTransferEntity Create(IOffchainTransfer commonTransfer)
            {
                return new OffchainTransferEntity
                {
                    PartitionKey = commonTransfer.ClientId,
                    RowKey = commonTransfer.Id,
                    AssetId = commonTransfer.AssetId,
                    Amount = commonTransfer.Amount,
                    ClientId = commonTransfer.ClientId,
                    Type = commonTransfer.Type,
                    OrderId = commonTransfer.OrderId,
                    CreatedDt = commonTransfer.CreatedDt,
                    ChannelClosing = commonTransfer.ChannelClosing,
                    Onchain = commonTransfer.Onchain
                };
            }
        }
    }

    public class OffchainTransferRepository : IOffchainTransferRepository
    {
        private readonly INoSQLTableStorage<OffchainTransferEntity> _storage;

        public OffchainTransferRepository(INoSQLTableStorage<OffchainTransferEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IOffchainTransfer> CreateTransfer(string transactionId, string clientId, string assetId, decimal amount, OffchainTransferType type, string externalTransferId, string orderId, bool channelClosing = false)
        {
            var entity = OffchainTransferEntity.ByCommon.Create(transactionId, clientId, assetId, amount, type, externalTransferId, orderId, channelClosing);
            var byClient = OffchainTransferEntity.ByClient.Create(entity);

            await Task.WhenAll(_storage.InsertAsync(entity), _storage.InsertAsync(byClient));

            return entity;
        }

        public async Task<IOffchainTransfer> GetTransfer(string id)
        {
            return await _storage.GetDataAsync(OffchainTransferEntity.ByCommon.GeneratePartitionKey(), id);
        }

        public async Task<IEnumerable<IOffchainTransfer>> GetTransfersByOrder(string clientId, string orderId)
        {
            var query = new TableQuery<OffchainTransferEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, clientId))
                .Where(TableQuery.GenerateFilterCondition("OrderId", QueryComparisons.Equal, orderId));

            return await _storage.WhereAsync(query);
        }

        public async Task CompleteTransfer(string transferId, bool? onchain = null)
        {
            var item = await _storage.ReplaceAsync(OffchainTransferEntity.ByCommon.GeneratePartitionKey(), transferId,
                entity =>
                {
                    entity.Completed = true;
                    if (onchain != null)
                        entity.Onchain = onchain.Value;
                    return entity;
                });

            await _storage.DeleteAsync(item.ClientId, transferId);
        }

        public async Task UpdateTransfer(string transferId, string externalTransferId, bool closing = false, bool? onchain = null)
        {
            var item = await _storage.ReplaceAsync(OffchainTransferEntity.ByCommon.GeneratePartitionKey(), transferId,
                 entity =>
                 {
                     entity.ExternalTransferId = externalTransferId;
                     entity.ChannelClosing = closing;
                     if (onchain != null)
                         entity.Onchain = onchain.Value;
                     return entity;
                 });

            await _storage.ReplaceAsync(item.ClientId, transferId,
                entity =>
                {
                    entity.ExternalTransferId = externalTransferId;
                    entity.ChannelClosing = closing;
                    if (onchain != null)
                        entity.Onchain = onchain.Value;
                    return entity;
                });
        }
    }
}
