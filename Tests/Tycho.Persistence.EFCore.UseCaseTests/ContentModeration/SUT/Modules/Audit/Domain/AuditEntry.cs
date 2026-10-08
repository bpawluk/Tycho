namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Domain;

internal class AuditEntry(Guid id, string subject, int subjectId, string status)
{
    public Guid Id { get; private set; } = id;
    public string Subject { get; private set; } = subject;
    public int SubjectId { get; private set; } = subjectId;
    public string Status { get; private set; } = status;
}
