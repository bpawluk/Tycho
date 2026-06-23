# Contract and Events API Analysis

This note summarizes how the current app/module contract and event-definition APIs work across the Tycho solution. It is intended as a change guide for future redesign of:

- `Tycho/Apps/IAppContract.cs`
- `Tycho/Modules/IModuleContract.cs`
- `Tycho/Apps/IAppEvents.cs`
- `Tycho/Modules/IModuleEvents.cs`

## Solution Shape

The solution contains three product projects:

- `Tycho`: the runtime framework. It defines app/module base classes, request/event abstractions, DI setup, brokers, routing, in-memory inbox/outbox, generated setup reflection, and the public DSL interfaces.
- `Tycho.Utils.SourceGenerator`: the incremental Roslyn source generator. It reads `[TychoDefinition]` app/module classes and emits facade, publisher, event serializer, setup, parent, and app extension code.
- `Tycho.Persistence.EFCore`: optional persistence for Tycho inbox/outbox and transaction services. It plugs into Tycho via service registration and does not define the DSL, but many use-case tests exercise the DSL with EF-backed persistence.

The test projects are:

- `Tests/Tycho.UnitTests`: runtime unit tests for app/module setup, request/event brokers, routing, serialization, and lifecycle behavior.
- `Tests/Tycho.IntegrationTests`: generated-code/runtime integration tests for request and event flow through app and module hierarchies.
- `Tests/Tycho.Utils.SourceGenerator.IntegrationTests`: direct source-generator snapshot tests using `Verify.SourceGenerators`.
- `Tests/Tycho.Persistence.EFCore.UnitTests`: EF persistence unit tests.
- `Tests/Tycho.Persistence.EFCore.UseCaseTests`: larger app/module examples using generated Tycho APIs and EF-backed inbox/outbox/transactions.

## Public DSL Surface

### App contract

`IAppContract` is used in `TychoApp.DefineContract(IAppContract app)`. It declares requests that the app facade accepts:

- `Forwards<TRequest, TModule>()`
- `Forwards<TRequest, TResponse, TModule>()`
- `ForwardsAs<TRequest, TTargetRequest, TModule>(Func<TRequest, TTargetRequest>)`
- `ForwardsAs<TRequest, TResponse, TTargetRequest, TTargetResponse, TModule>(Func<TRequest, TTargetRequest>, Func<TTargetResponse, TResponse>)`
- `Handles<TRequest, THandler>()`
- `Handles<TRequest, TResponse, THandler>()`

At runtime, `AppContract` maps these to upstream request registrations through `Tycho.Requests.Registrating.Registrator`.

### Module contract

`IModuleContract` is used in `TychoModule.DefineContract(IModuleContract module)`. It includes the same downstream-facing declarations as `IAppContract` plus parent requirements:

- `Forwards...`
- `ForwardsAs...`
- `Handles...`
- `Requires<TRequest>()`
- `Requires<TRequest, TResponse>()`

At runtime, `ModuleContract` maps `Forwards`, `ForwardsAs`, and `Handles` to upstream registrations in the module. `Requires` does not register a handler. It validates that the parent-provided downstream broker can execute the required request. The source generator also uses `Requires` to generate a typed parent interface/facade for the module.

### App events

`IAppEvents` is used in `TychoApp.DefineEvents(IAppEvents app)`. It declares:

- `Handles<TEvent, THandler>()`
- `Routes<TEvent>()`

At runtime, `AppEvents.Handles` registers a final event handler. `AppEvents.Routes` returns `Tycho.Apps.IEventRouting<TEvent>`, whose app-level implementation supports forwarding to child modules:

- `Forwards<TModule>()`
- `ForwardsAs<TTargetEvent, TModule>(Func<TEvent, TTargetEvent>)`

### Module events

`IModuleEvents` is used in `TychoModule.DefineEvents(IModuleEvents module)`. It declares the same top-level methods:

- `Handles<TEvent, THandler>()`
- `Routes<TEvent>()`

