using IntegrationTests.DefiningGenericModules.SUT;
using IntegrationTests.DefiningGenericModules.SUT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Tycho;
using Tycho.Messaging;

namespace IntegrationTests.DefiningGenericModules;

public class DefiningGenericModulesTests
{
    [Theory]
    [InlineData(typeof(BaseClass))]
    [InlineData(typeof(DerivedClass))]
    [InlineData(typeof(OtherClass))]
    public async Task Tycho_Enables_DefiningGenericModules(Type typeParameter)
    {
        // Arrange
        var expectedResult = Activator.CreateInstance(typeParameter)!;
        var module = await GetGenericModule(typeParameter);

        // Act
        var result = await ExecuteGenericRequestWithResponse(module, typeParameter, expectedResult);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task Tycho_DoesNotEnableYet_DefiningPolymorphicContracts()
    {
        // Arrange
        var expectedResult = new DerivedClass();
        var module = await new GenericModule<BaseClass>().Build();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        {
            return module.Execute<GenericRequestWithResponse<DerivedClass>, DerivedClass>(new(expectedResult));
        });
    }

    private static async Task<object> ExecuteGenericRequestWithResponse(IModule module, Type requestParameterType, object requestParameter)
    {
        Type genericRequestWithResponseType = typeof(GenericRequestWithResponse<>).MakeGenericType(requestParameterType);
        var request = Activator.CreateInstance(genericRequestWithResponseType, requestParameter)!;

        MethodInfo executor = typeof(IMessageBroker)
            .GetMethods()
            .Where(method => method.Name == "Execute")
            .First(method => method.GetGenericArguments().Length == 2)
            .MakeGenericMethod(genericRequestWithResponseType, requestParameterType);

        dynamic resultTask = executor.Invoke(module, new object[] { request, CancellationToken.None })!;
        return await resultTask;
    }

    private static Task<IModule> GetGenericModule(Type type)
    {
        Type genericModuleType = typeof(GenericModule<>).MakeGenericType(type);
        var moduleDefinition = Activator.CreateInstance(genericModuleType)!;

        MethodInfo builder = genericModuleType.GetMethod("Build")!;
        return (builder.Invoke(moduleDefinition, null) as Task<IModule>)!;
    }
}
