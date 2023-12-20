using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus;
using Share.Entites;
using Share.Events;

namespace Producer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Producer Starting..");
            using (var bootstrapper = AbpBootstrapper.Create<ProducerModule>())
            {
                bootstrapper.Initialize();
                Console.WriteLine("Producer Started!");

            }
            using (var personRepositoryWrapper = IocManager.Instance.ResolveAsDisposable<IRepository<Person>>())
            {
                var personRepository = personRepositoryWrapper.Object;
                var person = new Person()
                {

                    Name = "John",
                    Age = 36,
                    PhoneNumber = "18588888888"

                };

                personRepository.Insert(person);

                var person2 = new Person()
                {

                    Name = "John2",
                    Age = 36,
                    PhoneNumber = "18588888889"

                };
                personRepository.Insert(person2);

                var persons = personRepository.GetAllList();
                foreach (var p in persons)
                {
                    p.Age += 1;
                    personRepository.Update(p);
                    Console.WriteLine($"Entity Updated - Name:{p.Name}, Age:{p.Age}, PhoneNumber:{p.PhoneNumber}");

                }
                foreach (var p in persons)
                {
                    personRepository.Delete(p);
                    Console.WriteLine($"Entity Deleted - Name:{p.Name}, Age:{p.Age}, PhoneNumber:{p.PhoneNumber}");

                }
            }
            var eventBus = IocManager.Instance.Resolve<IEventBus>();


            eventBus.Trigger<NotificationEventData>(new NotificationEventData()
            {
                Title = "Hi",
                Message = "Customized definition event test!",
                Id = 100,
                IsRead = true,
            });

            Console.ReadLine();

        }
    }
}
