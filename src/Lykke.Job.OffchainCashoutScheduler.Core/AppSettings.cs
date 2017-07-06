namespace Lykke.Job.OffchainCashoutScheduler.Core
{
    public class AppSettings
    {
        public OffchainCashoutSchedulerSettings OffchainCashoutSchedulerJob { get; set; }
        public AppNotificationSettings AppNotifications { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }

        public class OffchainCashoutSchedulerSettings
        {
            public DbSettings Db { get; set; }

            public string BitcoinApiUrl { get; set; }
        }

        public class DbSettings
        {
            public string LogsConnString { get; set; }
            public string ClientPersonalInfoConnString { get; set; }
            public string OffchainConnString { get; set; }
        }

        public class SlackNotificationsSettings
        {
            public AzureQueueSettings AzureQueue { get; set; }

            public int ThrottlingLimitSeconds { get; set; }
        }

        public class AzureQueueSettings
        {
            public string ConnectionString { get; set; }

            public string QueueName { get; set; }
        }

        public class AppNotificationSettings
        {
            public string HubConnString { get; set; }

            public string HubName { get; set; }
        }
    }
}