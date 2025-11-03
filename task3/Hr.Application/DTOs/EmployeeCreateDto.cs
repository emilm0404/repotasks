using System.ComponentModel.DataAnnotations;

namespace Hr.Application.DTOs;

public class EmployeeCreateDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = "";

    [Required, MaxLength(100)]
    public string LastName { get; set; } = "";

    [Required, MaxLength(32), RegularExpression("^[A-Z0-9-]{3,20}$")]
    public string EmployeeNumber { get; set; } = "";
}
