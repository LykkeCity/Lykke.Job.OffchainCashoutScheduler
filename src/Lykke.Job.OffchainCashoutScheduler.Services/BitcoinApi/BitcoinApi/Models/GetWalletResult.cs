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

    public partial class GetWalletResult
    {
        /// <summary>
        /// Initializes a new instance of the GetWalletResult class.
        /// </summary>
        public GetWalletResult()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the GetWalletResult class.
        /// </summary>
        public GetWalletResult(string multiSigAddress = default(string), string coloredMultiSigAddress = default(string))
        {
            MultiSigAddress = multiSigAddress;
            ColoredMultiSigAddress = coloredMultiSigAddress;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "multiSigAddress")]
        public string MultiSigAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "coloredMultiSigAddress")]
        public string ColoredMultiSigAddress { get; set; }

    }
}
