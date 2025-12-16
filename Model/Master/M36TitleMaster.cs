namespace atmglobalapi.Model.Master
{
    public class M36TitleMaster
    {
        public int Type { get; set; }          // 1–5
        public int? Id { get; set; }
        public string? TitleName { get; set; }
        public string? StatusCode { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}
