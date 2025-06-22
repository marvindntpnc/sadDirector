using Microsoft.VisualBasic;
using SadDirector.Domain.TeacherInfo.enums;

namespace SadDirector.Services;

public static class SadDirectorHelper
{
    public static string ConvertTeacherEducationToString(TeacherEducation teacherEducation)
    {
        switch (teacherEducation)
        {
            case TeacherEducation.Higher:
                return "Высшее";
            case TeacherEducation.MediumSpecial:
                return "Средне-специальное";
            default:
                return "Бакалавр";
        }
    }

    public static string ConvertTeacherCategoryToString(TeacherCategory teacherCategory)
    {
        switch (teacherCategory)
        {
            case TeacherCategory.First:
                return "1 категория";
            case TeacherCategory.Higher:
                return "Высшая";
            default:
                return "Специалист";
        }
    }

    public static string ConvertTeacherDegreeToString(TeacherDegree teacherDegree)
    {
        switch (teacherDegree)
        {
            case TeacherDegree.Director:
                return "Директор";
            case TeacherDegree.Methodist:
                return "Методист";
            default:
                return "";
        }
    }

    public static string GetTeacherExperienceTerm(DateTime experienceStartDate)
    {
        var experienceTermYear=DateAndTime.DateDiff(DateInterval.Year,experienceStartDate, DateTime.Now);
        if (DateTime.Now.Month < experienceStartDate.Month)
            experienceTermYear -= 1;
        
        var experienceTermMonth=DateAndTime.DateDiff(DateInterval.Month,experienceStartDate, DateTime.Now);
        
        var experienceTermDay=int.Abs(DateTime.Now.Day-experienceStartDate.Day);
        
        return $"{experienceTermYear}лет, {experienceTermMonth%12} месяцев, {experienceTermDay%30.5} дней";
    }

    public static string GetClassroomName(int classroomId)
    {
        switch (classroomId)
        {
            case 1:
                return "Спортзал";
            case 2:
                return "Библиотека";
            default:
                return "";
        }
    }
}