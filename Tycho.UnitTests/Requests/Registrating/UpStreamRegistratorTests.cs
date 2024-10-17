using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Modules;
using Tycho.Requests.Handling;
using Tycho.Requests.Registrating;
using Tycho.Requests.Registrating.Registrations;
using Tycho.Structure;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Requests.Registrating;

public class UpStreamRegistratorTests
{
    private readonly Internals _internals;
    private readonly Registrator _sut;

    public UpStreamRegistratorTests()
    {
        _internals = new Internals(typeof(object));
        _sut = new Registrator(_internals);
    }

    [Fact]
    public void Forward_NewRequest_RegistersForwarder()
    {
        // Arrange
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardUpStreamRequest<TestRequest, TestModule>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IUpStreamHandlerRegistration<TestRequest>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestForwarder<TestRequest, TestModule>>(registration.Handler);
    }

    [Fact]
    public void Forward_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IUpStreamHandlerRegistration<TestRequest>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardUpStreamRequest<TestRequest, TestModule>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Forward_NewRequestWithResponse_RegistersForwarder()
    {
        // Arrange
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardUpStreamRequest<TestRequestWithResponse, string, TestModule>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IUpStreamHandlerRegistration<TestRequestWithResponse, string>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestForwarder<TestRequestWithResponse, string, TestModule>>(registration.Handler);
    }

    [Fact]
    public void Forward_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IUpStreamHandlerRegistration<TestRequestWithResponse, string>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardUpStreamRequest<TestRequestWithResponse, string, TestModule>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Handle_NewRequest_RegistersHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut.HandleUpStreamRequest<TestRequest, TestRequestHandler>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IUpStreamHandlerRegistration<TestRequest>>();
        Assert.NotNull(registration);
        Assert.IsType<TestRequestHandler>(registration.Handler);
    }

    [Fact]
    public void Handle_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IUpStreamHandlerRegistration<TestRequest>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.HandleUpStreamRequest<TestRequest, TestRequestHandler>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Handle_NewRequestWithResponse_RegistersHandler()
    {
        // Arrange
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.HandleUpStreamRequest<TestRequestWithResponse, string, TestRequestHandler>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IUpStreamHandlerRegistration<TestRequestWithResponse, string>>();
        Assert.NotNull(registration);
        Assert.IsType<TestRequestHandler>(registration.Handler);
    }

    [Fact]
    public void Handle_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IUpStreamHandlerRegistration<TestRequestWithResponse, string>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.HandleUpStreamRequest<TestRequestWithResponse, string, TestRequestHandler>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}