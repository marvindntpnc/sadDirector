using SadDirector.Domain.TeachingPlan.enums;

namespace SadDirector.Models
{
    public class TeacherLevelModel
    {
        public StudyClassLevel StudyLevel { get; set; }
        public int HoursSummary { get => TeacherInfo.Sum(ti => ti.SubjectHours); }
        public List<TeacherInfoModel> TeacherInfo { get; set; }
    }
}
