using System;
using System.Threading.Tasks;
using FluentAssertions;
using Hr.Application.DTOs;
using Hr.Application.Services;
using Hr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hr.Tests;

public class EmployeeServiceTests : IDisposable
{
    private readonly HrDbContext _db;
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        var options = new DbContextOptionsBuilder<HrDbContext>()
            .UseInMemoryDatabase($"HrTests_{Guid.NewGuid():N}")
            .Options;

        _db = new HrDbContext(options);
        _sut = new EmployeeService(_db);
    }

    [Fact]
    public async Task Create_Normalizes_Input_And_Enforces_Uniqueness()
    {
        var dto = new EmployeeCreateDto
        {
            FirstName = "  Jane ",
            LastName = " Doe ",
            EmployeeNumber = "e-1001"
        };

        var created = await _sut.CreateAsync(dto, default);

        created.EmployeeNumber.Should().Be("E-1001");
        created.FirstName.Should().Be("Jane");
        created.LastName.Should().Be("Doe");

        var duplicate = new EmployeeCreateDto
        {
            FirstName = "John",
            LastName = "Smith",
            EmployeeNumber = "E-1001"
        };

        await _sut.Awaiting(s => s.CreateAsync(duplicate, default))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*unique*");
    }

    [Fact]
    public async Task Update_With_Stale_RowVersion_Throws_Concurrency()
    {
        var created = await _sut.CreateAsync(new EmployeeCreateDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            EmployeeNumber = "E-2000"
        }, default);

        var update = new EmployeeUpdateDto
        {
            FirstName = "Janet",
            LastName = "Doe",
            EmployeeNumber = "E-2000",
            RowVersionBase64 = created.RowVersionBase64
        };

        var updated = await _sut.UpdateAsync(created.Id, update, default);
        updated.FirstName.Should().Be("Janet");
        updated.RowVersionBase64.Should().NotBe(created.RowVersionBase64);

        var staleUpdate = new EmployeeUpdateDto
        {
            FirstName = "Jane",
            LastName = "Doe",
            EmployeeNumber = "E-2000",
            RowVersionBase64 = created.RowVersionBase64
        };

        await _sut.Awaiting(s => s.UpdateAsync(created.Id, staleUpdate, default))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Concurrency conflict*");
    }

    [Fact]
    public async Task Update_With_Invalid_RowVersion_Fails_Validation()
    {
        var created = await _sut.CreateAsync(new EmployeeCreateDto
        {
            FirstName = "Alex",
            LastName = "Roe",
            EmployeeNumber = "E-3000"
        }, default);

        var invalid = new EmployeeUpdateDto
        {
            FirstName = "Alex",
            LastName = "Roe",
            EmployeeNumber = "E-3000",
            RowVersionBase64 = "not-base64"
        };

        await _sut.Awaiting(s => s.UpdateAsync(created.Id, invalid, default))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("rowVersionBase64 must be valid base64*");
    }

    [Fact]
    public async Task Delete_With_Stale_RowVersion_Throws_Concurrency()
    {
        var created = await _sut.CreateAsync(new EmployeeCreateDto
        {
            FirstName = "Chris",
            LastName = "Pine",
            EmployeeNumber = "E-4000"
        }, default);

        var current = await _sut.GetByIdAsync(created.Id, default);
        current.Should().NotBeNull();

        var deleteBytes = Convert.FromBase64String(created.RowVersionBase64);

        await _sut.Awaiting(s => s.DeleteAsync(created.Id, deleteBytes, default))
            .Should().NotThrowAsync();

        var recreated = await _sut.CreateAsync(new EmployeeCreateDto
        {
            FirstName = "Chris",
            LastName = "Pine",
            EmployeeNumber = "E-5000"
        }, default);

        var staleBytes = new byte[8]; 

        await _sut.Awaiting(s => s.DeleteAsync(recreated.Id, staleBytes, default))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Concurrency conflict*");
    }

    [Fact]
    public async Task Delete_With_Missing_RowVersion_Fails_Validation()
    {
        var created = await _sut.CreateAsync(new EmployeeCreateDto
        {
            FirstName = "Pat",
            LastName = "Lee",
            EmployeeNumber = "E-6000"
        }, default);

        await _sut.Awaiting(s => s.DeleteAsync(created.Id, Array.Empty<byte>(), default))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("rowVersion is required.");
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
