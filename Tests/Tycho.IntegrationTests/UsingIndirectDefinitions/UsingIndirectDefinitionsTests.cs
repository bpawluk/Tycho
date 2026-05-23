using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT;
using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Handlers;
using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT.Modules;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions;

public sealed class UsingIndirectDefinitionsTests
{
    [Fact]
    public void TychoDoesNotEnableYet_RequestsDefinedInLocalHelperMethod_AreIncludedInGeneratedFacade()
    {
        AssertRequestIsNotIncludedInGeneratedFacade<TestRequestFromLocalHelper>();
    }

    [Fact]
    public void TychoDoesNotEnableYet_RequestsDefinedInLocalStaticHelperMethod_AreIncludedInGeneratedFacade()
    {
        AssertRequestIsNotIncludedInGeneratedFacade<TestRequestFromLocalStaticHelper>();
    }

    [Fact]
    public void TychoDoesNotEnableYet_RequestsDefinedInExternalHelperClass_AreIncludedInGeneratedFacade()
    {
        AssertRequestIsNotIncludedInGeneratedFacade<TestRequestFromHelperClass>();
    }

    [Fact]
    public void TychoDoesNotEnableYet_RequestsDefinedInExternalStaticHelperMethod_AreIncludedInGeneratedFacade()
    {
        AssertRequestIsNotIncludedInGeneratedFacade<TestRequestFromHelperStaticClass>();
    }

    [Fact]
    public void TychoDoesNotEnableYet_RequestsDefinedInExternalExtensionMethod_AreIncludedInGeneratedFacade()
    {
        AssertRequestIsNotIncludedInGeneratedFacade<TestRequestFromHelperExtension>();
    }

    [Fact]
    public void TychoDoesNotEnableYet_EventsDefinedInLocalHelperMethod_AreIncludedInGeneratedPublisher()
    {
        AssertEventIsNotIncludedInGeneratedPublisher<TestEventFromLocalHelper>();
    }

    [Fact]
    public void TychoDoesNotEnableYet_EventsDefinedInLocalStaticHelperMethod_AreIncludedInGeneratedPublisher()
    {
        AssertEventIsNotIncludedInGeneratedPublisher<TestEventFromLocalStaticHelper>();
    }

    [Fact]
    public void TychoDoesNotEnableYet_EventsDefinedInExternalHelperClass_AreIncludedInGeneratedPublisher()
    {
        AssertEventIsNotIncludedInGeneratedPublisher<TestEventFromHelperClass>();
    }

    [Fact]
    public void TychoDoesNotEnableYet_EventsDefinedInExternalStaticHelperMethod_AreIncludedInGeneratedPublisher()
    {
        AssertEventIsNotIncludedInGeneratedPublisher<TestEventFromHelperStaticClass>();
    }

    [Fact]
    public void TychoDoesNotEnableYet_EventsDefinedInExternalExtensionMethod_AreIncludedInGeneratedPublisher()
    {
        AssertEventIsNotIncludedInGeneratedPublisher<TestEventFromHelperExtension>();
    }

    [Fact(Timeout = 5000)]
    public async Task TychoDoesNotEnableYet_ModulesDefinedInLocalHelperMethod_AreIncludedInGeneratedSetup()
    {
        await AssertModuleFacadeIsNotIncludedInGeneratedSetup<ILocalHelperModule, TestRequestUsingLocalStructureModuleHandler>(
            async sut => await sut.ExecuteAsync(new TestRequestUsingLocalStructureModule(), TestContext.Current.CancellationToken));
    }

    [Fact(Timeout = 5000)]
    public async Task TychoDoesNotEnableYet_ModulesDefinedInLocalStaticHelperMethod_AreIncludedInGeneratedSetup()
    {
        await AssertModuleFacadeIsNotIncludedInGeneratedSetup<ILocalStaticHelperModule, TestRequestUsingLocalStaticStructureModuleHandler>(
            async sut => await sut.ExecuteAsync(new TestRequestUsingLocalStaticStructureModule(), TestContext.Current.CancellationToken));
    }

    [Fact(Timeout = 5000)]
    public async Task TychoDoesNotEnableYet_ModulesDefinedInExternalHelperClass_AreIncludedInGeneratedSetup()
    {
        await AssertModuleFacadeIsNotIncludedInGeneratedSetup<IHelperClassModule, TestRequestUsingHelperClassStructureModuleHandler>(
            async sut => await sut.ExecuteAsync(new TestRequestUsingHelperClassStructureModule(), TestContext.Current.CancellationToken));
    }

    [Fact(Timeout = 5000)]
    public async Task TychoDoesNotEnableYet_ModulesDefinedInExternalStaticHelperMethod_AreIncludedInGeneratedSetup()
    {
        await AssertModuleFacadeIsNotIncludedInGeneratedSetup<IHelperStaticClassModule, TestRequestUsingHelperStaticStructureModuleHandler>(
            async sut => await sut.ExecuteAsync(new TestRequestUsingHelperStaticStructureModule(), TestContext.Current.CancellationToken));
    }

    [Fact(Timeout = 5000)]
    public async Task TychoDoesNotEnableYet_ModulesDefinedInExternalExtensionMethod_AreIncludedInGeneratedSetup()
    {
        await AssertModuleFacadeIsNotIncludedInGeneratedSetup<IHelperExtensionModule, TestRequestUsingHelperExtensionStructureModuleHandler>(
            async sut => await sut.ExecuteAsync(new TestRequestUsingHelperExtensionStructureModule(), TestContext.Current.CancellationToken));
    }

    private static void AssertRequestIsNotIncludedInGeneratedFacade<TRequest>()
    {
        bool hasRequestExecuteMethod = typeof(ITestApp)
            .GetMethods()
            .Any(method =>
                method.Name == "ExecuteAsync" &&
                method.GetParameters().Length > 0 &&
                method.GetParameters()[0].ParameterType == typeof(TRequest));
        Assert.False(hasRequestExecuteMethod);
    }

    private static void AssertEventIsNotIncludedInGeneratedPublisher<TEvent>()
    {
        bool hasEventPublishMethod = typeof(TestApp.IPublisher)
            .GetMethods()
            .Any(method =>
                method.Name == "PublishAsync" &&
                method.GetParameters().Length > 0 &&
                method.GetParameters()[0].ParameterType == typeof(TEvent));
        Assert.False(hasEventPublishMethod);
    }

    private static async Task AssertModuleFacadeIsNotIncludedInGeneratedSetup<TMissingFacade, TFailedHandler>(
        Func<ITestApp, Task> runRequest)
    {
        ITestApp sut = await new TestApp().RunAsync();
        try
        {
            InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await runRequest(sut);
            });

            Assert.Contains(typeof(TMissingFacade).FullName!, exception.Message, StringComparison.Ordinal);
            Assert.Contains(typeof(TFailedHandler).FullName!, exception.Message, StringComparison.Ordinal);
            Assert.True(
                exception.Message.Contains("Unable to resolve service for type", StringComparison.Ordinal) ||
                exception.Message.Contains("No service for type", StringComparison.Ordinal),
                $"Unexpected error message: {exception.Message}");
        }
        finally
        {
            await sut.DisposeAsync();
        }
    }
}
