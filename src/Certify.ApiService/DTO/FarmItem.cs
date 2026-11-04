namespace Certify.ApiService.DTO
{
    public class FarmItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public List<AuditItem> Audits { get; set; } = new List<AuditItem>();
    }
}
