namespace SadDirector.Models
{
    public class VacancySummaryModel
    {
        public List<VacancyModel> RequiredVacancyInfo { get; set; }
        public List<VacancyModel> FormedVacancyInfo { get; set; }
        public List<VacancyModel> ExtraVacancyInfo { get; set; }
    }
}
