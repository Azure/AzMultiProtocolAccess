using Bogus;
using Newtonsoft.Json;

namespace AzureStorageMPAdemo
{

    public class Person
    {

        private static int _userId = 100;
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Title { get; set; }
        public DateTime DOB { get; set; }
        public string? Email { get; set; }
        public Gender Gender { get; set; }
        public string? SSN { get; set; }
        public string? Suffix { get; set; }
        public string? Phone { get; set; }


    public static Faker<Person> FakeData { get; } =
      new Faker<Person>()
          .RuleFor(p => p.Id, f => _userId++)
          .RuleFor(p => p.FirstName, f => f.Name.FirstName())
          .RuleFor(p => p.MiddleName, f => f.Name.FirstName())
          .RuleFor(p => p.LastName, f => f.Name.LastName())
          .RuleFor(p => p.Title, f => f.Name.Prefix(f.Person.Gender))
          .RuleFor(p => p.Suffix, f => f.Name.Suffix())
          .RuleFor(p => p.Email, (f, p) => f.Internet.Email(p.FirstName, p.LastName))
          .RuleFor(p => p.DOB, f => f.Date.Past(18))
          .RuleFor(p => p.Gender, f => f.PickRandom<Gender>())
          .RuleFor(p => p.SSN, f => f.Random.Replace("###-##-####"))
          .RuleFor(p => p.Phone, f => f.Phone.PhoneNumber("(###)-###-####"));
    }

    public enum Gender
    {
        Male,
        Female,
        Genderless
    }

}
