using AlarmSystem.Dtos;
using AlarmSystem.Models;
using AutoMapper;

namespace AlarmSystem.Profiles
{
    public class CreateGroupProfile : Profile
    {
        public CreateGroupProfile()
        {
            CreateMap<AlarmGroupDto, AlarmGroup>();
        }
    }
}
