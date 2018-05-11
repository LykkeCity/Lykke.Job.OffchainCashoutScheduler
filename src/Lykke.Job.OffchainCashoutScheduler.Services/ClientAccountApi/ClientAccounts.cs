using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Common.PasswordTools;
using Lykke.Job.OffchainCashoutScheduler.Core.Services;
using Newtonsoft.Json;

namespace Lykke.Job.OffchainCashoutScheduler.Services.ClientAccountApi
{
    public class ClientAccount : IClientAccount
    {
        [JsonProperty("NotificationsId")]
        public string NotificationsId { get; set; }

    }


    public class ClientAccounts : IClientAccounts
    {
        private static HttpClient httpClient = new HttpClient();

        private readonly string _connectionString;

        public ClientAccounts(string connectionString)
        {
            _connectionString = connectionString;

            httpClient.BaseAddress = new Uri($"{_connectionString}/api/ClientAccountInformation/getClientById");
            httpClient.DefaultRequestHeaders.Accept.Clear();
        }

       
        public async Task<IClientAccount> GetByIdAsync(string id)
        {
            IClientAccount client = null;

            HttpResponseMessage response = await httpClient.GetAsync("?id=" + id);
            if (response.IsSuccessStatusCode)
            {
                client = await response.Content.ReadAsAsync<ClientAccount>();
            }

            return client;
        }

        
    }
}
