namespace atmglobalapi.Model.User
{
    public class A01User
    {
        // For Registration
        public string? UserName { get; set; }
        public string? LoginId { get; set; }
        public string? Password { get; set; }  // ✅ Changed from PasswordHash to Password
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? RoleIds { get; set; } // Comma separated: "1,2,3"

        // For Login
        public int? UserId { get; set; }

        // For Password Change/Reset
        public int? Type { get; set; } // 1=Change Own, 2=Admin Reset
        public string? OldPassword { get; set; }  // ✅ Changed from OldPasswordHash
        public string? NewPassword { get; set; }  // ✅ Changed from NewPasswordHash

        // Audit
        public int? ActionBy { get; set; }
        public bool? IsSystem { get; set; }
    }
}