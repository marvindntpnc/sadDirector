using Microsoft.EntityFrameworkCore;
using SadDirector.Data;
using SadDirector.Domain;
using SadDirector.Domain.StudyClasses;
using SadDirector.Domain.TeacherInfo;
using SadDirector.Domain.TeachingPlan;
using SadDirector.Domain.TeachingPlan.enums;
using SadDirector.Models;
using Spire.Xls;


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

    public async Task<string?> GetSubjectNameByIdAsync(int subjectId,bool isExtra=false)
    {
        if (isExtra)
            return (await _dbContext.ExtraSubjects.FirstOrDefaultAsync(ex => ex.Id == subjectId))?.ShortName;

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

    public async Task<List<SubjectInfo>> GetTeacherInfosAsync(int teacherId,bool isRequired, bool isExtra) {

        return await _dbContext.SubjectInfos.Where(si => si.TeacherId == teacherId 
        && si.IsExtra== isExtra
        && si.IsRequired== isRequired).ToListAsync();
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

    public async Task<List<StudyClass>> GetStudyClassListAsync(StudyClassLevel? level=null)
    {
        if (level != null)
            return await _dbContext.StudyClasses.Where(sc=>sc.StudyClassLevel==level).ToListAsync();
        else
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

    public async Task<List<TeachingProgram>> GetStudyClassTeachingProgramAsync(int studyClassId,bool? isRequired=null)
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
        if (isRequired != null)
            return list.Where(p => p.IsRequired == isRequired).ToList();

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

    public async Task GenerateExcelDocumentAsync(ExportTariffModel model)
    {
        var userName = Environment.UserName;
        //Создание объекта Workbook 
        Workbook workbook = new Workbook();

        //Получение первой рабочей страницы
        Worksheet s = workbook.Worksheets[0];
        s.Name = model.TariffName;

        
        //Запись данных в определенные ячейки
        s.Range[1, 1, 100, 57].Style.Font.FontName = "Times New Roman";
        s.Range[1, 1, 100, 57].Style.Font.Size = 16;
        
        s.SetRowHeight(17,120);
        s.SetRowHeight(18,22);

        s.Range[17, 1].Value = "\u2116 п/п";
        s.Range[17, 1, 18, 1].Merge();
        
        s.Range[17, 2].Value = "Ф.И.О.";
        s.Range[17, 2, 18, 2].Merge();
        
        s.Range[17, 3].Value = "Образование";
        s.Range[17, 3, 18, 3].Merge();
        
        s.Range[17, 4].Value = "Категория";
        s.Range[17, 4, 18, 4].Merge();
        
        s.Range[17, 5].Value = "Тарифный разряд";
        s.Range[17, 5].Style.Rotation = 90;
        s.Range[17, 5, 18, 5].Merge();
        
        s.Range[17, 6].Value = "Педстаж";
        s.Range[17, 6, 18, 6].Merge();
        
        s.Range[17, 7].Value = "Звание";
        s.Range[17, 7, 18, 7].Merge();
        
        s.Range[17, 8].Value = "Предмет, должность";
        s.Range[17, 8, 18, 8].Merge();

        s.Range[17, 9].Value = "Обязательная  часть";
        s.Range[17,9,17,11].Merge();
        s.Range[18, 9].Value = "1\u22124";
        s.Range[18, 10].Style.NumberFormat = "Text";
        s.Range[18, 10].Value = "5\u22129";
        s.Range[18, 11].Style.NumberFormat = "Text";
        s.Range[18, 11].Value = "10\u221211";
        
        s.Range[17, 12].Value = "Часть, формируемая участниками образовательных отношений";
        s.Range[17,12,17,14].Merge();
        s.Range[18, 12].Style.NumberFormat = "Text";
        s.Range[18, 12].Value = "1\u22124";
        s.Range[18, 13].Style.NumberFormat = "Text";
        s.Range[18, 13].Value = "5\u22129";
        s.Range[18, 14].Style.NumberFormat = "Text";
        s.Range[18, 14].Value = "10\u221211";

        int column = 15;
        
        Dictionary<int,int> extraSubjectColumns = new Dictionary<int,int>();
        foreach (var extraSubject in model.ExtraSubjectList)
        {
            s.Range[17, column].Value = $"Внеурочная деятельность \"{extraSubject.FullName}\"";
            s.Range[17,column,17,column+2].Merge();
            s.Range[18, column].Style.NumberFormat = "Text";
            s.Range[18, column].Value = "1\u22124";
            s.Range[18, column+1].Style.NumberFormat = "Text";
            s.Range[18, column+1].Value = "5\u22129";
            s.Range[18, column+2].Style.NumberFormat = "Text";
            s.Range[18, column+2].Value = "10\u221211";
            
            extraSubjectColumns.Add(extraSubject.Id, column);
            column += 3;
        }
        
        int boolColumnStart=column;
        
        s.Range[17, column].Value = "Школьный музей";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;
        
        s.Range[17, column].Value = "Школьный театр";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;
        
        s.Range[17, column].Value = "Школьный хор";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;
        
        s.Range[17, column].Value = "Юнармия";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;
        
        s.Range[17, column].Value = "Cпортклуб";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Индивидуальное обучение на дому";
        s.Range[17,column,17,column+2].Merge();
        s.Range[18, column].Style.NumberFormat = "Text";
        s.Range[18, column].Value = "1\u22124";
        s.Range[18, column+1].Style.NumberFormat = "Text";
        s.Range[18, column+1].Value = "5\u22129";
        s.Range[18, column+2].Style.NumberFormat = "Text";
        s.Range[18, column+2].Value = "10\u221211";
        column += 3;


        s.Range[17, column].Value = "Всего";
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Директор";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Зам.директ.";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Воспит. ГПД";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Педагог - психолог";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Соц. педагог";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Педагог-организатор";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Зав.библиот.";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Учитель-логопед";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Классн.рук-во";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Зав.кабинетом";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Cоветник  по воспитанию";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Осн. / совм.";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        column++;

        s.Range[17, column].Value = "Подпись";
        s.Range[17, column].Style.Rotation = 90;
        s.Range[17, column, 18, column].Merge();
        
        s.Range[17, 1, 18, 57].Style.WrapText = true;

        for (int i = 1; i <= column; i++)
        {
            s.Range[19, i].Value = i.ToString();
        }
        
        s.Range[17,1,19,column].Style.VerticalAlignment = VerticalAlignType.Center;
        s.Range[17,1,19,column].Style.HorizontalAlignment = HorizontalAlignType.Center;

        int row = 20;
        int num = 1;
        foreach (var teacher in model.TeachersSummaryInfo)
        {
            s.Range[row, 1].Value = num.ToString();
            var subjectCount = new List<TeacherSubjects>();
            if (teacher.TeacherId > 0)
            {
                var teacherPersonalInfo = await GetTeacherByIdAsync(teacher.TeacherId);
                foreach (var subject in await GetTeacherSubjects(teacher.TeacherId))
                {
                    if (teacher.RequiredTeacherInfo.TeacherBeginnersInfo.TeacherInfo.Any(tp =>
                            tp.SubjectId == subject.SubjectId) ||
                        teacher.RequiredTeacherInfo.TeacherMiddleInfo.TeacherInfo.Any(tp =>
                            tp.SubjectId == subject.SubjectId) ||
                        teacher.RequiredTeacherInfo.TeacherHighInfo.TeacherInfo.Any(tp =>
                            tp.SubjectId == subject.SubjectId) ||
                        teacher.FormedTacherInfo.TeacherBeginnersInfo.TeacherInfo.Any(tp =>
                            tp.SubjectId == subject.SubjectId) ||
                        teacher.FormedTacherInfo.TeacherMiddleInfo.TeacherInfo.Any(tp =>
                            tp.SubjectId == subject.SubjectId) ||
                        teacher.FormedTacherInfo.TeacherHighInfo.TeacherInfo.Any(
                            tp => tp.SubjectId == subject.SubjectId))
                        subjectCount.Add(subject);
                }


                s.Range[row, 2].Value =
                    $"{teacherPersonalInfo.Surname} {teacherPersonalInfo.Name} {teacherPersonalInfo.SecondName}";
                s.Range[row, 3].Value =
                    SadDirectorHelper.ConvertTeacherEducationToString(teacherPersonalInfo.Education);
                s.Range[row, 4].Value =
                    SadDirectorHelper.ConvertTeacherCategoryToString(teacherPersonalInfo.TeacherCategory);
                s.Range[row, 5].Value = teacherPersonalInfo.TeacherTariffCategory.ToString();
                s.Range[row, 6].Value = SadDirectorHelper.GetTeacherExperienceTerm(teacherPersonalInfo.ExperienceFrom);
                s.Range[row, 7].Value =
                    SadDirectorHelper.ConvertTeacherDegreeToString(teacherPersonalInfo.TeacherDegree);

                int teacherBoolColumn = boolColumnStart;
                s.Range[row, teacherBoolColumn].Value = teacherPersonalInfo.Museum ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.Theater ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.Chorus ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.Scouts ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.SportClub ? "1" : "0";

                teacherBoolColumn += 5;
                s.Range[row, teacherBoolColumn].Value = teacherPersonalInfo.IsDirector ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.IsHeadTeacher ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.IsFacilitator ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.IsPsychologist ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.IsSocial ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.AfterClassesTeacher ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.IsLibraryManager ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.IsLogopedist ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = (await GetStudyClassByIdAsync(teacherPersonalInfo.StudyClassId))?.Name;
                s.Range[row, ++teacherBoolColumn].Value = SadDirectorHelper.GetClassroomName(teacherPersonalInfo.ClassroomId);
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.IsMentor ? "1" : "0";
                s.Range[row, ++teacherBoolColumn].Value = teacherPersonalInfo.IsMain ? "осн." : "совм.";
               
            }
            else
            {
                subjectCount = GetVacancySubjectList(teacher);
                s.Range[row, 2].Value = "Вакансия";
                //s.Range[row,2,row+subjectCount.Count-1,7].Merge();
            }

            //Dictionary<subjectId,rowNum>
                Dictionary<int,int> subjectsRowNums = new Dictionary<int,int>();
                int subjectRowNum = row;

                if (teacher.RequiredTeacherInfo.TeacherBeginnersInfo.TeacherInfo.Count>0)
                {
                    (subjectsRowNums,subjectRowNum)=FillTeacherSubjectInfo(s,
                        subjectsRowNums,
                        subjectRowNum,
                        StudyClassLevel.Beginner,
                        true,
                        teacher.RequiredTeacherInfo.TeacherBeginnersInfo.TeacherInfo.GroupBy(p => p.SubjectId));
                    
                }

                if (teacher.RequiredTeacherInfo.TeacherMiddleInfo.TeacherInfo.Count > 0)
                {
                    (subjectsRowNums,subjectRowNum)=FillTeacherSubjectInfo(s,
                        subjectsRowNums,
                        subjectRowNum,
                        StudyClassLevel.Middle,
                        true,
                        teacher.RequiredTeacherInfo.TeacherMiddleInfo.TeacherInfo.GroupBy(p => p.SubjectId));
                    
                }

                if (teacher.RequiredTeacherInfo.TeacherHighInfo.TeacherInfo.Count > 0)
                {
                    (subjectsRowNums,subjectRowNum)=FillTeacherSubjectInfo(s,
                        subjectsRowNums,
                        subjectRowNum,
                        StudyClassLevel.High,
                        true,
                        teacher.RequiredTeacherInfo.TeacherHighInfo.TeacherInfo.GroupBy(p => p.SubjectId));
                }
                
                if (teacher.FormedTacherInfo.TeacherBeginnersInfo.TeacherInfo.Count > 0)
                {
                    (subjectsRowNums,subjectRowNum)=FillTeacherSubjectInfo(s,
                        subjectsRowNums,
                        subjectRowNum,
                        StudyClassLevel.Beginner,
                        false,
                        teacher.FormedTacherInfo.TeacherBeginnersInfo.TeacherInfo.GroupBy(p => p.SubjectId));
                }
                
                if (teacher.FormedTacherInfo.TeacherMiddleInfo.TeacherInfo.Count > 0)
                {
                    (subjectsRowNums,subjectRowNum)=FillTeacherSubjectInfo(s,
                        subjectsRowNums,
                        subjectRowNum,
                        StudyClassLevel.Middle,
                        false,
                        teacher.FormedTacherInfo.TeacherMiddleInfo.TeacherInfo.GroupBy(p => p.SubjectId));
                }
                
                if (teacher.FormedTacherInfo.TeacherBeginnersInfo.TeacherInfo.Count > 0)
                {
                    (subjectsRowNums,subjectRowNum)=FillTeacherSubjectInfo(s,
                        subjectsRowNums,
                        subjectRowNum,
                        StudyClassLevel.High,
                        false,
                        teacher.FormedTacherInfo.TeacherHighInfo.TeacherInfo.GroupBy(p => p.SubjectId));
                }

                if (teacher.ExtraTeacherInfo.TeacherBeginnersInfo.TeacherInfo.Count > 0)
                {
                    foreach (var extraSubject in teacher.ExtraTeacherInfo.TeacherBeginnersInfo.TeacherInfo)
                    {
                        int colNum=extraSubjectColumns.FirstOrDefault(esc=>esc.Key==extraSubject.SubjectId).Value;
                        
                        s.Range[row, colNum].Value = teacher.ExtraTeacherInfo.TeacherBeginnersInfo.HoursSummary.ToString();
                        
                        if (subjectCount.Count > 1)
                            s.Range[row, colNum, row + subjectCount.Count - 1, colNum].Merge();
                    }
                }
                if (teacher.ExtraTeacherInfo.TeacherMiddleInfo.TeacherInfo.Count > 0)
                {
                    foreach (var extraSubject in teacher.ExtraTeacherInfo.TeacherMiddleInfo.TeacherInfo)
                    {
                        int colNum=extraSubjectColumns.FirstOrDefault(esc=>esc.Key==extraSubject.SubjectId).Value+1;
                        
                        s.Range[row, colNum].Value = teacher.ExtraTeacherInfo.TeacherMiddleInfo.HoursSummary.ToString();
                        
                        if (subjectCount.Count > 1)
                            s.Range[row, colNum, row + subjectCount.Count - 1, colNum].Merge();
                    }
                }
                if (teacher.ExtraTeacherInfo.TeacherHighInfo.TeacherInfo.Count > 0)
                {
                    foreach (var extraSubject in teacher.ExtraTeacherInfo.TeacherHighInfo.TeacherInfo)
                    {
                        int colNum=extraSubjectColumns.FirstOrDefault(esc=>esc.Key==extraSubject.SubjectId).Value+2;
                        
                        s.Range[row, colNum].Value = teacher.ExtraTeacherInfo.TeacherHighInfo.HoursSummary.ToString();
                        
                        if (subjectCount.Count > 1)
                            s.Range[row, colNum, row + subjectCount.Count - 1, colNum].Merge();
                    }
                }
                
                
                if (subjectCount.Count>1)
                {
                    s.Range[row,1,row+subjectCount.Count-1,1].Merge();
                    s.Range[row,2,row+subjectCount.Count-1,2].Merge();
                    s.Range[row,3,row+subjectCount.Count-1,3].Merge();
                    s.Range[row,4,row+subjectCount.Count-1,4].Merge();
                    s.Range[row,5,row+subjectCount.Count-1,5].Merge();
                    s.Range[row,6,row+subjectCount.Count-1,6].Merge();
                    s.Range[row,7,row+subjectCount.Count-1,7].Merge();
                }
    
                num++;
                row += subjectCount.Count;
        }
        
        s = FormDocumentHeader(model,s, column);

        for (int i = 20; i < row; i++)
        {
            s.SetRowHeight(i,120);
            //s.Range[i,boolColumnStart+8].Formula=$"=Sum(R{i}C{9}:R{i}C{boolColumnStart+9})";
        }

        row++;
        /*for (int i = 8; i <= boolColumnStart+8; i++)
        {
             s.Range[row,i].Formula=$"=Sum(R{20}C{i}:R{row-1}C{i})";   
        }*/
        //Сохранение в файл Excel$"C:\\Users\\{userName}\\Downloads
        workbook.SaveToFile($"C:/Users/{userName}/Downloads/SadDirectorTariff - {model.TariffPeriod}.xlsx", ExcelVersion.Version2010);
    }

    private Worksheet SetColumnWidth(Worksheet s)
    {
        
        s.SetColumnWidth(1,5.57);
        s.SetColumnWidth(2,24.57);
        s.SetColumnWidth(3,12.29);
        s.SetColumnWidth(4,16.29);
        s.SetColumnWidth(5,9.71);
        s.SetColumnWidth(6,14);
        s.SetColumnWidth(7,13.57);
        s.SetColumnWidth(8,25);
        s.SetColumnWidth(9,10);
        s.SetColumnWidth(10,10);
        s.SetColumnWidth(11,10);
        s.SetColumnWidth(12,10);
        s.SetColumnWidth(13,10);
        s.SetColumnWidth(14,10);
        s.SetColumnWidth(15,10);
        s.SetColumnWidth(16,10);
        s.SetColumnWidth(17,10);
        s.SetColumnWidth(18,10);
        s.SetColumnWidth(19,10);
        s.SetColumnWidth(20,10);
        s.SetColumnWidth(21,10);
        s.SetColumnWidth(22,10);
        s.SetColumnWidth(23,10);
        s.SetColumnWidth(24,10);
        s.SetColumnWidth(25,10);
        s.SetColumnWidth(26,10);
        s.SetColumnWidth(27,10);
        s.SetColumnWidth(28,10);
        s.SetColumnWidth(29,10);
        s.SetColumnWidth(30,10);
        s.SetColumnWidth(31,10);
        s.SetColumnWidth(32,10);
        s.SetColumnWidth(33,10);
        s.SetColumnWidth(34,10);
        s.SetColumnWidth(35,10);
        s.SetColumnWidth(36,10);
        s.SetColumnWidth(37,10);
        s.SetColumnWidth(38,10);
        s.SetColumnWidth(39,10);
        s.SetColumnWidth(40,10);
        s.SetColumnWidth(41,10);
        s.SetColumnWidth(42,10);
        s.SetColumnWidth(43,10);
        s.SetColumnWidth(44,10);
        s.SetColumnWidth(45,10);
        s.SetColumnWidth(46,10);
        s.SetColumnWidth(47,10);
        s.SetColumnWidth(48,10);
        s.SetColumnWidth(49,10);
        s.SetColumnWidth(50,10);
        s.SetColumnWidth(51,10);
        s.SetColumnWidth(52,10);
        s.SetColumnWidth(53,10);
        s.SetColumnWidth(54,10);
        s.SetColumnWidth(55,10);
        s.SetColumnWidth(56,10);
        s.SetColumnWidth(57,12.57);

        return s;
    }

    private Worksheet FormDocumentHeader(ExportTariffModel model, Worksheet s, int columnCount)
    {
        s.SetRowHeight(1,19);
        
        s.Range[1, 1].Value = "ГОСУДАРСТВЕННОЕ БЮДЖЕТНОЕ ОБЩЕОБРАЗОВАТЕЛЬНОЕ УЧРЕЖДЕНИЕ \"ШКОЛА \u2116 20 ГОРОДСКОГО ОКРУГА ДОНЕЦК\" ДОНЕЦКОЙ НАРОДНОЙ РЕСПУБЛИКИ";
        s.Range[1, 1, 1, columnCount].Merge();
        s.Range[1, 1, 1, columnCount].Style.HorizontalAlignment = HorizontalAlignType.Center;
        
        s.Range[2, columnCount-8].Value = "Согласовано з ПК (Протокол \u2116           от            ) ";
        s.Range[4, columnCount-11].Value = "Председатель ПК";
        s.Range[4, columnCount-4].Value = "Н.Н. Чурляева";
        s.Range[6,columnCount/2].Value="ПРИКАЗ";
        s.Range[6,columnCount/2].Style.Font.IsBold = true;
        s.Range[6, columnCount-2].Value = "М.П.";
        s.Range[8, 1].Value = "от 30.08.2024 г.";
        s.Range[8, columnCount-13].Value = "\u2116 ___/кд/л__\n";
        s.Range[9, 1].Value = "Об утверждении тарификации учителей";
        s.Range[10, 1].Value = $"на {model.TariffName} {model.TariffPeriod} учебного года";
        s.Range[13, 2].Value = "ПРИКАЗЫВАЮ:";
        s.Range[13,2].Style.Font.IsBold = true;
        s.Range[15, 2].Value = $"1. Утвердить  тарификацию на  {model.TariffName} {model.TariffPeriod} учебного года со следующей педагогической нагрузкой:";
        s=SetColumnWidth(s);
        s.Range[1, 1, 16, 57].Style.WrapText = false;

        return s;
    }

    private (Dictionary<int,int>,int) FillTeacherSubjectInfo(Worksheet s,
        Dictionary<int,int> subjectsRowNums,
        int subjectRowNum,
        StudyClassLevel level,
        bool isRequired,
        IEnumerable<IGrouping<int,TeacherInfoModel>> subjectGroups)
    {
        int colNum = 9;
        switch (level)
        {   
            case StudyClassLevel.Middle:
                colNum = 10;
                break;
            case StudyClassLevel.High:
                colNum = 11;
                break;
        }

        if (!isRequired)
            colNum +=3;

        if (subjectGroups.ToList().Count>0)
        {
            foreach (var group in subjectGroups)
            {
                if (!subjectsRowNums.Any(sr => sr.Key == group.Key))
                {
                    if (subjectsRowNums.Any(sr => sr.Value == subjectRowNum))
                        subjectsRowNums.Add(group.Key, subjectsRowNums.Max(st => st.Value) + 1);
                    else
                        subjectsRowNums.Add(group.Key, subjectRowNum);
                }
                    
                
                subjectRowNum=subjectsRowNums.FirstOrDefault(sr=>sr.Key==group.Key).Value;
                    
                s.Range[subjectRowNum, 8].Value = group.ToList().FirstOrDefault().SubjectName;
                s.Range[subjectRowNum, colNum].Value = group.ToList().Sum(sh=>sh.SubjectHours).ToString();
                subjectRowNum++;
            }
        }
        return (subjectsRowNums,subjectRowNum);
    }

    private List<TeacherSubjects> GetVacancySubjectList(TeacherSummaryModel teacher)
    {
        var list=new List<TeacherSubjects>();
        list = FormGroupedList(list, teacher.RequiredTeacherInfo.TeacherBeginnersInfo.TeacherInfo);
        list = FormGroupedList(list, teacher.RequiredTeacherInfo.TeacherMiddleInfo.TeacherInfo);
        list = FormGroupedList(list, teacher.RequiredTeacherInfo.TeacherHighInfo.TeacherInfo);
        list = FormGroupedList(list, teacher.FormedTacherInfo.TeacherBeginnersInfo.TeacherInfo);
        list = FormGroupedList(list, teacher.FormedTacherInfo.TeacherMiddleInfo.TeacherInfo);
        list = FormGroupedList(list, teacher.FormedTacherInfo.TeacherHighInfo.TeacherInfo);
        return list;
    }

    private List<TeacherSubjects> FormGroupedList(List<TeacherSubjects> list,List<TeacherInfoModel> info)
    {
        var groupedSubjects=info.GroupBy(i=>i.SubjectId);
        foreach (var i in groupedSubjects)
        {
            if (list.All(li => li.SubjectId != i.Key))
                list.Add(new TeacherSubjects { SubjectId = i.Key });
        }
        return list;
    }
}