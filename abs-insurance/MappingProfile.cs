using AutoMapper;

using Shared.DTOs;
using Entities.Models;

namespace abs_insurance;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Policy, ReadPolicyDto>().ForMember(dest => dest.Components,
            opts => 
                opts.MapFrom(src => src.Components));

        CreateMap<CreatePolicyDto, Policy>().ForMember(dest => dest.Components,
            opts =>
                opts.MapFrom(src => src.Components));

        CreateMap<CreatePolicyComponentDto, PolicyComponent>().ForMember(dest =>
            dest.Name, opts =>
            opts.MapFrom(src => Enum.Parse<ComponentName>(src.Name.Replace(" ", ""), true)));
        
        CreateMap<PolicyComponent, ReadPolicyComponentDto>().ForMember(dest => dest.Name,opts =>
            opts.MapFrom(src => string.Join("", System.Text.RegularExpressions.Regex.Matches(
                src.Name.ToString(),"[A-Z][a-z]*").Select(m => m.Value))));
        
        CreateMap<UserRegistrationDto, User>();
        CreateMap<UserCreationDto, User>();
    }
    
    
}