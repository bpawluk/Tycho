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
            Type appType = appInstance.GetType();
            try
            {
                AddGeneratedSetup(appType, appServices);
            }
            catch
            {
                throw new NotImplementedException(
                    $"Failed to provide automated setup for {appType.Name} app. " +
                     "Make sure your app definition is marked with TychoDefinition attribute");
            }
        }

        public static void AddGeneratedSetup(this TychoModule moduleInstance, IServiceCollection moduleServices)
        {
            Type moduleType = moduleInstance.GetType();
            try
            {
                AddGeneratedSetup(moduleType, moduleServices);
            }
            catch
            {
                throw new NotImplementedException(
                    $"Failed to provide automated setup for {moduleType.Name} module. " +
                     "Make sure your module definition is marked with TychoDefinition attribute");
            }
        }

        private static void AddGeneratedSetup(Type ownerType, IServiceCollection services)
        {
            Type setupType = GetGeneratedSetupType(ownerType);
            MethodInfo setupMethod = setupType.GetMethod(GeneratedSetupMethod, BindingFlags.Public | BindingFlags.Static);
            setupMethod.Invoke(null, new object[] { services });
        }

        private static Type GetGeneratedSetupType(Type ownerType)
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

            Type generatedSetupType = ownerType.Assembly.GetType(generatedSetupTypeFullName, true);

            return generatedSetupType.ContainsGenericParameters
                ? generatedSetupType.MakeGenericType(ownerType.GetGenericArguments())
                : generatedSetupType;
        }
    }
}
