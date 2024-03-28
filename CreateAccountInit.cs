using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Framework.Initialization;
using EPiServer.Framework;
using Microsoft.AspNetCore.Identity;


[InitializableModule]
[ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
public class EpiserverInitialization : IInitializableModule
{
    public const string USERNAME = "sysadmin";
    public const string PASSWORD = "Simple123#";

    private static readonly string[] _roles = { "WebAdmins", "WebEditors" };

    public void Initialize(InitializationEngine context)
    {
        var appUserManager = context.Locate.Advanced.GetService<ApplicationUserManager<ApplicationUser>>();
        var roleManager = context.Locate.Advanced.GetService<RoleManager<IdentityRole>>();
        var sysAdminExists = appUserManager.Users.ToList().Any(x => x.UserName == USERNAME);
        if(!sysAdminExists)
        {
            var newUser = CreateUser(appUserManager, USERNAME, PASSWORD, "noreply@mysite.com");
            AddUserToRoles(appUserManager, roleManager, newUser, _roles);
            appUserManager.UpdateAsync(newUser).GetAwaiter().GetResult();
        }
    }

    private ApplicationUser CreateUser(ApplicationUserManager<ApplicationUser> store, string username, string password, string email)
    {
        ApplicationUser applicationUser = new ApplicationUser
        {
            Email = email,
            EmailConfirmed = true,
            LockoutEnabled = true,
            IsApproved = true,
            UserName = username,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        IPasswordHasher<ApplicationUser> hasher = store.PasswordHasher;
        string passwordHash = hasher.HashPassword(applicationUser, password);
        applicationUser.PasswordHash = passwordHash;

        store.CreateAsync(applicationUser).GetAwaiter().GetResult();

        //Get the user associated with our username
        ApplicationUser createdUser = store.FindByNameAsync(username).GetAwaiter().GetResult();
        return createdUser;
    }

    private void AddUserToRoles(ApplicationUserManager<ApplicationUser> store, RoleManager<IdentityRole> roleManager, ApplicationUser user, string[] roles)
    {
        using (roleManager)
        {
            IList<string> userRoles = store.GetRolesAsync(user).GetAwaiter().GetResult();
            foreach (string roleName in roles)
            {
                if (roleManager.FindByNameAsync(roleName).GetAwaiter().GetResult() == null)
                {
                    roleManager.CreateAsync(new IdentityRole { Name = roleName }).GetAwaiter().GetResult();
                }
                if (!userRoles.Contains(roleName))
                    store.AddToRoleAsync(user, roleName).GetAwaiter().GetResult();
            }
        }
    }

    public void Uninitialize(InitializationEngine context)
    {
    }

    public void Preload(string[] parameters)
    {
    }
}