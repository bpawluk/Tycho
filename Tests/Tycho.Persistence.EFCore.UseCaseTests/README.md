# Persistence use-case tests

The same OnlineStore, BloggingWebsite, ContentModeration, and HomeDashboard
scenarios share a provider matrix under .NET 8, 9, and 10. Assertions and
application flows are shared across providers. Each framework now exercises its
corresponding EF Core major version, including the production persistence library
and its unit tests:

| Target | EF Core | SQLite / SQL Server | PostgreSQL provider | MySQL provider |
| --- | --- | --- | --- | --- |
| .NET 8 | 8.0.30 | 8.0.30 | Npgsql 8.0.11 | Oracle 10.0.9 (net8.0 asset for EF Core 8) |
| .NET 9 | 9.0.19 | 9.0.19 | Npgsql 9.0.4 | Oracle 10.0.9 (net9.0 asset for EF Core 9) |
| .NET 10 | 10.0.11 | 10.0.11 | Npgsql 10.0.3 | Oracle 10.0.9 (net10.0 asset for EF Core 10) |

.NET 9 is retained to validate EF Core 9 as a distinct compatibility target.
There are 48 application-flow cases across SQLite, SQL Server, PostgreSQL, and
MySQL across the three framework targets. This project contains application use
cases; direct claim and transition operation tests have been removed.

## Run

Install the .NET 10 SDK and .NET 8, 9, and 10 runtimes, and provide a reachable Linux Docker
engine with enough memory to run SQL Server, PostgreSQL, and MySQL. SQL Server requires
an x64 container host. Testcontainers pulls the pinned images on the first run;
Docker/image startup failures fail active tests rather than skipping them.

```powershell
dotnet test --project Tests/Tycho.Persistence.EFCore.UseCaseTests/Tycho.Persistence.EFCore.UseCaseTests.csproj --configuration Release --max-parallel-test-modules 1
```

The module limit runs framework targets sequentially, reducing container memory
usage. Test classes may run concurrently within each target.

## Isolation and diagnostics

An assembly fixture starts each server lazily and shares it across scenarios in
one framework target. SQLite uses temporary files and needs no Docker server.
Every scenario/provider row has a separate database for each concrete DbContext
type. Repeated instances of the same module intentionally share that context's
database within the scenario, preserving the Audit and Climate use cases.

The test applications create their actual EF models using `EnsureCreatedAsync`.
Tests stop/dispose applications before deleting databases. Database cleanup uses
an independent deadline even after test cancellation; the fixture disposes all
containers at the end. SQLite files are deleted rather than left in the working
directory. A test-scoped async binding supplies provider options to all module
contexts without sharing mutable configuration between parallel scenarios.

Output includes provider/runtime, the last 128 SQL commands with parameter types,
and the last 16 database command errors with their SQL. Sensitive-data logging is
disabled; command traces contain parameter types rather than values. Commands have 30-second timeouts;
eventual assertions have a 30-second deadline. The outer six-minute test deadline
allows up to five minutes for a cold container start.

Images resolved on 2026-10-09 (refresh deliberately and rerun all targets):

- SQL Server: `mcr.microsoft.com/mssql/server:2022-latest@sha256:4402d880dd4c34bfa7d8705e56a86cd6c88da80a1f6bbbe741f999e76264a090`
- MySQL: `mysql:8.4@sha256:6ea90827b1100f8f2ae306a539f86d2c264a26ed435a2a9f75551dd5c3aeb242`
- PostgreSQL: `postgres:17-bookworm@sha256:3645570cccdfa447589da9f57dd740faa29b30938e861289a5574b6ca6b03826`

## Persistence operation implementations

Database reads and writes use static operations under `Operations/`. Operations
with only a default implementation call it directly. The inbox and outbox claim
operations define typed delegates and a `ProviderDispatcher<TDelegate>` that selects
an implementation using `Database.ProviderName`, falling back to its default.
Delegate assignments validate claim implementation signatures at compile time.
Consumers, writers, and cleaners pass their scoped context to these operations;
transaction coordination, serialization, duplicate verification, and commit
notifications remain in their existing orchestration layers.
Transaction and outbox write orchestration call `DbContext.SaveChangesAsync`
directly; saving tracked changes has no separate operation interface.

All query logic lives in internal static classes under
`Operations/Implementations/Default/`, `MySql/`, `PostgreSql/`, or `SqlServer/`.
Every operation has a default EF implementation. Operation classes, execution
methods, delegates, provider constants, dispatcher, and persistence types are
internal. Operations need no DI registrations or operation
interfaces. Dispatch uses exact provider names for Oracle MySQL, Npgsql PostgreSQL,
and Microsoft SQL Server. Unknown providers use the default EF implementation.
The production library has no dependency on these provider packages.

