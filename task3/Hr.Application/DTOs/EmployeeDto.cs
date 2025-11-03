namespace Hr.Application.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string EmployeeNumber { get; set; } = "";
    public string RowVersionBase64 { get; set; } = ""; 
}
