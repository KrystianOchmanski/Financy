using Domain;

namespace Financy.Application.DTOs.TransactionDTOs
{
	public class TransactionFilterDTO
	{
		public decimal? MinAmount { get; set; }

		public decimal? MaxAmount { get; set; }

		public string? StartDate { get; set; }

		public string? EndDate { get; set; }

		public TransactionType? Type { get; set; }

		public int? AccountId { get; set; }

		public int? CategoryId { get; set; }

		public DateOnly? GetParsedStartDate()
		{
			return DateOnly.TryParse(StartDate, out var date) ? date : null;
		}

		public DateOnly? GetParsedEndDate()
		{
			return DateOnly.TryParse(EndDate, out var date) ? date : null;
		}
	}
}
