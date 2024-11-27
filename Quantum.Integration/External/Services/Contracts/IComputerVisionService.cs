using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.IO;
using System.Threading.Tasks;

namespace Quantum.Integration.Services.Contracts
{
    public interface IComputerVisionService
	{
		Task<ImageAnalysis> AnalyzeImageWithCVClient(Stream imageStream);
	}
}
