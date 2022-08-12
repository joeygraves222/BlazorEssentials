using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorEssentials.Models
{
    public class PushNotificationRecipient
    {
        public int Id { get; set; }
        public string Endpoint { get; set; }
        public string p256dh { get; set; }
        public string auth { get; set; }
        public string UsersList { get; set; }
        public bool ContainsUser(int userId)
        {
            try
            {
                var ids = JsonSerializer.Deserialize<List<int>>(UsersList);

                return ids.Contains(userId);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public class PushNotificationPayload
    {
        public List<int> RecipientIds { get; set; }
        public string MessageJSON { get; set; }

    }

    public class PushNotificationSubscriptionPayload
    {
        public string endpoint { get; set; }
        public string p256dh { get; set; }
        public string auth { get; set; }
    }
}
