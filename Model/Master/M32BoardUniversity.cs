//Model\Master\M32BoardUniversity.cs
namespace atmglobalapi.Model.Master
{
    public class M32BoardUniversity
    {
        public int Type { get; set; }              // 1=Insert, 2=Update, 3=Delete, 4=Archive, 5=Get
        public int? Id { get; set; }
        public string? BoardName { get; set; }
        public string? BoardCode { get; set; }
        public string? BoardType { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}