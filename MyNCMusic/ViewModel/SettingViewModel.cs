using MyNCMusic.Helper;
using MyNCMusic.Models;
using MyNCMusic.Services;
using Newtonsoft.Json;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
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

        public WriteableBitmap QrBitmap { get; set; }
        public string Br { get; set; }
        public bool IsWaitingConfirm { get; set; } = false;
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
        }

        public ICommand GetQrCommand=>new DelegateCommand(()=>
        {
            CreateQr(ApiUri);
        });

        public ICommand LoginCommand => new DelegateCommand(async() =>
        {
            if (string.IsNullOrEmpty(ApiUri) || string.IsNullOrEmpty(PhoneOrEmail) || string.IsNullOrEmpty(Password))
            {
                MyUserControl.NotifyPopup.ShowError("缺失信息");
                return;
            }
            ConfigService.ApiUri = ApiUri;
            ConfigService.PhoneOrEmail = PhoneOrEmail;
            ConfigService.Password = OtherHelper.Encrypt(Password);

            string result = null;
            Controls.WaitingPopup.Show();
            if (ConfigService.PhoneOrEmail.Contains('@'))
            {
                result = await Http.GetAsync(ConfigService.ApiUri + @"/login?email=" + ConfigService.PhoneOrEmail + "&md5_password=" + ConfigService.Password);
            }
            else
            {
                result = await Http.GetAsync(ConfigService.ApiUri + @"/login/cellphone?phone=" + ConfigService.PhoneOrEmail + "&md5_password=" + ConfigService.Password);
            }
            Controls.WaitingPopup.Hide();
            if (!string.IsNullOrEmpty(result))
            {
                LoginRoot loginRoot = JsonConvert.DeserializeObject<LoginRoot>(result);
                if (loginRoot == null || loginRoot.code != 200)
                {
                    ConfigService.Uid = -1;
                    MyUserControl.NotifyPopup.ShowError("登陆失败");
                    return;
                }
                else
                {
                    CookieHelper.WriteCookiesToDisk(ConfigService.Folder.Path + "/" + CookieHelper.SavedFileName, Http.cookies);
                    ConfigService.Uid = loginRoot.account.id;
                    ConfigService.SaveConfig();
                    await Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync(String.Empty);
                }
            }
        });

        private bool KeepCheckingQr = false;
        private CancellationTokenSource CancellationTokenSource;
        private async void CreateQr(string api = null)
        {
            TryCancelQr();
            if (api == null)
            {
                api = ConfigService.ApiUri;
            }
            if(api == null)
            {
                MyUserControl.NotifyPopup.ShowError("无效API");
                return;
            }
            string json = await Http.GetAsync(api + "/login/qr/key");
            if (!string.IsNullOrEmpty(json))
            {
                var jo = JsonConvert.DeserializeObject<Models.ResponseRoot<Models.Qr.QrKey>>(json);
                string key = jo.Data.Unikey;
                json = await Http.GetAsync(api + $"/login/qr/create?key={key}&qrimg=true");
                if(!string.IsNullOrEmpty(json))
                {
                    var qrimg = JsonConvert.DeserializeObject<Models.ResponseRoot<Models.Qr.QrImg>>(json);
                    try
                    {
                        QrBitmap = await Base64StringToBitmapAsync(qrimg.Data.Qrimg.Substring(22));
                        KeepCheckingQr = true;
                        CancellationTokenSource = new CancellationTokenSource();
                        _ = Task.Run(async() =>
                        {
                            while(KeepCheckingQr)
                            {
                                System.Threading.Thread.Sleep(500);
                                if(CancellationTokenSource.IsCancellationRequested)
                                {
                                    break;
                                }
                                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                json = await Http.GetAsync(api + $"/login/qr/check?key={key}&timerstamp={Convert.ToInt64(ts.TotalSeconds)}");
                                var status = JsonConvert.DeserializeObject<Models.ResponseRoot<Models.Qr.QrCheck>>(json);
                                switch(status.Code)
                                {
                                    case 800:
                                        {
                                            var jsonRefresh = await Http.GetAsync(api + $"/login/qr/create?key={key}&qrimg=true");
                                            var qrimgRefresh = JsonConvert.DeserializeObject<Models.ResponseRoot<Models.Qr.QrImg>>(json);
                                            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,async() =>
                                            {
                                                QrBitmap = await Base64StringToBitmapAsync(qrimgRefresh.Data.Qrimg.Substring(22));
                                            });
                                        }
                                        break;//过期
                                    case 801: break;//等待扫码
                                    case 802:
                                        {
                                            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                                            {
                                                IsWaitingConfirm = true;
                                            });
                                        }
                                        break;//确认
                                    case 803:
                                        {
                                            CookieHelper.WriteCookiesToDisk(ConfigService.Folder.Path + "/" + CookieHelper.SavedFileName, Http.cookies);
                                            KeepCheckingQr = false;
                                            ConfigService.ApiUri = ApiUri;
                                            ConfigService.SaveConfig();
                                            await Windows.ApplicationModel.Core.CoreApplication.RequestRestartAsync(String.Empty);
                                        }
                                        break;//授权登录成功,返回 cookies
                                }
                            }
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                            {
                                IsWaitingConfirm = false;
                            });
                        }, CancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        MyUserControl.NotifyPopup.ShowError(ex.Message);
                    }
                }
            }
        }
        public void TryCancelQr()
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Cancel();
                QrBitmap = null;
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
