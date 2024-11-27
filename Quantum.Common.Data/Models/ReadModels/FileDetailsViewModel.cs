using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum.Data.Models.ReadModels
{
    public class FileDetailsViewModel
    {
        public string Color { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public object ImageAnalysis { get { return JsonConvert.DeserializeObject(ImageAnalysisRaw); } }

        public string ImageAnalysisRaw { get; set; }
    }
}
