namespace PMS.Domain.Entities;

public class Billings
{

	public int Id { get; set; }

	public int TicketId { get; set; }

	public int PaymentMethodId { get; set; }

	public decimal? Amount { get; set; }

	public DateTime CreatedAt { get; set; }

}