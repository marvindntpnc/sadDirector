using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SadDirector.Data;
using SadDirector.Domain;
using SadDirector.Factories;
using SadDirector.Models;
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
    public async Task<IActionResult> CreateOrUpdateTariff(int tariffId)
    {
        var model = await _sadDirectorModelFactory.PrepareTariffModelAsync(tariffId);
        return View("~/Views/CreateOrUpdateTariff.cshtml",model);
    }

    public async Task<IActionResult> TeachersInfo(int tariffId)
    {
        var model = await _sadDirectorModelFactory.PrepareTeacherListModelAsync(tariffId);
        return View("~/Views/TeachersInfo.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateTeacher(TeacherModel model)
    {
        await _sadDirectorService.CreateOrUpdateTeacherAsync(model);
        return Json(new
        {
            success=true
        });
    }

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
}