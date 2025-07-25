using SadDirector.Domain.TeacherInfo.enums;

namespace SadDirector.Models;

public class TeacherModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string SecondName { get; set; }
    public TeacherEducation TeacherEducation { get; set; }
    public TeacherCategory TeacherCategory { get; set; }
    public int TariffCategory { get; set; }
    public DateTime ExperienceFrom { get; set; }
    public TeacherDegree TeacherDegree { get; set; }
    public List<int> SubjectIds { get; set; }
    public string TeacherSubjectList { get; set; }
    public int StudyClassId { get; set; }
    public int ClassroomId { get; set; }
    public bool IsDirector { get; set; }
    public bool IsHeadTeacher { get; set; }
    public bool IsMentor { get; set; }
    public bool AfterClassesTeacher { get; set; }
    public bool IsPsychologist { get; set; }
    public bool IsSocial { get; set; }
    public bool IsFacilitator { get; set; }
    public bool IsLibraryManager { get; set; }
    public bool IsLogopedist { get; set; }
    public bool IsMain { get; set; }
    public bool Museum { get; set; }
    public bool Theater { get; set; }
    public bool Chorus { get; set; }
    public bool Scouts { get; set; }
    public bool SportClub { get; set; }
}