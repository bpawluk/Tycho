using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;
using Tycho.UnitTests._Data.Requests;
using TychoV2.Modules;
using TychoV2.Requests.Handling;
using TychoV2.Requests.Registrating;
using TychoV2.Requests.Registrating.Registrations;
using TychoV2.Structure;

namespace Tycho.UnitTests.Requests.Registrating;

public class DownStreamRegistratorTests
{
    private readonly Registrator _sut;
    private readonly Internals _internals;

    public DownStreamRegistratorTests()
    {
        _internals = new Internals(string.Empty);
        _sut = new Registrator(_internals);
    }

    [Fact]
    public void Expose_NewRequest_RegistersExposer()
    {
        var targetModuleMock = new Mock<IParent>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ExposeDownStreamRequest<OtherModule, TestRequest>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestExposer<TestRequest>>(registration.Handler);
    }

    [Fact]
    public void Expose_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ExposeDownStreamRequest<OtherModule, TestRequest>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Expose_NewRequestWithResponse_RegistersExposer()
    {
        var targetModuleMock = new Mock<IParent>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ExposeDownStreamRequest<OtherModule, TestRequestWithResponse, string>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestExposer<TestRequestWithResponse, string>>(registration.Handler);
    }

    [Fact]
    public void Expose_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ExposeDownStreamRequest<OtherModule, TestRequestWithResponse, string>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Forward_NewRequest_RegistersForwarder()
    {
        // Arrange
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardDownStreamRequest<OtherModule, TestRequest, TestModule>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestForwarder<TestRequest, TestModule>>(registration.Handler);
    }

    [Fact]
    public void Forward_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardDownStreamRequest<OtherModule, TestRequest, TestModule>();

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
        _sut.ForwardDownStreamRequest<OtherModule, TestRequestWithResponse, string, TestModule>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestForwarder<TestRequestWithResponse, string, TestModule>>(registration.Handler);
    }

    [Fact]
    public void Forward_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardDownStreamRequest<OtherModule, TestRequestWithResponse, string, TestModule>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Handle_NewRequest_RegistersHandler()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut.HandleDownStreamRequest<OtherModule, TestRequest, TestRequestHandler>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<TestRequestHandler>(registration.Handler);
    }

    [Fact]
    public void Handle_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.HandleDownStreamRequest<OtherModule, TestRequest, TestRequestHandler>();

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
        _sut.HandleDownStreamRequest<OtherModule, TestRequestWithResponse, string, TestRequestHandler>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<TestRequestHandler>(registration.Handler);
    }

    [Fact]
    public void Handle_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.HandleDownStreamRequest<OtherModule, TestRequestWithResponse, string, TestRequestHandler>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Ignore_NewRequest_RegistersIgnorer()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut.IgnoreDownStreamRequest<OtherModule, TestRequest>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestIgnorer<TestRequest>>(registration.Handler);
    }

    [Fact]
    public void Ignore_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.IgnoreDownStreamRequest<OtherModule, TestRequest>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Ignore_NewRequestWithResponse_RegistersIgnorer()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut.IgnoreDownStreamRequest<OtherModule, TestRequestWithResponse, string>();
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestIgnorer<TestRequestWithResponse, string>>(registration.Handler);
    }

    [Fact]
    public void Ignore_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.IgnoreDownStreamRequest<OtherModule, TestRequestWithResponse, string>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}
