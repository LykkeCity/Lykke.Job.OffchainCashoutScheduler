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

    public partial class MultipleTransferRequest
    {
        /// <summary>
        /// Initializes a new instance of the MultipleTransferRequest class.
        /// </summary>
        public MultipleTransferRequest()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the MultipleTransferRequest class.
        /// </summary>
        public MultipleTransferRequest(System.Guid? transactionId = default(System.Guid?), string asset = default(string), string destination = default(string), IList<ToOneAddress> sources = default(IList<ToOneAddress>))
        {
            TransactionId = transactionId;
            Asset = asset;
            Destination = destination;
            Sources = sources;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "transactionId")]
        public System.Guid? TransactionId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asset")]
        public string Asset { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "destination")]
        public string Destination { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "sources")]
        public IList<ToOneAddress> Sources { get; set; }

    }
}
