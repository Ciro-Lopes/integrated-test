using System.Data;
using dotnet.Entities;
using Npgsql;

namespace dotnet.Repositories
{
    public class PersonRepository
    {
        private readonly string? _connectionString;
        private NpgsqlConnection? _connection;

        public PersonRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Postgres");
        }

        public async Task<Person?> UpdateAsync(Person person)
        {
            await OpenConnectionAsync();

            var query = "UPDATE persons SET age = @age WHERE id = @id RETURNING id, name, birthdate, age";

            await using var command = new NpgsqlCommand(query, _connection);
            command.Parameters.AddWithValue("age", person.Age);
            command.Parameters.AddWithValue("id", person.Id);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Person
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    BirthDate = reader.GetDateTime(2),
                    Age = reader.GetInt32(3)
                };
            }

            return null;
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            await OpenConnectionAsync();

            var query = "SELECT id, birthdate FROM persons WHERE id = @id";

            await using var command = new NpgsqlCommand(query, _connection);
            command.Parameters.AddWithValue("id", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Person
                {
                    Id = reader.GetInt32(0),
                    BirthDate = reader.GetDateTime(1),
                };
            }

            return null;
        }

        public async Task OpenConnectionAsync()
        {
            if (_connection is null || _connection.State != ConnectionState.Open)
            {
                _connection = new NpgsqlConnection(_connectionString);
                await _connection.OpenAsync();
            }
        }
    }
}