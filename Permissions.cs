using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Moov2.Orchard.FindReplace
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission AccessFindReplace = new Permission { Description = "Access to Find/Replace", Name = "AccessFindReplace" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { AccessFindReplace }
                }
            };
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                AccessFindReplace
            };
        }
    }
}