Consumers call one claim-and-return operation. Its internal `ClaimResult<TEntry>`
distinguishes no available claim, a claimed entry, and an entry missing after a
successful update. The two-step paths preserve the existing missing-entry warnings.
The default path calls the existing ordered EF update and untracked read helpers.
MySQL uses its tailored ordered update followed by the same EF read: neither path
reads when the update affects anything other than exactly one row.

PostgreSQL selects an ordered candidate inside a `FOR UPDATE SKIP LOCKED` CTE,
updates by owner and entry ID, and returns the row with `RETURNING`. SQL Server
updates an ordered `TOP (1)` CTE using `UPDLOCK, READPAST, READCOMMITTEDLOCK` and
`OUTPUT INSERTED`. Each native path issues one database command and fully consumes
the result before exposing a claimed entry. No LINQ is composed over the DML SQL.
Claim operations dispatch by database provider. Query filters and custom value
converters on framework entities are unsupported; no model validation or
customization fallback is performed. Provider and EF errors propagate to the caller.
Renamed tables, columns, and supported schemas remain supported. Native
implementations require a single table mapping. A command failure never triggers
runtime fallback.

These paths prefer the oldest available eligible entry; locked candidates can be
skipped. They do not promise strict FIFO, starvation freedom, or absence of every
deadlock. SQL Server READPAST skips row/key locks, not page locks, and temporary scan
locks can make a poll return no claim even with work remaining. Workers poll again.
The native locking contract targets PostgreSQL read committed and SQL Server read
committed with READ_COMMITTED_SNAPSHOT either OFF or ON. The remaining application
scenarios do not explicitly verify both SQL Server settings; see the coverage gaps
below. The library does not change transaction isolation settings.

Internal consumers receive both the entry ID and claim ID from the processor,
including failure paths and inbox deserialization failures. Completion/failure
commands use the default EF implementation for every provider, including MySQL.
Each executes one conditional update checking owner, entry ID, state, and claim;
stale claims and mismatched entry IDs affect zero rows. No preliminary SELECT or
provider-specific index hint is needed.
This does not establish freedom from every deadlock or guarantee eventual progress
under arbitrary contention. Set
`TYCHO_MYSQL_DEADLOCK_DIAGNOSTICS=1` to capture engine reports and transition query
plans; these opt-in diagnostics can contain test data values.

MySQL inbox/outbox claims use one parameterized
`UPDATE ... WHERE ... ORDER BY Updated, EntryId LIMIT 1`. Table/schema names
and column names come from EF metadata and use provider identifier quoting.
Values use EF's standard parameterization
for their CLR types. `Database.ExecuteSqlRawAsync` preserves
the current transaction, timeout, cancellation, and SQL interceptors without
manually building EF relational commands or retrying the statement.

Claims are not automatically retried: if a claim succeeds but its acknowledgement
is lost, replaying the ordered update could claim another entry with the same
claim identifier. Default EF claim commands suppress provider retries using the
non-retrying scope at the unified operation boundary, including result reads.
Every provider rejects an enclosing active retrying EF strategy. Claim failures propagate to
the worker, which logs the failure, backs off, and polls again in a fresh scope
with a new claim identifier. Any successful but unacknowledged claim remains
leased until expiration and is then eligible under the normal attempt limits.

The MySQL raw claim implementation requires a single table mapping.
Renamed tables and columns are supported. Custom value converters and global
query filters on Tycho's internal claim entities are outside the supported model.
MySQL's ordered default EF update is not a valid fallback.
The raw writes preserve owner
filtering, eligibility rules, attempt increments, claim identifiers, and lease
expiration. Successful claiming still requires exactly one affected row, followed
by reading the entry using its claim identifier.

## Coverage boundaries

The application scenarios exercise persistence through public application flows on
all four providers. The EFCore unit tests use SQLite and cover claim eligibility,
oldest-entry selection, owner isolation, failed-entry retries, expired and active
leases, returned data, valid and stale completion/failure claims, and wrong entry
IDs. Transaction unit tests cover commit, rollback, cancellation, and retry-strategy
handling for the transaction wrapper. Processor unit tests cover handler/delivery
failure and cancellation using mocked consumers. Those checks do not establish
provider-specific claim concurrency or claim execution-strategy behavior.

