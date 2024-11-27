namespace SadDirector.Models;

public class ExtraProgramModel
{
    public List<StudyClassModel> StudyClasses { get; set; } = new();
    public List<SubjectModel> ExtraSubjects { get; set; } = new();
    public List<SubjectProgramModel> ExtraSubjectPrograms { get; set; } = new();
}