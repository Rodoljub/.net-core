using AutoMapper;
using Quantum.Core.Mapping.Services.Contracts;
using Quantum.Core.Models;
using Quantum.Data.Entities;
using System;
using System.Threading.Tasks;

namespace Quantum.Core.Mapping.Services
{
	public class MappingReportedContentService : IMappingReportedContentService
	{
		private IMapper _mapper;

		public MappingReportedContentService(
			IMapper mapper)
		{
			_mapper = mapper;
		}

		public async Task<ReportedContentReason> MapReportedContentReasonFromString(string reportedContentReasonDescriptions, string clrTypeId)
		{
			var mappedReportedContentReasons = _mapper.Map<string, ReportedContentReason>(reportedContentReasonDescriptions);
			mappedReportedContentReasons.ReportedContentTypeID = clrTypeId;
			return await Task.FromResult(mappedReportedContentReasons);
		}

		public async Task<ReportedContent> MapReportedContentFromReportedContentModel(ReportedContentModel model, string clrTypeId, string userId)
		{
			var reportedContent = _mapper.Map<ReportedContentModel, ReportedContent>(model);
			reportedContent.ReportedTypeID = clrTypeId;
			reportedContent.ReporterId = userId;
			return await Task.FromResult(reportedContent);
		}

		public async Task<ReportedContentReasonModel> MapReportedContentReasonModelFromReportedContentReason(ReportedContentReason model)
		{
			var reportedContentReason = _mapper.Map<ReportedContentReason, ReportedContentReasonModel>(model);
			reportedContentReason.ReportedContentTypeName = model.ReportedContentType.Name;
			return await Task.FromResult(reportedContentReason);
		}
	}
}
