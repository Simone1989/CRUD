using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_CRUD
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public int DirectorId { get; set; }

        public Movie(int id, string title, int releaseYear, int directorId)
        {
            this.Id = id;
            this.Title = title;
            this.ReleaseYear = releaseYear;
            this.DirectorId = directorId;
        }
    }
}
