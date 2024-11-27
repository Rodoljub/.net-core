
using Quantum.Data.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Quantum.Data.Entities
{
    public class View : BaseEntity
    {
		public Item Item { get; set; }

		[ForeignKey("Item")]
		public virtual string ItemId { get; set; }

		public string UserId { get; set; }

		public string IPAddress { get; set; }
	}
}
