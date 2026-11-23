using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Modules.Instance;
using Tycho.Requests.Handling;
using Tycho.Requests.Registration;
using Tycho.Requests.Registration.Registrations;
using Tycho.Structure;
using Tycho.Structure.Parent;
using Tycho.TestUtils.Handlers;
using Tycho.TestUtils.Modules;
using Tycho.TestUtils.Requests;

namespace Tycho.UnitTests.Requests.Registration;

public class DownstreamRegistratorTests
{
    private readonly Internals _internals;
    private readonly Registrator _sut;

    public DownstreamRegistratorTests()
    {
        _internals = new AppInternals(Host.CreateEmptyApplicationBuilder(default), typeof(DownstreamRegistratorTests));
        _internals.GetHostBuilder().Services
                  .AddSingleton(_internals);
        _sut = new Registrator(_internals);
    }

    [Fact]
    public void Expose_NewRequest_RegistersExposer()
    {
        var targetModuleMock = new Mock<IParentReference>();
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ExposeDownstreamRequest<OtherModule, TestRequest>();
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequest, OtherModule>? registration = _internals.GetService<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestExposer<TestRequest>>(registration.Handler);
    }

    [Fact]
    public void Expose_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ExposeDownstreamRequest<OtherModule, TestRequest>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Expose_NewRequestWithResponse_RegistersExposer()
    {
        var targetModuleMock = new Mock<IParentReference>();
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ExposeDownstreamRequest<OtherModule, TestRequestWithResponse, string>();
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>? registration = _internals.GetService<
            IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestExposer<TestRequestWithResponse, string>>(registration.Handler);
    }

    [Fact]
    public void Expose_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ExposeDownstreamRequest<OtherModule, TestRequestWithResponse, string>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Expose_NewMappedRequest_RegistersExposer()
    {
        var mapMock = new Mock<Func<TestRequest, OtherRequest>>();
        var targetModuleMock = new Mock<IParentReference>();
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ExposeMappedDownstreamRequest<OtherModule, TestRequest, OtherRequest>(mapMock.Object);
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequest, OtherModule>? registration = _internals.GetService<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<MappedRequestExposer<TestRequest, OtherRequest>>(registration.Handler);
    }

    [Fact]
    public void Expose_ExistingMappedRequest_ThrowsArgumentException()
    {
        // Arrange
        var mapMock = new Mock<Func<TestRequest, OtherRequest>>();
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ExposeMappedDownstreamRequest<OtherModule, TestRequest, OtherRequest>(mapMock.Object);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Expose_NewMappedRequestWithResponse_RegistersExposer()
    {
        var mapRequestMock = new Mock<Func<TestRequestWithResponse, OtherRequestWithResponse>>();
        var mapResponseMock = new Mock<Func<string, string>>();
        var targetModuleMock = new Mock<IParentReference>();
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ExposeMappedDownstreamRequest<
            OtherModule,
            TestRequestWithResponse, string,
            OtherRequestWithResponse, string>(
                mapRequestMock.Object, mapResponseMock.Object);
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>? registration = _internals.GetService<
            IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
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
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ExposeMappedDownstreamRequest<
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
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardDownstreamRequest<OtherModule, TestRequest, TestModule>();
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequest, OtherModule>? registration = _internals.GetService<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestForwarder<TestRequest, TestModule>>(registration.Handler);
    }

    [Fact]
    public void Forward_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardDownstreamRequest<OtherModule, TestRequest, TestModule>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Forward_NewRequestWithResponse_RegistersForwarder()
    {
        // Arrange
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardDownstreamRequest<OtherModule, TestRequestWithResponse, string, TestModule>();
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>? registration = _internals
            .GetService<IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestForwarder<TestRequestWithResponse, string, TestModule>>(registration.Handler);
    }

    [Fact]
    public void Forward_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardDownstreamRequest<OtherModule, TestRequestWithResponse, string, TestModule>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Forward_NewMappedRequest_RegistersForwarder()
    {
        // Arrange
        var mapMock = new Mock<Func<TestRequest, OtherRequest>>();
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardMappedDownstreamRequest<OtherModule, TestRequest, OtherRequest, TestModule>(mapMock.Object);
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequest, OtherModule>? registration = _internals.GetService<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<MappedRequestForwarder<TestRequest, OtherRequest, TestModule>>(registration.Handler);
    }

    [Fact]
    public void Forward_ExistingMappedRequest_ThrowsArgumentException()
    {
        // Arrange
        var mapMock = new Mock<Func<TestRequest, OtherRequest>>();
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardMappedDownstreamRequest<
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
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardMappedDownstreamRequest<
            OtherModule,
            TestRequestWithResponse, string,
            OtherRequestWithResponse, string,
            TestModule>(
                mapRequestMock.Object, mapResponseMock.Object);
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>? registration = _internals
            .GetService<IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
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
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardMappedDownstreamRequest<
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
        _sut.HandleDownstreamRequest<OtherModule, TestRequest, TestRequestHandler>();
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequest, OtherModule>? registration = _internals.GetService<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<TestRequestHandler>(registration.Handler);
    }

    [Fact]
    public void Handle_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.HandleDownstreamRequest<OtherModule, TestRequest, TestRequestHandler>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Handle_NewRequestWithResponse_RegistersHandler()
    {
        // Arrange
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.HandleDownstreamRequest<OtherModule, TestRequestWithResponse, string, TestRequestHandler>();
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>? registration = _internals.GetService<
            IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<TestRequestHandler>(registration.Handler);
    }

    [Fact]
    public void Handle_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.HandleDownstreamRequest<OtherModule, TestRequestWithResponse, string, TestRequestHandler>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Ignore_NewRequest_RegistersIgnorer()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut.IgnoreDownstreamRequest<OtherModule, TestRequest>();
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequest, OtherModule>? registration = _internals.GetService<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestIgnorer<TestRequest>>(registration.Handler);
    }

    [Fact]
    public void Ignore_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequest, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.IgnoreDownstreamRequest<OtherModule, TestRequest>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Ignore_NewRequestWithResponse_RegistersIgnorer()
    {
        // Arrange
        // - no arrangement required

        // Act
        _sut.IgnoreDownstreamRequest<OtherModule, TestRequestWithResponse, string>();
        _internals.Build();

        // Assert
        IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>? registration = _internals.GetService<
            IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestIgnorer<TestRequestWithResponse, string>>(registration.Handler);
    }

    [Fact]
    public void Ignore_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IDownstreamRequestRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.IgnoreDownstreamRequest<OtherModule, TestRequestWithResponse, string>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}
