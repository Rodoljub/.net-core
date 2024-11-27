using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Quantum.Core.Models;
using Quantum.Data.Entities;
using Quantum.Data.Models;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services.Contracts
{
    public interface ITagService
    {
        Task CreateTags(string[] tags, IIdentity identity);

        Task<IEnumerable<TagModel>> GetTags();

		Task<List<TagModel>> CreateItemTags(ImageAnalysis imageAnalysis, Item item, IdentityUser user);

		Task UpdateDisplayTags(Item item, ItemUpdateModel model, IdentityUser user);

		Task<List<TagModel>> SearchTags(string searchQuery, string selectedTags);
	}
}
