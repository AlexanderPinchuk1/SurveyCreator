using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BSUIR.Survey.WebApp.Models
{
    public class EditUserViewModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Role")]
        [Remote(action: "CheckRole", controller: "Administration", ErrorMessage = "Wrong role")]
        public string Role { get; set; }
    }
}
