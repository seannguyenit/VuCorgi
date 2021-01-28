#region Using

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace VuBongBongWeb
{
    public class AccountLoginModel
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class AccountForgotPasswordModel
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
    }

    //public class AccountResetPasswordModel
    //{
    //    [Required]
    //    [DataType(DataType.Password)]
    //    public string Password { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    [Compare("Password")]
    //    public string PasswordConfirm { get; set; }
    //}

    public class AccountResetPasswordModel
    {
        [Required]
        public Guid UserId { get; set; }

        //[Required]
        //[StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }

    }

    public class AccountRegistrationModel
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(20)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [StringLength(20)]
        public string PasswordConfirm { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(250)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(250)]
        [Compare("Email")]
        public string EmailConfirm { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20)]
        public string Phone { get; set; }
        [Required]
        [StringLength(250)]
        public string FullName { get; set; }
        [StringLength(1000)]
        public string Slogan { get; set; }
        [StringLength(1000)]
        public string Company { get; set; }
        [Required]
        [StringLength(1000)]
        public string Address { get; set; }
        [Required]
        [StringLength(1000)]
        public string City { get; set; }
        public DateTime? DOB { get; set; }

        public string ReturnUrl { get; set; }
    }
}