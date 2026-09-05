//HintName: AppOuter.AppInner.TestApp.Extensions.g.cs
public static partial class TestAppSetupExtensions
{
    public static global::AppOuter.AppInner.TestAppBuilder CreateAppBuilder(this AppOuter.AppInner.TestApp app)
    {
        var appBuilderBase = app.CreateAppBuilderBase();
        return new global::AppOuter.AppInner.TestAppBuilder(appBuilderBase);
    }

    public static global::Microsoft.Extensions.Hosting.IHostApplicationBuilder AddTestApp(this global::Microsoft.Extensions.Hosting.IHostApplicationBuilder builder, AppOuter.AppInner.TestApp appDefinition)
    {
        if (builder == null)
        {
            throw new global::System.ArgumentNullException(nameof(builder));
        }

        if (appDefinition == null)
        {
            throw new global::System.ArgumentNullException(nameof(appDefinition));
        }

        if (global::System.Linq.Enumerable.Any(builder.Services, descriptor => descriptor.ServiceType == typeof(global::AppOuter.AppInner.ITestApp)))
        {
            throw new global::System.InvalidOperationException("The application is already registered in the host.");
        }

        global::AppOuter.AppInner.TestAppBuilder appBuilder = appDefinition.CreateAppBuilder();
        global::Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions.AddSingleton(builder.Services, provider => appBuilder.Build(provider));
        global::Microsoft.Extensions.DependencyInjection.ServiceCollectionHostedServiceExtensions.AddHostedService<global::Tycho.Hosting.Services.AppHostedLifecycleService<global::AppOuter.AppInner.ITestApp>>(builder.Services);

        return builder;
    }
}
