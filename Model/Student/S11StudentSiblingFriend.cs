namespace atmglobalapi.Model.Student
{
    public class S11StudentSiblingFriend
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public int? StudentId { get; set; }
        public bool? IsSchool { get; set; }
        public int? RelatedStudentId { get; set; }

        public string FullName { get; set; }
        public string ContactNo { get; set; }
        public string EmailId { get; set; }

        public int? RelationId { get; set; }
        public int? CourseId { get; set; }
        public int? BatchTypeId { get; set; }
    }
}
