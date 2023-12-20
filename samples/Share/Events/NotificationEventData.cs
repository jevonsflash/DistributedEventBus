using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Events.Bus;

namespace Share.Events
{
    public class NotificationEventData : EventData
    {
        public int Id { get; set; }
       
        public string Title { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }
    }
}
