using SadDirector.Domain.TeachingPlan.enums;

namespace SadDirector.Domain.TeachingPlan;

public class TeachingProgram
{
    public int Id { get; set; }
    public int StudyClassId { get; set; }
    public int SubjectId { get; set; }
    public int Hours { get; set; }
    public int StudyClassLevelId { get; set; }
    public StudyClassLevel StudyClassLevel{
        get => (StudyClassLevel)StudyClassLevelId;
        set => StudyClassLevelId = (int)value;
    }
    public bool IsRequired { get; set; }
}