using System.Text.Json;
using System;
using dotnet.Entities;
using dotnet.Repositories;

namespace dotnet.Services
{
    public class PersonService
    {
        private readonly PersonRepository _personRepository;

        public PersonService(PersonRepository personRepository) 
        {
            _personRepository = personRepository;
        }

        public async Task ProcessMessageAsync(string message)
        {
            if (!int.TryParse(message, out int personId))
            {
                Console.WriteLine("Error on serialize PersonId!");
                return;
            }

            Person? personSaved = await _personRepository.GetByIdAsync(personId);
            if (personSaved == null)
            {
                Console.WriteLine("Person not found!");
                return;
            }

            personSaved.CalculateAge();

            Person? personUpdated = await _personRepository.UpdateAsync(personSaved);

            Console.WriteLine($"Person updated: {JsonSerializer.Serialize(personUpdated)}");
        }
    }
}
