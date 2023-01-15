using System;
using Tycho.Contract;

namespace Tycho.Structure
{
    public interface ISubstructureDefinition
    {
        ISubstructureDefinition AddSubmodule<Module>(Action<IOutboxConsumer> contractFullfilment)
            where Module : TychoModule, new();
    }
}
