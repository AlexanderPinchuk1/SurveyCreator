using Microsoft.AspNetCore.Identity;

namespace BSUIR.Survey.Domain.Identity
{
    public class User : IdentityUser<Guid>
    {
        public DateTime RegistrationDateTime { get; set; }
    }
}
