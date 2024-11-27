using Microsoft.AspNetCore.Mvc.Rendering;

namespace SadDirector.Models;

public class TeachingPlanModel
{
    public int TariffId { get; set; }
    public StudyLevelModel BeginnersStudyLevel { get; set; } = new();
    public StudyLevelModel MiddleStudyLevel { get; set; } = new();
    public StudyLevelModel HighStudyLevel { get; set; } = new();
    public ExtraProgramModel ExtraPrograms { get; set; } = new();
}