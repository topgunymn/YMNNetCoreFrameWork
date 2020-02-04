using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YMNNetCoreFrameWork.Core.Authoratication
{
    [Table("UserTokens")]
   public    class UserToken:IdentityUserToken<int>
    {


        public DateTime? DeletionTime { get; set; }

        [MaxLength(255)]
        public string DeleterUserId { get; set; }

        /// <summary>
        ///租户编号
        /// </summary>
        public int TenantId { get; set; }
    }
}
