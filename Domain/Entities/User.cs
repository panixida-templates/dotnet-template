using Common.Enums;

using Entities.Core;

namespace Entities;

public sealed class User : BaseEntity<int>
{
    public Role Role { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int Age { get; set; }
    public DateTime Birthday { get; set; }

    public User(
        int id,
        Role role,
        string name,
        string email,
        string phone,
        int age,
        DateTime birthday) : base(id)
    {
        Role = role;
        Name = name;
        Email = email;
        Phone = phone;
        Age = age;
        Birthday = birthday;
    }
}
