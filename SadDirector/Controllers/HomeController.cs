﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SadDirector.Data;
using SadDirector.Domain;
using SadDirector.Domain.TeachingPlan.enums;
using SadDirector.Factories;
using SadDirector.Models;
using SadDirector.Models.Lists;
using SadDirector.Services;

namespace SadDirector.Controllers;

public class HomeController : Controller
{
    private readonly SadDirectorModelFactory _sadDirectorModelFactory;
    private readonly SadDirectorService _sadDirectorService;

    public HomeController(
        SadDirectorModelFactory sadDirectorModelFactory,
        SadDirectorService sadDirectorService)
    {
        
        _sadDirectorModelFactory = sadDirectorModelFactory;
        _sadDirectorService = sadDirectorService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _sadDirectorModelFactory.PrepareTariffListModelAsync();
        return View("~/Views/Index.cshtml",model);
    }
    public async Task<IActionResult> AddTariff()
    {
        var tariff=await _sadDirectorService.AddNewTariffAsync();
        return RedirectToAction("CreateOrUpdateTariff",new {tariffId=tariff.Id});
    }
    public async Task<IActionResult> CreateOrUpdateTariff(int tariffId,bool isDownloaded=false)
    {
        var model = await _sadDirectorModelFactory.PrepareTariffModelAsync(tariffId);
        model.IsDownloaded=isDownloaded;
        return View("~/Views/CreateOrUpdateTariff.cshtml",model);
    }

    #region Teachers

    public async Task<IActionResult> TeachersInfo(int tariffId)
    {
        var model = await _sadDirectorModelFactory.PrepareTeacherListModelAsync(tariffId);
        return View("~/Views/TeachersInfo.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateTeacher(TeacherModel model)
    {
        try
        {
            await _sadDirectorService.CreateOrUpdateTeacherAsync(model);
            return Json(new
            {
                success=true
            });
        }
        catch (Exception e)
        {
            return Json(new
            {
                success=false,
                error=e.Message
            });
        }
    }

    #endregion
    
    #region Teaching Plan
    public async Task<IActionResult> TeachingPlan(int tariffId)
    {
        var model = await _sadDirectorModelFactory.PrepareTeachingPlanModelAsync(tariffId);
        return View("~/Views/TeachingPlan.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> CreateNewStudyClass(StudyClassModel model)
    {
        await _sadDirectorService.AddNewStudyClassAsync(model);
        return Json(new
        {
            success = true
        });
    }
    [HttpPost]
    public async Task<IActionResult> CreateNewStudySubject(SubjectModel model)
    {
        await _sadDirectorService.AddNewSubjectAsync(model);
        return Json(new
        {
            success = true
        });
    }
    [HttpPost]
    public async Task<IActionResult> CreateNewExtraSubject(SubjectModel model)
    {
        await _sadDirectorService.AddNewExtraSubjectAsync(model);
        return Json(new
        {
            success = true
        });
    }
    [HttpPost]
    public async Task<IActionResult> DeleteSubjectProgram(int subjectId, StudyClassLevel studyLevel)
    {
        await _sadDirectorService.DeleteSubjectProgramAsync(subjectId,studyLevel);
        return Json(new
        {
            success = true
        });
    }
    [HttpPost]
    public async Task<IActionResult> UpdateSubjectProgram(SubjectProgramModel[] subjectPrograms,bool isSeparated)
    {
        await _sadDirectorService.UpdateSubjectProgramAsync(subjectPrograms,isSeparated);
        return Json(new
        {
            success = true
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateExtraSubjectProgram(SubjectProgramModel[] subjectPrograms)
    {
        await _sadDirectorService.UpdateExtraSubjectProgramAsync(subjectPrograms);
        return Json(new
        {
            success = true
        });
    }
    [HttpPost]
    public async Task<IActionResult> DeleteExtraSubject(int extraSubjectId)
    {
        await _sadDirectorService.DeleteExtraSubjectAsync(extraSubjectId);
        return Json(new
        {
            success = true
        });
    }
    [HttpPost]
    public async Task<IActionResult> DeleteStudyClass(int classId)
    {
        await _sadDirectorService.DeleteStudyClassAsync(classId);
        return Json(new
        {
            success = true
        });
    }
    [HttpPost]
    public async Task<IActionResult> UpdateSubjectsList(int[] requiredSubjectIds, int[] formedSubjectIds,StudyClassLevel studyLevel)
    {
        await _sadDirectorService.UpdateSubjectsListAsync(requiredSubjectIds,formedSubjectIds, studyLevel);
        return Json(new
        {
            success = true
        });
    }
    #endregion
    
    #region Study Classes
    
    public async Task<IActionResult> StudyClasses(int tariffId)
    {
        var model = await _sadDirectorModelFactory.PrepareStudyClassListModel(tariffId);
        return View("~/Views/StudyClasses.cshtml",model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStudyClassSubjectInfo(SubjectInfoModel[] model, int studyClassId,int studentsCount, bool isRequired,bool isExtra)
    {
        foreach (var info in model)
        {
            await _sadDirectorService.UpdateStudyClassSubjectInfoAsync(info,studyClassId,studentsCount,isRequired,isExtra);
        }
        return Json(new
        {
            success = true
        });
    }

    #endregion

    public async Task<IActionResult> GenerateTariffExcelDocument(int tariffId)
    {
        try
        {
            var model=await _sadDirectorModelFactory.PrepareExportTariffModelAsync(tariffId);
            await _sadDirectorService.GenerateExcelDocumentAsync(model);
            return RedirectToAction("CreateOrUpdateTariff",new{tariffId=tariffId,isDownloaded=true});
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}