The removed `ClaimOperationTests` and `MySqlOperationTests` contained direct
operation integration checks. Important coverage worth restoring in a dedicated
integration suite, outside this application use-case project, includes:

- Competing connections cannot claim the same entry; a populated queue yields
  distinct claims. Ordinary application flows do not synchronize competing claims.
- A transient failure after a successful claim write must not replay the claim and
  lease another entry. Claim-specific rejection of an enclosing retrying strategy
  and cancellation before mutation are also not covered by the transaction-wrapper
  tests.
- PostgreSQL and SQL Server skip a locked oldest entry and revisit it after rollback;
  SQL Server was explicitly exercised with READ_COMMITTED_SNAPSHOT both OFF and ON.
  Application scenarios do not vary that setting or deliberately hold candidate locks.
- Claim writes and MySQL completion/failure updates participate in an explicit
  transaction and are restored by rollback. Generic SQLite transaction tests do not
  verify those commands against the native providers.
- On MySQL, replacing a claim immediately before completion/failure cannot let an
  old worker modify the new claim. Existing SQLite consumer tests cover stale
  claims and wrong entry IDs, but do not force this race on MySQL.
- Native claim SQL and MySQL transition commands honor renamed columns and quoted
  table/schema names. Application scenarios exercise their configured table names,
  but do not rename every framework column or use identifiers containing spaces.

The deleted tests also checked exact SQL command counts and untracked operation
results. These are implementation-level checks with lower priority than exclusion,
lease fencing, rollback, and preventing claim replay. Basic eligibility, state
transitions, and stale/wrong-entry rejection remain covered by the consumer unit
tests, although SQLite coverage does not validate native SQL on other providers.

These gaps are documented rather than replaced with new operation tests in this
project. The application scenarios do not guarantee freedom from deadlocks,
multi-process crash recovery, or the concurrency and failure properties above.

## Official MySQL provider status

All three targets reference Oracle's `MySql.EntityFrameworkCore` 10.0.9 and use
`MySql.Data` / `UseMySQL`. There are no alternate MySQL EF providers or
provider-specific retry settings. All four MySQL scenarios are enabled on each
framework target and retain the same application assertions as the other providers.

Validated on 2026-10-10: all 48 provider use cases passed across .NET 8, 9, and 10.
The full solution run passed 1,434 tests with no failures or skips.

Repeated .NET 10 runs also observed MySQL deadlocks in BloggingWebsite and
ContentModeration, despite both scenarios subsequently passing. Passing application flows
therefore do not establish freedom from deadlocks or reliable recovery under
contention. Assertion timeouts report the unmet condition; inspect the captured
database command failures when diagnosing them.

Concurrent framework runs reproduced a HomeDashboard downstairs temperature
mismatch: five readings were stored instead of four after a deadlock during
HomeDashboard inbox completion. The handler previously called a room module's
request endpoint, which published immediately through that module's in-memory
outbox outside the HomeDashboard transaction. Retrying the rolled-back handler
published the same reading again with a new event identity.

The sample now publishes room-specific forwarding events through the scoped
HomeDashboard publisher. Mapped routes select the room's Climate module, while
the outgoing messages are staged in HomeDashboard's EF outbox and commit with
inbox completion. A rollback discards both. The assertion still requires the
exact expected reading count and values. On a mismatch, output includes the
expected and actual readings and captured background worker warnings/errors.
This corrects the sample's transaction boundary; it does not remove MySQL
deadlocks or make cross-module request side effects transactional.
After the correction, all 48 provider cases passed with the three framework
targets running concurrently (`--max-parallel-test-modules 3`).

The default ordered, limited EF claim update previously generated invalid
`UPDATE ... FROM` SQL on all three Oracle provider targets. The MySQL-specific
claim commands described above replace that translation while preserving claim
eligibility and state transitions. MySQL completion/failure commands and all remaining operations use the defaults.

Connections to the disposable local MySQL container explicitly disable TLS with
`SslMode = MySqlSslMode.Disabled`. With TLS enabled, Oracle's SSL authentication
cancellation callback threw an `IOException` during worker shutdown in two use
cases. This suite tests SQL and application behavior; TLS negotiation, certificate
validation, and TLS shutdown behavior are outside its coverage. Production
connection settings are unaffected.

MySQL contexts use root credentials confined to the disposable container so
scenarios can create/drop their isolated databases.

These tests exercise persistence through complete application flows. They do not
prove multi-process claim exclusion, deterministic contention, lease recovery,
or behavior after a process crash; those need focused scenarios in later stages.
