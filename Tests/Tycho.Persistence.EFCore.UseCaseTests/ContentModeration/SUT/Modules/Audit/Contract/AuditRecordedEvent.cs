using Tycho.Events;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Contract;

public record AuditRecordedEvent(Guid AuditId, string Subject, int SubjectId) : IEvent;
