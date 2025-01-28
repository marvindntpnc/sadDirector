using SadDirector.Domain.TeachingPlan.enums;

namespace SadDirector.Domain.TeachingPlan;

public class StudyClass
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int StudyClassLevelId { get; set; }
    public StudyClassLevel StudyClassLevel{
        get => (StudyClassLevel)StudyClassLevelId;
        set => StudyClassLevelId = (int)value;
    }
}