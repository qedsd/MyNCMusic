using MyNCMusic.Helper;
using MyNCMusic.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNCMusic.Services
{
    public static class LoginHelper
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        /// <returns></returns>
        public static LoginRoot LoginAccount()
        {
            if (Http.cookies.GetCookies(new Uri(ConfigService.ApiUri + "/login")).Count != 0)//存在cookies，检查登陆状态
            {
                var status = GetLoginStatus();
                if (status != null && status.Data.code == 200)
                    return status.Data;
            }
                string result = null;
                if (ConfigService.PhoneOrEmail.Contains('@'))
                {
                    result = Http.Get(ConfigService.ApiUri + @"/login?email=" + ConfigService.PhoneOrEmail + "&md5_password=" + OtherHelper.Encrypt(ConfigService.Password));
                }
                else
                {
                    result = Http.Get(ConfigService.ApiUri + @"/login/cellphone?phone=" + ConfigService.PhoneOrEmail + "&md5_password=" + OtherHelper.Encrypt(ConfigService.Password));
                }
                if (result == null || result.Equals(""))
                    return null;
                try
                {
                    return JsonConvert.DeserializeObject<LoginRoot>(result);
                }
                catch (Exception er) { OtherHelper.ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 检查登陆状态
        /// </summary>
        /// <returns></returns>
        static LoginStatus GetLoginStatus()
        {
            string result = Http.Get(ConfigService.ApiUri + "/login/status");
            return JsonConvert.DeserializeObject<LoginStatus>(result);
        }
    }
}
