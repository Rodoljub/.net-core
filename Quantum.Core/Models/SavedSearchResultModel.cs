using System;
using System.Collections.Generic;
using System.Text;

namespace Quantum.Core.Models
{
    public class SavedSearchResultModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string SearchText { get; set; }

        public string[] SearchTags { get; set; }
    }
}
