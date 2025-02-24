using HotelManagement2.Models.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement2.Models
{

	public class Account:IdentityUser
	{
		public Account()
		{
			Customers = new HashSet<Customer>();
		}
		
		public AccountType Type { get; set; }
		public bool Active { get; set; }

		// Navigation properties
		public virtual Staff? Staff { get; set; }
		public virtual ICollection<Customer> Customers { get; set; }
	}
}
