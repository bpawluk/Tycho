namespace Tycho.DependencyInjection;

public interface IInstanceCreator
{
    T CreateInstance<T>() where T : class;
}
