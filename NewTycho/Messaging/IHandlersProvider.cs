using System.Collections.Generic;

namespace NewTycho.Messaging
{
    public interface IHandlersProvider
    {
        IReadOnlyList<string> IdentifyHandlers();
        IReadOnlyList<IHandle> GetHandlers();
    }
}
