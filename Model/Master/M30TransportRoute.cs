namespace atmglobalapi.Model.Master
{
    public class M30TransportRoute
    {
        public int? Id { get; set; }
        public string? RouteName { get; set; }
        public string? StartPoint { get; set; }
        public string? EndPoint { get; set; }
        public int? TotalStops { get; set; }
        public decimal? DistanceKM { get; set; }
        public string? EstimatedTime { get; set; }

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