At runtime, `ModuleEvents.Handles` registers a final event handler. `ModuleEvents.Routes` returns `Tycho.Modules.IEventRouting<TEvent>`, whose module-level implementation supports both upward and downward routing:

- `Exposes()`
- `ExposesAs<TOtherEvent>(Func<TEvent, TOtherEvent>)`
- `Forwards<TModule>()`
- `ForwardsAs<TOtherEvent, TModule>(Func<TEvent, TOtherEvent>)`

## Runtime Lifecycle

Apps and modules are configured in a similar sequence.

For apps, `TychoApp.RunBaseAsync()`:

1. Prevents running the same app instance twice.
2. Initializes `AppBuilder`.
3. Calls `this.AddGeneratedSetup(_builder.Services)`.
4. Calls user `RegisterServices(IServiceCollection)`.
5. Calls user `DefineContract(IAppContract)`.
6. Calls user `DefineEvents(IAppEvents)`.
7. Calls user `IncludeModules(IAppStructure)`.
8. Builds the app internals and runs `Startup(IServiceProvider)`.

For modules, `TychoModule.RunAsync()` follows the same sequence, but the parent must first provide:

- a contract-fulfilling `IRequestBroker` via `FulfillContract`
- a parent `IEventBroker` via `PassEventBroker`
- optional globals/settings via `WithGlobals` and `WithSettings`

The order matters. Generated setup runs before user service registration, and the contract/event definitions run before `BuildAsync()` finalizes `Internals`.

## Runtime Request Flow

Request declarations become DI registrations used by request brokers:

- App and module `Forwards` register upstream request handlers implemented by `RequestForwarder`.
- App and module `ForwardsAs` register upstream mapped request handlers implemented by `MappedRequestForwarder`.
- App and module `Handles` register upstream request handlers directly.
- Parent `IContractFulfillment` actions in `Uses<TModule>(...)` register downstream behavior for a child module requirement.

There are two request directions:

- Upstream: a caller executes against the current app/module facade. The current owner handles the request directly or forwards it into one of its child modules.
- Downstream: a child module asks its parent to fulfill a `Requires` request. The parent can expose, forward, map, handle, or ignore that request depending on the `IContractFulfillment` implementation available at that hierarchy level.

The app-level `IContractFulfillment` has `Forward`, `ForwardAs`, `Handle`, and `Ignore`. The module-level `IContractFulfillment` additionally has `Expose` and `ExposeAs`, because a module can pass a requirement further upward to its own parent.

Transactional request handlers are detected by the brokers via `ITransactionalRequestHandler`; those handlers are wrapped in the configured `ITransaction`.

## Runtime Event Flow

Event declarations become event registrations used by `IEventBroker`:

- `Handles<TEvent, THandler>` registers a final event registration.
- App route `Forwards<TModule>` pushes a downstream route step to a child module.
- App route `ForwardsAs<TTargetEvent, TModule>` maps the event and pushes a downstream route step.
- Module route `Forwards`/`ForwardsAs` works like the app version for child modules.
- Module route `Exposes`/`ExposesAs` routes an event upward through the parent reference.

`AppEvents.BuildAsync()` and `ModuleEvents.BuildAsync()` register event infrastructure:

- in-memory inbox/outbox defaults unless custom implementations are already registered
- `OutboxActivity`, `OutboxProcessor`
- `InboxActivity`, `InboxProcessor`
- `ITransaction`, defaulting to `EmptyTransaction`
- scoped `IEventBroker`
- transient `IEventPublisher`
- delivery strategies
- `IPayloadSerializer`

Modules register one extra delivery strategy: `UpStreamRouteDelivery`.

Event publishing uses `IEventPublisher`. It routes the payload through the scoped event broker, writes routed events to the outbox, and later processors deliver them to final inboxes or other modules. Transactional event handlers are detected via `ITransactionalEventHandler`.

## Source Generator Entry Point

The generator starts in `TychoSourceGenerator.Initialize`.

It finds class declarations with `[TychoDefinition]` using `ForAttributeWithMetadataName`, then classifies each type as:

