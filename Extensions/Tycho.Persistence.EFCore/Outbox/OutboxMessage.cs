using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tycho.Persistence.EFCore.Outbox;

[Table("Outbox")]
[Index(nameof(Created))]
internal class OutboxMessage
{
    [Key]
    public Guid Id { get; set; } = Guid.Empty;

    [Required]
    public string Handler { get; set; } = string.Empty;

    [Required]
    public string Payload { get; set; } = string.Empty;

    [Required]
    public MessageState State { get; set; } = MessageState.New;

    [Required]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime Updated { get; set; } = DateTime.UtcNow;

    [Required]
    public int DeliveryCount { get; set; } = 0;
}

internal enum MessageState
{
    New,
    Processing,
    Failed
}