namespace SadDirector.Models.Lists;

public class StudyClassListModel
{
    public int TariffId { get; set; }
    public List<StudyClassInfoModel> StudyClasses { get; set; } = new();
}