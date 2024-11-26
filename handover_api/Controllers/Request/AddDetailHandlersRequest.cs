namespace handover_api.Controllers.Request
{
    public class AddDetailHandlersRequest
    {
        public string handoverDetailId { get; set; } = null!;
        public List<UserRequest> UserList { set; get; } = null!;
    }

    public class UserRequest
    {
        public string UserId { get; set; } = null!;
        public string Type { get; set; } = null!;

        public string? Remarks { get; set; }
    }
}
