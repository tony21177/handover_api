namespace handover_api.Common
{
    public class CommonResponse<T>
    {
        public bool Result { get; set; }
        public string Message { get; set; } = "";

        public T? Data { get; set; }
    }
}
