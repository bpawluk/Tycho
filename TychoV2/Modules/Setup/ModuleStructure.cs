using Microsoft.Extensions.Configuration;
using System;
using TychoV2.Structure;

namespace TychoV2.Modules.Setup
{
    internal class ModuleStructure : IModuleStructure
    {
        private readonly Internals _internals;

        public ModuleStructure(Internals internals)
        {
            _internals = internals;
        }

        public IModuleStructure AddSubmodule<Module>(
            Action<IContractFulfillment>? contractFulfillment = null, 
            Action<IConfigurationBuilder>? configurationDefinition = null) 
            where Module : TychoModule, new()
        {
            throw new NotImplementedException();
        }

        public void Build()
        {
        }
    }
}
