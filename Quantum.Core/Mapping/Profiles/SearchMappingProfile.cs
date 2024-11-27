using AutoMapper;
using Quantum.Core.Models;
using Quantum.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quantum.Core.Mapping.Profiles
{
    public class SearchMappingProfile : Profile
    {
        public SearchMappingProfile()
        {
            CreateMap<SaveSearchModel, SaveSearchResults>();

            CreateMap<SaveSearchTags, SaveSearchTags>();

            CreateMap<SaveSearchResults, SavedSearchResultModel>();
        }
    }
}
