using System;
using System.ComponentModel.DataAnnotations;
using Tycho.Persistence.EFCore.Common;

namespace Tycho.Persistence.EFCore.Outbox;

internal class OutboxEntry
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [Required] 
    public string Event { get; set; } = string.Empty;

    [Required]
    public string Handler { get; set; } = string.Empty;

    [Required]
    public string Route { get; set; } = string.Empty;

    [Required]
    public string Payload { get; set; } = string.Empty;

    [Required]
    public EntryState State { get; set; } = EntryState.New;

    [Required]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime Updated { get; set; } = DateTime.UtcNow;

    [Required]
    public uint DeliveryAttempts { get; set; } = 0;
}
