using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Tycho.Modules.Instance;
using Tycho.Requests.Handling;
using Tycho.Requests.Registration;
using Tycho.Requests.Registration.Registrations;
using Tycho.Structure;
using Tycho.TestUtils.Handlers;
using Tycho.TestUtils.Modules;
using Tycho.TestUtils.Requests;

namespace Tycho.UnitTests.Requests.Registration;

public class UpstreamRegistratorTests
{
    private readonly Internals _internals;
    private readonly Registrator _sut;

    public UpstreamRegistratorTests()
    {
        _internals = new AppInternals(Host.CreateEmptyApplicationBuilder(default), typeof(UpstreamRegistratorTests));
        _internals.GetHostBuilder().Services
                  .AddSingleton(_internals);
        _sut = new Registrator(_internals);
    }

    [Fact]
    public void Forward_NewRequest_RegistersForwarder()
    {
        // Arrange
        var targetModuleMock = new Mock<IModule<TestModule>>();
        _internals.GetHostBuilder().Services.AddSingleton(targetModuleMock.Object);

        // Act
        _sut.ForwardUpstreamRequest<TestRequest, TestModule>();
        _internals.Build();

        // Assert
        IUpstreamRequestRegistration<TestRequest>? registration = _internals.GetService<IUpstreamRequestRegistration<TestRequest>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestForwarder<TestRequest, TestModule>>(registration.Handler);
    }

    [Fact]
    public void Forward_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IUpstreamRequestRegistration<TestRequest>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardUpstreamRequest<TestRequest, TestModule>();

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
        _sut.ForwardUpstreamRequest<TestRequestWithResponse, string, TestModule>();
        _internals.Build();

        // Assert
        IUpstreamRequestRegistration<TestRequestWithResponse, string>? registration = _internals.GetService<IUpstreamRequestRegistration<TestRequestWithResponse, string>>();
        Assert.NotNull(registration);
        Assert.IsType<RequestForwarder<TestRequestWithResponse, string, TestModule>>(registration.Handler);
    }

    [Fact]
    public void Forward_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IUpstreamRequestRegistration<TestRequestWithResponse, string>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardUpstreamRequest<TestRequestWithResponse, string, TestModule>();

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
        _sut.ForwardMappedUpstreamRequest<TestRequest, OtherRequest, TestModule>(mapMock.Object);
        _internals.Build();

        // Assert
        IUpstreamRequestRegistration<TestRequest>? registration = _internals.GetService<IUpstreamRequestRegistration<TestRequest>>();
        Assert.NotNull(registration);
        Assert.IsType<MappedRequestForwarder<TestRequest, OtherRequest, TestModule>>(registration.Handler);
    }

    [Fact]
    public void Forward_ExistingMappedRequest_ThrowsArgumentException()
    {
        // Arrange
        var mapMock = new Mock<Func<TestRequest, OtherRequest>>();
        var registrationMock = new Mock<IUpstreamRequestRegistration<TestRequest>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardMappedUpstreamRequest<
            TestRequest, OtherRequest, TestModule>(
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
        _sut.ForwardMappedUpstreamRequest<
            TestRequestWithResponse, string,
            OtherRequestWithResponse, string,
            TestModule>(
                mapRequestMock.Object, mapResponseMock.Object);
        _internals.Build();

        // Assert
        IUpstreamRequestRegistration<TestRequestWithResponse, string>? registration = _internals
            .GetService<IUpstreamRequestRegistration<TestRequestWithResponse, string>>();
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
        var registrationMock = new Mock<IUpstreamRequestRegistration<TestRequestWithResponse, string>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.ForwardMappedUpstreamRequest<
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
        _sut.HandleUpstreamRequest<TestRequest, TestRequestHandler>();
        _internals.Build();

        // Assert
        IUpstreamRequestRegistration<TestRequest>? registration = _internals.GetService<IUpstreamRequestRegistration<TestRequest>>();
        Assert.NotNull(registration);
        Assert.IsType<TestRequestHandler>(registration.Handler);
    }

    [Fact]
    public void Handle_ExistingRequest_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IUpstreamRequestRegistration<TestRequest>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.HandleUpstreamRequest<TestRequest, TestRequestHandler>();

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
        _sut.HandleUpstreamRequest<TestRequestWithResponse, string, TestRequestHandler>();
        _internals.Build();

        // Assert
        IUpstreamRequestRegistration<TestRequestWithResponse, string>? registration = _internals.GetService<IUpstreamRequestRegistration<TestRequestWithResponse, string>>();
        Assert.NotNull(registration);
        Assert.IsType<TestRequestHandler>(registration.Handler);
    }

    [Fact]
    public void Handle_ExistingRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        var registrationMock = new Mock<IUpstreamRequestRegistration<TestRequestWithResponse, string>>();
        _internals.GetHostBuilder().Services.AddSingleton(registrationMock.Object);

        // Act
        void Act() => _sut.HandleUpstreamRequest<TestRequestWithResponse, string, TestRequestHandler>();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }
}
