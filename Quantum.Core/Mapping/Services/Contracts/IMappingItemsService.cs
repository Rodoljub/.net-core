using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Quantum.Core.Models;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services.Contracts
{
    public interface IMappingItemsService
    {
		Task<ItemViewModel> MapItemViewModelFromItem(Item item, int itemsCount, IEnumerable<Tag> tags, bool filterTags = false, string userId = null, int width = 320);

		Task<Item> MapItemFromItemModel(ItemCreateModel model, ImageAnalysis imageAnalysi, string fileId, string userProfileId);

	}
}
