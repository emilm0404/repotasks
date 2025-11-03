using Hr.Application.DTOs;

namespace Hr.Application.Abstractions;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetAsync(string? search, string? sort, string? dir, int page, int pageSize, CancellationToken ct);
    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto, CancellationToken ct);
    Task<EmployeeDto> UpdateAsync(int id, EmployeeUpdateDto dto, CancellationToken ct);
    Task DeleteAsync(int id, byte[] rowVersion, CancellationToken ct);
}
