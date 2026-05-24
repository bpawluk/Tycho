namespace Tycho.IntegrationTests.UsingGenericAppsAndModules.SUT;

public abstract class PayloadBase;

public interface IMarker;

public sealed class SamplePayload : PayloadBase, IMarker;
