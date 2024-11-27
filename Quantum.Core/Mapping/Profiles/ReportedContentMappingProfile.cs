using AutoMapper;
using Quantum.Core.Models;
using Quantum.Data.Entities;

namespace Quantum.Core.Mapping.Profiles
{
	public class ReportedContentMappingProfile : Profile
	{
		public ReportedContentMappingProfile()
		{
			CreateMap<string, ReportedContentReason>()
				.ForMember(rc => rc.Description, opt =>
					opt.MapFrom((src) => src));

			CreateMap<ReportedContentModel, ReportedContent>();

			CreateMap<ReportedContentReason, ReportedContentReasonModel>();
		}
	}
}
