namespace atmglobalapi.Model.Master
{
    public class M41RoleMaster
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public string? RoleName { get; set; }
        public string? RoleCode { get; set; }
        public string? Description { get; set; }

        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}
