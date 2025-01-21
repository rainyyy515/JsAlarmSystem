using AlarmSystem.Models;
using AlarmSystem.ViewModels;
using AutoMapper;

namespace AlarmSystem.Profiles
{
    public class AlarmItemProfile : Profile
    {
        public AlarmItemProfile()
        {
            CreateMap<AlarmSettings, AlarmItemViewModel>().ForMember(dest => dest.TimeInterval, opt => opt.MapFrom(src => $"{src.StartTime} ~ {src.EndTime}"));
        }
    }
}
