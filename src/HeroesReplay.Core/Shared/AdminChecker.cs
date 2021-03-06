using System.Security.Principal;

namespace HeroesReplay.Core.Shared
{
    public class AdminChecker
    {
        public bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}