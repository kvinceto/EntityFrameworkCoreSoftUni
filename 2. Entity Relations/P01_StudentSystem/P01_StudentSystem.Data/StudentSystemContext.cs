namespace P01_StudentSystem.Data
{
    using Microsoft.EntityFrameworkCore;
    using P01_StudentSystem.Data.Models;

    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {

        }

        public StudentSystemContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Student> Students { get; set; } = null!;

        public DbSet<Course> Courses { get; set; } = null!;

        public DbSet<Resource> Resources { get; set; } = null!;

        public DbSet<Homework> Homeworks { get; set; } = null!;

        public DbSet<StudentCourse> StudentsCourses { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.Config.ConectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentId);
                entity
                .Property(s => s.Name)
                .HasColumnType("varchar(100)");
                entity
                .Property(s => s.PhoneNumber)
                .IsRequired(false)
                .HasColumnType("char(10)")
                .HasMaxLength(10)
                .IsFixedLength(true);
                entity
                .Property(s => s.RegisteredOn)
                .HasColumnType("datetime")
                .IsRequired(true);
                entity
                .Property(s => s.Birthday)
                .HasColumnType("datetime")
                .IsRequired(false);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.CourseId);
                entity
                .Property(c => c.Name)
                .IsRequired(true)
                .HasColumnType("nvarchar(80)");
                entity
                .Property(c => c.Description)
                .IsRequired(false)
                .IsUnicode(true);
                entity
                .Property(c => c.StartDate)
                .IsRequired(true)
                .HasColumnType("datetime");
                entity
                .Property(c => c.EndDate)
                .IsRequired(true)
                .HasColumnType("datetime");
                entity
                 .Property(c => c.Price)
                 .IsRequired(true)
                 .HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(r => r.ResourceId);
                entity
                .Property(r => r.Name)
                .IsRequired(true)
                .IsUnicode(true)
                .HasColumnType("nvarchar(50)");
                entity
                .Property(r => r.Url)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnType("varchar(max)");
                entity
                .Property(r => r.ResourceType)
                .IsRequired(true)
                .HasColumnType("varchar(12)");
                entity
                .HasOne(r => r.Course)
                .WithMany(c => c.Resources)
                .HasForeignKey(r => r.CourseId);
            });

            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasKey(h => h.HomeworkId);
                entity
                .Property(h => h.Content)
                .IsRequired(true)
                .IsUnicode(false)
                .HasColumnType("varchar(max)");
                entity
                .Property(h => h.ContentType)
                .IsRequired(true)
                .HasColumnType("varchar(11)");
                entity
                .Property(h => h.SubmissionTime)
                .IsRequired(true)
                .HasColumnType("datetime2");
                entity
                .HasOne(h => h.Student)
                .WithMany(s => s.Homeworks)
                .HasForeignKey(h => h.StudentId);
                entity
                .HasOne(h => h.Course)
                .WithMany(c => c.Homeworks)
                .HasForeignKey(h => h.CourseId);
            });

            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentsCourses)
                .HasForeignKey(sc => sc.StudentId);
                entity
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentsCourses)
                .HasForeignKey(sc => sc.CourseId);
                entity
                .HasKey(sc => new { sc.StudentId, sc.CourseId });
            });
        }
    }
}