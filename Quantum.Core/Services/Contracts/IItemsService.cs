using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Quantum.Core.Models;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
    public interface IItemsService
	{
		Task<AnalyzingImageViewModel> CreateItemForAnalysis(ItemCreateModel model, IIdentity identity);

		Task<string> UpdateItem(ItemUpdateModel model, IIdentity identity);

		Task<ItemViewModel> GetItemView(string itemId, IIdentity identity);

		Task<string> DeleteItem(string itemId, IIdentity identity);

        Task UpdateAnalysedItem(File file, ImageAnalysis imageAnalysis);

        Task<List<AnalyzingImageViewModel>> GetAnalyzingImages(string[] filesIds, IIdentity identity);

		void Aggragatiosns(string userId);
    }
}
