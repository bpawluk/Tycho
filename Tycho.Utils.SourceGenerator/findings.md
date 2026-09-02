## 1. High — Nested applications generate uncompilable builder and extension types

Tycho.Utils.SourceGenerator/TemplateModels/AppBuilderTM.cs:52 and Tycho.Utils.SourceGenerator/TemplateModels/AppExtensionsTM.cs:61 construct facade names from only the application’s simple name and own generic parameters.

Meanwhile, facade types remain nested inside the application’s containing types. For an application such as Outer.Inner.TestApp, the builder refers to ITestApp and TestAppFacade, but the actual types are Outer.Inner.ITestApp and Outer.Inner.TestAppFacade.

Consequences:

- Every nested application’s new builder/Hosting extensions are invalid.
- Generic containing-type arguments are omitted.
- Two nested applications with the same simple name in one namespace produce colliding namespace-level builder and extension class declarations.

This is the clearest example of generated types bypassing the type-reference model.

## 2. High — The reference renderer loses type identity

Tycho.Utils.SourceGenerator/Models/System/TypeReferenceModel.cs:81 renders generic arguments with ReferenceName, which omits containing types. Tycho.Utils.SourceGenerator/Models/System/TypeParameterConstraintModel.cs:22 does the same for type constraints.

Therefore types such as:

Container.NestedType
Dictionary<string, Container.NestedType>
where T : Container.NestedConstraint

can be rendered as unqualified NestedType or NestedConstraint. Importing their namespace does not make nested types directly visible.

UseType also renders simple names plus using directives, so two referenced types with the same name in different namespaces become ambiguous.

## 3. High — Method signature matching does not match method signatures

Tycho.Utils.SourceGenerator/Models/System/MethodSignatureModel.cs:24 compares:

- method name,
- parameter count,
- return type,

but never compares parameter types.

This can classify unrelated overloads as Tycho definition methods. Since several pipelines subsequently use Single(...), adding an unrelated overload such as another one-parameter DefineContract can crash the generator.

Additionally, Tycho.Utils.SourceGenerator/Models/System/TypeReferenceModel.cs:45 ignores containing types, making nested types with otherwise identical names match incorrectly.

## 4. High — Invalid or incomplete definitions can crash pipelines

The builder, extensions, and parent pipelines filter their supported definition kinds before transforming. Facade, publisher, event-serializer, and setup pipelines do not.

For example, Tycho.Utils.SourceGenerator/Pipelines/TychoFacadePipeline.cs:54 executes Single(...) before the later Unknown guard. Tycho.Utils.SourceGenerator/Pipelines/TychoSetupPipeline.cs:47 uses FirstOrDefault and subsequently dereferences the default method model.

An unrelated class decorated with [TychoDefinition], an abstract intermediate definition, or temporarily incomplete code can therefore produce a generator exception instead of either no output or a controlled diagnostic.

## 5. Medium — AppBuilder still performs ad hoc generic type composition

Tycho.Utils.SourceGenerator/Templates/AppBuilder.sbncs:20 and Tycho.Utils.SourceGenerator/Templates/AppExtensions.sbncs:23 append type_parameters_suffix directly to app_builder_class.

That differs from established template models, which expose separate declaration and constructed names such as FacadeClass and FacadeClassWithTypeParams.

AppBuilderSymbols.GetAppBuilderClass should own the generated identifier construction, while a generated-type reference model should own its generic arguments and containment. The templates should consume completed names rather than assemble C# type syntax.

Flattening containing and application type parameters also mishandles duplicate parameter names: parameter names are deduplicated independently from constraints, potentially producing one parameter with multiple duplicate where clauses.

## 6. Medium — Several MethodSignatureModel references are inaccurate

Examples:

- Tycho.Utils.SourceGenerator/References/Microsoft/ServiceProviderServiceExtensionsReference.cs:13 models GetRequiredService as accepting and returning IServiceCollection.
- Tycho.Utils.SourceGenerator/References/System/TaskReference.cs:11 gives Task.ConfigureAwait a ConfiguredValueTaskAwaitable result.
- AddSingletonMethodSignature models the one-argument overload, but AppExtensions uses the factory overload.

These currently work only because callers extract MethodName and never depend on the modeled signature. The reference layer is therefore conflating two responsibilities: exact semantic signatures and simple member-name constants.

## 7. Medium — Submodule facade references also discard containment

Tycho.Utils.SourceGenerator/TemplateModels/AppSetupTM.cs:100 and Tycho.Utils.SourceGenerator/TemplateModels/ModuleSetupTM.cs:104 call UseType(moduleType) only to import a namespace, then rebuild facade names from moduleType.Name.

Nested submodules or nested types used as their generic arguments will not be referenced correctly unless they happen to be visible from the setup type’s containing scope.

## 8. Medium — Generated output order is not guaranteed

Tycho.Utils.SourceGenerator/Extractors/MethodInvokationsExtractor.cs:19 accumulates invocations in a HashSet and emits its enumeration directly. Output ordering can vary by runtime or hashing behavior, causing unstable generated files and unnecessary incremental rebuilds.

Deduplication should be followed by an explicit stable ordering based on full type identity.

## 9. Low — Stale architecture remains after the Hosting refactor

Unused or obsolete elements include:

- EventDispatcherModel
- ServiceProviderServiceExtensionsReference
- ConfiguredTaskAwaitableReference
- FuncReference
- GuidReference
- IHostApplicationBuilderReference.ConfigurationPropertyName
- unused ValueTaskClass and ConfigureAwaitMethod members in both facade template models

These obscure which reference models are authoritative.
