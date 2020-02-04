using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YMNNetCoreFrameWork.Core.EntityBase
{
    public class EnityBase<TKey>
    {
        [Key]
        public TKey Id { get; set; }
        public const int MaxUserIdLength = 255;
        public DateTime? CreateTime { get; set; }

        [MaxLength(MaxUserIdLength)]
        public string CreateUserId { get; set; }


        public DateTime? LastModificationTime { get; set; }

        [MaxLength(MaxUserIdLength)]
        public string LastModifiterUserId { get; set; }

        public bool IsDelete { get; set; }


        public DateTime? DeletionTime { get; set; }

        [MaxLength(MaxUserIdLength)]
        public string DeleterUserId { get; set; }

        /// <summary>
        ///租户编号
        /// </summary>
        public int TenantId { get; set; }
    }
}
