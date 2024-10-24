using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.Requests.Handling;
using Tycho.Requests.Registrating;
using Tycho.Requests.Registrating.Registrations;
using Tycho.Structure;
using Tycho.UnitTests._Data.Handlers;
using Tycho.UnitTests._Data.Modules;
using Tycho.UnitTests._Data.Requests;

namespace Tycho.UnitTests.Requests.Registrating;

public class DownStreamRegistratorTests
{
    private readonly Internals _internals;
    private readonly Registrator _sut;

    public DownStreamRegistratorTests()
    {
        _internals = new Internals(typeof(object));
        _internals.GetServiceCollection()
                  .AddSingleton(_internals);
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
        var registration = _internals.GetService<
            IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
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
    public void Expose_NewMappedRequest_RegistersExposer()
    {
        var mapMock = new Mock<Func<TestRequest, OtherRequest>>();
        var targetModuleMock = new Mock<IParent>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ExposeMappedDownStreamRequest<OtherModule, TestRequest, OtherRequest>(mapMock.Object);
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<MappedRequestExposer<TestRequest, OtherRequest>>(registration.Handler);
    }

    [Fact]
    public void Expose_ExistingMappedRequest_ThrowsArgumentException()
    {
        // Arrange
        var mapMock = new Mock<Func<TestRequest, OtherRequest>>();
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ExposeMappedDownStreamRequest<OtherModule, TestRequest, OtherRequest>(mapMock.Object);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Expose_NewMappedRequestWithResponse_RegistersExposer()
    {
        var mapRequestMock = new Mock<Func<TestRequestWithResponse, OtherRequestWithResponse>>();
        var mapResponseMock = new Mock<Func<string, string>>();
        var targetModuleMock = new Mock<IParent>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ExposeMappedDownStreamRequest<
            OtherModule, 
            TestRequestWithResponse, string,
            OtherRequestWithResponse, string>(
                mapRequestMock.Object, mapResponseMock.Object);
        _internals.Build();

        // Assert
        var registration = _internals.GetService<
            IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<MappedRequestExposer<
            TestRequestWithResponse, string,
            OtherRequestWithResponse, string>>(
                registration.Handler);
    }

    [Fact]
    public void Expose_ExistingMappedRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var mapRequestMock = new Mock<Func<TestRequestWithResponse, OtherRequestWithResponse>>();
        var mapResponseMock = new Mock<Func<string, string>>();
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ExposeMappedDownStreamRequest<
            OtherModule,
            TestRequestWithResponse, string,
            OtherRequestWithResponse, string>(
                mapRequestMock.Object, mapResponseMock.Object);

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
        var registration = _internals
            .GetService<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
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
    public void Forward_NewMappedRequest_RegistersForwarder()
    {
        // Arrange
        var mapMock = new Mock<Func<TestRequest, OtherRequest>>();
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardMappedDownStreamRequest<OtherModule, TestRequest, OtherRequest, TestModule>(mapMock.Object);
        _internals.Build();

        // Assert
        var registration = _internals.GetService<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<MappedRequestForwarder<TestRequest, OtherRequest, TestModule>>(registration.Handler);
    }

    [Fact]
    public void Forward_ExistingMappedRequest_ThrowsArgumentException()
    {
        // Arrange
        var mapMock = new Mock<Func<TestRequest, OtherRequest>>();
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardMappedDownStreamRequest<
            OtherModule, TestRequest, OtherRequest, TestModule>(
                mapMock.Object);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Forward_NewMappedRequestWithResponse_RegistersForwarder()
    {
        // Arrange
        var mapRequestMock = new Mock<Func<TestRequestWithResponse, OtherRequestWithResponse>>();
        var mapResponseMock = new Mock<Func<string, string>>();
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetServiceCollection().AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardMappedDownStreamRequest<
            OtherModule,
            TestRequestWithResponse, string,
            OtherRequestWithResponse, string,
            TestModule>(
                mapRequestMock.Object, mapResponseMock.Object);
        _internals.Build();

        // Assert
        var registration = _internals
            .GetService<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<MappedRequestForwarder<
            TestRequestWithResponse, string, 
            OtherRequestWithResponse, string,
            TestModule>>(
                registration.Handler);
    }

    [Fact]
    public void Forward_ExistingMappedRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var mapRequestMock = new Mock<Func<TestRequestWithResponse, OtherRequestWithResponse>>();
        var mapResponseMock = new Mock<Func<string, string>>();
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardMappedDownStreamRequest<
            OtherModule, 
            TestRequestWithResponse, string, 
            OtherRequestWithResponse, string,
            TestModule>(
                mapRequestMock.Object, mapResponseMock.Object);

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
        Assert.IsType<ScopedRequestHandler<TestRequest, TestRequestHandler>>(registration.Handler);
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
        var registration = _internals.GetService<
            IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<ScopedRequestHandler<TestRequestWithResponse, string, TestRequestHandler>>(registration.Handler);
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
        var registration = _internals.GetService<
            IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
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