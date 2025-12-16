namespace atmglobalapi.Model.Master
{
    public class M30TransportRoute
    {
        public int Type { get; set; }              // 1=Insert, 2=Update, 3=Delete, 4=Archive, 5=Get
        public int? Id { get; set; }
        public string? RouteName { get; set; }
        public string? RouteCode { get; set; }
        public string? StartPoint { get; set; }
        public string? EndPoint { get; set; }
        public int? TotalStops { get; set; }
        public decimal? DistanceKM { get; set; }
        public string? EstimatedTime { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}