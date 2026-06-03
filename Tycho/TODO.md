# Simplify IDE Navigation

## TODO

All Tycho Definitions should be easily navigable in the IDE.

This includes:
- Apps Definitions 
  - Move IPublisher out of <AppName> partial class and rename from IPublisher to I<AppName>Publisher
  - Move Setup from <AppName> to a new dedicated partial class AppSetup
- Modules Definitions
  - Move IParent out of <ModuleName> partial class and rename from IParent to I<ModuleName>Parent
  - Move IPublisher out of <ModuleName> partial class and rename from IPublisher to I<ModuleName>Publisher
  - Move Setup from <ModuleName> to a new dedicated partial class ModuleSetup
- Requests Definitions - already easily navigable - no changes needed
- Request Handlers Definitions - already easily navigable - no changes needed
- Events Definitions - already easily navigable - no changes needed
- Event Handlers Definitions - already easily navigable - no changes needed

## Tasks
- [x] Move IPublisher out of <AppName> partial class and rename from IPublisher to I<AppName>Publisher
- [x] Move IPublisher out of <ModuleName> partial class and rename from IPublisher to I<ModuleName>Publisher
- [x] Move IParent out of <ModuleName> partial class and rename from IParent to I<ModuleName>Parent
- [x] Move Setup from <ModuleName> to a new dedicated partial class ModuleSetup
- [ ] Move Setup from <AppName> to a new dedicated partial class AppSetup
