//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AWL.Citrix.Service.Models;
//using Microsoft.AspNet.SignalR;
//using Microsoft.AspNet.SignalR.Hubs;
//using System.Collections.Concurrent;
//using System.Threading;
//using AWL.Data;
//using Microsoft.AspNet.SignalR.Infrastructure;

//namespace AWL.Citrix.Service.Hubs
//{
//    public class NotificationTicker
//    {
//        public static int NoticationTickerInterval { get; set; }

//        private readonly IDALService dal;
//        private readonly ConcurrentDictionary<int, Models.Notification> notifications;
//        private readonly object updateNotificationsLock = new object();
//        private readonly Timer timer;
//        private readonly TimeSpan updateInterval;
//        private volatile bool updatingNotifications;
//        private IHubConnectionContext<dynamic> clients;

//        public NotificationTicker(IHubConnectionContext<dynamic> clients, IDALService dal)
//        {
//            this.clients = clients;
//            this.dal = dal;

//            notifications = new ConcurrentDictionary<int, Models.Notification>();
//            ReadAllNotifications().ForEach(notification => notifications.TryAdd(notification.ID, notification));
//            updateInterval = TimeSpan.FromSeconds(NoticationTickerInterval);
//            updatingNotifications = false;

//            timer = new Timer(UpdateNotifications, null, updateInterval, updateInterval);
//        }

//        private List<Models.Notification> ReadAllNotifications()
//        {
//            List<Models.Notification> result = new List<Models.Notification>();
//            using (var db = dal.CreateNewContext())
//            {
//                var notifications = db.Notifications
//                    .Where(
//                        notification =>
//                            notification.Enabled && notification.ShowInStoreFront &&
//                            (!notification.ShowUntil.HasValue || notification.ShowUntil.Value >= DateTime.Now)
//                    )
//                    .Select(
//                        notification =>
//                            new Models.Notification
//                            {
//                                ID = notification.NotificationID,
//                                Message = notification.Message,
//                                Type = notification.NotificationType,
//                                Closable = notification.Closable
//                            }
//                    );

//                result = notifications.ToList();
//            }
//            return result;
//        }

//        public IEnumerable<Models.Notification> GetAllNotifications()
//        {
//            return notifications.Values;
//        }

//        private void UpdateNotifications(object state)
//        {
//            lock (updateNotificationsLock)
//            {
//                if (!updatingNotifications)
//                {
//                    updatingNotifications = true;
//                    Models.Notification cached;
//                    var newNotifications = ReadAllNotifications();
//                    // Add and update notifications
//                    foreach (var notification in newNotifications)
//                    {
//                        if (notifications.TryGetValue(notification.ID, out cached))
//                        {
//                            if (!notification.Equals(cached) && notifications.TryUpdate(notification.ID, notification, cached))
//                            {
//                                BroadCastUpdateNotification(notification);
//                            }
//                        }
//                        else if (notifications.TryAdd(notification.ID, notification))
//                        {
//                            BroadCastAddNotification(notification);
//                        }
//                    }
//                    // Remove old notifications
//                    foreach (var notification in notifications.Values.Except(newNotifications))
//                    {
//                        if (notifications.TryRemove(notification.ID, out cached))
//                        {
//                            BroadCastRemoveNotification(notification);
//                        }
//                    }
//                    updatingNotifications = false;
//                }
//            }
//        }

//        private void BroadCastUpdateNotification(Models.Notification notification)
//        {
//            clients.All.updateNotification(notification);
//        }

//        private void BroadCastRemoveNotification(Models.Notification notification)
//        {
//            clients.All.removeNotification(notification);
//        }

//        private void BroadCastAddNotification(Models.Notification notification)
//        {
//            clients.All.addNotification(notification);
//        }
//    }
//}
