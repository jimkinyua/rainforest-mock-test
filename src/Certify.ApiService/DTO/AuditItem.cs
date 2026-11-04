using Certify.ApiService.Domain;

namespace Certify.ApiService.DTO
{
    public class AuditItem
    {
        public int Score { get; set; }
        public CertificationLevel Level { get; set; }
        public string Auditor { get; set; } = string.Empty;
    }
}
