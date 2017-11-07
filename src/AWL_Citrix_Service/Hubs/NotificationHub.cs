//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.SignalR.Hubs;
//using AWL.Citrix.Service.Models;

//namespace AWL.Citrix.Service.Hubs
//{
//    [HubName("notificationHub")]
//    public class NotificationHub : Hub
//    {
//        private readonly NotificationTicker notificationTicker;

//        public NotificationHub(NotificationTicker notificationTicker)
//        {
//            this.notificationTicker = notificationTicker;
//        }

//        public IEnumerable<Notification> GetAllNotifications()
//        {
//            return notificationTicker.GetAllNotifications();
//        }
//    }
//}
