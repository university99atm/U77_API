//Model\Master\M31TransportStop.cs
namespace atmglobalapi.Model.Master
{
    public class M31TransportStop
    {
        public int Type { get; set; }              // 1=Insert, 2=Update, 3=Delete, 4=Archive, 5=Get
        public int? Id { get; set; }
        public int? TransportRouteId { get; set; }
        public string? StopName { get; set; }
        public string? StopCode { get; set; }
        public int? Sequence { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}