using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tycho.Persistence.EFCore.Outbox;

[Index(nameof(Created))]
internal class OutboxEntry
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

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
