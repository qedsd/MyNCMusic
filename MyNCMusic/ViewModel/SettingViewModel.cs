using MyNCMusic.Helper;
using MyNCMusic.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.ViewModel
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    internal class SettingViewModel
    {
        public string ApiUri { get; set; }
        public string PhoneOrEmail { get; set; }
        public string Password { get; set; }
        //private bool isQr = false;
        //public bool IsQr
        //{
        //    get => isQr;
        //    set
        //    {
        //        isQr = value;
        //        if(isQr)
        //        {
        //            CreateQr();
        //        }
        //    }
        //}
        public WriteableBitmap QrBitmap { get; set; }
        public string Br { get; set; }
        public SettingViewModel()
        {
            switch(ConfigService.Br)
            {
                case 128000:Br = "128k";break;
                case 320000: Br = "320k"; break;
                default: Br = "无损";break;
            }
            ApiUri = ConfigService.ApiUri;
            PhoneOrEmail = ConfigService.PhoneOrEmail;
            Password = ConfigService.Password;
            CreateQr();
        }

        private async void CreateQr()
        {
            string json = await Http.GetAsync(ConfigService.ApiUri + "/login/qr/key");
            if (!string.IsNullOrEmpty(json))
            {
                var jo = JsonConvert.DeserializeObject<Models.ResponseRoot<Models.Qr.QrKey>>(json);
                string key = jo.Data.Unikey;
                json = await Http.GetAsync(ConfigService.ApiUri + $"/login/qr/create?key={key}&qrimg=true");
                if(!string.IsNullOrEmpty(json))
                {
                    var qrimg = JsonConvert.DeserializeObject<Models.ResponseRoot<Models.Qr.QrImg>>(json);
                    try
                    {
                        QrBitmap = await Base64StringToBitmapAsync(qrimg.Data.Qrimg.Substring(22));
                        _=Task.Run(async() =>
                        {
                            while(true)
                            {
                                System.Threading.Thread.Sleep(500);
                                json = await Http.GetAsync(ConfigService.ApiUri + $"/login/qr/check?key={key}");
                                var status = JsonConvert.DeserializeObject<Models.ResponseRoot<Models.Qr.QrCheck>>(json);
                                switch(status.Code)
                                {
                                    case 800:break;//过期
                                    case 801: break;//等待扫码
                                    case 802: break;//确认
                                    case 803: break;//授权登录成功,返回 cookies
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        MyUserControl.NotifyPopup.ShowError(ex.Message);
                    }
                }
            }
        }

        public static async Task<WriteableBitmap> Base64StringToBitmapAsync(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
            await stream.WriteAsync(bytes.AsBuffer());
            stream.Seek(0);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            WriteableBitmap writebleBitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
            await writebleBitmap.SetSourceAsync(stream);
            return writebleBitmap;
        }
    }
}
