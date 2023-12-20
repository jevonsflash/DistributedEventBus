using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Share.Entites;

namespace Consumer
{
    public class LocalEntityChangedEventHandler :
        IEventHandler<EntityUpdatedEventData<Person>>,
        IEventHandler<EntityCreatedEventData<Person>>,
        IEventHandler<EntityDeletedEventData<Person>>,
        ITransientDependency
    {


        /// <summary>
        /// Constructor
        /// </summary>
        public LocalEntityChangedEventHandler()
        {

        }

        void IEventHandler<EntityUpdatedEventData<Person>>.HandleEvent(EntityUpdatedEventData<Person> eventData)
        {
            var person = eventData.Entity;
            Console.WriteLine($"Local Entity Updated - Name:{person.Name}, Age:{person.Age}, PhoneNumber:{person.PhoneNumber}");
        }

        void IEventHandler<EntityCreatedEventData<Person>>.HandleEvent(EntityCreatedEventData<Person> eventData)
        {
            var person = eventData.Entity;
            Console.WriteLine($"Local Entity Created - Name:{person.Name}, Age:{person.Age}, PhoneNumber:{person.PhoneNumber}");

        }

        void IEventHandler<EntityDeletedEventData<Person>>.HandleEvent(EntityDeletedEventData<Person> eventData)
        {
            var person = eventData.Entity;
            Console.WriteLine($"Local Entity Deleted - Name:{person.Name}, Age:{person.Age}, PhoneNumber:{person.PhoneNumber}");

        }
    }
}
