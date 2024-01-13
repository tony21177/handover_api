using AutoMapper;
using handover_api.Controllers.Request;
using handover_api.Models;

namespace MaiBackend.Common.AutoMapper
{
    public class APIMappingProfile : Profile
    {
        public APIMappingProfile()
        {
            CreateMap<UpdateAuthlayerRequest, Authlayer>()
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

    }
}
