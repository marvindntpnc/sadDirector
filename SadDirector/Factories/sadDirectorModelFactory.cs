using Microsoft.AspNetCore.Mvc.Rendering;
using SadDirector.Domain;
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
        var model = new TeachingPlanModel
        {
            TariffId = tariffId
        };
        
        model.BeginnersStudyLevel=await PrepareStudyLevelModel(StudyClassLevel.Beginner);
        model.MiddleStudyLevel=await PrepareStudyLevelModel(StudyClassLevel.Middle);
        model.HighStudyLevel=await PrepareStudyLevelModel(StudyClassLevel.High);
        model.ExtraPrograms = await PrepareExtraProgramsModel();
        return model;
    }

    private async Task<StudyLevelModel> PrepareStudyLevelModel(StudyClassLevel studyClassLevel)
    {
        var model = new StudyLevelModel
        {
            StudyLevel = studyClassLevel
        };
        var subjectList = await _sadDirectorService.GetSubjectListAsync();
        foreach (var subject in subjectList)
        {
            switch (studyClassLevel)
            {
                case StudyClassLevel.Beginner:
                    model.RequiredSubjects.Add(new SelectListItem
                    {
                        Text = subject.Name,
                        Value = subject.Id.ToString(),
                        Selected = subject.AssignAsRequiredToBeginners
                    });
                    
                    model.FormedSubjects.Add(new SelectListItem
                    {
                        Text = subject.Name,
                        Value = subject.Id.ToString(),
                        Selected = subject.AssignAsFormedToBeginners
                    });
                    break;
                case StudyClassLevel.Middle:
                    model.RequiredSubjects.Add(new SelectListItem
                    {
                        Text = subject.Name,
                        Value = subject.Id.ToString(),
                        Selected = subject.AssignAsRequiredToMiddle
                    });
                    
                    model.FormedSubjects.Add(new SelectListItem
                    {
                        Text = subject.Name,
                        Value = subject.Id.ToString(),
                        Selected = subject.AssignAsFormedToMiddle
                    });
                    break;
                case StudyClassLevel.High:
                    model.RequiredSubjects.Add(new SelectListItem
                    {
                        Text = subject.Name,
                        Value = subject.Id.ToString(),
                        Selected = subject.AssignAsRequiredToHigh
                    });
                    
                    model.FormedSubjects.Add(new SelectListItem
                    {
                        Text = subject.Name,
                        Value = subject.Id.ToString(),
                        Selected = subject.AssignAsFormedToHigh
                    });
                    break;
            }
        }
        var studyClassList = await _sadDirectorService.GetStudyClassListAsync();
        var classes=studyClassList.Where(sc=>sc.StudyClassLevel==studyClassLevel).ToList();
        var requiredSubjects = new List<StudySubject>();
        var formedSubjects = new List<StudySubject>();
        switch (studyClassLevel)
        {
            case StudyClassLevel.Beginner:
                requiredSubjects=subjectList.Where(ss=>ss.AssignAsRequiredToBeginners).ToList();
                formedSubjects=subjectList.Where(ss=>ss.AssignAsFormedToBeginners).ToList();
                break;
            case StudyClassLevel.Middle:
                requiredSubjects=subjectList.Where(ss=>ss.AssignAsRequiredToMiddle).ToList();
                formedSubjects=subjectList.Where(ss=>ss.AssignAsFormedToMiddle).ToList();
                break;
            case StudyClassLevel.High:
                requiredSubjects=subjectList.Where(ss=>ss.AssignAsRequiredToHigh).ToList();
                formedSubjects=subjectList.Where(ss=>ss.AssignAsFormedToHigh).ToList();
                break;
        }
        
        var requiredSubjectProgram = new List<SubjectProgramModel>();
        var formedSubjectProgram = new List<SubjectProgramModel>();
        
        foreach (var subject in requiredSubjects)
        {
            model.RequiredSubjectList.Add(new SubjectModel
            {
                Name = subject.Name,
                Id = subject.Id,
                SubjectHoursTotal = 0
            });
        }
        foreach (var subject in formedSubjects)
        {
            model.FormedSubjectList.Add(new SubjectModel
            {
                Name = subject.Name,
                Id = subject.Id,
                SubjectHoursTotal = 0
            });
        }

        foreach (var studyClass in classes)
        {
            var studyClassModel = new StudyClassModel
            {
                Id = studyClass.Id,
                Name = studyClass.Name,
                StudyLevel = studyClass.StudyClassLevel
            };

            var teachingProgramList = await _sadDirectorService.GetStudyClassTeachingProgramAsync(studyClass.Id);
            foreach (var teachingProgram in teachingProgramList)
            {
                if (teachingProgram.IsRequired)
                {
                    studyClassModel.TotalRequiredHours += teachingProgram.Hours;
                }
                else
                {
                    studyClassModel.TotalFormedHours += teachingProgram.Hours;
                }
            }

            model.StudyClassList.Add(studyClassModel);
            foreach (var subject in requiredSubjects)
            {
                var requiredSubjectHours =
                    teachingProgramList.FirstOrDefault(sh => sh.SubjectId == subject.Id && sh.IsRequired);
                if (requiredSubjectHours != null)
                {
                    model.RequiredSubjectList.FirstOrDefault(s => s.Id == subject.Id).SubjectHoursTotal +=
                        requiredSubjectHours.Hours;
                    requiredSubjectProgram.Add(new SubjectProgramModel
                    {
                        StudyClassId = studyClass.Id,
                        Hours = requiredSubjectHours.Hours,
                        SubjectId = subject.Id
                    });
                }
            }
            foreach (var subject in formedSubjects)
            {
                var formedSubjectHours =
                    teachingProgramList.FirstOrDefault(sh => sh.SubjectId == subject.Id && !sh.IsRequired);
                if (formedSubjectHours != null)
                {
                    model.FormedSubjectList.FirstOrDefault(s => s.Id == subject.Id).SubjectHoursTotal +=
                        formedSubjectHours.Hours;
                    formedSubjectProgram.Add(new SubjectProgramModel
                    {
                        StudyClassId = studyClass.Id,
                        Hours = formedSubjectHours.Hours,
                        SubjectId = subject.Id
                    });
                }
            }

            model.RequiredSubjectProgramModel = requiredSubjectProgram;
            model.FormedSubjectProgramModel = formedSubjectProgram;
        }

        return model;
    }

    private async Task<ExtraProgramModel> PrepareExtraProgramsModel()
    {
        var model = new ExtraProgramModel();
        var extraSubjectList = await _sadDirectorService.GetExtraSubjectListAsync();
        foreach (var subject in extraSubjectList)
        {
            model.ExtraSubjects.Add(new SubjectModel
            {
                Name = subject.FullName,
                ShortName = subject.ShortName,
                Id = subject.Id,
                SubjectHoursTotal = 0
            });
        }
        var studyClassList = await _sadDirectorService.GetStudyClassListAsync();
        var extraSubjectProgram = new List<SubjectProgramModel>();
        foreach (var studyClass in studyClassList)
        {
            var studyClassModel = new StudyClassModel
            {
                Id = studyClass.Id,
                Name = studyClass.Name,
                StudyLevel = studyClass.StudyClassLevel
            };

            var extraProgramList = await _sadDirectorService.GetStudyClassExtraProgramAsync(studyClass.Id);
            foreach (var teachingProgram in extraProgramList)
            {
                studyClassModel.TotalRequiredHours += teachingProgram.Hours;
            }

            model.StudyClasses.Add(studyClassModel);
            foreach (var subject in extraSubjectList)
            {
                var extraSubjectHours =
                    extraProgramList.FirstOrDefault(sh => sh.ExtraSubjectId == subject.Id);
                if (extraSubjectHours != null)
                {
                    model.ExtraSubjects.FirstOrDefault(s => s.Id == subject.Id).SubjectHoursTotal +=
                        extraSubjectHours.Hours;
                    extraSubjectProgram.Add(new SubjectProgramModel
                    {
                        StudyClassId = studyClass.Id,
                        Hours = extraSubjectHours.Hours,
                        SubjectId = subject.Id
                    });
                }
            }

            model.ExtraSubjectPrograms = extraSubjectProgram;
        }
        return model;
    }
}