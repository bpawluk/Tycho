using NewTycho.Modules;
using NewTycho.Requests;

namespace NewTycho
{
    public interface IModule : IExecute
    {
    }

    public interface IModule<TModule> : IModule
        where TModule : TychoModule
    {
    }
}
