using Microsoft.EntityFrameworkCore;
using SadDirector.Data;
using SadDirector.Domain;
using SadDirector.Models;

namespace SadDirector.Services;

public class SadDirectorService
{
    private readonly ApplicationContext _dbContext;

    public SadDirectorService(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tariff?> GetTariffByIdAsync(int id)
    {
       return await _dbContext.Tariffs.FirstOrDefaultAsync(t=>t.Id==id);
    }
    public async Task<List<Tariff>> GetTariffListAsync()
    {
        return await _dbContext.Tariffs.ToListAsync();
    }
    public async Task<Tariff> AddNewTariffAsync()
    {
        var tariff = new Tariff
        {
            Name = "Второе полугодие",
            Period = "январь 2025 - май 2025"
        };
        _dbContext.Tariffs.Add(tariff);
        await _dbContext.SaveChangesAsync();
        return tariff;
    }
    
    public async Task<List<Teacher>> GetTeacherListAsync()
    {
        return await _dbContext.Teachers.ToListAsync();
    }
    public async Task CreateOrUpdateTeacherAsync(TeacherModel model)
    {
        var teacher =await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Id == model.Id);
        if (teacher == null)
        {
            teacher = new Teacher();
        }

        teacher.Name = model.Name;
        teacher.Surname=model.Surname;
        teacher.SecondName=model.SecondName;
        teacher.EducationId=(int)model.TeacherEducation;
        teacher.TeacherCategoryId=(int)model.TeacherCategory;
        teacher.TeacherTariffCategory=model.TariffCategory;
        teacher.ExperienceFrom=model.ExperienceFrom;
        teacher.TeacherDegreeId=(int)model.TeacherDegree;
        teacher.StudyClassId=model.StudyClassId;
        teacher.ClassroomId=model.ClassroomId;
        teacher.IsDirector=model.IsDirector;
        teacher.IsHeadTeacher=model.IsHeadTeacher;
        teacher.IsMentor=model.IsMentor;
        teacher.AfterClassesTeacher=model.AfterClassesTeacher;
        teacher.IsPsychologist=model.IsPsychologist;
        teacher.IsSocial=model.IsSocial;
        teacher.IsFacilitator=model.IsFacilitator;
        teacher.IsLibraryManager=model.IsLibraryManager;
        teacher.IsLogopedist=model.IsLogopedist;
        teacher.IsMain=model.IsMain;
        teacher.Museum=model.Museum;
        teacher.Theater=model.Theater;
        teacher.Chorus=model.Chorus;
        teacher.Scouts=model.Scouts;
        teacher.SportClub=model.SportClub;


        if (model.Id == 0)
        {
            _dbContext.Teachers.Add(teacher);
        }
        else
        {
            _dbContext.Teachers.Update(teacher);
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<StudySubject>> GetSubjectListAsync()
    {
        return await _dbContext.StudySubjects.ToListAsync();
    }
    public async Task<List<StudyClass>> GetStudyClassListAsync()
    {
        return await _dbContext.StudyClasses.ToListAsync();
    }

    public async Task AddNewStudyClassAsync(StudyClassModel model)
    {
        var studyClass = new StudyClass
        {
            Name = model.Name,
            StudyLevel = model.StudyLevel
        };
        _dbContext.StudyClasses.Add(studyClass);
        await _dbContext.SaveChangesAsync();
    }
    public async Task AddNewSubjectAsync(SubjectModel model)
    {
        var subject = new StudySubject
        {
            Name = model.Name
        };
        _dbContext.StudySubjects.Add(subject);
        await _dbContext.SaveChangesAsync();
    }
}