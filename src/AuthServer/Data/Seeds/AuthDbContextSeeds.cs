using AuthServer.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Data.Seeds;

public static class AuthDbContextSeeds
{
    public static void Seed(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "c32ba259-6094-474b-a730-60b8aae724e2", Name = Common.Constants.ADMIN, NormalizedName = Common.Constants.ADMIN
            },
            new IdentityRole
            {
                Id = "d999706f-5829-4be8-bc51-05383533dfb3", Name = Common.Constants.COUNTERPART, NormalizedName = Common.Constants.COUNTERPART
            },
            new IdentityRole
            {
                Id = "eb161112-0780-4099-94cc-c89a78257aff", Name = Common.Constants.PLAYER, NormalizedName = Common.Constants.PLAYER
            }
        );

        builder.Entity<User>().HasData(
            new User
            {
                Id = "9de65cd0-9b44-4266-a902-d8d907a13671",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                FullName = "Quản trị viên",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                PasswordHash = "AQAAAAIAAYagAAAAEFbM0iIX4wZv1ay/yZApBfh5f6Rv60QDiMxUAvvu+lUfdj3SNhAJpoI+jcvg+v9DbQ==", // password: Admin@123
                SecurityStamp = "TQXRJCFWDCRPAM7NWGC6DL2G3W5MMXKT",
                ConcurrencyStamp = "b5c97c3c-4201-452b-a3c8-e3a74cc1e1f9",
                CreatedBy = Common.Constants.SYSTEM,
                Role = Common.Constants.ADMIN
            }
        );

        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "c32ba259-6094-474b-a730-60b8aae724e2",
                UserId = "9de65cd0-9b44-4266-a902-d8d907a13671"
            }
        );
    }
}