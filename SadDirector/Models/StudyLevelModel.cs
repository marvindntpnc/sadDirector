using Microsoft.AspNetCore.Mvc.Rendering;
using SadDirector.Domain;
using SadDirector.Domain.TeachingPlan.enums;

namespace SadDirector.Models;

public class StudyLevelModel
{
    public List<SelectListItem> RequiredSubjects { get; set; } = new();
    public List<SelectListItem> FormedSubjects { get; set; } = new();
    public StudyClassLevel StudyLevel { get; set; }
    public List<StudyClassModel> StudyClassList { get; set; } = new();
    public List<SubjectModel> RequiredSubjectList { get; set; } = new();
    public List<SubjectModel> FormedSubjectList { get; set; } = new();
    public List<SubjectProgramModel> RequiredSubjectProgramModel { get; set; } = new();
    public List<SubjectProgramModel> FormedSubjectProgramModel { get; set; } = new();
}