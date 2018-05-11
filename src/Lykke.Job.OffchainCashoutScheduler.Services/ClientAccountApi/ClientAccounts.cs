﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Common.PasswordTools;
using Lykke.Job.OffchainCashoutScheduler.Core.Services;
using Newtonsoft.Json;

namespace Lykke.Job.OffchainCashoutScheduler.Services.ClientAccountApi
{
    public class ClientAccount : IClientAccount, IPasswordKeeping
    {
        [JsonProperty("Registered")]
        public DateTime Registered { get; set; }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Phone")]
        public string Phone { get; set; }

        [JsonProperty("Pin")]
        public string Pin { get; set; }

        [JsonProperty("NotificationsId")]
        public string NotificationsId { get; set; }

        public string Salt { get; set; }
        public string Hash { get; set; }

        [JsonProperty("PartnerId")]
        public string PartnerId { get; set; }

        [JsonProperty("IsReviewAccount")]
        public bool IsReviewAccount { get; set; }
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
