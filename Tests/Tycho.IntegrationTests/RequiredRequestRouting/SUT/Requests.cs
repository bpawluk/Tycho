using Tycho.Requests;

namespace Tycho.IntegrationTests.RequiredRequestRouting.SUT;

public sealed record PlainCommand(string Value) : IRequest;
public sealed record PlainQuery(string Value) : IRequest<string>;
public sealed record MappedCommand(string Value) : IRequest;
public sealed record MappedQuery(string Value) : IRequest<string>;
public sealed record IgnoredCommand(string Value) : IRequest;
public sealed record IgnoredQuery(string Value) : IRequest<string>;
public sealed record TargetCommand(string Value) : IRequest;
public sealed record TargetQuery(string Value) : IRequest<int>;
public sealed record DirectIgnoredCommand(string Value) : IRequest;
public sealed record DirectIgnoredQuery(string Value) : IRequest<string>;
