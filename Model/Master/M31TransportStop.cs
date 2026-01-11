//Model\Master\M31TransportStop.cs
namespace atmglobalapi.Model.Master
{
    public class M31TransportStop
    {
        public int? Id { get; set; }
        public int? TransportRouteId { get; set; }
        public string? StopName { get; set; }
        public int? Sequence { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

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