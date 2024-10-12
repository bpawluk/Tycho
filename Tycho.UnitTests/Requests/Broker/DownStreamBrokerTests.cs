﻿using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tycho.UnitTests._Data.Modules;
using Tycho.UnitTests._Data.Requests;
using TychoV2.Requests;
using TychoV2.Requests.Broker;
using TychoV2.Requests.Registrating.Registrations;
using TychoV2.Structure;

namespace Tycho.UnitTests.Requests.Broker;

public class DownStreamBrokerTests
{
    private readonly DownStreamBroker<TestModule> _sut;
    private readonly Internals _internals;

    public DownStreamBrokerTests()
    {
        _internals = new Internals(string.Empty);
        _sut = new DownStreamBroker<TestModule>(_internals);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestThatIsRegistered_ReturnsTrue(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequest, TestModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequest>();

        // Assert
        Assert.True(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestThatIsRegisteredUpstream_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IUpStreamHandlerRegistration<TestRequest>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequest>();

        // Assert
        Assert.False(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestThatIsRegisteredForOtherModule_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequest, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequest>();

        // Assert
        Assert.False(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_MissingRequest_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequest>();

        // Assert
        Assert.False(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestWithResponseThatIsRegistered_ReturnsTrue(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequestWithResponse, string, TestModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequestWithResponse, string>();

        // Assert
        Assert.True(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestWithResponseThatIsRegisteredUpstream_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IUpStreamHandlerRegistration<TestRequestWithResponse, string>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequestWithResponse, string>();

        // Assert
        Assert.False(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_RequestWithResponseThatIsRegisteredForOtherModule_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequestWithResponse, string, OtherModule>>();
        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);

        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequestWithResponse, string>();

        // Assert
        Assert.False(canExecute);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecute_MissingRequestWithResponse_ReturnsFalse(bool buildInternals)
    {
        // Arrange
        if (buildInternals)
        {
            _internals.Build();
        }

        // Act
        bool canExecute = _sut.CanExecute<TestRequestWithResponse, string>();

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    public async Task Execute_RequestThatIsRegistered_CallsHandler()
    {
        // Arrange
        var request = new TestRequest();
        var handlerMock = new Mock<IRequestHandler<TestRequest>>();

        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequest, TestModule>>();
        registrationMock.Setup(x => x.Handler).Returns(handlerMock.Object);

        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);
        _internals.Build();

        // Act
        await _sut.Execute(request, CancellationToken.None);

        // Assert
        handlerMock.Verify(h => h.Handle(request, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Execute_MissingRequest_ThrowsInvalidOperationException()
    {
        // Arrange
        _internals.Build();

        // Act
        async Task Act() => await _sut.Execute<TestRequest>(new(), CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task Execute_NullRequest_ThrowsArgumentException()
    {
        // Arrange
        TestRequest requestData = null!;

        // Act
        async Task Act() => await _sut.Execute(requestData, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
    }

    [Fact]
    public async Task Execute_RequestWithResponseThatIsRegistered_CallsHandler()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        var response = "success";

        var handlerMock = new Mock<IRequestHandler<TestRequestWithResponse, string>>();
        handlerMock.Setup(h => h.Handle(request, CancellationToken.None))
                   .ReturnsAsync(response);

        var registrationMock = new Mock<IDownStreamHandlerRegistration<TestRequestWithResponse, string, TestModule>>();
        registrationMock.Setup(x => x.Handler).Returns(handlerMock.Object);

        _internals.GetServiceCollection().AddSingleton(registrationMock.Object);
        _internals.Build();

        // Act
        var result = await _sut.Execute<TestRequestWithResponse, string>(request, CancellationToken.None);

        // Assert
        Assert.Equal(response, result);
        handlerMock.Verify(h => h.Handle(request, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Execute_MissingRequestWithResponse_ThrowsInvalidOperationException()
    {
        // Arrange
        _internals.Build();

        // Act
        async Task Act() => await _sut.Execute<TestRequestWithResponse, string>(new(), CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(Act);
    }

    [Fact]
    public async Task Execute_NullRequestWithResponse_ThrowsArgumentException()
    {
        // Arrange
        TestRequestWithResponse requestData = null!;

        // Act
        async Task Act() => await _sut.Execute<TestRequestWithResponse, string>(requestData, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(Act);
    }
}
