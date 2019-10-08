using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOs
{
    public class UserForRegister
    {
        [Required(ErrorMessage="Required")]
        public string username { get; set; }
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="StringLength(8,MinimumLength=4")]
        public string password { get; set; }
        public string Gender{get;set;}
        public string KnownAs {get;set;}
        public DateTime DateOfBirth { get; set; }
        public string City {get;set;}
        public string Country {get;set;}
        public DateTime Created{get;set;}
        public DateTime LastActive {get;set;}
        public UserForRegister() {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}