- app: derives from `Tycho.Apps.TychoApp`
- module: derives from `Tycho.Modules.TychoModule`
- unknown: anything else

For each candidate, it extracts only relevant method definitions by name:

- `DefineContract`
- `DefineEvents`
- `IncludeModules`

Those names are taken from `TychoAppReference` and `TychoModuleReference`.

Important: the generator does not just inspect direct statements. `MethodInvokationsExtractor` recursively traverses invoked methods and local functions in the same compilation. This is why generator tests include helper methods, helper classes, static helpers, and extension methods under `Input/AppWithIndirectDefinitions`.

## Generator Shadow API

The generator has a shadow model of the public DSL in these files:

- `Tycho.Utils.SourceGenerator/References/Tycho/Apps/IAppContractReference.cs`
- `Tycho.Utils.SourceGenerator/References/Tycho/Modules/IModuleContractReference.cs`
- `Tycho.Utils.SourceGenerator/References/Tycho/Apps/IAppEventsReference.cs`
- `Tycho.Utils.SourceGenerator/References/Tycho/Modules/IModuleEventsReference.cs`
- `Tycho.Utils.SourceGenerator/References/Tycho/Apps/TychoAppReference.cs`
- `Tycho.Utils.SourceGenerator/References/Tycho/Modules/TychoModuleReference.cs`

These reference files encode exact method names, parameter type shapes, return types, and type parameter names used to identify requests, responses, events, and modules.

If the public DSL changes, update these reference files in lockstep. Otherwise:

- runtime code can compile but generated facades/publishers may omit members
- generator pipelines can fail on `Single(...)`
- snapshots can change unexpectedly
- indirect definitions can stop being recognized

## Generator Pipelines

### Facade pipeline

`TychoFacadePipeline` reads invocations from `DefineContract` whose signatures match downstream contract-defining methods:

- app contract `Forwards`, `ForwardsAs`, `Handles`
- module contract `Forwards`, `ForwardsAs`, `Handles`

It extracts `TRequest` and optional `TResponse`, removes duplicates, and emits:

- app facade interface and app facade implementation
- module facade interface and module facade implementation

The generated facade methods are named `ExecuteAsync` and call the generic base execution method.

### Parent pipeline

`TychoParentPipeline` runs only for modules. It reads `DefineContract` invocations whose signatures match upstream contract-defining methods:

- `Requires<TRequest>()`
- `Requires<TRequest, TResponse>()`

It emits:

- module parent interface
- module parent implementation

This generated parent surface is how module internals can call requests expected from the parent hierarchy.

### Publisher pipeline

`TychoPublisherPipeline` reads `DefineEvents` invocations matching handled or routed events:

- `Handles<TEvent, THandler>`
- `Routes<TEvent>`

It extracts `TEvent`, removes duplicates, and emits:

- publisher interface
- publisher implementation

The generated publisher methods are named `PublishAsync` and call the generic publisher base.

### Event serializer pipeline

`TychoEventSerializerPipeline` reads only handled events:

- `Handles<TEvent, THandler>`

It emits an event serializer class that registers those event payload types for deserialization. Routed-only events are publishable because `Serialize` works on the runtime payload, but only final handled events need generated deserialization registration for inbox processing.

### Setup pipeline

`TychoSetupPipeline` reads `IncludeModules` invocations and emits a setup class with a static `Setup(IServiceCollection)` method.

The generated setup registers:

- `IEventSerializer` to the generated event serializer
- generated publisher interface to generated publisher
- generated module parent interface to generated parent implementation for modules
- generated child module facade interfaces to generated child module facades

`GeneratedSetupExtensions` discovers the generated setup type by reflection using the runtime app/module type name plus `Setup`.

### App extensions pipeline

`TychoExtensionsPipeline` runs only for apps and emits public extension methods:

- `WithConfiguration(...)`
- `WithLogging(...)`
- `RunAsync(...)`
- `AddApp(...)`

The generated `RunAsync` returns the app facade interface, not the raw `IApp`.

## Template Impact

The templates under `Tycho.Utils.SourceGenerator/Templates` shape the generated public API. Contract/event DSL changes can require updates to:

