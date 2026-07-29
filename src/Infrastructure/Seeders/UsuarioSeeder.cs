using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Seeders
{
   public class AppUserSeeder
    {
       

        public static void Seed(ModelBuilder builder)
        {
            var usuarioAdmin = new AppUser {Id=AppUser.AdminUserId, IsActive = true, MustChangePassword = false,AccessIdentifier=AppUser.adminUserEmail, RegisteredAt=DateTime.Now, Name="Administrador del sistema", UserType= "internal-user", Password= "52A5D13A7FD60FFFFF425FA65C3830A165969AA983F06C365E48BAC0F8C75CD9",  };
           builder.Entity<AppUser>().HasData(usuarioAdmin);
        }
    }
}
