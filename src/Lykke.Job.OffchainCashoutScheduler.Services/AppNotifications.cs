using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Lykke.Job.OffchainCashoutScheduler.Core.Services;
using Newtonsoft.Json;

namespace Lykke.Job.OffchainCashoutScheduler.Services
{
    public enum Device
    {
        Android, Ios
    }

    public interface IIosNotification { }

    public interface IAndroidNotification { }

    public class IosFields
    {
        [JsonProperty("alert")]
        public string Alert { get; set; }
        [JsonProperty("type")]
        public NotificationType Type { get; set; }
    }

    public class AndroidPayloadFields
    {
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("entity")]
        public string Entity { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class AssetsCreditedFieldsIos : IosFields
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("assetId")]
        public string AssetId { get; set; }
    }

    public class AssetsCreditedFieldsAndroid : AndroidPayloadFields
    {
        public class BalanceItemModel
        {
            [JsonProperty("amount")]
            public double Amount { get; set; }
            [JsonProperty("assetId")]
            public string AssetId { get; set; }
        }

        [JsonProperty("balanceItem")]
        public BalanceItemModel BalanceItem { get; set; }
    }

    public class PushTxDialogFieldsIos : IosFields
    {
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("assetId")]
        public string AssetId { get; set; }
        [JsonProperty("addressFrom")]
        public string AddressFrom { get; set; }
        [JsonProperty("addressTo")]
        public string AddressTo { get; set; }
    }

    public class PushTxDialogFieldsAndroid : AndroidPayloadFields
    {
        public class PushDialogTxItemModel
        {
            [JsonProperty("amount")]
            public double Amount { get; set; }
            [JsonProperty("assetId")]
            public string AssetId { get; set; }
            [JsonProperty("addressFrom")]
            public string AddressFrom { get; set; }
            [JsonProperty("addressTo")]
            public string AddressTo { get; set; }
        }

        [JsonProperty("pushTxItem")]
        public PushDialogTxItemModel PushTxItem { get; set; }
    }

    public class IosNotification : IIosNotification
    {
        [JsonProperty("aps")]
        public IosFields Aps { get; set; }
    }

    public class AndoridPayloadNotification : IAndroidNotification
    {
        [JsonProperty("data")]
        public AndroidPayloadFields Data { get; set; }
    }

    public class DataNotificationFields : IosFields
    {
        [JsonProperty("content-available")]
        public int ContentAvailable { get; set; } = 1;
    }

    public class SrvAppNotifications : IAppNotifications
    {
        private readonly string _connectionString;
        private readonly string _hubName;

        public SrvAppNotifications(string connectionString, string hubName)
        {
            _connectionString = connectionString;
            _hubName = hubName;
        }

        public async Task SendDataNotificationToAllDevicesAsync(string[] notificationIds, NotificationType type, string entity, string id = "")
        {
            var apnsMessage = new IosNotification
            {
                Aps = new DataNotificationFields
                {
                    Type = type
                }
            };

            var gcmMessage = new AndoridPayloadNotification
            {
                Data = new AndroidPayloadFields
                {
                    Entity = EventsAndEntities.GetEntity(type),
                    Event = EventsAndEntities.GetEvent(type),
                    Id = id
                }
            };

            await SendIosNotificationAsync(notificationIds, apnsMessage);
            await SendAndroidNotificationAsync(notificationIds, gcmMessage);
        }

        public async Task SendTextNotificationAsync(string[] notificationIds, NotificationType type, string message)
        {
            var apnsMessage = new IosNotification
            {
                Aps = new IosFields
                {
                    Alert = message,
                    Type = type
                }
            };

            var gcmMessage = new AndoridPayloadNotification
            {
                Data = new AndroidPayloadFields
                {
                    Entity = EventsAndEntities.GetEntity(type),
                    Event = EventsAndEntities.GetEvent(type),
                    Message = message,
                }
            };

            await SendIosNotificationAsync(notificationIds, apnsMessage);
            await SendAndroidNotificationAsync(notificationIds, gcmMessage);
        }

        public async Task SendAssetsCreditedNotification(string[] notificationsIds, double amount, string assetId, string message)
        {
            var apnsMessage = new IosNotification
            {
                Aps = new AssetsCreditedFieldsIos
                {
                    Alert = message,
                    Amount = amount,
                    AssetId = assetId,
                    Type = NotificationType.AssetsCredited
                }
            };

            var gcmMessage = new AndoridPayloadNotification
            {
                Data = new AssetsCreditedFieldsAndroid
                {
                    Entity = EventsAndEntities.GetEntity(NotificationType.AssetsCredited),
                    Event = EventsAndEntities.GetEvent(NotificationType.AssetsCredited),
                    BalanceItem = new AssetsCreditedFieldsAndroid.BalanceItemModel
                    {
                        AssetId = assetId,
                        Amount = amount,
                    },
                    Message = message,
                }
            };

            await SendIosNotificationAsync(notificationsIds, apnsMessage);
            await SendAndroidNotificationAsync(notificationsIds, gcmMessage);
        }

        public async Task SendRawIosNotification(string notificationId, string payload)
        {
            await SendRawNotificationAsync(Device.Ios, new[] { notificationId }, payload);
        }

        public async Task SendRawAndroidNotification(string notificationId, string payload)
        {
            await SendRawNotificationAsync(Device.Android, new[] { notificationId }, payload);
        }

        private async Task SendIosNotificationAsync(string[] notificationIds, IIosNotification notification)
        {
            await SendRawNotificationAsync(Device.Ios, notificationIds, notification.ToJson(ignoreNulls: true));
        }

        private async Task SendAndroidNotificationAsync(string[] notificationIds, IAndroidNotification notification)
        {
            await SendRawNotificationAsync(Device.Android, notificationIds, notification.ToJson(ignoreNulls: true));
        }

        private async Task SendRawNotificationAsync(Device device, string[] notificationIds, string payload)
        {
            try
            {
                notificationIds = notificationIds?.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                if (notificationIds != null && notificationIds.Any())
                {
                    var hub = CustomNotificationHubClient.CreateClientFromConnectionString(_connectionString, _hubName);

                    if (device == Device.Ios)
                        await hub.SendAppleNativeNotificationAsync(payload, notificationIds);
                    else
                        await hub.SendGcmNativeNotificationAsync(payload, notificationIds);
                }
            }
            catch (Exception)
            {
                //TODO: process exception
            }
        }

        public async Task SendPushTxDialogAsync(string[] notificationsIds, double amount, string assetId, string addressFrom,
            string addressTo, string message)
        {
            var apnsMessage = new IosNotification
            {
                Aps = new PushTxDialogFieldsIos
                {
                    Alert = message,
                    Amount = amount,
                    AssetId = assetId,
                    Type = NotificationType.PushTxDialog,
                    AddressFrom = addressFrom,
                    AddressTo = addressTo
                }
            };

            var gcmMessage = new AndoridPayloadNotification
            {
                Data = new PushTxDialogFieldsAndroid
                {
                    Entity = EventsAndEntities.GetEntity(NotificationType.PushTxDialog),
                    Event = EventsAndEntities.GetEvent(NotificationType.PushTxDialog),
                    PushTxItem = new PushTxDialogFieldsAndroid.PushDialogTxItemModel
                    {
                        Amount = amount,
                        AssetId = assetId,
                        AddressFrom = addressFrom,
                        AddressTo = addressTo
                    },
                    Message = message,
                }
            };

            await SendIosNotificationAsync(notificationsIds, apnsMessage);
            await SendAndroidNotificationAsync(notificationsIds, gcmMessage);
        }
    }

    public class CustomNotificationHubClient
    {
        private readonly string _sharedAccessKey;
        private readonly string _sharedAccessKeyName;
        private readonly string _url;

        public CustomNotificationHubClient(string sharedAccessKey, string sharedAccessKeyName, string baseUrl, string hubName)
        {
            _sharedAccessKey = sharedAccessKey;
            _sharedAccessKeyName = sharedAccessKeyName;
            _url = string.Format("https://{0}/{1}/messages/?api-version=2015-08", baseUrl, hubName);
        }

        public static CustomNotificationHubClient CreateClientFromConnectionString(string connectionString,
            string hubName)
        {
            var regexp = new Regex(@"sb://(?<url>[A-z\.\-]*)/;SharedAccessKeyName=(?<keyName>[A-z0-9]*);.*SharedAccessKey=(?<key>[A-z0-9+=/]*)");
            var match = regexp.Match(connectionString);
            var baseUrl = match.Groups["url"].Value;
            var accessKey = match.Groups["key"].Value;
            var accessKeyName = match.Groups["keyName"].Value;

            return new CustomNotificationHubClient(accessKey, accessKeyName, baseUrl, hubName);
        }

        public async Task SendGcmNativeNotificationAsync(string jsonPayload, string[] ids)
        {
            var headers = new Dictionary<string, string>
            {
                {"ServiceBusNotification-Format", "gcm"},
                {"ServiceBusNotification-Tags", string.Join("||", ids)}
            };

            await SendNotification(jsonPayload, headers);
        }

        public async Task SendAppleNativeNotificationAsync(string jsonPayload, string[] ids)
        {
            var headers = new Dictionary<string, string>
            {
                {"ServiceBusNotification-Format", "apple"},
                {"ServiceBusNotification-Tags", string.Join("||", ids)},
                {"ServiceBusNotification-Apns-Expiry", DateTime.UtcNow.AddDays(10).ToString("s")}
            };

            await SendNotification(jsonPayload, headers);
        }

        public async Task SendNotification(string payload, Dictionary<string, string> headers)
        {
            var request = (HttpWebRequest)WebRequest.Create(_url);
            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";

            var epochTime = (long)(DateTime.UtcNow - new DateTime(1970, 01, 01)).TotalSeconds;
            var expiry = epochTime + (long)TimeSpan.FromHours(1).TotalSeconds;

            var encodedUrl = WebUtility.UrlEncode(_url);
            var stringToSign = encodedUrl + "\n" + expiry;
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_sharedAccessKey));

            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var sasToken = $"SharedAccessSignature sr={encodedUrl}&sig={WebUtility.UrlEncode(signature)}&se={expiry}&skn={_sharedAccessKeyName}";

            request.Headers[HttpRequestHeader.Authorization] = sasToken;

            foreach (var header in headers)
                request.Headers[header.Key] = header.Value;

            using (var stream = await request.GetRequestStreamAsync())
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(payload);
                streamWriter.Flush();
            }

            await request.GetResponseAsync();
        }
    }
}
