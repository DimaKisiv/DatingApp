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
    }
}