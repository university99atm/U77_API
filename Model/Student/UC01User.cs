namespace atmglobalapi.Model.User
{
    public class UC01User
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string SRNNo { get; set; }
        public int? UserID { get; set; }
        public int? RollId { get; set; }

        public bool? IsLocked { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool? Status { get; set; }
        public bool? Archive { get; set; }
        public bool? IsActive { get; set; }
    }
}
