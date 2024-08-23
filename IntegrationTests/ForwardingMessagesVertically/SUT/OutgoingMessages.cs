﻿using Tycho.Messaging.Payload;

namespace IntegrationTests.ForwardingMessagesVertically.SUT;

internal record MappedEvent(TestResult Result) : IEvent;

internal record MappedRequest(TestResult Result) : IRequest;

internal record MappedRequestWithResponse(TestResult Result) : IRequest<string>;
