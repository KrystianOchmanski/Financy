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
            if (!string.IsNullOrEmpty(StartDate) && !DateOnly.TryParse(StartDate, out var date))
            {
                throw new FormatException($"StartDate '{StartDate}' is not a valid date format. Provide YYYY-MM-DD.");
            }

            return DateOnly.TryParse(StartDate, out var startDate) ? startDate : null;
        }

        public DateOnly? GetParsedEndDate()
        {
            if (!string.IsNullOrEmpty(EndDate) && !DateOnly.TryParse(EndDate, out var date))
            {
                throw new FormatException($"EndDate '{EndDate}' is not a valid date format. Provide YYYY-MM-DD");
            }

            return DateOnly.TryParse(EndDate, out var endDate) ? endDate : null;
        }
    }
}
