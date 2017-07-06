using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Job.OffchainCashoutScheduler.Core;
using Lykke.Job.OffchainCashoutScheduler.Core.Domain.Settings;

namespace Lykke.Job.OffchainCashoutScheduler.AzureRepositories.Settings
{
    public class SettingsEntity : BaseEntity
    {
        public static string GeneratePartitionKey()
        {
            return "Setting";
        }

        public string Asset => RowKey;

        public decimal Value { get; set; }

        public SettingsEntity(string key, decimal value)
        {
            PartitionKey = GeneratePartitionKey();
            RowKey = key;
            Value = value;
        }

        public SettingsEntity()
        {

        }
    }

    public class HubCashoutHubCashoutSettingsRepository : IHubCashoutSettingsRepository
    {
        private readonly INoSQLTableStorage<SettingsEntity> _table;

        private readonly Dictionary<string, decimal> _defaults = new Dictionary<string, decimal>
        {
            { Constants.BtcAssetId, Constants.BtcMinCashout },
            { Constants.LkkAssetId, Constants.LkkMinCashout }
        };

        public HubCashoutHubCashoutSettingsRepository(INoSQLTableStorage<SettingsEntity> table)
        {
            _table = table;
        }

        public async Task<decimal> Get(string asset)
        {
            var data = await _table.GetDataAsync(SettingsEntity.GeneratePartitionKey(), asset);

            return data?.Value ?? _defaults[asset];
        }

        public Task Set(string key, decimal value)
        {
            return _table.InsertOrReplaceAsync(new SettingsEntity(key, value));
        }

        public async Task<Dictionary<string, decimal>> GetAll()
        {
            var data = await _table.GetDataAsync(SettingsEntity.GeneratePartitionKey());

            return data.ToDictionary(x => x.Asset, x => x.Value);
        }

        public async Task SetAll(Dictionary<string, decimal> settings)
        {
            foreach (var setting in settings)
            {
                await Set(setting.Key, setting.Value);
            }
        }
    }
}
