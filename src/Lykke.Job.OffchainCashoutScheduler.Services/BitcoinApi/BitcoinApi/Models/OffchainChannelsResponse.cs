// Code generated by Microsoft (R) AutoRest Code Generator 1.1.0.0
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

    public partial class OffchainChannelsResponse
    {
        /// <summary>
        /// Initializes a new instance of the OffchainChannelsResponse class.
        /// </summary>
        public OffchainChannelsResponse()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the OffchainChannelsResponse class.
        /// </summary>
        public OffchainChannelsResponse(IList<OffchainChannelInfo> channels = default(IList<OffchainChannelInfo>))
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
        public IList<OffchainChannelInfo> Channels { get; set; }

    }
}