using WebApplication1.Data;

namespace TimeClock.Extensions
{
    public static class MyExtensions
    {

        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext.User == null)
            {
                return string.Empty;
            }
            return httpContext.User.Claims.Single(x => x.Type == "id").Value;
        }

        public static bool IsAdmin(this DataContext dataContext, string userId)
        {
            var userRoleId = dataContext.UserRoles.FirstOrDefault(x => x.UserId == userId).RoleId;
            var adminRoleId = dataContext.Roles.FirstOrDefault(x => x.Name == "Admin").Id;
            if (userRoleId.Equals(adminRoleId))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
