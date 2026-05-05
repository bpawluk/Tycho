Map all possible entry points for APPs and MODUles:
- Incoming REQUEST - BROKERS (may need to be split to internal and external broker)
- Incoming EVENT routing and delivery - BROKERS (may need to be split to internal and external broker)

Create DI Scope at each of these entry points and ensure that all dependencies are resolved within that scope.

Remove service resolution using Internals from the scoped paths.
- EventBroker (internal must be provided scope, external can create its own scope)