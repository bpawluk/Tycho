using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Tycho.Persistence.EFCore.Database;

internal sealed class TableMapping
{
    private readonly IEntityType _entity;
    private readonly StoreObjectIdentifier _storeObject;
    private readonly ISqlGenerationHelper _sql;

    public string Name { get; }

    public TableMapping(TychoDbContext dbContext, Type entityType)
    {
        _entity = dbContext.Model.FindEntityType(entityType)!;
        ITable table = _entity.GetTableMappings().Single().Table;
        _storeObject = StoreObjectIdentifier.Table(table.Name, table.Schema);
        _sql = dbContext.GetService<ISqlGenerationHelper>();
        Name = _sql.DelimitIdentifier(table.Name, table.Schema);
    }

    public string GetColumn(string propertyName)
    {
        string columnName = _entity.FindProperty(propertyName)!.GetColumnName(_storeObject)!;
        return _sql.DelimitIdentifier(columnName!);
    }
}
