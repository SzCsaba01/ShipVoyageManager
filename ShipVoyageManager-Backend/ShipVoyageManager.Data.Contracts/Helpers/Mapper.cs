using AutoMapper;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Port;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.Ship;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.User;
using ShipVoyageManager.Data.Contracts.Helpers.DTO.VisitedCountry;
using ShipVoyageManager.Data.Object;

namespace ShipVoyageManager.Data.Contracts.Helpers;
public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<ShipEntity, ShipDto>();
        CreateMap<ShipDto, ShipEntity>();

        CreateMap<PortEntity, PortDto>();
        CreateMap<PortDto, PortEntity>();

        CreateMap<VoyageEntity, VoyageDto>()
            .ForMember(dest => dest.ShipName, opt => opt.MapFrom(src => src.Ship.Name))
            .ForMember(dest => dest.DeparturePortName, opt => opt.MapFrom(src => src.DeparturePort.Name))
            .ForMember(dest => dest.ArrivalPortName, opt => opt.MapFrom(src => src.ArrivalPort.Name));
        CreateMap<VoyageDto, VoyageEntity>();

        CreateMap<UserRegistrationDto, UserEntity>();

        CreateMap<VisitedCountryEntity, VisitedCountryDto>();
        CreateMap<VisitedCountryDto, VisitedCountryEntity>();

        CreateMap<UserEntity, UserDto>();
    }
}
