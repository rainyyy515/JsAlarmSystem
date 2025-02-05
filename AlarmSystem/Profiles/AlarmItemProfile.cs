using AlarmSystem.Models;
using AlarmSystem.ViewModels;
using AutoMapper;

namespace AlarmSystem.Profiles
{
    public class AlarmItemProfile : Profile
    {
        public AlarmItemProfile()
        {
            CreateMap <AlarmItem, AlarmItemViewModel>().ReverseMap();
        }
    }
}
