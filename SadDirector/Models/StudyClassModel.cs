using SadDirector.Domain;
using SadDirector.Domain.TeachingPlan.enums;

namespace SadDirector.Models;

public class StudyClassModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public StudyClassLevel StudyLevel { get; set; }
    public int TotalRequiredHours { get; set; }
    public int TotalFormedHours { get; set; }
    public int TotalExtraHours { get; set; }
}