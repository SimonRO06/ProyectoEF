using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos.Auth;
using AutoMapper;
using Domain.Entities.Auth;

namespace Api.Mapping;
public class UserMemberProfile : Profile
{
    public UserMemberProfile()
    {
        CreateMap<UserMember, UserMemberDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Rols.Select(r => r.Name).ToList()));
    }
}