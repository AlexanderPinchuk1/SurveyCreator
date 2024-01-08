using System.ComponentModel.DataAnnotations;

namespace BSUIR.Survey.Domain
{
    public class GuidIdAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            return value switch
            {
                Guid guid => guid != Guid.Empty,
                _ => false
            };
        }
    }
}
