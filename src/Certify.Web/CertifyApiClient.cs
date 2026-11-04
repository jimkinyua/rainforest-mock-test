using System.ComponentModel.DataAnnotations;

namespace Certify.Web;

public class CertifyApiClient(HttpClient httpClient)
{
    public async Task<List<FarmSummary>> GetFarmsAsync(string? term ="", CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<List<FarmSummary>>($"/farms?country={Uri.EscapeDataString(term)}", cancellationToken) ?? [];
    }

    public async Task AddFarmAsync(NewFarmDTO farm, CancellationToken cancellationToken = default)
    {
       var response = await httpClient.PostAsJsonAsync("/farms", farm, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}

public record FarmSummary(int Id, string Name, string Country);

public class NewFarmDTO
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;
}
