// Code generated by Microsoft (R) AutoRest Code Generator 1.2.2.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Lykke.Job.OffchainCashoutScheduler.BitcoinApi.Models
{
    using Lykke.Job;
    using Lykke.Job.OffchainCashoutScheduler;
    using Lykke.Job.OffchainCashoutScheduler.BitcoinApi;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class OffchainBalanceResponse
    {
        /// <summary>
        /// Initializes a new instance of the OffchainBalanceResponse class.
        /// </summary>
        public OffchainBalanceResponse()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OffchainBalanceResponse class.
        /// </summary>
        public OffchainBalanceResponse(IDictionary<string, OffchainBalanceInfo> channels = default(IDictionary<string, OffchainBalanceInfo>))
        {
            Channels = channels;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "channels")]
        public IDictionary<string, OffchainBalanceInfo> Channels { get; set; }

    }
}
