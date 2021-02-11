using MyNCMusic.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TheGuideToTheNewEden.Helper;
using Windows.Storage;

namespace MyNCMusic.Services
{
    [Serializable]
    public class ConfigService
    {
        public static StorageFolder Folder = ApplicationData.Current.LocalFolder;
        public static string ImageFilename = "playingAlbum.jpg";
        public static string ApiUri = "http://localhost:3000";
        public static long Uid = -1;
        public static string PhoneOrEmail = "";
        public static string Password = "";
        public string _apiUri { get; set; }
        public string _phoneOrEmail { get; set; }
        public string _password { get; set; }
        public long _uid { get; set; }

        private ConfigService() { }

        public static async void LoadConfig()
        {
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("config.xml") is StorageFile)
            {
                ConfigService configService = XmlSerializerHelper.DeserializeFromXml(Folder.Path + "/config.xml", typeof(ConfigService)) as ConfigService;
                ApiUri = configService._apiUri == null ? ApiUri : configService._apiUri;
                Uid = configService._uid == 0 ? Uid : configService._uid;
                PhoneOrEmail = configService._phoneOrEmail == null ? PhoneOrEmail : configService._phoneOrEmail;
                Password = configService._password == null ? Password : configService._password;
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

        /// <summary>
        ///
        /// </summary>
        public static void SaveConfig()
        {
            XmlSerializerHelper.SerializeToXml(Folder.Path + "/config.xml", new ConfigService()
            {
                _apiUri=ApiUri,
                _phoneOrEmail=PhoneOrEmail,
                _password=Password,
                _uid=Uid
            });
        }


        
    }
}
