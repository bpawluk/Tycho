using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tycho.Apps;
using Tycho.Modules;

namespace Tycho.Utils
{
    internal static class GeneratedSetupExtensions
    {
        private const string GeneratedSetupClassSuffix = "Setup";
        private const string GeneratedSetupMethod = "Setup";

        public static void AddGeneratedSetup(this TychoApp appInstance, IServiceCollection appServices)
        {
            AddGeneratedSetup(appInstance.GetType(), appServices);
        }

        public static void AddGeneratedSetup(this TychoModule moduleInstance, IServiceCollection moduleServices)
        {
            AddGeneratedSetup(moduleInstance.GetType(), moduleServices);
        }

        private static void AddGeneratedSetup(Type ownerType, IServiceCollection services)
        {
            Type setupType = GetGeneratedSetupType(ownerType) ?? throw new NotImplementedException(
                $"Generated setup was not found for '{ownerType.FullName}'. " +
                $"Make sure the definition is marked with TychoDefinitionAttribute.");

            MethodInfo setupMethod = setupType.GetMethod(GeneratedSetupMethod, BindingFlags.Public | BindingFlags.Static) ?? throw new NotImplementedException(
                $"Generated setup type '{setupType.FullName}' is missing the public static '{GeneratedSetupMethod}' method.");

            try
            {
                setupMethod.Invoke(null, new object[] { services });
            }
            catch (TargetInvocationException exception)
            {
                throw new InvalidOperationException($"Generated setup for '{ownerType.FullName}' failed.", exception.InnerException ?? exception);
            }
        }

        private static Type? GetGeneratedSetupType(Type ownerType)
        {
            Type ownerTypeDefinition = ownerType.IsGenericType ? ownerType.GetGenericTypeDefinition() : ownerType;

            int nameReplacePosition = ownerTypeDefinition.Name.LastIndexOf('`');
            string generatedSetupTypeName = nameReplacePosition >= 0
                ? $"{ownerTypeDefinition.Name[..nameReplacePosition]}{GeneratedSetupClassSuffix}{ownerTypeDefinition.Name[nameReplacePosition..]}"
                : $"{ownerTypeDefinition.Name}{GeneratedSetupClassSuffix}";

            int fullNameReplacePosition = Math.Max(ownerTypeDefinition.FullName.LastIndexOf('.'), ownerTypeDefinition.FullName.LastIndexOf('+')) + 1;
            string generatedSetupTypeFullName = fullNameReplacePosition > 0
                ? $"{ownerTypeDefinition.FullName[..fullNameReplacePosition]}{generatedSetupTypeName}"
                : generatedSetupTypeName;

            Type? generatedSetupType = ownerType.Assembly.GetType(generatedSetupTypeFullName);

            return generatedSetupType is { ContainsGenericParameters: true }
                ? generatedSetupType.MakeGenericType(ownerType.GetGenericArguments())
                : generatedSetupType;
        }
    }
}
