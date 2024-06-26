﻿using System.ComponentModel.DataAnnotations;

namespace FlightsApp.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
