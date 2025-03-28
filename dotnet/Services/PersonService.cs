namespace dotnet.Services
{
    public class PersonService : IPersonService
    {
        public PersonService() { }

        public Task ProcessMessage(string message)
        {
            // Calcula idade
            // Salva na base

            return Task.CompletedTask;
        }
    }
}
