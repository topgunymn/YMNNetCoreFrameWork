using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace YMNNetCoreFrameWork.EntityFrameworkCore
{
   public   class YMNContext:DbContext
    {
        public YMNContext(DbContextOptions<YMNContext> options)
            : base(options) {

        }
    }
}
