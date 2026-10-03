using Domain;
using System.ComponentModel.DataAnnotations;

namespace Financy.Application.DTOs.AccountDTOs
{
    public class CreateAccountDTO
    {
        [Required(ErrorMessage = "Account name is required")]
        public string Name { get; set; } = null!;

        public decimal StartingBalance { get; set; } = 0;

        public static implicit operator Account(CreateAccountDTO dto)
        {
            return new Account
            {
                Id = 0,
                Name = dto.Name,
                InitialBalance = dto.StartingBalance,
            };
        }
    }
}
