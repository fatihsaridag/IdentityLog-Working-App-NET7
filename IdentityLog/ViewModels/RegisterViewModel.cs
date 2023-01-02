using System.ComponentModel.DataAnnotations;

namespace IdentityLog.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name ="Adınız")]
        [Required(ErrorMessage ="Adınız kısmını lütfen giriniz")]
        public string FirstName { get; set; }


        [Display(Name = "Soyadınız")]
        [Required(ErrorMessage = "Soyadınız kısmını lütfen giriniz")]
        public string LastName { get; set; }


        [Display(Name = "Kullanıcı Adınız")]
        [Required(ErrorMessage = "Kullanıcı Adınız kısmını lütfen giriniz")]
        public string UserName { get; set; }


        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email kısmını lütfen giriniz")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Şifreniz")]
        [Required(ErrorMessage = "Şifreniz kısmını lütfen giriniz")]
        [DataType(DataType.Password)]   
        public string Password { get; set; }

        [Display(Name = "Şifre Tekrarı")]
        [Required(ErrorMessage = "Şifre Tekrarı kısmını lütfen giriniz")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string Repassword { get; set; }
    }
}
