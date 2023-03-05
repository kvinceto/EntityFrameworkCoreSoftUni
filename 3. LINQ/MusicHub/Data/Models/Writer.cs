﻿namespace MusicHub.Data.Models
{
    public class Writer
    {
        public Writer()
        {
            this.Songs = new HashSet<Song>();
        }

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Pseudonym { get; set; }

        public ICollection<Song> Songs { get; set; }
    }
}