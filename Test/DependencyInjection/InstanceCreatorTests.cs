using System;
using Tycho.DependencyInjection;

namespace Test.DependencyInjection;

public class InstanceCreatorTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly IInstanceCreator _instanceCreator;

    public InstanceCreatorTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _instanceCreator = new InstanceCreator(_serviceProviderMock.Object);
    }

    [Fact]
    public void CreateInstance_ADependencyMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        _serviceProviderMock.Setup(provider => provider.GetService(typeof(ISomeInterface)))
                            .Returns(new SomeClass());
        _serviceProviderMock.Setup(provider => provider.GetService(typeof(ISomeGenericInterface<string>)))
                            .Returns(new SomeGenericClass<string>());

        // Act
        TestClass? result = null;
        Assert.Throws<InvalidOperationException>(() => { result = _instanceCreator.CreateInstance<TestClass>(); });

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void CreateInstance_AllDependenciesAvailable_ReturnsANewInstance()
    {
        // Arrange
        _serviceProviderMock.Setup(provider => provider.GetService(typeof(ISomeInterface)))
                            .Returns(new SomeClass());
        _serviceProviderMock.Setup(provider => provider.GetService(typeof(ISomeGenericInterface<string>)))
                            .Returns(new SomeGenericClass<string>());
        _serviceProviderMock.Setup(provider => provider.GetService(typeof(SomeOtherClass)))
                            .Returns(new SomeOtherClass());

        // Act
        var result = _instanceCreator.CreateInstance<TestClass>();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.SomeInterface);
        Assert.NotNull(result.SomeGenericInterface);
        Assert.NotNull(result.SomeOtherClass);
    }

    public interface ISomeInterface { }
    public class SomeClass : ISomeInterface { }

    public interface ISomeGenericInterface<T> { }
    public class SomeGenericClass<T> : ISomeGenericInterface<T> { }

    public class SomeOtherClass { }

    public class TestClass
    {
        public ISomeInterface SomeInterface { get; }
        public ISomeGenericInterface<string> SomeGenericInterface { get; }
        public SomeOtherClass SomeOtherClass { get; }

        public TestClass(
            ISomeInterface someInterface,
            ISomeGenericInterface<string> someGenericInterface,
            SomeOtherClass someOtherClass)
        {
            SomeInterface = someInterface;
            SomeGenericInterface = someGenericInterface;
            SomeOtherClass = someOtherClass;
        }
    }
}
