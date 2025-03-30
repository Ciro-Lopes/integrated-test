namespace dotnet.Services
{
    public class PersonService
    {
        public PersonService() { }

        public Task ProcessMessage(string message)
        {
            // Calcula idade
            // Salva na base
            Console.WriteLine(message);

            return Task.CompletedTask;
        }
    }
}
