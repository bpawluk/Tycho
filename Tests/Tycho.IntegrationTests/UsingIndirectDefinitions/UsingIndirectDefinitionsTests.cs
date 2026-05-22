using Tycho.IntegrationTests.UsingIndirectDefinitions.SUT;

namespace Tycho.IntegrationTests.UsingIndirectDefinitions;

public sealed class UsingIndirectDefinitionsTests
{
    [Fact(Timeout = 5000)]
    public void TychoDoesNotEnableYet_RequestsDefinedInLocalHelperMethod_AreIncludedInGeneratedFacade()
    {
        AssertRequestIsNotIncludedInGeneratedFacade<TestRequestFromLocalHelper>();
    }

    [Fact(Timeout = 5000)]
    public void TychoDoesNotEnableYet_RequestsDefinedInLocalStaticHelperMethod_AreIncludedInGeneratedFacade()
    {
        AssertRequestIsNotIncludedInGeneratedFacade<TestRequestFromLocalStaticHelper>();
    }

    [Fact(Timeout = 5000)]
    public void TychoDoesNotEnableYet_RequestsDefinedInExternalHelperClass_AreIncludedInGeneratedFacade()
    {
        AssertRequestIsNotIncludedInGeneratedFacade<TestRequestFromHelperClass>();
    }

    [Fact(Timeout = 5000)]
    public void TychoDoesNotEnableYet_RequestsDefinedInExternalStaticHelperMethod_AreIncludedInGeneratedFacade()
    {
        AssertRequestIsNotIncludedInGeneratedFacade<TestRequestFromHelperStaticClass>();
    }

    [Fact(Timeout = 5000)]
    public void TychoDoesNotEnableYet_RequestsDefinedInExternalExtensionMethod_AreIncludedInGeneratedFacade()
    {
        AssertRequestIsNotIncludedInGeneratedFacade<TestRequestFromHelperExtension>();
    }

    [Fact(Timeout = 5000)]
    public void TychoDoesNotEnableYet_EventsDefinedInLocalHelperMethod_AreIncludedInGeneratedPublisher()
    {
        AssertEventIsNotIncludedInGeneratedPublisher<TestEventFromLocalHelper>();
    }

    [Fact(Timeout = 5000)]
    public void TychoDoesNotEnableYet_EventsDefinedInLocalStaticHelperMethod_AreIncludedInGeneratedPublisher()
    {
        AssertEventIsNotIncludedInGeneratedPublisher<TestEventFromLocalStaticHelper>();
    }

    [Fact(Timeout = 5000)]
    public void TychoDoesNotEnableYet_EventsDefinedInExternalHelperClass_AreIncludedInGeneratedPublisher()
    {
        AssertEventIsNotIncludedInGeneratedPublisher<TestEventFromHelperClass>();
    }

    [Fact(Timeout = 5000)]
    public void TychoDoesNotEnableYet_EventsDefinedInExternalStaticHelperMethod_AreIncludedInGeneratedPublisher()
    {
        AssertEventIsNotIncludedInGeneratedPublisher<TestEventFromHelperStaticClass>();
    }

    [Fact(Timeout = 5000)]
    public void TychoDoesNotEnableYet_EventsDefinedInExternalExtensionMethod_AreIncludedInGeneratedPublisher()
    {
        AssertEventIsNotIncludedInGeneratedPublisher<TestEventFromHelperExtension>();
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
}
