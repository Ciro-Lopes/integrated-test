namespace dotnet.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }

        public void CalculateAge()
        {
            var today = DateTime.Today;
            Age = today.Year - BirthDate.Year;

            // Check if you haven't had a birthday this year
            if (BirthDate.Date > today.AddYears(-Age))
                Age--;
        }
    }
}
