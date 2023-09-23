using System.ComponentModel.DataAnnotations;

namespace demerge_blog_API.Utility
{
    public class AllowExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            if(file is not null)
            {
                if(!_extensions.Contains(file.ContentType))
                {
                    return new ValidationResult($"This extension <<{file.ContentType}>> not allowed");
                }
            }

            return ValidationResult.Success;
        }
    }
}
