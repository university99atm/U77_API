namespace atmglobalapi.Model.Student
{
    public class S12StudentStatusHistory
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public int? StudentId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? StatusDate { get; set; }
        public string Reason { get; set; }
        public int? SectorId { get; set; }
    }
}
