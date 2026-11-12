using Tycho.Requests;

namespace Tycho.Persistence.EFCore.UseCaseTests.ContentModeration.SUT.Modules.Audit.Contract;

public record RecordAuditRequest(Guid Id, string Subject, int SubjectId, string Status) : IRequest;
