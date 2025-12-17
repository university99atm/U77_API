namespace atmglobalapi.Model.Student
{
    public class S10StudentLastSchool
    {
        public int Type { get; set; }
        public int? Id { get; set; }

        public int? StudentId { get; set; }
        public int? InstituteId { get; set; }
        public int? SectorId { get; set; }
        public string BatchCode { get; set; }

        public string PrincipalName { get; set; }
        public string PrincipalMobile { get; set; }
        public string PrincipalEmail { get; set; }

        public string BestSchoolTeacherName { get; set; }
        public string BestSchoolTeacherMobile { get; set; }
        public string BestSchoolTeacherEmail { get; set; }

        public string BestCoachingTeacherName { get; set; }
        public string BestCoachingTeacherMobile { get; set; }
        public string BestCoachingTeacherEmail { get; set; }
    }
}
