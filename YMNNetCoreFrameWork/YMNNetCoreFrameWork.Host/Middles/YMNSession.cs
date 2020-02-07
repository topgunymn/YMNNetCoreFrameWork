using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YMNNetCoreFrameWork.Host.Middles
{
    public class YMNSession
    {

        #region Initialize

        private static IHttpContextAccessor _httpContextAccessor;

        private static ISession _session => _httpContextAccessor.HttpContext.Session;

        /// <summary>
        /// 注入session
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Attribute

       
        /// <summary>
        ///用户编号 
        /// </summary>
        public static long UserId
        {
            get => _session == null ? 0 : Convert.ToInt64( _session.GetString("CurrentUser_UserId"));
            set => _session.SetString("CurrentUser_UserId", value.ToString() );
        }

        /// <summary>
        /// 登录名
        /// </summary>
        public static string UserName
        {
            get => _session == null ? "" : _session.GetString("CurrentUser_UserName");
            set => _session.SetString("CurrentUser_UserName", !string.IsNullOrEmpty(value) ? value : "");
        }

        ///// <summary>
        ///// 用户登录账户
        ///// </summary>
        //public static string UserAccount
        //{
        //    get => _session == null ? "" : _session.GetString("CurrentUser_UserAccount");
        //    set => _session.SetString("CurrentUser_UserAccount", !string.IsNullOrEmpty(value) ? value : "");
        //}

        public static int TenantId {
            get => _session == null ?  0: _session.GetInt32("CurrentUser_TenantId").Value;
            set => _session.SetInt32("CurrentUser_TenantId",value);
            }

        ///// <summary>
        ///// 用户头像地址
        ///// </summary>
        //public static string UserImage
        //{
        //    get => _session == null ? "" : _session.GetString("CurrentUser_UserImage");
        //    set => _session.SetString("CurrentUser_UserImage", !string.IsNullOrEmpty(value) ? value : "");
        //}

        ///// <summary>
        ///// 用户角色
        ///// </summary>
        //public static string UserRole
        //{
        //    get => _session == null ? "" : _session.GetString("CurrentUser_UserRole");
        //    set => _session.SetString("CurrentUser_UserRole", !string.IsNullOrEmpty(value) ? value : "");
        //}


        #endregion
    }
}
