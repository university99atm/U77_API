namespace atmglobalapi.Model.User
{
    public class A04RoleMenuPermission
    {
        public int Type { get; set; }  // 1=Replace | 2=Upsert | 3=Remove
        public int RoleId { get; set; }
        
        // List of menu permissions
        public List<MenuPermissionItem>? MenuList { get; set; }

        // Audit
        public int? ActionBy { get; set; }
        public bool? IsSystem { get; set; }
    }

    public class MenuPermissionItem
    {
        public int MenuId { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanAdd { get; set; } = false;
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        public bool CanApprove { get; set; } = false;
    }
}