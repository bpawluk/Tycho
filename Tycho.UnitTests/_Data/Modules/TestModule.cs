﻿using Microsoft.Extensions.DependencyInjection;
using Tycho.Modules;

namespace Tycho.UnitTests._Data.Modules;

public class TestModule : TychoModule
{
    protected override void DefineContract(IModuleContract module) { }

    protected override void DefineEvents(IModuleEvents module) { }

    protected override void IncludeModules(IModuleStructure module) { }

    protected override void RegisterServices(IServiceCollection module) { }
}