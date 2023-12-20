using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Share.Entites;
using Share.Events;

namespace Consumer
{
    public class NotificationEventHandler :
        IEventHandler<NotificationEventData>,      
        ITransientDependency
    {


        /// <summary>
        /// Constructor
        /// </summary>
        public NotificationEventHandler()
        {

        }

      
        void IEventHandler<NotificationEventData>.HandleEvent(NotificationEventData eventData)
        {
            Console.WriteLine($"Id: {eventData.Id}");
            Console.WriteLine($"Title: {eventData.Title}");
            Console.WriteLine($"Message: {eventData.Message}");
            Console.WriteLine($"IsRead: {eventData.IsRead}");

        }
    }
}
