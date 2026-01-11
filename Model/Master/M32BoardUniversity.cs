//Model\Master\M32BoardUniversity.cs
namespace atmglobalapi.Model.Master
{
    public class M32BoardUniversity
    {
        public int? Id { get; set; }
        public string? BoardName { get; set; }
        public string? BoardType { get; set; }

        // 1=Active, 0=Inactive, 2=Archived
        public int? Status { get; set; }

        // Pagination
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Search { get; set; }

        // Audit
        public bool? System { get; set; }

        public int Type { get; set; }
    }
}