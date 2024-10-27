using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Events;
using Tycho.Persistence.EFCore.IntegrationTests._Utils;
using Tycho.Persistence.EFCore.IntegrationTests.SUT.Handlers;
using Tycho.Requests;

namespace Tycho.Persistence.EFCore.IntegrationTests.SUT;

// Handles
public record BeginTestWorkflowRequest(TestResult Result) : IRequest;

// Events
public record TestEvent(TestResult Result) : IEvent;

internal class TestApp(TestWorkflow<TestResult> testWorkflow) : TychoApp
{
    private readonly TestWorkflow<TestResult> _testWorkflow = testWorkflow;

    protected override void DefineContract(IAppContract app)
    {
        app.Handles<BeginTestWorkflowRequest, BeginTestWorkflowRequestHandler>();
    }

    protected override void IncludeModules(IAppStructure app)
    {
        app.Uses<TestModule>();
    }

    protected override void MapEvents(IAppEvents app)
    {
        app.Handles<TestEvent, TestEventHandler>();
    }

    protected override void RegisterServices(IServiceCollection app)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var dbOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        app.AddSingleton(_testWorkflow)
           .AddSingleton(connection)
           .AddSingleton(dbOptions)
           .AddTychoPersistence<TestDbContext>();
    }

    protected override async Task Startup(IServiceProvider app)
    {
        using var context = app.GetRequiredService<TestDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}