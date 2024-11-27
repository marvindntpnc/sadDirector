using SadDirector.Domain;

namespace SadDirector.Models;

public class SubjectProgramModel
{
    public int ExtraSubjectId { get; set; }
    public int SubjectId { get; set; }
    public int StudyClassId { get; set; }
    public int Hours { get; set; }
    public bool IsRequired { get; set; }
}