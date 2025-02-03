using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace Common.Param
{
    public class GetLoginParam
    {
        [Required(ErrorMessage = "El campo Usuario es requerido.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "El campo Usuario solo debe contener letras, números y guiones bajos.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "El campo Password es requerido.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial.")]
        public string Password { get; set; } = null!;
    }
}
