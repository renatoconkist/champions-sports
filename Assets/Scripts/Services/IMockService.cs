namespace Champions.Services
{
    /// <summary>
    /// Marker interface: all mock service implementations must implement this
    /// so they can be swapped for real implementations via a scripting define symbol.
    /// Gate with: #if CHAMPIONS_MOCK_SERVICES
    /// </summary>
    public interface IMockService { }
}
