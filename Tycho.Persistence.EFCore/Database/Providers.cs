namespace Tycho.Persistence.EFCore.Database;

internal static class Providers
{
    /// <summary>The official Oracle MySQL EF Core provider.</summary>
    public const string MySql = "MySql.EntityFrameworkCore";

    /// <summary>The Microsoft SQL Server EF Core provider.</summary>
    public const string SqlServer = "Microsoft.EntityFrameworkCore.SqlServer";

    /// <summary>The Npgsql PostgreSQL EF Core provider.</summary>
    public const string PostgreSql = "Npgsql.EntityFrameworkCore.PostgreSQL";

    /// <summary>The Microsoft SQLite EF Core provider.</summary>
    public const string Sqlite = "Microsoft.EntityFrameworkCore.Sqlite";
}
