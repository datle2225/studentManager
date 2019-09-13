using System;
using System.ComponentModel.DataAnnotations;

namespace studentManager.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime DayOfBirth { get; set; }
        public string ProfilePicPath {get; set; }
    }
}