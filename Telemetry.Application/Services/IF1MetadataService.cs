namespace Telemetry.Application.Services
{
    public interface IF1MetadataService
    {
        Task<List<string>> GetAvailableGrandsPrixAsync(int year);
        Task<List<string>> GetDriversForGpAsync(int year, string gp);
    }
}
