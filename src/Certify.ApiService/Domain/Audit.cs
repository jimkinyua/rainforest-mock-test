namespace Certify.ApiService.Domain;

public class Audit
{
    public int Id { get; set; }
    public int FarmId { get; set; }
    public Farm Farm { get; set; } = null!;
    public int Score { get; set; }
    public CertificationLevel Level { get; set; }
    public string Auditor { get; set; } = string.Empty;
    public DateTime AuditedOn { get; set; }
}
