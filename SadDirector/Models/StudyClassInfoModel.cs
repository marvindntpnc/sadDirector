namespace SadDirector.Models;

public class StudyClassInfoModel
{
    public int StudyClassId { get; set; }
    public string StudyClassName { get; set; }
    public int StudentCount { get; set; }
    public List<SubjectInfoModel> StudyClassRequiredSubjects { get; set; } = new();
    public List<SubjectInfoModel> StudyClassFormedSubjects { get; set; } = new();
    public List<SubjectInfoModel> StudyClassExtraSubjects { get; set; } = new();
}