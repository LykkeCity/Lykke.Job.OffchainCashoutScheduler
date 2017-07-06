using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.OffchainCashoutScheduler.Models
{
    public class SettingsModel
    {
        public IEnumerable<SettingValue> Settings { get; set; }
    }

    public class SettingValue
    {
        public string Asset { get; set; }
        public decimal Value { get; set; }
    }
}
