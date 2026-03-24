namespace TryMeTumble.Application.Interfaces
{
    public interface IWebsiteMetadataClient
    {
        Task<(string? Title, string? Description)> FetchMetadataAsync(string url);
    }
}