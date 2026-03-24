namespace TryMeTumble.Application.Interfaces
{
    public interface ISeederService
    {
        Task<int> SeedWebsitesAsync(int count);
    }
}