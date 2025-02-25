using Microsoft.AspNetCore.Mvc.Rendering;
using SadDirector.Domain;
using SadDirector.Domain.TeacherInfo.enums;
using SadDirector.Domain.TeachingPlan;
using SadDirector.Domain.TeachingPlan.enums;
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

    #region Teachers

    public async Task<TeacherListModel> PrepareTeacherListModelAsync(int tariffId)
    {
        var teacherList = await _sadDirectorService.GetTeacherListAsync();
        var model = new TeacherListModel
        {
            TariffId = tariffId,
            TeacherList = new List<TeacherModel>()
        };
        
        model.TeachersClassroom.Add(new SelectListItem
        {
            Value = "0",
            Text = "Нет"
        });
        model.TeachersClassroom.Add(new SelectListItem
        {
            Value = "1",
            Text = "Спортзал"
        });
        model.TeachersClassroom.Add(new SelectListItem
        {
            Value = "2",
            Text = "Библиотека"
        });
        for (int i = 1; i < 16; i++)
        {
            model.TeachersTariffCategory.Add(new SelectListItem
            {
                Value = i.ToString(),
                Text = i.ToString()
            });
        }
        model.TeachersDegrees.Add(new SelectListItem
        {
            Value = "0",
            Text = "Нет"
        });
        foreach (TeacherDegree degree in Enum.GetValues(typeof(TeacherDegree)))
        {
            string degreeName = string.Empty;
            
            switch (degree)
            {
                case TeacherDegree.Director:
                    degreeName = "Директор";
                    break;
                case TeacherDegree.Methodist:
                    degreeName = "Методист";
                    break;
            }
            model.TeachersDegrees.Add(new SelectListItem
            {
                Value = ((int)degree).ToString(),
                Text = degreeName
            });
        }

        foreach (TeacherCategory category in Enum.GetValues(typeof(TeacherCategory)))
        {
            string categoryName = string.Empty;
            
            switch (category)
            {
                case TeacherCategory.First:
                    categoryName = "Первая";
                    break;
                case TeacherCategory.Higher:
                    categoryName = "Высшая";
                    break;
                case TeacherCategory.Specialist:
                    categoryName = "Специалист";
                    break;
            }
            model.TeachersCategory.Add(new SelectListItem
            {
                Value = ((int)category).ToString(),
                Text = categoryName
            });
        }

        foreach (TeacherEducation education in Enum.GetValues(typeof(TeacherEducation)))
        {
            string educationName = string.Empty;
            
            switch (education)
            {
                case TeacherEducation.Bachelor:
                    educationName = "Бакалавр";
                    break;
                case TeacherEducation.MediumSpecial:
                    educationName = "Среднее-специальное";
                    break;
                case TeacherEducation.Higher:
                    educationName = "Высшее";
                    break;
            }
            model.TeachersEducationLevels.Add(new SelectListItem
            {
                Value = ((int)education).ToString(),
                Text = educationName
            });
        }
        
        var subjectList=await _sadDirectorService.GetSubjectListAsync();
        var studyClassesList = await _sadDirectorService.GetStudyClassListAsync();

        foreach (var subject in subjectList)
        {
            model.TeachersSubjects.Add(new SelectListItem
            {
                Value = subject.Id.ToString(),
                Text = subject.Name,
                Selected = false
            });
        }

        foreach (var studyClass in studyClassesList)
        {
            model.TeachersStudyClasses.Add(new SelectListItem
            {
                Value = studyClass.Id.ToString(),
                Text = studyClass.Name
            });
        }

        foreach (var teacher in teacherList)
        {
            var teacherSubjects = await _sadDirectorService.GetTeacherSubjects(teacher.Id);

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
                SubjectIds = teacherSubjects.Select(x => x.SubjectId).ToList(),
                TeacherSubjectList = string.Join(", " , teacherSubjects.Select(x => _sadDirectorService.GetSubjectNameByIdAsync(x.SubjectId).Result))
            });
        }
        return model;
    }

    #endregion

    #region Teaching Plan

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
                SubjectHoursTotal = 0,
                IsSeparated = subject.IsSeparated
            });
        }
        foreach (var subject in formedSubjects)
        {
            model.FormedSubjectList.Add(new SubjectModel
            {
                Name = subject.Name,
                Id = subject.Id,
                SubjectHoursTotal = 0,
                IsSeparated = subject.IsSeparated
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
                    model.RequiredSubjectList.FirstOrDefault(s => s.Id == subject.Id)!.SubjectHoursTotal +=
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
                    model.FormedSubjectList.FirstOrDefault(s => s.Id == subject.Id)!.SubjectHoursTotal +=
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
                Name = subject!.FullName,
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
                    extraProgramList.FirstOrDefault(sh => sh.ExtraSubjectId == subject!.Id);
                if (extraSubjectHours != null)
                {
                    model.ExtraSubjects.FirstOrDefault(s => s.Id == subject!.Id)!.SubjectHoursTotal +=
                        extraSubjectHours.Hours;
                    extraSubjectProgram.Add(new SubjectProgramModel
                    {
                        StudyClassId = studyClass.Id,
                        Hours = extraSubjectHours.Hours,
                        SubjectId = subject!.Id
                    });
                }
            }

            model.ExtraSubjectPrograms = extraSubjectProgram;
        }
        return model;
    }

    #endregion

    #region Study Classes

    public async Task<StudyClassListModel> PrepareStudyClassListModel(int tariffId)
    {
        var model = new StudyClassListModel
        {
            TariffId = tariffId
        };
        
        var studyClassList = await _sadDirectorService.GetStudyClassListAsync();
        foreach (var studyClass in studyClassList)
        {
            var studyClassInfo = new StudyClassInfoModel
            {
                StudyClassId = studyClass.Id,
                StudyClassName = studyClass.Name,
                StudentCount = studyClass.StudentCount
            };
            var studyClassSubjectList=await _sadDirectorService.GetStudyClassTeachingProgramAsync(studyClass.Id);
            foreach (var program in studyClassSubjectList)
            {
                var subjectInfoList = await _sadDirectorService.GetStudyClassSubjectInfo(studyClass.Id, program.SubjectId, program.IsRequired);
                var subjectInfo = new SubjectInfoModel();
                if (subjectInfoList.Count>1)
                {
                    var mainPart = subjectInfoList.FirstOrDefault(info => info.IsMainPart);
                    var secondaryPart = subjectInfoList.FirstOrDefault(info => !info.IsMainPart);

                    subjectInfo.SubjectId = program.SubjectId;
                    subjectInfo.CurrentHours = mainPart.CurrentHours;
                    subjectInfo.IsSeparated =
                        (await _sadDirectorService.GetSubjectByIdAsync(program.SubjectId))!.IsSeparated &&
                        studyClass.StudentCount >= 25;
                    subjectInfo.SubjectName =
                        await _sadDirectorService.GetSubjectNameByIdAsync(program.SubjectId) ?? "";
                    subjectInfo.PlanHours = program.Hours;
                    subjectInfo.TeacherId = mainPart.TeacherId;
                    subjectInfo.TeacherName =
                        await _sadDirectorService.GetTeacherFullNameByIdAsync(mainPart.TeacherId) ?? "";
                    subjectInfo.CurrentHoursSecondary=secondaryPart.CurrentHours;
                    subjectInfo.TeacherNameSecondary=await _sadDirectorService.GetTeacherFullNameByIdAsync(secondaryPart.TeacherId) ?? "";

                }
                else
                {
                    var info = subjectInfoList.FirstOrDefault();
                    
                    subjectInfo.SubjectId = program.SubjectId;
                    subjectInfo.CurrentHours = info.CurrentHours;
                    subjectInfo.IsSeparated = (await _sadDirectorService.GetSubjectByIdAsync(program.SubjectId))!.IsSeparated && studyClass.StudentCount>=25;
                    subjectInfo.SubjectName = await _sadDirectorService.GetSubjectNameByIdAsync(program.SubjectId)??"";
                    subjectInfo.PlanHours = program.Hours;
                    subjectInfo.TeacherId = info.TeacherId;
                    subjectInfo.TeacherName = await _sadDirectorService.GetTeacherFullNameByIdAsync(info.TeacherId)??"";
                }
                
                var subjectTeacherList =
                    await _sadDirectorService.GetTeacherListBySubjectIdAsync(program.SubjectId);
                foreach (var teacher in subjectTeacherList)
                {
                    subjectInfo.AvailableTeachers.Add(new SelectListItem
                    {
                        Text = await _sadDirectorService.GetTeacherFullNameByIdAsync(teacher.Id) ?? "",
                        Value = teacher.Id.ToString(),
                        Selected = subjectInfo.TeacherId == teacher.Id
                    });
                }


                if (program.IsRequired)
                {
                    studyClassInfo.StudyClassRequiredSubjects.Add(subjectInfo);
                }
                else
                {
                    studyClassInfo.StudyClassFormedSubjects.Add(subjectInfo);
                }
                
            }
            
            var studyClassExtraProgram=await _sadDirectorService.GetStudyClassExtraProgramAsync(studyClass.Id);
            foreach (var extra in studyClassExtraProgram)
            {
                var subjectInfoList = await _sadDirectorService.GetStudyClassSubjectInfo(studyClass.Id, extra.ExtraSubjectId,isExtra:true);
                foreach (var info in subjectInfoList)
                {
                    var subjectInfo = new SubjectInfoModel
                    {
                        SubjectId = extra.ExtraSubjectId,
                        CurrentHours = info.CurrentHours,
                        IsSeparated = false,
                        SubjectName = await _sadDirectorService.GetExtraSubjectNameByIdAsync(extra.ExtraSubjectId)??"",
                        PlanHours = extra.Hours,
                        TeacherId = info.TeacherId,
                        TeacherName = await _sadDirectorService.GetTeacherFullNameByIdAsync(info.TeacherId)??"",
                    };
                    var subjectTeacherList =
                        await _sadDirectorService.GetTeacherListAsync();
                    foreach (var teacher in subjectTeacherList)
                    {
                        subjectInfo.AvailableTeachers.Add(new SelectListItem
                        {
                            Text = await _sadDirectorService.GetTeacherFullNameByIdAsync(teacher.Id)??"",
                            Value = teacher.Id.ToString(),
                            Selected = info.TeacherId == teacher.Id
                        });
                    }
                    
                    studyClassInfo.StudyClassExtraSubjects.Add(subjectInfo);
                    
                }
            }
            
            model.StudyClasses.Add(studyClassInfo);
        }

        return model;
    }
    #endregion
}