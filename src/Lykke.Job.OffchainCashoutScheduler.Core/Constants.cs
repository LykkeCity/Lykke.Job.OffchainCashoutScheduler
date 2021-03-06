﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Job.OffchainCashoutScheduler.Core
{
    public class Constants
    {
        public const string BtcAssetId = "BTC";
        public const string LkkAssetId = "LKK";

        public const decimal BtcDefaultCashout = 10.0M;
        public const decimal LkkDefaultCashout = 100000M;

        public const string HubCashoutSettingsKey = "AutoHubCashout";
        public const string ManualHubCashoutSettingsKey = "ManualHubCashout";
        public const string HoursBeforeCommitmentBroadcastingSettingsKey = "HoursBeforeBroadcasting";
        public const string MaxCommitmentBroadcastCountSettingsKey = "MaxCommitmentBroadcastCount";
    }
}
