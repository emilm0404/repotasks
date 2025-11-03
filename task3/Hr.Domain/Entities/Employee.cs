namespace Hr.Domain.Entities;

public class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string EmployeeNumber { get; set; } = null!; 

    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
