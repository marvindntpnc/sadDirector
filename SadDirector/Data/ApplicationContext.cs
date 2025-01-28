using Microsoft.EntityFrameworkCore;
using SadDirector.Domain;
using SadDirector.Domain.TeacherInfo;
using SadDirector.Domain.TeachingPlan;

namespace SadDirector.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();  
    }
    public DbSet<Teacher> Teachers { get; set; } = null!;
    public DbSet<Tariff> Tariffs { get; set; } = null!;
    public DbSet<TeacherSubjects> TeachersSubjects{ get; set; } = null!;
    public DbSet<StudySubject> StudySubjects { get; set; } = null!;
    public DbSet<StudyClass> StudyClasses { get; set; } = null!;
    public DbSet<TeachingProgram> TeachingPrograms { get; set; } = null!;
    public DbSet<ExtraSubject?> ExtraSubjects { get; set; } = null!;
    public DbSet<ExtraSubjectProgram> ExtraSubjectPrograms { get; set; } = null!;
}