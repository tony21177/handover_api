using AutoMapper;
using handover_api.Controllers.Dto;
using handover_api.Controllers.Request;
using handover_api.Models;
using System.Globalization;
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
            //.ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => string.Join(",", src.PhotoUrls)))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Member, MemberDto>()
            //.ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
            //    src.PhotoUrl != null ? src.PhotoUrl.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList() : null))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Member, Member>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CreateOrUpdateAnnoucementRequest, Announcement>()
            .ForMember(dest => dest.BeginPublishTime, opt => opt.MapFrom(src => ParseDateString(src.BeginPublishTime)))
            .ForMember(dest => dest.EndPublishTime, opt => opt.MapFrom(src => ParseDateString(src.EndPublishTime)))
            .ForMember(dest => dest.BeginViewTime, opt => opt.MapFrom(src => ParseDateString(src.BeginViewTime)))
            .ForMember(dest => dest.EndViewTime, opt => opt.MapFrom(src => ParseDateString(src.EndViewTime)))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

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

        public static DateTime? ParseDateString(string? dateString)
        {
            CultureInfo culture = new("zh-TW");
            if (DateTime.TryParseExact(dateString, "yyy/M/dd", culture, DateTimeStyles.None, out DateTime result)
        || DateTime.TryParseExact(dateString, "yyy/MM/dd", culture, DateTimeStyles.None, out result))
            {
                return result;
            }
            return null;
        }

    }
}
