using Hr.Application.Abstractions;
using Hr.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Hr.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeDto>>> List(
        [FromQuery] string? search,
        [FromQuery] string? sort = "LastName",
        [FromQuery] string? dir = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var result = await _service.GetAsync(search, sort, dir, page, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(EmployeeCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
        {
            var created = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("unique", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = ex.Message, Status = StatusCodes.Status409Conflict });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> Update(int id, EmployeeUpdateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
        {
            var updated = await _service.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            var message = ex.Message ?? "Conflict";
            if (message.StartsWith("Concurrency", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new ProblemDetails { Title = "Conflict", Detail = message, Status = StatusCodes.Status409Conflict });
            }

            if (message.Contains("rowVersionBase64", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = message, Status = StatusCodes.Status400BadRequest });
            }

            return Conflict(new ProblemDetails { Title = "Conflict", Detail = message, Status = StatusCodes.Status409Conflict });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] string rowVersionBase64, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(rowVersionBase64))
            return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = "rowVersionBase64 is required." });

        byte[] rowVersion;
        try
        {
            rowVersion = Convert.FromBase64String(rowVersionBase64);
        }
        catch (FormatException)
        {
            return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = "rowVersionBase64 must be valid base64." });
        }

        try
        {
            await _service.DeleteAsync(id, rowVersion, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = ex.Message, Status = StatusCodes.Status409Conflict });
        }
    }
}
