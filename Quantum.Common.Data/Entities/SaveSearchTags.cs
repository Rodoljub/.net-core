using Quantum.Data.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quantum.Data.Entities
{
    public class SaveSearchTags : BaseEntity
    {
        [ForeignKey("Tag")]
        public string TagId { get; set; }

        public virtual Tag Tag { get; set; }

        [ForeignKey("SaveSearchResults")]
        public string SaveSearchResultsId { get; set; }

        public virtual SaveSearchResults SaveSearchResults { get; set; }
    }
}
