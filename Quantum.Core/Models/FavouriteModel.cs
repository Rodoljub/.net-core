using System.ComponentModel.DataAnnotations;

namespace Quantum.Core.Models
{
	public class FavouriteModel
    {
		[Required]
		public string EntityId { get; set; }

		[Required]
		public string Type { get; set; }
	}
}
