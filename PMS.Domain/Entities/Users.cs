namespace PMS.Domain.Entities;

public class Users
{

	public int Id { get; set; }

	public int RoleId { get; set; }

	public string UserName { get; set; }

	public string PasswordHash { get; set; }

	public bool? IsActive { get; set; }

}