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

    public partial class CreateHubCommitmentModel
    {
        /// <summary>
        /// Initializes a new instance of the CreateHubCommitmentModel class.
        /// </summary>
        public CreateHubCommitmentModel()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CreateHubCommitmentModel class.
        /// </summary>
        public CreateHubCommitmentModel(string clientPubKey = default(string), decimal? amount = default(decimal?), string asset = default(string), string signedByClientChannel = default(string))
        {
            ClientPubKey = clientPubKey;
            Amount = amount;
            Asset = asset;
            SignedByClientChannel = signedByClientChannel;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "clientPubKey")]
        public string ClientPubKey { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "amount")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "asset")]
        public string Asset { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "signedByClientChannel")]
        public string SignedByClientChannel { get; set; }

    }
}
