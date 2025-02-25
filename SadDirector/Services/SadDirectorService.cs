using Microsoft.EntityFrameworkCore;
using SadDirector.Data;
using SadDirector.Domain;
using SadDirector.Domain.StudyClasses;
using SadDirector.Domain.TeacherInfo;
using SadDirector.Domain.TeachingPlan;
using SadDirector.Domain.TeachingPlan.enums;
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
        return await _dbContext.Tariffs.FirstOrDefaultAsync(t => t.Id == id);
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

    public async Task<string?> GetSubjectNameByIdAsync(int subjectId)
    {
        return (await _dbContext.StudySubjects.FirstOrDefaultAsync(cs => cs.Id == subjectId))?.Name;
    }

    public async Task<string?> GetExtraSubjectNameByIdAsync(int subjectId)
    {
        return (await _dbContext.ExtraSubjects.FirstOrDefaultAsync(cs => cs.Id == subjectId))?.ShortName;
    }

    public async Task<StudySubject?> GetSubjectByIdAsync(int studySubjectId)
    {
        return await _dbContext.StudySubjects.FirstOrDefaultAsync(cs => cs.Id == studySubjectId);
    }

    public async Task<StudyClass?> GetStudyClassByIdAsync(int studyClassId)
    {
        return await _dbContext.StudyClasses.FirstOrDefaultAsync(cs => cs.Id == studyClassId);
    }

    public async Task<Teacher?> GetTeacherByIdAsync(int teacherId)
    {
        return await _dbContext.Teachers.FirstOrDefaultAsync(cs => cs.Id == teacherId);
    }

    public async Task<string?> GetTeacherFullNameByIdAsync(int teacherId)
    {
        if (teacherId > 0)
        {
            var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(cs => cs.Id == teacherId);
            return $"{teacher.Surname} {teacher.Name} {teacher.SecondName}";
        }

        return null;
    }

    public async Task<List<Teacher>?> GetTeacherListBySubjectIdAsync(int subjectId)
    {
        var teacherSubjectsList =
            await _dbContext.TeachersSubjects.Where(cs => cs.SubjectId == subjectId).ToListAsync();
        var list = new List<Teacher>();
        foreach (var teacherSubject in teacherSubjectsList)
        {
            list.Add(await GetTeacherByIdAsync(teacherSubject.TeacherId));
        }

        return list;
    }

    #region Teachers

    public async Task<List<Teacher>> GetTeacherListAsync()
    {
        return await _dbContext.Teachers.ToListAsync();
    }

    public async Task<List<TeacherSubjects>> GetTeacherSubjects(int teacherId)
    {
        return await _dbContext.TeachersSubjects.Where(t => t.TeacherId == teacherId).ToListAsync();
    }

    public async Task CreateOrUpdateTeacherAsync(TeacherModel model)
    {
        var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(t => t.Id == model.Id);
        if (teacher == null)
        {
            teacher = new Teacher();
        }

        teacher.Name = model.Name;
        teacher.Surname = model.Surname;
        teacher.SecondName = model.SecondName;
        teacher.EducationId = (int)model.TeacherEducation;
        teacher.TeacherCategoryId = (int)model.TeacherCategory;
        teacher.TeacherTariffCategory = model.TariffCategory;
        teacher.ExperienceFrom = model.ExperienceFrom;
        teacher.TeacherDegreeId = (int)model.TeacherDegree;
        teacher.StudyClassId = model.StudyClassId;
        teacher.ClassroomId = model.ClassroomId;
        teacher.IsDirector = model.IsDirector;
        teacher.IsHeadTeacher = model.IsHeadTeacher;
        teacher.IsMentor = model.IsMentor;
        teacher.AfterClassesTeacher = model.AfterClassesTeacher;
        teacher.IsPsychologist = model.IsPsychologist;
        teacher.IsSocial = model.IsSocial;
        teacher.IsFacilitator = model.IsFacilitator;
        teacher.IsLibraryManager = model.IsLibraryManager;
        teacher.IsLogopedist = model.IsLogopedist;
        teacher.IsMain = model.IsMain;
        teacher.Museum = model.Museum;
        teacher.Theater = model.Theater;
        teacher.Chorus = model.Chorus;
        teacher.Scouts = model.Scouts;
        teacher.SportClub = model.SportClub;


        if (model.Id == 0)
        {
            _dbContext.Teachers.Add(teacher);
        }
        else
        {
            _dbContext.Teachers.Update(teacher);
        }

        await _dbContext.SaveChangesAsync();

        var currentTeacherSubjects = _dbContext.TeachersSubjects.Where(t => t.TeacherId == teacher.Id).ToList();
        foreach (var teachersSubject in currentTeacherSubjects)
        {
            _dbContext.TeachersSubjects.Remove(teachersSubject);
        }

        foreach (var subjectId in model.SubjectIds)
        {
            _dbContext.TeachersSubjects.Add(new TeacherSubjects
            {
                TeacherId = teacher.Id,
                SubjectId = subjectId
            });
        }

        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region Teaching Plan

    public async Task<List<StudySubject>> GetSubjectListAsync(StudyClassLevel? studyLevel = null)
    {
        if (studyLevel != null)
        {
            switch (studyLevel)
            {
                case StudyClassLevel.Beginner:
                    return await _dbContext.StudySubjects
                        .Where(tp => tp.AssignAsFormedToBeginners || tp.AssignAsRequiredToBeginners).ToListAsync();
                case StudyClassLevel.Middle:
                    return await _dbContext.StudySubjects
                        .Where(tp => tp.AssignAsFormedToMiddle || tp.AssignAsRequiredToMiddle).ToListAsync();
                case StudyClassLevel.High:
                    return await _dbContext.StudySubjects
                        .Where(tp => tp.AssignAsFormedToHigh || tp.AssignAsRequiredToHigh).ToListAsync();
            }
        }

        return await _dbContext.StudySubjects.ToListAsync();
    }

    public async Task<List<StudyClass>> GetStudyClassListAsync()
    {
        return await _dbContext.StudyClasses.ToListAsync();
    }

    public async Task<List<ExtraSubject?>> GetExtraSubjectListAsync()
    {
        return await _dbContext.ExtraSubjects.ToListAsync();
    }

    public async Task<ExtraSubject?> GetExtraSubjectByIdAsync(int extraSubjectId)
    {
        return await _dbContext.ExtraSubjects.FirstOrDefaultAsync(es => es.Id == extraSubjectId);
    }

    public async Task<List<ExtraSubjectProgram>> GetExtraSubjectProgramByExtraSubjectIdAsync(int extraSubjectId)
    {
        return await _dbContext.ExtraSubjectPrograms.Where(es => es.ExtraSubjectId == extraSubjectId).ToListAsync();
    }

    public async Task AddNewStudyClassAsync(StudyClassModel model)
    {
        var studyClass = new StudyClass
        {
            Name = model.Name,
            StudyClassLevelId = (int)model.StudyLevel
        };
        _dbContext.StudyClasses.Add(studyClass);
        await _dbContext.SaveChangesAsync();
        var subjectsList = await GetSubjectListAsync(model.StudyLevel);
        var extraSubjectsList = await GetExtraSubjectListAsync();
        foreach (var studySubject in subjectsList)
        {
            switch (studyClass.StudyClassLevel)
            {
                case StudyClassLevel.Beginner:
                    if (studySubject.AssignAsRequiredToBeginners)
                    {
                        var teachingProgram = new TeachingProgram
                        {
                            StudyClassId = studyClass.Id,
                            SubjectId = studySubject.Id,
                            Hours = 0,
                            StudyClassLevel = studyClass.StudyClassLevel,
                            IsRequired = true
                        };
                        _dbContext.TeachingPrograms.Add(teachingProgram);
                    }

                    if (studySubject.AssignAsFormedToBeginners)
                    {
                        var teachingProgram = new TeachingProgram
                        {
                            StudyClassId = studyClass.Id,
                            SubjectId = studySubject.Id,
                            Hours = 0,
                            StudyClassLevel = studyClass.StudyClassLevel,
                            IsRequired = false
                        };
                        _dbContext.TeachingPrograms.Add(teachingProgram);
                    }

                    break;
                case StudyClassLevel.Middle:
                    if (studySubject.AssignAsRequiredToMiddle)
                    {
                        var teachingProgram = new TeachingProgram
                        {
                            StudyClassId = studyClass.Id,
                            SubjectId = studySubject.Id,
                            Hours = 0,
                            StudyClassLevel = studyClass.StudyClassLevel,
                            IsRequired = true
                        };
                        _dbContext.TeachingPrograms.Add(teachingProgram);
                    }

                    if (studySubject.AssignAsFormedToMiddle)
                    {
                        var teachingProgram = new TeachingProgram
                        {
                            StudyClassId = studyClass.Id,
                            SubjectId = studySubject.Id,
                            Hours = 0,
                            StudyClassLevel = studyClass.StudyClassLevel,
                            IsRequired = false
                        };
                        _dbContext.TeachingPrograms.Add(teachingProgram);
                    }

                    break;
                case StudyClassLevel.High:
                    if (studySubject.AssignAsRequiredToHigh)
                    {
                        var teachingProgram = new TeachingProgram
                        {
                            StudyClassId = studyClass.Id,
                            SubjectId = studySubject.Id,
                            Hours = 0,
                            StudyClassLevel = studyClass.StudyClassLevel,
                            IsRequired = true
                        };
                        _dbContext.TeachingPrograms.Add(teachingProgram);
                    }

                    if (studySubject.AssignAsFormedToHigh)
                    {
                        var teachingProgram = new TeachingProgram
                        {
                            StudyClassId = studyClass.Id,
                            SubjectId = studySubject.Id,
                            Hours = 0,
                            StudyClassLevel = studyClass.StudyClassLevel,
                            IsRequired = false
                        };
                        _dbContext.TeachingPrograms.Add(teachingProgram);
                    }

                    break;
            }
        }

        foreach (var extraSubject in extraSubjectsList)
        {
            var teachingProgram = new ExtraSubjectProgram
            {
                StudyClassId = studyClass.Id,
                ExtraSubjectId = extraSubject.Id,
                Hours = 0
            };
            _dbContext.ExtraSubjectPrograms.Add(teachingProgram);
        }

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

    public async Task AddNewExtraSubjectAsync(SubjectModel model)
    {
        var subject = new ExtraSubject
        {
            FullName = model.Name,
            ShortName = model.ShortName
        };
        _dbContext.ExtraSubjects.Add(subject);
        await _dbContext.SaveChangesAsync();
        var studyClassList = await GetStudyClassListAsync();
        foreach (var studyClass in studyClassList)
        {
            var teachingProgram = new ExtraSubjectProgram
            {
                StudyClassId = studyClass.Id,
                ExtraSubjectId = subject.Id,
                Hours = 0
            };
            _dbContext.ExtraSubjectPrograms.Add(teachingProgram);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<TeachingProgram>> GetStudyClassTeachingProgramAsync(int studyClassId)
    {
        var programsList=await _dbContext.TeachingPrograms.Where(tp => tp.StudyClassId == studyClassId).ToListAsync();
        var studyClass = await GetStudyClassByIdAsync(studyClassId);
        var list=new List<TeachingProgram>();
        foreach (var program in programsList)
        {
            var subject = await GetSubjectByIdAsync(program.SubjectId);
            switch (studyClass.StudyClassLevel)
            {
                case StudyClassLevel.Beginner:
                    if (!subject.AssignAsRequiredToBeginners &&
                        !subject.AssignAsFormedToBeginners)
                    {
                        _dbContext.TeachingPrograms.Remove(program);
                    }
                    else
                    {
                        list.Add(program);
                    }
                    break;
                case StudyClassLevel.Middle:
                    if (!subject.AssignAsRequiredToMiddle &&
                        !subject.AssignAsFormedToMiddle)
                    {
                        _dbContext.TeachingPrograms.Remove(program);
                    }
                    else
                    {
                        list.Add(program);
                    }
                    break;
                case StudyClassLevel.High:
                    if (!subject.AssignAsRequiredToHigh &&
                        !subject.AssignAsFormedToHigh)
                    {
                        _dbContext.TeachingPrograms.Remove(program);
                    }
                    else
                    {
                        list.Add(program);
                    }
                    break;
            }
            await _dbContext.SaveChangesAsync();
        }

        return list;
    }

    public async Task<List<ExtraSubjectProgram>> GetStudyClassExtraProgramAsync(int studyClassId)
    {
        return await _dbContext.ExtraSubjectPrograms.Where(tp => tp.StudyClassId == studyClassId).ToListAsync();
    }

    public async Task DeleteSubjectProgramAsync(int subjectId, StudyClassLevel studyLevel)
    {
        var subjectPrograms = await _dbContext.TeachingPrograms
            .Where(tp => tp.SubjectId == subjectId && tp.StudyClassLevel == studyLevel).ToListAsync();
        foreach (var subjectProgram in subjectPrograms)
        {
            subjectProgram.Hours = 0;
            _dbContext.Update(subjectProgram);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateSubjectProgramAsync(SubjectProgramModel[] subjectPrograms,bool isSeparated)
    {
        var subject=await GetSubjectByIdAsync(subjectPrograms.FirstOrDefault().SubjectId);
        if (subject != null)
        {
            subject.IsSeparated = isSeparated;
            _dbContext.Update(subject);
        }

        foreach (var subjectProgram in subjectPrograms)
        {
            var teachingProgram = await _dbContext.TeachingPrograms.FirstOrDefaultAsync(tp =>
                tp.StudyClassId == subjectProgram.StudyClassId &&
                tp.SubjectId == subjectProgram.SubjectId &&
                tp.IsRequired == subjectProgram.IsRequired);
            if (teachingProgram != null)
            {
                teachingProgram.Hours = subjectProgram.Hours;
                _dbContext.Update(teachingProgram);
            }
            else
            {
                var studyClass =
                    await _dbContext.StudyClasses.FirstOrDefaultAsync(sc => sc.Id == subjectProgram.StudyClassId);
                _dbContext.TeachingPrograms.Add(new TeachingProgram
                {
                    StudyClassId = subjectProgram.StudyClassId,
                    SubjectId = subjectProgram.SubjectId,
                    IsRequired = subjectProgram.IsRequired,
                    Hours = subjectProgram.Hours,
                    StudyClassLevel = studyClass.StudyClassLevel
                });
            }
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateExtraSubjectProgramAsync(SubjectProgramModel[] subjectPrograms)
    {
        foreach (var subjectProgram in subjectPrograms)
        {
            var teachingProgram = await _dbContext.ExtraSubjectPrograms.FirstOrDefaultAsync(tp =>
                tp.StudyClassId == subjectProgram.StudyClassId &&
                tp.ExtraSubjectId == subjectProgram.ExtraSubjectId);
            if (teachingProgram != null)
            {
                teachingProgram.Hours = subjectProgram.Hours;
                _dbContext.Update(teachingProgram);
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteExtraSubjectAsync(int extraSubjectId)
    {
        var extraSubject = await GetExtraSubjectByIdAsync(extraSubjectId);
        var extraSubjectProgram = await GetExtraSubjectProgramByExtraSubjectIdAsync(extraSubjectId);
        if (extraSubject != null)
            _dbContext.Remove(extraSubject);

        if (extraSubjectProgram != null && extraSubjectProgram.Count > 0)
            _dbContext.RemoveRange(extraSubjectProgram);

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteStudyClassAsync(int classId)
    {
        var studyClass = await _dbContext.StudyClasses.FirstOrDefaultAsync(sc => sc.Id == classId);
        if (studyClass != null)
        {
            _dbContext.StudyClasses.Remove(studyClass);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task UpdateSubjectsListAsync(int[] requiredSubjectIds, int[] formedSubjectIds,
        StudyClassLevel studyLevel)
    {
        var subjectList = await GetSubjectListAsync();
        foreach (var subject in subjectList)
        {
            switch (studyLevel)
            {
                case StudyClassLevel.Beginner:
                    subject.AssignAsRequiredToBeginners = false;
                    subject.AssignAsFormedToBeginners = false;
                    _dbContext.Update(subject);
                    break;
                case StudyClassLevel.Middle:
                    subject.AssignAsRequiredToMiddle = false;
                    subject.AssignAsFormedToMiddle = false;
                    _dbContext.Update(subject);
                    break;
                case StudyClassLevel.High:
                    subject.AssignAsRequiredToHigh = false;
                    subject.AssignAsFormedToHigh = false;
                    _dbContext.Update(subject);
                    break;
            }
        }

        foreach (var requiredSubjectId in requiredSubjectIds)
        {
            var requiredSubject = subjectList.FirstOrDefault(sc => sc.Id == requiredSubjectId);
            if (requiredSubject != null)
            {
                var classes = new List<StudyClass>();
                switch (studyLevel)
                {
                    case StudyClassLevel.Beginner:
                        requiredSubject.AssignAsRequiredToBeginners = true;
                        classes = (await GetStudyClassListAsync())
                            .Where(sc => sc.StudyClassLevel == StudyClassLevel.Beginner).ToList();

                        foreach (var studyClass in classes)
                        {
                            var studyClassTeachingProgram = await _dbContext.TeachingPrograms.FirstOrDefaultAsync(
                                sctp =>
                                    sctp.SubjectId == requiredSubjectId &&
                                    sctp.StudyClassId == studyClass.Id &&
                                    sctp.IsRequired == true);
                            if (studyClassTeachingProgram == null)
                            {
                                _dbContext.TeachingPrograms.Add(new TeachingProgram
                                {
                                    StudyClassId = studyClass.Id,
                                    SubjectId = requiredSubjectId,
                                    Hours = 0,
                                    StudyClassLevel = studyClass.StudyClassLevel,
                                    IsRequired = true
                                });
                            }
                        }

                        _dbContext.Update(requiredSubject);
                        break;
                    case StudyClassLevel.Middle:
                        requiredSubject.AssignAsRequiredToMiddle = true;
                        classes = (await GetStudyClassListAsync())
                            .Where(sc => sc.StudyClassLevel == StudyClassLevel.Beginner).ToList();
                        foreach (var studyClass in classes)
                        {
                            var studyClassTeachingProgram = await _dbContext.TeachingPrograms.FirstOrDefaultAsync(
                                sctp =>
                                    sctp.SubjectId == requiredSubjectId &&
                                    sctp.StudyClassId == studyClass.Id &&
                                    sctp.IsRequired == true);
                            if (studyClassTeachingProgram == null)
                            {
                                _dbContext.TeachingPrograms.Add(new TeachingProgram
                                {
                                    StudyClassId = studyClass.Id,
                                    SubjectId = requiredSubjectId,
                                    Hours = 0,
                                    StudyClassLevel = studyClass.StudyClassLevel,
                                    IsRequired = true
                                });
                            }
                        }

                        _dbContext.Update(requiredSubject);
                        break;
                    case StudyClassLevel.High:
                        requiredSubject.AssignAsRequiredToHigh = true;
                        classes = (await GetStudyClassListAsync())
                            .Where(sc => sc.StudyClassLevel == StudyClassLevel.Beginner).ToList();
                        foreach (var studyClass in classes)
                        {
                            var studyClassTeachingProgram = await _dbContext.TeachingPrograms.FirstOrDefaultAsync(
                                sctp =>
                                    sctp.SubjectId == requiredSubjectId &&
                                    sctp.StudyClassId == studyClass.Id &&
                                    sctp.IsRequired == true);
                            if (studyClassTeachingProgram == null)
                            {
                                _dbContext.TeachingPrograms.Add(new TeachingProgram
                                {
                                    StudyClassId = studyClass.Id,
                                    SubjectId = requiredSubjectId,
                                    Hours = 0,
                                    StudyClassLevel = studyClass.StudyClassLevel,
                                    IsRequired = true
                                });
                            }
                        }

                        _dbContext.Update(requiredSubject);
                        break;
                }
            }
        }

        foreach (var formedSubjectId in formedSubjectIds)
        {
            var formedSubject = subjectList.FirstOrDefault(sc => sc.Id == formedSubjectId);
            if (formedSubject != null)
            {
                var classes = new List<StudyClass>();
                switch (studyLevel)
                {
                    case StudyClassLevel.Beginner:
                        formedSubject.AssignAsFormedToBeginners = true;
                        classes = (await GetStudyClassListAsync())
                            .Where(sc => sc.StudyClassLevel == StudyClassLevel.Beginner).ToList();
                        foreach (var studyClass in classes)
                        {
                            var studyClassTeachingProgram = await _dbContext.TeachingPrograms.FirstOrDefaultAsync(
                                sctp =>
                                    sctp.SubjectId == formedSubjectId &&
                                    sctp.StudyClassId == studyClass.Id &&
                                    sctp.IsRequired == false);
                            if (studyClassTeachingProgram == null)
                            {
                                _dbContext.TeachingPrograms.Add(new TeachingProgram
                                {
                                    StudyClassId = studyClass.Id,
                                    SubjectId = formedSubjectId,
                                    Hours = 0,
                                    StudyClassLevel = studyClass.StudyClassLevel,
                                    IsRequired = false
                                });
                            }
                        }

                        _dbContext.Update(formedSubject);
                        break;
                    case StudyClassLevel.Middle:
                        formedSubject.AssignAsFormedToMiddle = true;
                        classes = (await GetStudyClassListAsync())
                            .Where(sc => sc.StudyClassLevel == StudyClassLevel.Middle).ToList();
                        foreach (var studyClass in classes)
                        {
                            var studyClassTeachingProgram = await _dbContext.TeachingPrograms.FirstOrDefaultAsync(
                                sctp =>
                                    sctp.SubjectId == formedSubjectId &&
                                    sctp.StudyClassId == studyClass.Id &&
                                    sctp.IsRequired == false);
                            if (studyClassTeachingProgram == null)
                            {
                                _dbContext.TeachingPrograms.Add(new TeachingProgram
                                {
                                    StudyClassId = studyClass.Id,
                                    SubjectId = formedSubjectId,
                                    Hours = 0,
                                    StudyClassLevel = studyClass.StudyClassLevel,
                                    IsRequired = false
                                });
                            }
                        }

                        _dbContext.Update(formedSubject);
                        break;
                    case StudyClassLevel.High:
                        formedSubject.AssignAsFormedToHigh = true;
                        classes = (await GetStudyClassListAsync())
                            .Where(sc => sc.StudyClassLevel == StudyClassLevel.High).ToList();
                        foreach (var studyClass in classes)
                        {
                            var studyClassTeachingProgram = await _dbContext.TeachingPrograms.FirstOrDefaultAsync(
                                sctp =>
                                    sctp.SubjectId == formedSubjectId &&
                                    sctp.StudyClassId == studyClass.Id &&
                                    sctp.IsRequired == false);
                            if (studyClassTeachingProgram == null)
                            {
                                _dbContext.TeachingPrograms.Add(new TeachingProgram
                                {
                                    StudyClassId = studyClass.Id,
                                    SubjectId = formedSubjectId,
                                    Hours = 0,
                                    StudyClassLevel = studyClass.StudyClassLevel,
                                    IsRequired = false
                                });
                            }
                        }

                        _dbContext.Update(formedSubject);
                        break;
                }
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    #endregion

    #region StudyClasses

    public async Task<List<SubjectInfo>> GetStudyClassSubjectInfo(int studyClassId, int subjectId, bool isRequired=false,bool isExtra=false)
    {
        var studyClass = await GetStudyClassByIdAsync(studyClassId);
        var isSeparated = (await GetSubjectByIdAsync(subjectId))!.IsSeparated && studyClass.StudentCount >= 25;
        List<SubjectInfo> list = await _dbContext.SubjectInfos
            .Where(si => si.StudyClassId == studyClassId &&
                         si.SubjectId == subjectId &&
                         si.IsRequired==isRequired).ToListAsync();
        if (list.Count == 0)
        {
            var info = new SubjectInfo
            {
                SubjectId = subjectId,
                StudyClassId = studyClassId,
                IsMainPart = true,
                IsRequired = isRequired,
                IsExtra = isExtra
            };
            _dbContext.SubjectInfos.Add(info);

            if (isSeparated && !isExtra)
            {
                var secondary = new SubjectInfo
                {
                    SubjectId = subjectId,
                    StudyClassId = studyClassId,
                    IsMainPart = false,
                    IsRequired = isRequired
                };
                _dbContext.SubjectInfos.Add(secondary);
            }

            await _dbContext.SaveChangesAsync();
            list.Add(info);
        }

        return list;
    }

    public async Task UpdateStudyClassSubjectInfoAsync(SubjectInfoModel model,int studyClassId, int studentsCount, bool isRequired,bool isExtra)
    {
        var info = await _dbContext.SubjectInfos
            .FirstOrDefaultAsync(si => si.StudyClassId == studyClassId &&
                                       si.SubjectId == model.SubjectId &&
                                       si.IsMainPart==model.IsMain &&
                                       si.IsRequired==isRequired &&
                                       si.IsExtra==isExtra);

        var studyClass = await GetStudyClassByIdAsync(studyClassId);
        if (studyClass != null)
        {
            studyClass.StudentCount = studentsCount;
            _dbContext.Update(studyClass);
            await _dbContext.SaveChangesAsync();
        }

        if (info!=null)
        {
            info.TeacherId = model.TeacherId;
            info.IsMainPart = model.IsMain;
            info.CurrentHours = model.CurrentHours;
            info.IsRequired = isRequired;
            
            
            _dbContext.Update(info);
            await _dbContext.SaveChangesAsync();
        }
    }

    #endregion
}