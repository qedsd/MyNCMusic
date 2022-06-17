using MyNCMusic.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TheGuideToTheNewEden.Helper;
using Windows.Storage;

namespace MyNCMusic.Services
{
    [Serializable]
    [Bindable(true)]
    public class ConfigService
    {
        public static StorageFolder Folder = ApplicationData.Current.LocalFolder;
        public static string ImageFilename = "playingAlbum.jpg";
        public static string ApiUri = "http://localhost:3000";
        public static long Uid = -1;
        public static string PhoneOrEmail = "";
        public static string Password = "";
        /// <summary>
        /// 音乐码率
        /// 320k
        /// </summary>
        public static int Br = 320000;
        public string apiUri { get; set; }
        public string phoneOrEmail { get; set; }
        public string password { get; set; }
        public long uid { get; set; }

        private ConfigService() { }

        public static async Task LoadConfig()
        {
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("config.xml") is StorageFile)
            {
                ConfigService configService = XmlSerializerHelper.DeserializeFromXml(Folder.Path + "/config.xml", typeof(ConfigService)) as ConfigService;
                ApiUri = configService.apiUri == null ? ApiUri : configService.apiUri;
                Uid = configService.uid == 0 ? Uid : configService.uid;
                PhoneOrEmail = configService.phoneOrEmail == null ? PhoneOrEmail : configService.phoneOrEmail;
                Password = configService.password == null ? Password : configService.password;
            }
            ReadLocalCookie();
        }

        public static async void ReadLocalCookie()
        {
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(CookieHelper.SavedFileName) is StorageFile)
            {
                var ck = CookieHelper.ReadCookiesFromDisk(ConfigService.Folder.Path + "/" + CookieHelper.SavedFileName);
                if (ck != null)
                    Http.cookies = ck;
                else
                    Http.cookies = new CookieContainer();
            }
             else
            {
                Http.cookies = new CookieContainer();
            }
        }

        public static void SaveConfig()
        {
            XmlSerializerHelper.SerializeToXml(Folder.Path + "/config.xml", new ConfigService()
            {
                apiUri=ApiUri,
                phoneOrEmail=PhoneOrEmail,
                password=Password,
                uid=Uid
            });
        }
    }
}
