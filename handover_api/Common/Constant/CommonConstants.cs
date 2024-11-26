namespace handover_api.Common.Constant
{
    public class CommonConstants
    {
        public static class DetailHandlerUserType
        {
            public const string HANDOVER = "HANDOVER";
            public const string TAKEOVER = "TAKEOVER";

            public static List<string> GetAllValues()
            {
                return new List<string> { HANDOVER, TAKEOVER };
            }
        }

    }
}
