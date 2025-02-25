using Microsoft.AspNetCore.Mvc.Rendering;

namespace SadDirector.Models;

public class SubjectInfoModel
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; }
    public int PlanHours { get; set; }
    public int CurrentHours { get; set; }
    public bool IsSeparated { get; set; }
    public bool IsMain { get; set; }
    public int CurrentHoursSecondary { get; set; }
    public string TeacherNameSecondary { get; set; }
    public List<SelectListItem> AvailableTeachers { get; set; } = new();
}