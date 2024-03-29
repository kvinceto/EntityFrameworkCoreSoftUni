﻿namespace P01_StudentSystem.Data.Models
{
    using Enums;

    public class Homework
    {
        public Homework()
        {

        }

        public int HomeworkId { get; set; }

        public string Content { get; set; } = null!;

        public ContentType ContentType { get; set; }

        public DateTime SubmissionTime { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; } = null!;

        public int CourseId { get; set; }

        public Course Course { get; set; } = null!;
    }
}
