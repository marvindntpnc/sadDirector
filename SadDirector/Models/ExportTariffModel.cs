using SadDirector.Domain.TeachingPlan;

namespace SadDirector.Models;

public class ExportTariffModel
{
    public int TariffId { get; set; }
    public string TariffName { get; set; }
    public string TariffPeriod  { get; set; }
    public List<ExtraSubject?> ExtraSubjectList { get; set; }
    public List<TeacherSummaryModel> TeachersSummaryInfo { get; set; }
}