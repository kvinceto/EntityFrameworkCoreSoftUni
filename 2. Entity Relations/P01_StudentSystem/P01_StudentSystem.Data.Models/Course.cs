﻿namespace P01_StudentSystem.Data.Models
{
    public class Course
    {
        public Course()
        {
            this.Resources = new HashSet<Resource>();
            this.Homeworks = new HashSet<Homework>();
            this.StudentsCourses = new HashSet<StudentCourse>();
        }

        public int CourseId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        public ICollection<Resource> Resources { get; set; }

        public ICollection<Homework> Homeworks { get; set; }

        public ICollection<StudentCourse> StudentsCourses { get; set; }
    }
}
