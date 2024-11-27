using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Models;
using Quantum.Data.Repositories.Contracts;
using Quantum.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
    public class TagService : ITagService
    {
		private ILogger<TagService> _logger;
        private IConfiguration _config;
        private ITagRepository _tagRepo;
		private IUserManagerService _userMgrServ;
        private IMapper _mapper;
		private IItemTagRepository _itemTagRepo;

        public TagService(
			ILogger<TagService> logger,
            IConfiguration config,
            IMapper mapper,
			ITagRepository tagRepo,
			IUserManagerService userMgrServ,
			IItemTagRepository itemTagRepo
        )
        {
			_logger = logger;
            _config = config;
            _mapper = mapper;
            _tagRepo = tagRepo;
			_userMgrServ = userMgrServ;
			_itemTagRepo = itemTagRepo;
        }

        public async Task CreateTags(string[] tags, IIdentity identity)
        {
			var user = await _userMgrServ.GetAuthUser(identity);

            foreach (var tagName in tags)
            {
                await _tagRepo.Insert(new Tag() { Name = tagName.ToLower() }, user);
            }

            //await _tagRepo.Save();
        }

        public async Task<IEnumerable<TagModel>> GetTags()
        {
            var take = _config.GetAsInteger("Application:MaxTagsPageSize", 100);

            var tags = await _tagRepo.GetPaged(0, int.MaxValue);

            return tags.Select(t => _mapper.Map<Tag, TagModel>(t));
        }



		public async Task<List<TagModel>> CreateItemTags(ImageAnalysis imageAnalysis, Item item, IdentityUser user)
		{
			IEnumerable<string> analyzedImageConfidentTagsNames = GetAnalyzedImageConfidentTagsNames(imageAnalysis);

			if (analyzedImageConfidentTagsNames != null)
            {
                var maxTags = _config.GetAsInteger("Application:MaxTags", 25);

                var existingTags = await _tagRepo.Query(t => !t.IsDeleted && analyzedImageConfidentTagsNames.Contains(t.Name)).ToListAsync();

				List<Tag> newInsertedTags = await InsertNewTags(user, analyzedImageConfidentTagsNames, existingTags);
				if (newInsertedTags.Count() > 0)
                {
						await _tagRepo.Save();
				}

				await InsertItemTags(item, newInsertedTags);

                existingTags.Union(newInsertedTags).DistinctBy(t => t.Name);

                return GetTagModels(existingTags);
			}

			return null;
		}

		private IEnumerable<string> GetAnalyzedImageConfidentTagsNames(ImageAnalysis imageAnalysis)
		{
			if (imageAnalysis.Tags != null)
            {
				_logger.LogInformation("imageAnalysis.Tags != null");
				decimal confidence = _config.GetAsDecimal($"ComputerVision:AnalyzeImage:Tags:Confidence", 0.60M);

				var analyzedImageConfidentTags = imageAnalysis.Tags.Where(ta => ta.Confidence > Convert.ToDouble(confidence))
						.OrderByDescending(ta => ta.Confidence)
						.Select(ta => ta.Name.ToLower());

				_logger.LogInformation("analyzedImageConfidentTags");
				return analyzedImageConfidentTags;
			}

			_logger.LogInformation("GetAnalyzedImageConf null");
			return null;
		}

		private async Task<List<Tag>> InsertNewTags(IdentityUser user, IEnumerable<string> analyzedImageConfidentTagsNames, IEnumerable<Tag> existingTags)
		{
			var newTags = analyzedImageConfidentTagsNames.Except(existingTags.Select(ta => ta.Name));

			var newInsertedTags = new List<Tag>();

			if (newTags.Count() > 0) 
			{
				foreach (var tagName in newTags)
				{
					if (tagName.Count() <= 100)
                    {
						var newTag = new Tag() { Name = tagName.ToLower() };

						await _tagRepo.Insert(newTag, user, save: false);
						newInsertedTags.Add(newTag);
					}
					
				}

			}

			return newInsertedTags;

		}

		private async Task InsertItemTags(Item item, IEnumerable<Tag> newTags)
		{
			foreach (var itemTag in newTags)
			{
				await _itemTagRepo.Insert(new ItemTag { Item = item, Tag = itemTag, Display = true }, null);
			}

			//await _itemTagRepo.Save();
		}

		private List<TagModel> GetTagModels(IEnumerable<Tag> alliItemTags)
		{
            var maxTags = _config.GetAsInteger("Application:MaxTags", 25);

            var tags = alliItemTags.Select(_mapper.Map<Tag, TagModel>).Take(maxTags).ToList();

			return tags;
		}


		public async Task UpdateDisplayTags(Item item, ItemUpdateModel model, IdentityUser user)
		{
			var selectedTags = model.Tags.Take(_config.GetAsInteger("Application:MaxTags", 25)).Distinct().ToList();

            await UpdateItemTagsDisplay(item, selectedTags);

			List<string> newNotVerifiedTagsIds = await InsertNotVerifiedNewTags(user, selectedTags);

            newNotVerifiedTagsIds.ForEach(async t => await _tagRepo.Delete(t, user));
            //await DeleteNewNotVerifiedTags(user, newNotVerifiedTagsIds);
		}

		private async Task UpdateItemTagsDisplay(Item item, List<string> selectedTags)
		{
			//var itemTagsDisplayed = await _itemTagRepo.Query(
			//	it => !it.IsDeleted && it.ItemId == item.ID, true
			//).Distinct().Select(it => it.Tag.Name).ToListAsync();
			
			var itemTagsNotContainedInSelectedTags = await _itemTagRepo.Query(
				it => !it.IsDeleted && it.ItemId == item.ID && !selectedTags.Contains(it.Tag.Name), true
			).Distinct().ToListAsync();


			foreach (var itemTag in itemTagsNotContainedInSelectedTags)
			{
				itemTag.Display = false;
				await _itemTagRepo.Update(itemTag, null);
			}

			//await _itemTagRepo.Save();
		}

		private async Task<List<string>> InsertNotVerifiedNewTags(IdentityUser user, List<string> selectedTags)
		{
			var existingTags = await _tagRepo.Query(t => !t.IsDeleted && selectedTags.Contains(t.ID)).ToListAsync();

			var tagNames = existingTags.Select(x => x.Name);

			var tagIds = existingTags.Select(x => x.ID);

			var newTagsNotVerified = selectedTags.Except(tagNames).Except(tagIds);

			var newTagsNotVerifiedIds = new List<string>();

			foreach (var tagName in newTagsNotVerified)
			{
				var newTag = new Tag() { Name = tagName.ToLower() };
				await _tagRepo.Insert(newTag, user);

				newTagsNotVerifiedIds.Add(newTag.ID);
			}

			return newTagsNotVerifiedIds;
		}

		//private Task DeleteNewNotVerifiedTags(IdentityUser user, List<string> newTagsNotVerifiedIds)
		//{
		//	newTagsNotVerifiedIds.ForEach(async t => await _tagRepo.Delete(t, user, save: false));

		//   _tagRepo.Save();
		//}





		public async Task<List<TagModel>> SearchTags(string searchQuery, string selectedTags)
		{
			string[] selected = new string[] { };
			if (!string.IsNullOrWhiteSpace(selectedTags))
			{
				selected = selectedTags.Split(',');
			}
			
			var searchedTags = await _tagRepo.SearchTags(searchQuery, selected);

			return searchedTags;
		}
	}
}
