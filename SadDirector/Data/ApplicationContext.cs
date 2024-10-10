using Microsoft.EntityFrameworkCore;
using SadDirector.Domain;

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
}