namespace FinanceTracker.Api.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}