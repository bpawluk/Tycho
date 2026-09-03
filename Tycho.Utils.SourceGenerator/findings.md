# Source generator findings

## Open findings

### 2B. High — The reference renderer loses type identity

Templates rely on using directives instead of qualifying namespaces, so two referenced types with the same path in different namespaces can become ambiguous.

### 3. High — Method signature matching does not match method signatures

`MethodSignatureModel.Matches` compares:

- method name,
- parameter count,
- return type,

but never compares parameter types.

This can classify unrelated overloads as Tycho definition methods. Since several pipelines subsequently use `Single(...)`, adding an unrelated overload such as another one-parameter `DefineContract` can crash the generator.

Additionally, `TypeReferenceModel.Matches` ignores containing types, making nested types with otherwise identical names match incorrectly.

## Resolved findings

### 4. Resolved — Invalid or incomplete definitions can crash pipelines

Attributed definitions are now validated once before entering any generation pipeline. Unsupported and abstract types, and definitions without concrete implementations of all three required Tycho methods, are silently skipped without affecting valid definitions in the same compilation.

Required methods are resolved by their Roslyn override chains, so unrelated same-named overloads are ignored and concrete definitions can inherit implementations from intermediate base classes. Pipelines consume the validated methods directly, and malformed request, event, or submodule invocations are ignored instead of being projected through throwing `Single(...)` or nullable dereferences.

Covered by the invalid-definition and inherited-definition integration tests for apps and modules.

### 1. Resolved — Nested applications generated uncompilable builder and extension references

Generated builder types are now declared inside the application's containing types. `GeneratedTypeModel` constructs generated type references while preserving namespace, containment, and generic arguments, using `TypeDefinitionModel.GetReference` when starting from a definition.

Extension methods use the completed builder and facade references. Applications with the same simple name under different containing types no longer produce invalid references or conflicting members.

Covered by the `AppsWithSameNestedName`, `AppInGlobalNamespaceAndOuterTypes`, `AppInNamespaceAndOuterTypes`, and `AppInGenericOuterTypes` integration tests, which also validate the generated compilation.

### 2A. High — The reference renderer loses type identity

`TypeReferenceModel.BuildTypeSuffix` renders generic arguments with `ReferenceName`, which omits containing types. `TypeParameterConstraintModel.TypeConstraint` does the same for type constraints.

### 6. Resolved — Name-only members were modeled as method signatures

Reference members consumed only as method names now expose `...MethodName` string constants. Full `MethodSignatureModel` instances remain only where the generator performs semantic method matching.

The inaccurate unused `GetRequiredService` and `ConfigureAwait` signatures were removed instead of being preserved as misleading semantic descriptions.

### 7. Resolved — Submodule facade references discarded containment

`AppSetupTM.SubmoduleTM` and `ModuleSetupTM.SubmoduleTM` now construct facade references from the complete submodule `TypeReferenceModel` through `GeneratedTypeModel`. Nested containment and generic arguments are retained.

Covered by the nested generic submodules in the `AppWithSubmodules` and `ModuleWithSubmodules` integration tests.

### 8. Resolved — Generated output order is not guaranteed

`MethodInvokationsExtractor` accumulates invocations in a `HashSet` and emits its enumeration directly. Output ordering can vary by runtime or hashing behavior, causing unstable generated files and unnecessary incremental rebuilds.

Deduplication should be followed by an explicit stable ordering based on full type identity.

### 9. Resolved — Stale architecture remained after the Hosting refactor

The unused `EventDispatcherModel`, obsolete reference classes and members, and unused facade `ValueTask` and `ConfigureAwait` template-model members were removed. Reference models that became unused after replacing name-only signatures were removed as well.

## Ignored findings

### 5. Ignored — AppExtensions performs ad hoc generic parameter composition

Generated builder declarations and references now use `GeneratedTypeModel`, and the builder templates consume completed names instead of appending generic type syntax themselves.

`AppExtensionsTM` still manually flattens the containing types' and application's type parameters for its generic extension methods. Parameter names are deduplicated independently from constraints, which can produce an invalid or semantically incorrect method when nested types reuse a type-parameter name.

Generic method declaration construction should be represented by a model that keeps each parameter associated with its constraints and defines how shadowed type parameters are handled.
