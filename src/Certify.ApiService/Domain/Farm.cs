namespace Certify.ApiService.Domain;

public class Farm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public List<Audit> Audits { get; set; } = [];
}
