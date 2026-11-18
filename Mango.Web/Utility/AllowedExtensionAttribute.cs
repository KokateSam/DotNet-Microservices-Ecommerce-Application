using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Utility
{
    public class AllowedExtensionAttribute:ValidationAttribute
    {
        private readonly string[] _extensiosn;

        public AllowedExtensionAttribute(string[] extensions)
        { 
            _extensiosn = extensions;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as FormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensiosn.Contains(extension.ToLower()))
                {
                    return new ValidationResult("Please upload file of valid extension .JPG,.JPEG,.PNG");
                }
            }
            return ValidationResult.Success;
        }
    }
}
