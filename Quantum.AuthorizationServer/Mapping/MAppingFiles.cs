using AutoMapper;
using Quantum.Data.Entities;

namespace Quantum.AuthorizationServer.Mapping
{
    public class MappingFiles : Profile
	{
		public MappingFiles()
		{
			CreateMap<File, File>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

			//CreateMap<File, SaveImageFileModel>()
			//	.ForMember(sif => sif.Base64Image, opt => opt.ResolveUsing((src, dest, destMember, resContext) => resContext.Items["Base64Image"]))
			//	.ForMember(sif => sif.Type, opt => opt.ResolveUsing((src, dest, destMember, resContext) => resContext.Items["Type"]))
			//	.ForMember(sif => sif.Width, opt => opt.ResolveUsing((src, dest, destMember, resContext) => resContext.Items["Width"]));
		}
	}
}
