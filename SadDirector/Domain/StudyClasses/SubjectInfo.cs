namespace SadDirector.Domain.StudyClasses;

public class SubjectInfo
{
    public int Id { get; set; }
    public int StudyClassId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public bool IsMainPart { get; set; }
    public int CurrentHours { get; set; }
    public bool IsRequired { get; set; }
    public bool IsExtra { get; set; }
}