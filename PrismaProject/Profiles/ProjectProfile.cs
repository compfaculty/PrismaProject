using AutoMapper;
using PrismaProject.Dto;
using PrismaProject.Models;

namespace PrismaProject.Profiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            // Source -> Target
            CreateMap<Message, MessagePublishDto>();
            CreateMap<MessagePublishDto, Message>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id));
        }
    }
}