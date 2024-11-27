using Quantum.Data.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Quantum.Data.Entities
{
    public class SaveSearchResults :  BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string SearchText { get; set; }

        public virtual ICollection<SaveSearchTags> SaveSearchTags { get; set; }

    }
}
