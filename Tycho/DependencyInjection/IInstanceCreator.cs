namespace Tycho.DependencyInjection
{
    public interface IInstanceCreator
    {
        T CreateInstance<T>(params object[] parameters) where T : class;
    }
}
