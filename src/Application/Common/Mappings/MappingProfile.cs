using AutoMapper;
using Application.DTOs;
using Domain.Entities;
using System.IO;

namespace Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map receipt entity to the dtio
            CreateMap<Receipt, ReceiptDto>()
                .ForMember(dest => dest.ReceiptFileName, opt => 
                    opt.MapFrom(src => Path.GetFileName(src.ReceiptPath)));
        }
    }
} 