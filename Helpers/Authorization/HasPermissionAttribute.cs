using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DotnetSkeletonApp.Helpers.Authorization
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        const string POLICY_PREFIX = "PERMISSION_";

        public HasPermissionAttribute(string permission)
        {
            Permission = permission;
            Policy = $"{POLICY_PREFIX}{permission}";
        }

        public string Permission { get; }
    }
}