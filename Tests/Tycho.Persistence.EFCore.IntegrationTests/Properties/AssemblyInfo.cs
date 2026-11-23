using Tycho.Persistence.EFCore.TestUtils.Databases;
using Xunit.Sdk;
using Xunit.v3;

[assembly: AssemblyFixture(typeof(DatabaseServers))]
[assembly: Parallelization(Mode = ParallelMode.Collections, MaxThreads = 4)]
