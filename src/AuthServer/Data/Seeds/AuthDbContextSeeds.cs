using AuthServer.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Common;

namespace AuthServer.Data.Seeds;

public static class AuthDbContextSeeds
{
    public static void Seed(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", Name = Constants.ADMIN, NormalizedName = Constants.ADMIN
            },
            new IdentityRole
            {
                Id = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", Name = Constants.COUNTERPART, NormalizedName = Constants.COUNTERPART
            },
            new IdentityRole
            {
                Id = "cccccccc-cccc-cccc-cccc-cccccccccccc", Name = Constants.PLAYER, NormalizedName = Constants.PLAYER
            }
        );

        builder.Entity<User>().HasData(
            new User
            {
                Id = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                FullName = "Quản trị viên",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                PasswordHash = "AQAAAAIAAYagAAAAEFbM0iIX4wZv1ay/yZApBfh5f6Rv60QDiMxUAvvu+lUfdj3SNhAJpoI+jcvg+v9DbQ==", // password: Admin@123
                SecurityStamp = "TQXRJCFWDCRPAM7NWGC6DL2G3W5MMXKT",
                ConcurrencyStamp = "b5c97c3c-4201-452b-a3c8-e3a74cc1e1f9",
                CreatedBy = Constants.SYSTEM,
                Role = Constants.ADMIN
            }
        );

        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                RoleId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
                UserId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
            }
        );
    }
}