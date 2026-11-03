using System.ComponentModel.DataAnnotations;

namespace Certify.Web;

public class CertifyApiClient(HttpClient httpClient)
{
    public async Task<List<FarmSummary>> GetFarmsAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<List<FarmSummary>>("/farms", cancellationToken) ?? [];
    }

    public async Task AddFarmAsync(NewFarm farm, CancellationToken cancellationToken = default)
    {
        await httpClient.PostAsJsonAsync("/farms", farm, cancellationToken);
    }
}

public record FarmSummary(int Id, string Name, string Country);

public class NewFarm
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;
}
