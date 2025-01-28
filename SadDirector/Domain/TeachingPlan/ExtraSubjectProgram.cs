namespace SadDirector.Domain.TeachingPlan;

public class ExtraSubjectProgram
{
    public int Id { get; set; }
    public int ExtraSubjectId { get; set; }
    public int StudyClassId { get; set; }
    public int Hours { get; set; }
}