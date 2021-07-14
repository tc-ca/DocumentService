namespace DocumentService.Authorization
{
    public class RolePolicy
    {
        public const string RoleAssignmentRequiredReaders = "RoleAssignmentRequiredReaders";
        public const string RoleAssignmentRequiredWriters = "RoleAssignmentRequiredWriters";

        public const string PolicyConfigKeyReaders = "AuthorizationPolicies:RoleAssignmentRequiredReaders";
        public const string PolicyConfigKeyWriters = "AuthorizationPolicies:RoleAssignmentRequiredWriters";

        public string Role { get; set; }
    }
}
