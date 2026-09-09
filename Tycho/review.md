 1. DONE — Contract validation resolves scoped handlers from the root provider.
     /C:/Users/immal/source/repos/Tycho/Tycho/Structure/Internals.cs:57
     After building, HasService<T>() calls GetService(). A child module’s Requires<TRequest>() therefore constructs the parent’s request registration and
     its scoped handler merely to check availability. With scope validation enabled, this can prevent startup; otherwise, scoped dependencies are retained
     in the root scope. Forwarding registrations can also trigger premature construction of other modules. Check registration metadata without constructing
     services.

  2. DONE — Different request bindings silently reuse the first mapping.
     /C:/Users/immal/source/repos/Tycho/Tycho/Requests/Registrating/DownStreamRegistrator.cs:90
     Registrations distinguish TSourceModule, but their mapped forwarder service types do not. TryAddTransient() retains the first factory. If modules A and
     B map the same request types to module C using different delegates, both execute A’s mapping. Mapped exposure has the same problem, and upstream/
     downstream registrations can collide too. Store mapping delegates on each binding or include binding identity in the service type.

  3. High — Routing instantiates event handlers before delivery.
     /C:/Users/immal/source/repos/Tycho/Tycho/Events/Registrating/Registrations/FinalEventRegistration.cs:16, /C:/Users/immal/source/repos/Tycho/Tycho/
     Events/Broker/EventBroker.cs:25
     Routing resolves registrations whose constructors require actual handlers. Publishing consequently constructs destination handlers and their
     dependencies before writing the outbox. A handler constructor failure prevents publication. Cross-module routing also disposes its scope synchronously,
     which fails for resolved services that implement only IAsyncDisposable. Separate routing metadata from handler resolution; instantiate the selected
     handler during inbox processing.

  4. DONE — Shutdown can wait indefinitely despite cancellation.
     /C:/Users/immal/source/repos/Tycho/Tycho/Events/Inbox/InboxProcessor.cs:40, /C:/Users/immal/source/repos/Tycho/Tycho/Processor/JobRunner.cs:86
     Both processors ignore the token passed to StopAsync(). The runner waits for every running job, while its timeout only requests cancellation. A handler
     that does not observe cancellation can therefore prevent shutdown indefinitely. A stuck queue read similarly blocks JobProcessor.StopAsync(). Propagate
     the shutdown deadline through the waiting logic, with an explicit policy for unfinished work.

  5. Medium — Background processor errors disappear.
     /C:/Users/immal/source/repos/Tycho/Tycho/Processor/JobProcessor.cs:183
     Errors are reported exclusively through OnJobProcessorError, but neither inbox nor outbox processor subscribes to it. Queue-read failures, job
     timeouts, and exceptions escaping job execution can therefore produce no diagnostic output. Some fatal processing-loop failures leave a stopped worker
     behind an apparently started hosted service. Connect error reporting to logging and expose worker failure through health/status reporting.

  6. Medium — A notification race can delay messages by five minutes.
     /C:/Users/immal/source/repos/Tycho/Tycho/Processor/ProcessingSuspender.cs:40, /C:/Users/immal/source/repos/Tycho/Tycho/Processor/JobProcessor.cs:156
     If an entry arrives after an empty queue read but before suspension is installed, TryResume() sees no active pause and drops the notification. The
     processor then sleeps despite queued work—potentially for the default five-minute maximum interval. Use a notification mechanism that retains a pending
     signal until consumed.

  7. Medium — Event-routing cycles recurse without a guard.
     /C:/Users/immal/source/repos/Tycho/Tycho/Events/Registrating/Registrations/RelayEventRegistration.cs:22
     An app forwarding event E to a child that exposes E back to its parent creates synchronous recursive routing. There is no visited-route check or depth
     limit, so publication eventually overflows the stack. Mapped routes can form equivalent cycles. Validate routing relationships or track visited module/
     event pairs during routing and report the cycle.

  8. DONE — Identity changes when the defining assembly version changes.
     /C:/Users/immal/source/repos/Tycho/Tycho/Identity/TypeIdentifier.cs:45
     Event, handler, and module identities hash AssemblyQualifiedName, including assembly version. Changing that version changes identities even when the
     event contract is unchanged. Previously serialized events can then fail deserialization or destination lookup. If compatibility across deployments is
     intended, introduce stable contract identifiers and migration aliases.

  Other worthwhile improvements:

  - Define in-memory delivery guarantees. /C:/Users/immal/source/repos/Tycho/Tycho/Events/Inbox/InMemory/InMemoryInbox.cs:32 and its outbox counterpart
    remove entries before success; failure acknowledgments do nothing. Failed or canceled processing loses messages. Document this behavior explicitly or
    implement bounded retries. Also serialize the complete outbox batch before enqueueing: currently a later serialization failure leaves earlier entries
    published.

  - Make routing state immutable. /C:/Users/immal/source/repos/Tycho/Tycho/Events/Delivery/Strategies/FinalRouteDelivery.cs:27 and the other strategies pop
    steps before delivery succeeds. Retrying the same event object starts from an altered route. The serializer also shares the original Route reference.

  - Preserve setup exceptions. /C:/Users/immal/source/repos/Tycho/Tycho/Utils/GeneratedSetupExtensions.cs:21 replaces every failure with an attribute-
    related NotImplementedException, discarding the underlying cause. Preserve the inner exception and distinguish missing generated setup from execution
    failures.

  - Remove the hard-coded analyzer output path. /C:/Users/immal/source/repos/Tycho/Tycho/Tycho.csproj:56 assumes a particular bin layout and target
    framework. Resolve the referenced project’s actual output when packaging so custom output directories remain supported.
