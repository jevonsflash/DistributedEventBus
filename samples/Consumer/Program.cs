using Abp;

namespace Consumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Consumer Starting..");
            using (var bootstrapper = AbpBootstrapper.Create<ConsumerModule>())
            {
                bootstrapper.Initialize();
                Console.WriteLine("Consumer Started!");

            }
            Console.ReadLine();

        }
    }
}
