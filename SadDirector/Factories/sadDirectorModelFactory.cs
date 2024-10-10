using SadDirector.Models;
using SadDirector.Models.Lists;
using SadDirector.Services;

namespace SadDirector.Factories;

public class SadDirectorModelFactory
{
    private readonly SadDirectorService _sadDirectorService;

    public SadDirectorModelFactory(SadDirectorService sadDirectorService)
    {
        _sadDirectorService = sadDirectorService;
    }
    public async Task<TariffListModel> PrepareTariffListModelAsync()
    {
        var tariffList = await _sadDirectorService.GetTariffListAsync();
        var model = new TariffListModel
        {
            TariffList = new List<TariffModel>()
        };
        foreach (var tariff in tariffList)
        {
            model.TariffList.Add(new TariffModel
            {
                Id =tariff.Id,
                Name = tariff.Name,
                Period = tariff.Period
            });
        }
        return model;
    }
    public async Task<TariffModel> PrepareTariffModelAsync(int tariffId)
    {
        var tariff = await _sadDirectorService.GetTariffByIdAsync(tariffId);
        
        if (tariff == null)
            return new TariffModel();
        
        var model = new TariffModel
        {
            Id = tariff!.Id,
            Name=tariff.Name,
            Period = tariff.Period
        };
        return model;
    }
    public async Task<TeacherListModel> PrepareTeacherListModelAsync(int tariffId)
    {
        var teacherList = await _sadDirectorService.GetTeacherListAsync();
        var model = new TeacherListModel
        {
            TariffId = tariffId,
            TeacherList = new List<TeacherModel>()
        };
        foreach (var teacher in teacherList)
        {
            model.TeacherList.Add(new TeacherModel
            {
                Id = teacher.Id,
                Name = teacher.Name,
                Surname = teacher.Surname,
                SecondName = teacher.SecondName,
                TeacherEducation=teacher.Education,
                TeacherCategory=teacher.TeacherCategory,
                TariffCategory=teacher.TeacherTariffCategory,
                ExperienceFrom=teacher.ExperienceFrom,
                TeacherDegree=teacher.TeacherDegree,
                StudyClassId=teacher.StudyClassId,
                ClassroomId=teacher.ClassroomId,
                IsDirector=teacher.IsDirector,
                IsHeadTeacher=teacher.IsHeadTeacher,
                IsMentor=teacher.IsMentor,
                AfterClassesTeacher=teacher.AfterClassesTeacher,
                IsPsychologist=teacher.IsPsychologist,
                IsSocial=teacher.IsSocial,
                IsFacilitator=teacher.IsFacilitator,
                IsLibraryManager=teacher.IsLibraryManager,
                IsLogopedist=teacher.IsLogopedist,
                IsMain=teacher.IsMain,
                Museum=teacher.Museum,
                Theater=teacher.Theater,
                Chorus=teacher.Chorus,
                Scouts=teacher.Scouts,
                SportClub=teacher.SportClub,
                
                
            });
        }
        return model;
    }
    public async Task<TeachingPlanModel> PrepareTeachingPlanModelAsync(int tariffId)
    {
        var subjectList = await _sadDirectorService.GetSubjectListAsync();
        var studyClassList = await _sadDirectorService.GetStudyClassListAsync();
        var model = new TeachingPlanModel
        {
            Subjects = new List<SubjectModel>(),
            StudyClasses = new List<StudyClassModel>(),
            TariffId = tariffId
        };
        foreach (var subject in subjectList)
        {
            model.Subjects.Add(new SubjectModel
            {
                Id = subject.Id,
                Name = subject.Name
            });
        }
        foreach (var studyClass in studyClassList)
        {
            model.StudyClasses.Add(new StudyClassModel
            {
                Id = studyClass.Id,
                Name = studyClass.Name
            });
        }
        return model;
    }
}