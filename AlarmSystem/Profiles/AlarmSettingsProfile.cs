using AlarmSystem.Dtos;
using AlarmSystem.Models;
using AutoMapper;

namespace AlarmSystem.Profiles
{
    public class AlarmSettingsProfile : Profile
    {
        public AlarmSettingsProfile()
        {
            CreateMap<AlarmSettingsDto, AlarmSettings>();
        }
    }
}