- `AppFacade*.sbncs`
- `ModuleFacade*.sbncs`
- `ModuleParent*.sbncs`
- `AppPublisher*.sbncs`
- `ModulePublisher*.sbncs`
- `AppEventSerializer.sbncs`
- `ModuleEventSerializer.sbncs`
- `AppSetup.sbncs`
- `ModuleSetup.sbncs`
- `AppExtensions.sbncs`

Most method names exposed to users from generated code are not copied from the DSL. They come from template models and base-class references, such as `ExecuteAsync`, `PublishAsync`, `RunAsync`, and `Setup`.

## Tests and Usage

The API is used broadly in tests.

Source-generator integration tests:

- Cover app/module definitions in global namespace, namespaces, nested types, generic outer types, generic definitions, and constrained generics.
- Cover app downstream contract generation from `Forwards`, `ForwardsAs`, and `Handles`.
- Cover module downstream contract generation.
- Cover module upstream parent generation from `Requires`.
- Cover event publisher/serializer generation from `Handles` and `Routes`.
- Cover module setup from `IncludeModules`.
- Cover indirect definitions called through helper methods/classes/static classes/extensions.
- Verify generated output under `Tests/Tycho.Utils.SourceGenerator.IntegrationTests/Output`.

Runtime unit tests:

- Cover app/module lifecycle failure when no generated setup exists.
- Cover duplicate module registration.
- Cover event infrastructure default registration and custom inbox/outbox preservation.
- Cover request brokers, event brokers, route delivery, serialization, and in-memory inbox/outbox behavior.

Integration tests:

- Exercise generated app facade interfaces from `RunAsync`.
- Exercise request handling, request forwarding, mapped forwarding, horizontal and vertical module flow, generic requests/events, generic apps/modules, settings/configuration/logging, startup/cleanup, service registration, and host app setup.
- The app/module definition classes often use the four target interfaces directly in `DefineContract` and `DefineEvents`.

EF Core use-case tests:

- Model realistic apps such as Online Store, Blogging Website, Content Moderation, and Home Dashboard.
- Use generated app facades to send requests.
- Use generated publishers and event serializers indirectly through event handlers and processors.
- Use `AddTychoPersistence<TDbContext>()` to replace in-memory `IOutboxWriter`, `IOutboxConsumer`, `IInboxWriter`, `IInboxConsumer`, and `ITransaction`.

## Redesign Checklist

When changing the four target API files, check all of these areas:

1. Public interfaces in `Tycho/Apps` and `Tycho/Modules`.
2. Runtime implementations:
   - `Tycho/Apps/Setup/AppContract.cs`
   - `Tycho/Modules/Setup/ModuleContract.cs`
   - `Tycho/Apps/Setup/AppEvents.cs`
   - `Tycho/Modules/Setup/ModuleEvents.cs`
   - app/module event routing implementations if `Routes` or returned routing APIs change.
3. Request and event registrators if semantic behavior changes.
4. Source-generator reference models for exact names, parameter shapes, return types, and type parameter names.
5. `MethodSignatureModelExtensions` and `TypeArgumentExtensions` if the categorization model changes.
6. Generator pipelines if declarations are no longer simple generic method calls on the DSL object.
7. Template models and `.sbncs` templates if generated surface area changes.
8. Snapshot inputs and outputs in `Tests/Tycho.Utils.SourceGenerator.IntegrationTests`.
9. Integration and use-case SUT definitions that call the DSL directly.
10. XML docs and package-facing generated code expectations.

The highest-risk assumptions in the current design are:

- Method matching is signature based, but downstream extraction relies on type parameter names like `TRequest`, `TResponse`, and `TEvent`.
- `DefineContract` and `DefineEvents` are found by exact method names.
- `DefineContract`/`DefineEvents` are expected to exist exactly once for recognized apps/modules.
- Indirect method traversal only works for methods available in the same compilation syntax trees.
- Generated setup is discovered by reflection from type naming conventions.
- Generated event serializers only register handled events, not routed-only events.

