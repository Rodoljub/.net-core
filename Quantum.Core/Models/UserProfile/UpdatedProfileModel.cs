using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Core.Models.UserProfile
{
    public class UpdatedProfileModel
    {
        public string Name { get; set; }

        public AnalyzingImageViewModel AnalyzingImageViewModel { get; set; }
    }
}
