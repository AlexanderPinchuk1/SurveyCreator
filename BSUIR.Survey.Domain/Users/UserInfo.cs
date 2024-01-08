
using System.ComponentModel.DataAnnotations;

namespace BSUIR.Survey.Domain
{
    public class UserInfo
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string RegistrationDateTime { get; set; }

        [Required]
        public int CompletedSurveys { get; set; }

        [Required]
        public int CreatedSurveys { get; set; }
    }
}
