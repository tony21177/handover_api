namespace handover_api.Common
{
    public class CommonResponse<T>
    {
        public bool Result { get; set; }
        public string Message { get; set; } = "";

        public T? Data { get; set; }
        public static CommonResponse<dynamic> BuildNotAuthorizeResponse()
        {
            return new CommonResponse<dynamic>
            {
                Result = false,
                Message = "沒有權限",
            };
        }
    }
}
