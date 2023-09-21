using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using TimeClock.Domain;
using TimeClock.Options;

namespace TimeClock.Services
{
    public class IdentityService : IIdentityService
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;

        public IdentityService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _roleManager = roleManager;

        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User does not exist." }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Username/Password combination is wrong. Please try again." }
                };
            }
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password, bool isAdmin)
        {
            try
            {

                var existingUser = await _userManager.FindByEmailAsync(email);
                if ((existingUser != null))
                {
                    return new AuthenticationResult
                    {
                        Errors = new[] { "User already exists for this email address" }
                    };
                }

                var newUserId = Guid.NewGuid();
                var newUser = new IdentityUser
                {
                    Id = newUserId.ToString(),
                    Email = email,
                    UserName = email
                };

                var createdUser = await _userManager.CreateAsync(newUser, password);

                if (isAdmin)
                {
                    await _userManager.AddToRoleAsync(newUser, "Admin");
                }
                else
                {
                    await _userManager.AddToRoleAsync(newUser, "Non-Admin");
                }

                if (!createdUser.Succeeded)
                {
                    return new AuthenticationResult { Errors = createdUser.Errors.Select(x => x.Description) };
                }
                return await GenerateAuthenticationResultForUserAsync(newUser);
            }catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return null;
            }
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                     new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     new Claim(JwtRegisteredClaimNames.Email, user.Email),
                     new Claim("id", user.Id)
            };
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
            };
        }
    }
}
