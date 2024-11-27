using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Models;
using Quantum.Core.Services.Contracts;
using Quantum.Data.Entities;
using Quantum.Data.Repositories.Common.Contracts;
using Quantum.Data.Repositories.Contracts;
using Quantum.Integration.External.Services.Contracts;
using Quantum.Utility.Dictionary;
using Quantum.Utility.Extensions;
using Quantum.Utility.Infrastructure.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Quantum.Core.Services
{
	public class ReportedContentService : IReportedContentService
	{
		private IItemRepository _itemRepo;
		private ICommentRepository _commentRepo;
		private IReportedContentReasonRepository _reportedContentReasonRepo;
		private IReportedContentRepository _reportedContentRepo;
		private ICLRTypeRepository _clrTypeRepo;
		private IItemsService _itemServ;
		private ICommentService _commentServ;
		private IUserManagerService _userMgrServ;
		private IMapper _mapper;
		private IConfiguration _config;
		private IMappingReportedContentService _mappingReportedContentServ;
		private IBlobsStorageService _blobService;

		public ReportedContentService(
			IItemRepository itemRepo,
			ICommentRepository commentRepo,
			IReportedContentReasonRepository reportedContentReasonRepo,
			IReportedContentRepository reportedContentRepo,
			ICLRTypeRepository clrTypeRepo,
			IItemsService itemServ,
			ICommentService commentServ,
			IUserManagerService userMgrServ,
			IMapper mapper,
			IConfiguration config,
			IMappingReportedContentService mappingReportedContentServ,
			IBlobsStorageService blobService

		)
		{
			_itemRepo = itemRepo;
			_commentRepo = commentRepo;
			_reportedContentReasonRepo = reportedContentReasonRepo;
			_clrTypeRepo = clrTypeRepo;
			_itemServ = itemServ;
			_mapper = mapper;
			_config = config;
			_commentServ = commentServ;
			_userMgrServ = userMgrServ;
			_reportedContentRepo = reportedContentRepo;
			_mappingReportedContentServ = mappingReportedContentServ;
			_blobService = blobService;
		}

		public async Task CreateReportedContentReasons(ReportedContentReasonsModel model, IIdentity identity)
		{
			var user = await _userMgrServ.GetAuthUser(identity);

			var clrType = await _clrTypeRepo.GetClrTypeByName(model.Type);

			var mappedReportedContentReasons = model.ReportedContentReasonDescriptions.Select(rv => _mappingReportedContentServ.MapReportedContentReasonFromString(rv, clrType.ID).GetAwaiter().GetResult());

			foreach (var reportedDescription in mappedReportedContentReasons)
			{
				await _reportedContentReasonRepo.Insert(reportedDescription, user);
			}
		}

		public async Task<IEnumerable<ReportedContentReasonModel>> GetReportedContentReasons()
		{
			var reportedContentReasons = await _reportedContentReasonRepo.GetReportedContentReasons();

			var reportedContentResaonsModel = reportedContentReasons.Select(rcr => _mappingReportedContentServ.MapReportedContentReasonModelFromReportedContentReason(rcr).GetAwaiter().GetResult());

			return reportedContentResaonsModel;
		}

		public async Task SetReportedContent(ReportedContentModel model, IIdentity identity)
		{
			var user = await _userMgrServ.GetAuthUser(identity);

			await GetReportedContentReasonById(model.ReportedContentReasonId);

			var reportedContentByUser = await _reportedContentRepo.Query(rcbu => rcbu.ReporterId == user.Id && rcbu.ReportedId == model.ReportedId).AnyAsync();

			if (!reportedContentByUser)
			{
				var clrType = await _clrTypeRepo.GetClrTypeByName(model.ReportedTypeName);
				var maxReportedContent = _config.GetAsInteger("Application:MaxReportedContent", 20);
				int reportedContentCount = await _reportedContentRepo.Query(rcc => rcc.ReportedId == model.ReportedId).CountAsync();

				if (reportedContentCount >= maxReportedContent)
				{
					await DeleteEntityWithMaxReportedContent(model, user, clrType);
				}
				else
				{
					var reportedContent = await _mappingReportedContentServ.MapReportedContentFromReportedContentModel(model, clrType.ID, user.Id);

					await _reportedContentRepo.Insert(reportedContent, user);
				}
			}
		}

		private async Task GetReportedContentReasonById(string reportedContentReasonId)
		{
			var reportedContentReason = await _reportedContentReasonRepo.GetById(reportedContentReasonId);

			if (reportedContentReason == null)
			{
				throw new HttpStatusCodeException(HttpStatusCode.InternalServerError,
						   null,
						   Errors.ErrorReportedContentReasonNotFound,
						   null,
						   $"Reported Content Reason cannot be found: '{reportedContentReasonId}'.");
			}
		}

		private async Task DeleteEntityWithMaxReportedContent(ReportedContentModel model, IdentityUser user, CLR_Type clrType)
		{
			if (clrType.Name == typeof(Item).Name)
			{
				var item = await _itemRepo.GetItemById(model.ReportedId);

				await _itemRepo.DeleteItemMaxReported(model.ReportedId);

				await _blobService.DeleteWebPJpegImages(item.FileID);

				return;
			}

			if (clrType.Name == typeof(Comment).Name)
			{
				await _commentRepo.DeleteCommentMaxReported(model.ReportedId);

				return;
			}

			throw new HttpStatusCodeException(HttpStatusCode.InternalServerError,
					   "Oops something went wrong. Please try again later.",
					   Errors.GeneralError,
					   null,
					   $"Reported Content Reason cannot be found: '{model.ReportedContentReasonId}'.");
		}
	}
}
