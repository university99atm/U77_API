namespace atmglobalapi.Model.User
{
    public class UC02RoleMenuPermission
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public int? RoleId { get; set; }
        public int? MenuId { get; set; }

        public bool? CanView { get; set; }
        public bool? CanAdd { get; set; }
        public bool? CanEdit { get; set; }
        public bool? CanDelete { get; set; }
        public bool? CanApprove { get; set; }

        public bool? Status { get; set; }
        public bool? Archive { get; set; }
    }
}
