using AutoMapper;
using BjjInParadise.Core.Models;
using BJJInParadise.Web.Models;
using BJJInParadise.Web.ViewModels;

namespace BJJInParadise.Web
{
    public static class AutoMapperConfig
    {
       
            internal static void Configure()
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<User, UserViewModel>()
                        .ReverseMap();
                    cfg.CreateMap<User, UserBookingViewModel>()
                        .ReverseMap();
                    cfg.CreateMap<User, ApplicationUser>()
                        .ReverseMap();
                });
            }
        

    }
}