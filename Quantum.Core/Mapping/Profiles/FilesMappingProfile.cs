using AutoMapper;
using Quantum.Core.Models.File;
using Quantum.Data.Entities;
using Quantum.Integration.Internal.Models;

namespace Quantum.Core.Mapping.Profiles
{
    public class FilesMappingProfile : Profile
	{
		public FilesMappingProfile()
		{
			CreateMap<byte[], File>()
				.ForMember(fi => fi.file, opt => opt.MapFrom((src, dest, destMember, resContext) => src));

			CreateMap<File, File>()
				.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

			CreateMap<FileModel, File>()
				.ForMember(f => f.file, opt => opt.Ignore());

			CreateMap<File, SaveImageFileModel>();
		}
	}
}
