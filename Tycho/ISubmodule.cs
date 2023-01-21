namespace Tycho
{
    /// <summary>
    /// An interface that represents an instance of a particular submodule <para/>
    /// Use it in your module's code to inject the submodule's instance into it
    /// </summary>
    /// <typeparam name="Definition">A type that defines the submodule</typeparam>
    public interface ISubmodule<Definition> : IModule 
        where Definition : TychoModule { }
}
