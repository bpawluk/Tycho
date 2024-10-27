using Microsoft.EntityFrameworkCore;

namespace Tycho.Persistence.EFCore.IntegrationTests;

internal class TestDbContext(DbContextOptions<TestDbContext> options) : TychoDbContext(options) { }