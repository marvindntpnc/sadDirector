namespace SadDirector.Models;

public class TariffModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Period { get; set; }
    public List<TeacherSummaryModel> TeachersSummaryInfo { get; set; }
    public bool IsDownloaded { get; set; }
}