using AutoMapper;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Quantum.Core.Models.File;
using Quantum.Data.Entities;
using Quantum.Data.Models.ReadModels;

namespace Quantum.Core.Mapping.Profiles
{
    public class FileDetailsMappingProfile : Profile
    {
        public FileDetailsMappingProfile()
        {
            CreateMap<FileDetails, FileDetailsModel>();

            CreateMap<FileDetails, FileDetailsViewModel>();

            CreateMap<ImageAnalysis, FileDetails>()
                .ForMember(fd => fd.Width, opt => opt.MapFrom((src, dest, destMember, resContext) => src.Metadata.Width))
                .ForMember(fd => fd.Height, opt => opt.MapFrom((src, dest, destMember, resContext) => src.Metadata.Height))
                .ForMember(fd => fd.Color, opt => opt.MapFrom((src, dest, destMember, resContext) => src.Color.DominantColorBackground));

        }
    }
}
