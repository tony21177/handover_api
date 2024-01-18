namespace handover_api.Utils
{
    public class AuthValueAndPermissionSetting
    {
        public AuthValueAndPermissionSetting(short authValue, PermissionSetting? permissionSetting)
        {
            AuthValue = authValue;
            PermissionSetting = permissionSetting;
        }

        public short AuthValue { get; set; }
        public PermissionSetting PermissionSetting { get; set; }
    }
    public class PermissionSetting
    {
        public bool IsCreateAnnouce { get; set; }
        public bool IsUpdateAnnouce { get; set; }
        public bool IsDeleteAnnouce { get; set; }
        public bool IsHideAnnouce { get; set; }
        public bool IsCreateHandover { get; set; }
        public bool IsUpdateHandover { get; set; }
        public bool IsDeleteHandover { get; set; }
        public bool IsMemberControl { get; set; }
        public bool IsCheckReport { get; set; }
    }

    
}
