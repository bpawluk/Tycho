namespace Tycho.DependencyInjection
{
    internal interface IInstanceCreator
    {
        T CreateInstance<T>(params object[] parameters) where T : class;
    }
}
