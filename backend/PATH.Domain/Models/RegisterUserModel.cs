using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PATH.Domain.Models
{

    public class RegisterUserModel
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }


        [Required]
        [MaxLength(100)]
        [EmailAddress]

        public string Email { get; set; }


        [DataType(DataType.Password)]
        [MinLength(6)]
        [MaxLength(50)]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare(nameof(Password), ErrorMessage = "Password and Confirm Password must match.")]
        public string ConfirmPassword { get; set; }


        [DataType(DataType.Date)]
        [Required]
        public DateOnly DateOfBirth { get; set; }

    }
}
