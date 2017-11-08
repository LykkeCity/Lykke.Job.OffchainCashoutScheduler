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

    public partial class ApiException
    {
        /// <summary>
        /// Initializes a new instance of the ApiException class.
        /// </summary>
        public ApiException()
        {
          CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ApiException class.
        /// </summary>
        public ApiException(ApiError error = default(ApiError))
        {
            Error = error;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public ApiError Error { get; set; }

    }
}
