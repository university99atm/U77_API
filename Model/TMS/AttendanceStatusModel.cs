namespace atmglobalapi.Model.Attendance
{
    public class AttendanceStatusModel
    {
        public int RowId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string? Day { get; set; }
        public string? Shift { get; set; }
        public TimeSpan? In_Time { get; set; }
        public TimeSpan? Out_Time { get; set; }
        public string? In_Location { get; set; }
        public string? Out_Location { get; set; }
        public string? Working_Hour { get; set; }
        public string? OT_Hour { get; set; }
        public string? Less_Worked_Hour { get; set; }
        public string? OpsStatus { get; set; }
        public string? Status { get; set; }
        public string? Remark { get; set; }
        public string? Hr_Remarks { get; set; }
        public string? In_Pic { get; set; }
        public string? Out_Pic { get; set; }
        public string? In_Ip { get; set; }
        public string? Out_Ip { get; set; }
        public bool? IsAgree { get; set; }
        public TimeSpan? RequestInTime { get; set; }
        public TimeSpan? RequestOutTime { get; set; }
        public string? Incurrentlocationname { get; set; }
        public string? Outcurrentlocationname { get; set; }
        public string? Inlocationname { get; set; }
        public string? Outlocationname { get; set; }
        public string? AttendanceStatus { get; set; }
    }
}
