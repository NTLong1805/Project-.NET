using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement2.Models.ViewModel
{
	public class AuthViewModel
	{
		public string FormType {  get; set; } // để xác định form đang submit
		//SignIn
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress]
		public string LoginEmail { get; set; }

		[Required(ErrorMessage ="Password is required")]
		[DataType(DataType.Password)]
		public string LoginPassword { get; set; }

		[Display(Name = "Remember Me")]
		public bool RememberMe { get; set; }
	
		//SignUp
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress]
		public string RegisterEmail { get; set; }
		[Required(ErrorMessage = "Password is required.")]
		[DataType(DataType.Password)]
		public string RegisterPassword { get; set; }

		[Required(ErrorMessage ="UserName is required")]
		public string UserName {  get; set; }

		[Required(ErrorMessage = "Confirm Password is required.")]
		[DataType(DataType.Password)]
		[Compare("RegisterPassword", ErrorMessage = "Password do not match.")]
		public string ConfirmPassword { get; set; }



		public string? ReturnUrl {  get; set; }

		public IList<AuthenticationScheme>? ExternalLogins { get; set; }



		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (FormType == "SignIn")
			{
				if (string.IsNullOrEmpty(LoginEmail))
				{
					yield return new ValidationResult("Email is required", new[] { "LoginEmail" });
				}
				if (string.IsNullOrEmpty(LoginPassword))
				{
					yield return new ValidationResult("Password is required", new[] { "LoginEmail" });
				}
			}
            if (FormType == "SignUp")
            {
                if (string.IsNullOrWhiteSpace(UserName))
                    yield return new ValidationResult("Tên người dùng không được để trống.", new[] { "UserName" });

                if (string.IsNullOrWhiteSpace(RegisterEmail))
                    yield return new ValidationResult("Email không được để trống.", new[] { "RegisterEmail" });

                if (string.IsNullOrWhiteSpace(RegisterPassword))
                    yield return new ValidationResult("Mật khẩu không được để trống.", new[] { "RegisterPassword" });

                if (RegisterPassword != ConfirmPassword)
                    yield return new ValidationResult("Mật khẩu xác nhận không khớp.", new[] { "ConfirmPassword" });
            }
        }
	}
}
