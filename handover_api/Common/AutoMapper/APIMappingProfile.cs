using AutoMapper;
using handover_api.Controllers.Dto;
using handover_api.Controllers.Request;
using handover_api.Models;
using Member = handover_api.Models.Member;

namespace MaiBackend.Common.AutoMapper
{
    public class APIMappingProfile : Profile
    {
        public APIMappingProfile()
        {
            CreateMap<CreateAuthlayerRequest, Authlayer>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateAuthlayerRequest, Authlayer>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreateOrUpdateMemberRequest, Member>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => string.Join(",", src.PhotoUrls)))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Member, MemberDto>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                src.PhotoUrl != null ? src.PhotoUrl.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList() : null))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Member, Member>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, prop) => prop != null))
             ;
        }

        //public Dictionary<string, object>? MapSchema(ColumnDefinition src)
        //{
        //    var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.CanonicalExtendedJson };
        //    if (src.Schema != null)
        //    {
        //        return JsonSerializer.Deserialize<Dictionary<string, object>>(
        //            src.Schema.ToJson(jsonWriterSettings, null, null),
        //            new JsonSerializerOptions
        //            {
        //                WriteIndented = false
        //            });
        //    }
        //    return null;
        //}

    }
}
