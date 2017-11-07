using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWL.Citrix.Service.Models
{
    public class Notification
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public int Type { get; set; }
        public bool Closable { get; set; }

        public override bool Equals(object obj)
        {
            Notification notification = obj as Notification;

            return !Object.ReferenceEquals(null, notification)
                && ID == notification.ID
                && String.Equals(Message, notification.Message)
                && Type == notification.Type
                && Closable == notification.Closable;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, ID) ? ID.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Message) ? Message.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Type) ? Type.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Closable) ? Closable.GetHashCode() : 0);
                return hash;
            }
        }
    }
}
