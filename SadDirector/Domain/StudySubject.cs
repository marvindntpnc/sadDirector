namespace SadDirector.Domain;

public class StudySubject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool AssignAsRequiredToBeginners { get; set; }
    public bool AssignAsRequiredToMiddle { get; set; }
    public bool AssignAsRequiredToHigh { get; set; }
    public bool AssignAsFormedToBeginners { get; set; }
    public bool AssignAsFormedToMiddle { get; set; }
    public bool AssignAsFormedToHigh { get; set; }
}