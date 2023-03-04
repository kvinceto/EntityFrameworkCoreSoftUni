namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        public Student()
        {
            this.Homeworks = new HashSet<Homework>();
            this.StudentsCourses = new HashSet<StudentCourse>();
        }

        public int StudentId { get; set; }

        public string Name { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }

        public DateTime? Birthday { get; set; }

        public ICollection<Homework> Homeworks { get; set; }

        public ICollection<StudentCourse> StudentsCourses { get; set; }
    }
}