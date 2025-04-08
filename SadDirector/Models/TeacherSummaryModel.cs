using SadDirector.Domain.TeachingPlan.enums;

namespace SadDirector.Models
{
    public class TeacherSummaryModel
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public TeacherInfo RequiredTeacherInfo { get; set; }
        public TeacherInfo FormedTacherInfo { get; set; }
        public TeacherInfo ExtraTeacherInfo { get; set; }

    }
}
