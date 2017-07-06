using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.OffchainCashoutScheduler.Core.Domain.ClientPersonalInfo
{
    public abstract class TraderSettingsBase
    {
        public abstract string GetId();
        
        public static T CreateDefault<T>() where T : TraderSettingsBase, new()
        {
            if (typeof(T) == typeof(PushNotificationsSettings))
                return PushNotificationsSettings.CreateDefault() as T;

            return new T();
        }
    }

    public class PushNotificationsSettings : TraderSettingsBase
    {
        public override string GetId()
        {
            return "PushNotificationsSettings";
        }

        public bool Enabled { get; set; }

        public static PushNotificationsSettings CreateDefault()
        {
            return new PushNotificationsSettings
            {
                Enabled = true
            };
        }
    }

    public interface IClientSettingsRepository
    {
        Task<T> GetSettings<T>(string traderId) where T : TraderSettingsBase, new();
    }
}