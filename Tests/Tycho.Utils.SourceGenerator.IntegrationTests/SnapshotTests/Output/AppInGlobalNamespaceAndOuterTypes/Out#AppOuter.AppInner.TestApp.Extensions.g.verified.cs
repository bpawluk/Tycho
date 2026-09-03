//HintName: AppOuter.AppInner.TestApp.Extensions.g.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Tycho.Hosting.Services;

public static partial class TestAppSetupExtensions
{
    public static AppOuter.AppInner.TestAppBuilder CreateAppBuilder(this AppOuter.AppInner.TestApp app)
    {
        var appBuilderBase = app.CreateAppBuilderBase();
        return new AppOuter.AppInner.TestAppBuilder(appBuilderBase);
    }

    public static IHostApplicationBuilder AddTestApp(this IHostApplicationBuilder builder, AppOuter.AppInner.TestApp appDefinition)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (appDefinition == null)
        {
            throw new ArgumentNullException(nameof(appDefinition));
        }

        if (Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(AppOuter.AppInner.ITestApp)))
        {
            throw new InvalidOperationException("The application is already registered in the host.");
        }

        AppOuter.AppInner.TestAppBuilder appBuilder = appDefinition.CreateAppBuilder();
        ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
        ServiceCollectionHostedServiceExtensions.AddHostedService<AppHostedLifecycleService<AppOuter.AppInner.ITestApp>>(builder.Services);

        return builder;
    }
}
