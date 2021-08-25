using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Authorization
{
    public class ScopePolicy
    {
        public const string ReadWritePermission = "ApiScopes:ReadWrite";
        public const string ReadPermission = "ApiScopes:Read";
        public const string WritePermission = "ApiScopes:Write";
    }
}
