using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YMNNetCoreFrameWork.Core.Authoratication
{

    [Table("UserRoles")]
   public  class UserRole:IdentityUserRole<int>
    {
        [Key]
        public long Id { get; set; }

        public bool IsDelete { get; set; }


        public DateTime? DeletionTime { get; set; }

        [MaxLength(255)]
        public string DeleterUserId { get; set; }

        /// <summary>
        ///租户编号
        /// </summary>
        public int TenantId { get; set; }
    }
}
