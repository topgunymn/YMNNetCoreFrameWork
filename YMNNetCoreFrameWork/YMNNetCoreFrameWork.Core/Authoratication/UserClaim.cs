﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YMNNetCoreFrameWork.Core.Authoratication
{
    [Table("UserClaims")]
   public  class UserClaim:IdentityUserClaim<int>
    {
        public DateTime? CreateTime { get; set; }

        [MaxLength(255)]
        public string CreateUserId { get; set; }


        public int TenantId { get; set; }
    }
}
