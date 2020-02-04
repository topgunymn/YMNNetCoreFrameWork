using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using YMNNetCoreFrameWork.Core.Authoratication;

namespace YMNNetCoreFrameWork.EntityFrameworkCore
{
   public   class YMNContext:DbContext
    {
        public YMNContext(DbContextOptions<YMNContext> options)
            : base(options) {

        }

        #region 权限认证
        public DbSet<User> Users  { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Role> Roles  { get; set; }


        public DbSet<UserClaim> UserClaims { get; set; }

        public DbSet<RoleClaim> RoleClaims  { get; set; }

        public DbSet<UserLogin> UserLogins { get; set; }

        public DbSet<UserToken> UserTokens  { get; set; }
        
        #endregion
    }
}
