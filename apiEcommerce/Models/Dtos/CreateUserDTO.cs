using System.ComponentModel.DataAnnotations;

namespace apiEcommerce.Models.Dtos
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "El campo Nombre es requerido")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "El campo UserName es requerido")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "El campo Password es requerido")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "El campo Password es requerido")]
        public string? Role { get; set; }
    }
}
