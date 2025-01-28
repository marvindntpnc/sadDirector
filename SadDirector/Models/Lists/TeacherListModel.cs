using Microsoft.AspNetCore.Mvc.Rendering;

namespace SadDirector.Models.Lists;

public class TeacherListModel
{
    public List<TeacherModel> TeacherList { get; set; }
    public List<SelectListItem> TeachersEducationLevels { get; set; } = new();
    public List<SelectListItem> TeachersDegrees { get; set; } = new();
    public List<SelectListItem> TeachersCategory { get; set; } = new();
    public List<SelectListItem> TeachersSubjects { get; set; } = new();
    public List<SelectListItem> TeachersStudyClasses { get; set; } = new();
    public List<SelectListItem> TeachersClassroom { get; set; } = new();
    public List<SelectListItem> TeachersTariffCategory { get; set; } = new();
    public int TariffId { get; set; }
}