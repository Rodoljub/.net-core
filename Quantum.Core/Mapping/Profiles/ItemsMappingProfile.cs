using AutoMapper;
using Quantum.Core.Models;
using Quantum.Data.Entities;
using Quantum.Data.Models;
using Quantum.Data.Models.ReadModels;

namespace Quantum.Core.Mapping.Profiles
{
    public class ItemsMappingProfile : Profile
    {
		public ItemsMappingProfile()
		{
			CreateMap<Tag, TagModel>()
				.ForMember(t => t.Name, opt => opt.MapFrom((src, dest, destMember, resContext) => src.Name.ToLower()));
				//.ReverseMap();
				
			CreateMap<TagModel, Tag>()
				.ForMember(t => t.Name, opt => opt.MapFrom((src, dest, destMember, resContext) => src.Name.ToLower()));
			//.ReverseMap();

			CreateMap<Item, ItemCreateModel>();

			CreateMap<ItemCreateModel, Item>()
				.ForMember(it => it.File, opt => opt.Ignore());

			CreateMap<Item, ItemViewModel>();

			CreateMap<ItemCreateModel, File>();
		}
	}
}
