namespace PMS.Domain.Entities;

public class Tickets
{

	public int Id { get; set; }

	public string LicenseNo { get; set; }

	public string DriverName { get; set; }

	public string Company { get; set; }

	public string Model { get; set; }

	public DateTime Issued_At { get; set; }

	public DateTime? Expires_At { get; set; }

	public bool? Is_Used { get; set; }

	public int CategoryId { get; set; }

	public int? ParkingSpaceId { get; set; }

	public int UserId { get; set; }

}