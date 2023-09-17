using System.ComponentModel.DataAnnotations;

namespace Blog_DB_API.Utitlity
{
    public class AllowedExtensionAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            IFormFile? file = value as IFormFile;
            if(file != null)
            {
                if(!_extensions.Contains(file.ContentType))
                {
                    return new ValidationResult(errorMessage: $"This image extension <<{file.ContentType}>> is not allowed");
                }
            }
            return ValidationResult.Success;
        }
    }
}
