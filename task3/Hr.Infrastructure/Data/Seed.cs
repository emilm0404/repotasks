using Hr.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hr.Infrastructure.Data;

public static class Seed
{
    private static readonly string[] FirstNames =
    {
        "Emil","Alex","Sam","Jamie","Taylor","Jordan","Lee","Casey","Avery","Morgan","Riley",
        "Chris","Pat","Dana","Robin","Quinn","Skyler","Cameron","Jess","Kendall","Drew"
    };

    private static readonly string[] LastNames =
    {
        "Smith","Johnson","Williams","Brown","Jones","Garcia","Miller","Davis","Rodriguez","Martinez",
        "Hernandez","Lopez","Gonzalez","Wilson","Anderson","Michelsson","Thomas","Taylor","Moore","Jackson","Martin"
    };

    public static async Task EnsureSeedAsync(HrDbContext db, int count = 200, CancellationToken ct = default)
    {
        if (await db.Employees.AnyAsync(ct)) return;

        var rnd = new Random(42);
        var employees = new List<Employee>(count);

        for (int i = 1; i <= count; i++)
        {
            var f = FirstNames[rnd.Next(FirstNames.Length)];
            var l = LastNames[rnd.Next(LastNames.Length)];
            employees.Add(new Employee
            {
                FirstName = f,
                LastName = l,
                EmployeeNumber = $"E{i:000000}"
            });
        }

        await db.Employees.AddRangeAsync(employees, ct);
        await db.SaveChangesAsync(ct);
    }
}
