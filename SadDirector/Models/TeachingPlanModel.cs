namespace SadDirector.Models;

public class TeachingPlanModel
{
    public int TariffId { get; set; }

    public List<StudyClassModel> StudyClasses { get; set; }
    public List<SubjectModel> Subjects { get; set; }
}