using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<User> Users { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; }
}

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=helloapp.db");
    }
}

class Program
{
    static void Main()
    {
        using (ApplicationContext db = new ApplicationContext())
        {
          
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            Company microsoft = new Company { Name = "Microsoft" };
            Company google = new Company { Name = "Google" };

            db.Companies.AddRange(microsoft, google);

            User tan = new User { Name = "Tan", Age = 36, Company = microsoft };
            User vasya = new User { Name = "Vasya", Age = 39, Company = alice };
            User alice = new User { Name = "Alice", Age = 28, Company = google };
            User kate = new User { Name = "Kate", Age = 25, Company = google };

            db.Users.AddRange(tan, vasya, alice, kate);

            db.SaveChanges();
        }

        using (ApplicationContext db = new ApplicationContext())
        {
            
            var users = (from user in db.Users.Include(p => p.Company) where user.CompanyId == 1 select user).ToList();

            foreach (var user in users)
            {
                Console.WriteLine($"{user.Name} ({user.Age}) {user.Company?.Name}");
            }

           
            var filteredUsers = db.Users.Include(p => p.Company).Where(p => p.Company?.Name == "Google");

            foreach (var user in filteredUsers)
            {
                Console.WriteLine($"{user.Name} ({user.Age})");
            }

           
            var tomUsers = db.Users.Where(p => EF.Functions.Like(p.Name, "Tom"));

            foreach (var user in tomUsers)
            {
                Console.WriteLine($"{user.Name} ({user.Age})");
            }


            var joinedUsers = db.Users.Join(
                db.Companies,
                u => u.CompanyId,
                c => c.Id,
                (u, c) => new { User = u, Company = c }
            );

            foreach (var result in joinedUsers)
            {
                Console.WriteLine($"{result.User.Name} ({result.User.Age}) {result.Company.Name}");
            }
        }
    }
}