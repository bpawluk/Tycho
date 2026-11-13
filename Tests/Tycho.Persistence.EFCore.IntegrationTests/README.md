Persistence integration tests run against real SQLite, SQL Server, PostgreSQL, and MySQL databases.
Each scenario is a theory over `DatabaseProvider`, using the same server fixture as `UseCaseTests`.
SQL Server, PostgreSQL, and MySQL require a Linux Docker engine; servers start lazily and each test owns an isolated database.

Test database behavior through the general commands and queries in `Tycho.Persistence.EFCore.Operations`.
Do not invoke provider implementations or their internal helpers directly.
Operation tests own eligibility, ordering, ownership, claim transitions, persistence, and retention boundaries.
Keep one test class per general operation. Give each scenario its own `Method_Scenario_ExpectedBehavior` test with Arrange/Act/Assert comments; provider selection is the only theory input.
Consumer, writer, transaction, and retention-worker tests cover their own wiring and flow without duplicating operation matrices.

`Tycho.Persistence.EFCore.UnitTests` contains isolated tests with mocked dependencies and no database connections, including no SQLite in-memory connections or SQL interception harnesses.

Run each project with `dotnet test Tests/<project> -f net10.0` (or `net8.0` / `net9.0`).
