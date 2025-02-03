using Common.Type;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Param
{
    public class GetRegistrationUser : GetLoginParam
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Nombres es requerido.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El campo Nombres solo debe contener letras y espacios.")]
        public string Nombres { get; set; } = null!;

        [Required(ErrorMessage = "El campo Apellidos es requerido.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El campo Apellidos solo debe contener letras y espacios.")]
        public string Apellidos { get; set; } = null!;

        [Required(ErrorMessage = "El campo Tipo Identificación es requerido.")]
        public IdentificationType IdentificacionType { get; set; }

        [Required(ErrorMessage = "El campo Identificación es requerido.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La identificación debe ser un número de 10 dígitos.")]
        public string Identificacion { get; set; } = null!;

        [Required(ErrorMessage = "El campo Email es requerido.")]
        [EmailAddress(ErrorMessage = "El campo Email no tiene un formato válido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "El campo Fecha Creación es requerido.")]
        public DateTimeOffset FechaCreacion { get; set; } = DateTimeOffset.Now;

    }
}
