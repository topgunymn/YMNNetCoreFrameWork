using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YMNNetCoreFrameWork.Core.EntityBase;

namespace YMNNetCoreFrameWork.Core.Authoratication
{
    [Table("YMNPermissions")]
   public  class YMNPermissions:EnityBase<int>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [MaxLength(50)]
        public string DisplayName { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }
    }
}
