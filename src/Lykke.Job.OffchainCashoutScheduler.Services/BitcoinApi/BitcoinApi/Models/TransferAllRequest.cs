// Code generated by Microsoft (R) AutoRest Code Generator 1.1.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Lykke.Job.OffchainCashoutScheduler.BitcoinApi.Models
{
    using Lykke.Job;
    using Lykke.Job.OffchainCashoutScheduler;
    using Lykke.Job.OffchainCashoutScheduler.BitcoinApi;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class TransferAllRequest
    {
        /// <summary>
        /// Initializes a new instance of the TransferAllRequest class.
        /// </summary>
        public TransferAllRequest()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the TransferAllRequest class.
        /// </summary>
        public TransferAllRequest(System.Guid? transactionId = default(System.Guid?), string sourceAddress = default(string), string destinationAddress = default(string))
        {
            TransactionId = transactionId;
            SourceAddress = sourceAddress;
            DestinationAddress = destinationAddress;
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
        [JsonProperty(PropertyName = "sourceAddress")]
        public string SourceAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "destinationAddress")]
        public string DestinationAddress { get; set; }

    }
}
