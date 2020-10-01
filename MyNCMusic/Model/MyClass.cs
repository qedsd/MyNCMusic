using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Playback;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Security.Cryptography;

namespace MyNCMusic.Model
{
    class MyClass
    {
    }
    public class MyClassManager
    {
        public static StorageFolder folder = ApplicationData.Current.LocalFolder;
        public static string imageFilename = "playingAlbum.jpg";
        public static string apiUri = "http://localhost:3000";
        public static string avatarImgIdStr = "";
        public static long uid = -1;
        public static string phoneOrEmail = "";
        public static string password = "";

        public static async Task<BitmapImage> DownloadFile(Uri uri)
        {
            try
            {
                Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient();
                IBuffer buffer = await http.GetBufferAsync(uri);
                BitmapImage img = new BitmapImage();
                using (IRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(buffer);
                    stream.Seek(0);
                    await img.SetSourceAsync(stream);
                    await StorageImageFolder(stream, uri);
                    return img;
                }
            }
            catch (Exception) { return null; }
        }

        private static string Md5(string str)
        {
            HashAlgorithmProvider hashAlgorithm =
                 HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash cryptographic = hashAlgorithm.CreateHash();
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            cryptographic.Append(buffer);
            return CryptographicBuffer.EncodeToHexString(cryptographic.GetValueAndReset());
        }
        private static async Task<byte[]> ConvertIRandomAccessStreamByte(IRandomAccessStream stream)
        {
            DataReader read = new DataReader(stream.GetInputStreamAt(0));
            await read.LoadAsync((uint)stream.Size);
            byte[] temp = new byte[stream.Size];
            read.ReadBytes(temp);
            return temp;
        }
        private static async Task<BitmapImage> ReadLoaclBitmapImage(string name)
        {
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(name);
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage img = new BitmapImage();
                await img.SetSourceAsync(stream);
                return img;
            }
        }

        private static async Task<byte[]> StorageImageFolder(IRandomAccessStream stream, Uri uri)
        {
            StorageFile file = await folder.CreateFileAsync(imageFilename, CreationCollisionOption.ReplaceExisting);
            var by = await ConvertIRandomAccessStreamByte(stream);
            await FileIO.WriteBytesAsync(file, by);
            return by;
        }

        public static async Task<WriteableBitmap> OpenWriteableBitmapFile(StorageFile file)
        {
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                try
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    WriteableBitmap image = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                    image.SetSource(stream);

                    return image;
                }
                catch (Exception) { return new WriteableBitmap(200,200); }
            }
        }

        //颜色加深、减淡
        public static Color ChangeColor(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }
            if (red < 0) red = 0;
            if (red > 255) red = 255;
            if (green < 0) green = 0;
            if (green > 255) green = 255;
            if (blue < 0) blue = 0;
            if (blue > 255) blue = 255;
            return Color.FromArgb(color.A, Byte.Parse(((int)red).ToString()), Byte.Parse(((int)green).ToString()), Byte.Parse(((int)blue).ToString()));
        }

        //通用网络请求Get
        public static async Task<string> HttpClientGet(string uri)
        {
            var http = new HttpClient();
            //http.DefaultRequestHeaders.Add("User-Agent", "TheGuideToTheNewEden");
            http.DefaultRequestHeaders.Add("withCredentials", "true");
            HttpResponseMessage response = null;
            try
            {
                response = await http.GetAsync(new Uri(uri));
            }
            catch (Exception)
            {
                http.Dispose();
                return null;
            }
            http.Dispose();
            if (!response.IsSuccessStatusCode)
            {
                //var ttt=response.Headers.Location;
                //await HttpClientGet(apiUri + @"/login/refresh");
                //string r=await HttpClientGet(uri);
                return null;
            }
            return await response.Content.ReadAsStringAsync();
        }

        //获取歌单详细
        public static PlayListDetailRoot GetPlayListDetail(long id)
        {
            string result = Http.Get(apiUri + @"/playlist/detail?id="+id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<PlayListDetailRoot>(result);
            }
            catch (Exception er) {  ShowContentDialog(er.ToString()); return null; }
        }

        //获取歌曲详细
        public static MusicDetailRoot GetMusicDetail(string ids)
        {
            string result = Http.Get(apiUri + @"/song/detail?ids=" + ids);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MusicDetailRoot>(result);
            }
            catch (Exception) { return null; }
        }
        //获取歌曲详细
        public static MusicDetailRoot GetMusicDetail_post(string ids)
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);

            string result = Http.Post(apiUri + @"/song/detail?timestamp="+ Convert.ToInt64(ts.TotalSeconds).ToString(), "ids="+ids);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MusicDetailRoot>(result);
            }
            catch (Exception) { return null; }
        }

        //获取歌曲url
        public static SongUrlRoot GetMusicUrl(int id)
        {
            string result = Http.Get(apiUri + @"/song/url?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<SongUrlRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取专辑详细
        /// </summary>
        /// <param name="id">专辑id</param>
        /// <returns></returns>
        public static AlbumRoot GetMAlbum(long id)
        {
            string result = Http.Get(apiUri + @"/album?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<AlbumRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取喜欢的歌曲
        /// </summary>
        /// <returns></returns>
        public static FavoriteSongsRoot GetFavoriteSongs()
        {
            string result = Http.Get(apiUri + @"/likelist");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<FavoriteSongsRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取我的歌单
        /// </summary>
        /// <returns></returns>
        public static MyPlaylistRoot GetMyPlaylist()
        {
            string result = Http.Get(apiUri + @"/user/playlist?uid="+uid+ "&limit=1000");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MyPlaylistRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取我收藏的专辑
        /// </summary>
        /// <returns></returns>
        public static MyCollectionfAlbumRoot GetMyCollectionOfAlbum()
        {
            string result = Http.Get(apiUri + @"/album/sublist?limit=1000");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MyCollectionfAlbumRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取我收藏的歌手
        /// </summary>
        /// <returns></returns>
        public static MyCollectionfArtistRoot GetMyCollectionOfArtist()
        {
            string result = Http.Get(apiUri + @"/artist/sublist");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<MyCollectionfArtistRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }
        /// <summary>
        /// 获取歌手基本详细信息
        /// </summary>
        /// <param name="id">歌手id</param>
        /// <returns></returns>
        public static ArtistBaseDetailRoot GetArtistBaseDetail(long id)
        {
            string result = Http.Get(apiUri + @"/artists?id="+id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<ArtistBaseDetailRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取歌手所有（热门）专辑信息
        /// </summary>
        /// <param name="id">歌手id</param>
        /// <returns></returns>
        public static ArtistAllAlbumRoot GetArtistAllAlbums(long id)
        {
            string result = Http.Get(apiUri + @"/artist/album?id=" + id+ "&limit=1000");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<ArtistAllAlbumRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取歌词
        /// </summary>
        /// <param name="id">音乐id</param>
        /// <returns></returns>
        public static LyricRoot GetLyric(long id)
        {
            string result = Http.Get(apiUri + @"/lyric?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<LyricRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取歌曲评论
        /// </summary>
        /// <param name="id">音乐id</param>
        /// <returns></returns>
        public static CommentRoot GetSongsComment(long id)
        {
            string result = Http.Get(apiUri + @"/comment/music?id=" + id + "&limit=100");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<CommentRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 获取相似歌曲
        /// </summary>
        /// <param name="id">音乐id</param>
        /// <returns></returns>
        public static SimiSongsRoot GetSimiSongs(long id)
        {
            string result = Http.Get(apiUri + @"/simi/song?id=" + id);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<SimiSongsRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="type">1: 单曲, 10: 专辑, 100: 歌手, 1000: 歌单</param>
        /// <returns></returns>
        public static SearchRoot SearchClound(string keyword, int type)
        {
            string result = Http.Get(apiUri + @"/cloudsearch?keywords=" + keyword+ "&type="+type);
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<SearchRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 喜欢/不喜欢歌曲
        /// </summary>
        /// <param name="id">音乐id</param>
        /// <param name="b">true 即喜欢 , 若传 false, 则取消喜欢</param>
        /// <returns></returns>
        public static bool LoveOrDontLove_songs(long id,bool b)
        {
            string result = Http.Get(apiUri + @"/like?id=" + id+"&like="+b.ToString());
            if (result == null || result.Equals(""))
                return false;
            else
                return true;
        }

        /// <summary>
        /// 获取歌单评论
        /// </summary>
        /// <param name="id">歌单id</param>
        /// <returns></returns>
        public static CommentRoot GetPlayListComment(long id)
        {
            string result = Http.Get(apiUri + @"/comment/playlist?id=" + id + "&limit=100");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<CommentRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 收藏/不收藏歌单
        /// </summary>
        /// <param name="id">歌单id</param>
        /// <param name="t">1:收藏,2:取消收藏</param>
        /// <returns></returns>
        public static bool SubOrCancelPlayList(long id, int t)
        {
            string result = Http.Get(apiUri + @"/playlist/subscribe?t=" + t + "&id=" + id);
            if (result == null || result.Equals(""))
                return false;
            else
                return true;
        }

        /// <summary>
        /// 获取歌单评论
        /// </summary>
        /// <param name="id">歌单id</param>
        /// <returns></returns>
        public static CommentRoot GetAlbumComment(long id)
        {
            string result = Http.Get(apiUri + @"/comment/album?id=" + id + "&limit=100");
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<CommentRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }

        /// <summary>
        /// 错误弹窗
        /// </summary>
        /// <param name="content">错误显示string</param>
        public static async void ShowContentDialog(string content = "ERROR")
        {
            try
            {
                TextBlock textBlock = new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                textBlock.Text = content;
                var dialog = new ContentDialog()
                {
                    Title = "Error!",
                    Content = textBlock,
                    PrimaryButtonText = "关闭",
                    FullSizeDesired = false,
                };
                dialog.PrimaryButtonClick += (_s, _e) => { };
                await dialog.ShowAsync();
            }
            catch (Exception) { }
        }
        /// <summary>
        /// 带标题的弹窗
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        public static async void ShowContentDialog(string title = "ERRO", string content = "ERRO")
        {
            TextBlock textBlock = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Center
            };
            textBlock.Text = content;
            var dialog = new ContentDialog()
            {
                Title = title,
                Content = textBlock,
                PrimaryButtonText = "关闭",
                FullSizeDesired = false,
                
            };
            dialog.PrimaryButtonClick += (_s, _e) => { };
            try
            {
                await dialog.ShowAsync();
            }
            catch (Exception) {}
        }
        /// <summary>
        /// 获取分钟形式的时长
        /// </summary>
        /// <param name="dt">秒</param>
        /// <returns></returns>
        public static string GetDt(int dt)
        {
            TimeSpan ts = TimeSpan.FromSeconds(dt);
            //return String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);

            //TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(dt));
            string str = "";
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds;
            }
            else
                return String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
            return str;
        }

        /// <summary>
        /// 分析提取歌词
        /// </summary>
        /// <param name="lyricRoot">原始获得的歌词实例</param>
        /// <returns>用于滚动显示的歌词</returns>
        public static List<LyricStr> GetLyricStrs(LyricRoot lyricRoot)
        {
            List<LyricStr> lyricStrs = new List<LyricStr>();
            if (lyricRoot.lrc != null && lyricRoot.lrc.lyric != null)//原歌词
            {
                var array = lyricRoot.lrc.lyric.Split("\n");
                foreach (var temp in array)
                {
                    var array2 = temp.Split(new char[2] { '[', ']' });
                    if (array2.Length != 3)
                        continue;
                    var array3 = array2[1].Split(new char[2] { ':', '.' });
                    if (array3.Length != 3)
                        continue;
                    DateTime dateTime = new DateTime();
                    try
                    {
                        dateTime = dateTime.AddMinutes(double.Parse(array3[0]));
                        dateTime = dateTime.AddSeconds(double.Parse(array3[1]));
                        dateTime = dateTime.AddMilliseconds(double.Parse(array3[2]));
                        lyricStrs.Add(new LyricStr() { DateTime = dateTime, Original = array2[2] });
                    }
                    catch (Exception) { }
                }
            }
            if (lyricRoot.tlyric != null && lyricRoot.tlyric.lyric != null)//翻译歌词
            {
                var array = lyricRoot.tlyric.lyric.Split("\n");
                foreach (var temp in array)
                {
                    var array2 = temp.Split(new char[2] { '[', ']' });
                    if (array2.Length != 3)
                        continue;
                    var array3 = array2[1].Split(new char[2] { ':', '.' });
                    if (array3.Length != 3)
                        continue;
                    DateTime dateTime = new DateTime();
                    try
                    {
                        dateTime = dateTime.AddMinutes(double.Parse(array3[0]));
                        dateTime = dateTime.AddSeconds(double.Parse(array3[1]));
                        dateTime = dateTime.AddMilliseconds(double.Parse(array3[2]));
                        var found = lyricStrs.Find(p => p.DateTime.TimeOfDay == dateTime.TimeOfDay);
                        if (found != null)
                            found.Tran = array2[2];
                        else
                            lyricStrs.Add(new LyricStr() { DateTime = dateTime, Original = array2[2] });
                    }
                    catch (Exception) { }
                }
            }

            return lyricStrs;
        }

        /// <summary>
        /// 返回所有艺术家名字组成的string
        /// </summary>
        /// <param name="arItems">艺术家list</param>
        /// <returns></returns>
        public static string GetArNames_ArtistsItem(List<Artist> arItems)
        {
            string name = "";
            for (int i = 0; i < arItems.Count; i++)
            {
                if (i != 0)
                    name += "/";
                name += arItems[i].name;
            }
            return name;
        }

        /// <summary>
        /// 登录账号
        /// </summary>
        /// <returns></returns>
        public static LoginRoot LoginAccount()
        {
            //string result = Http.Get(apiUri + @"/login/cellphone?phone=" + phoneNumber + "&password=" + password);
            string result=null;
            if (phoneOrEmail.Contains('@'))
            {
                result = Http.Get(apiUri + @"/login?email=" + phoneOrEmail + "&md5_password=" + Encrypt(password));
            }
            else
            {
                result = Http.Get(apiUri + @"/login/cellphone?phone=" + phoneOrEmail + "&md5_password=" + Encrypt(password));
            }
            if (result == null || result.Equals(""))
                return null;
            try
            {
                return JsonConvert.DeserializeObject<LoginRoot>(result);
            }
            catch (Exception er) { ShowContentDialog(er.ToString()); return null; }
        }
        /// <summary>
        /// 转md5
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>md5加密后的字符串</returns>
        public static string Encrypt(string str)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));//转化为小写的16进制
            }
            return sBuilder.ToString();
        }
    }

    //推荐歌单
    public class Creator
    {
        public string remarkName { get; set; }
        public bool mutual { get; set; }
        public int vipType { get; set; }
        public int userId { get; set; }
        public string detailDescription { get; set; }
        public string defaultAvatar { get; set; }
        //public string expertTags { get; set; }
        public long avatarImgId { get; set; }
        public long backgroundImgId { get; set; }
        public int province { get; set; }
        public string avatarImgIdStr { get; set; }
        public string backgroundImgIdStr { get; set; }
        public int djStatus { get; set; }
        public bool followed { get; set; }
        public string backgroundUrl { get; set; }
        public int accountStatus { get; set; }
        public int gender { get; set; }
        public string avatarUrl { get; set; }
        public int authStatus { get; set; }
        public int userType { get; set; }
        public string nickname { get; set; }
        public long birthday { get; set; }
        public int city { get; set; }
        public string description { get; set; }
        public string signature { get; set; }
        public int authority { get; set; }
    }
    public class Recommend
    {
        public long id { get; set; }
        public int type { get; set; }
        public string name { get; set; }
        public string copywriter { get; set; }
        public string picUrl { get; set; }
        public long playcount { get; set; }
        public long createTime { get; set; }
        public Creator creator { get; set; }
        public int trackCount { get; set; }
        public int userId { get; set; }
        public string alg { get; set; }
    }
    public class RecommendRoot
    {
        public int code { get; set; }
        public bool featureFirst { get; set; }
        public bool haveRcmdSongs { get; set; }
        public List<Recommend> recommend { get; set; }
    }

    //推荐歌曲
    public class ArItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<string> tns { get; set; }
        public List<string> @alias { get; set; }

        public List<string> alia { get; set; }
    }

    public class Al
    {
        public int id { get; set; }
        public string name { get; set; }
        public string picUrl { get; set; }
        public List<string> tns { get; set; }
        public string pic_str { get; set; }
        public long pic { get; set; }

        public List<string> alia { get; set; }
    }

    public class H
    {
        public int br { get; set; }
        public long fid { get; set; }
        public int size { get; set; }
        public string vd { get; set; }
    }

    public class M
    {
        public int br { get; set; }
        public long fid { get; set; }
        public int size { get; set; }
        public string vd { get; set; }
    }

    public class L
    {
        public int br { get; set; }
        public long fid { get; set; }
        public int size { get; set; }
        public string vd { get; set; }
    }

    public class Privilege
    {
        public int id { get; set; }
        public int fee { get; set; }
        public int payed { get; set; }
        public int st { get; set; }
        public int pl { get; set; }
        public int dl { get; set; }
        public int sp { get; set; }
        public int cp { get; set; }
        public int subp { get; set; }
        public string cs { get; set; }
        public int maxbr { get; set; }
        public int fl { get; set; }
        public string toast { get; set; }
        public int flag { get; set; }
        public string preSell { get; set; }

        
        public int playMaxbr { get; set; }
        public int downloadMaxbr { get; set; }
        public List<ChargeInfoListItem> chargeInfoList { get; set; }
    }

    //public class DailySongsItem
    //{
    //    public string name { get; set; }
    //    public int id { get; set; }
    //    public int pst { get; set; }
    //    public int t { get; set; }
    //    public List<ArItem> ar { get; set; }
    //    public List<string> alia { get; set; }
    //    public int pop { get; set; }
    //    public int st { get; set; }
    //    public string rt { get; set; }
    //    public int fee { get; set; }
    //    public int v { get; set; }
    //    public string crbt { get; set; }
    //    public string cf { get; set; }
    //    public Al al { get; set; }
    //    public int dt { get; set; }
    //    public H h { get; set; }
    //    public M m { get; set; }
    //    public L l { get; set; }
    //    public string a { get; set; }
    //    public string cd { get; set; }
    //    public int no { get; set; }
    //    public string rtUrl { get; set; }
    //    public int ftype { get; set; }
    //    public List<string> rtUrls { get; set; }
    //    public int djId { get; set; }
    //    public int copyright { get; set; }
    //    public int s_id { get; set; }
    //    public long mark { get; set; }
    //    public int originCoverType { get; set; }
    //    public string noCopyrightRcmd { get; set; }
    //    public int mst { get; set; }
    //    public int cp { get; set; }
    //    public int mv { get; set; }
    //    public int rtype { get; set; }
    //    public string rurl { get; set; }
    //    public long publishTime { get; set; }
    //    public string reason { get; set; }
    //    public Privilege privilege { get; set; }
    //    public string alg { get; set; }
    //}

    public class RecommendReasonsItem
    {
        public int songId { get; set; }
        public string reason { get; set; }
    }

    public class Data
    {
        public List<SongsItem> dailySongs { get; set; }
        public List<string> orderSongs { get; set; }
        public List<RecommendReasonsItem> recommendReasons { get; set; }
    }

    public class RecommendMusicsRoot
    {
        public int code { get; set; }
        public Data data { get; set; }
    }

    /// <summary>
    /// 返回歌名
    /// </summary>
    public class GetAlia : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                List<string> alia = (List<string>)value;
                if(alia.Count==0)
                    return null;
                string str = " (";
                for (int i = 0; i < alia.Count; i++)
                {
                    if (i != 0)
                        str += "/";
                    str += alia[i];
                }
                return str+")";
            }
            else
                return null;
            //List<ArItem> arItems = value as List<ArItem>;
            //string name = "";
            //for (int i = 0; i < arItems.Count; i++)
            //{
            //    if (i != 0)
            //        name += "/";
            //    name += arItems[i].name;
            //}
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 返回艺术家名字
    /// </summary>
    public class GetArNames : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            List<ArItem> arItems = value as List<ArItem>;
            string name = "";
            for(int i=0;i<arItems.Count;i++)
            {
                if (i != 0)
                    name += "/";
                name += arItems[i].name;
            }
            return name;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 返回艺术家名字
    /// </summary>
    public class GetArNames_ArtistsItem : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "";
            List<Artist> arItems = value as List<Artist>;
            string name = "";
            for (int i = 0; i < arItems.Count; i++)
            {
                if (i != 0)
                    name += "/";
                name += arItems[i].name;
            }
            return name;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 返回时长
    /// </summary>
    public class GetDt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int dt = (int)value/1000;
            TimeSpan ts = TimeSpan.FromSeconds(dt);
            return String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 返回时长,源数据为秒
    /// </summary>
    public class GetDt_S : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int i=0;double d=0.0;
            if (value.GetType() == i.GetType())
            {
                int dt = (int)value;
                if (dt == 0)
                    return "0:00";
                TimeSpan ts = TimeSpan.FromSeconds(dt);
                return String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
            }
            else if (value.GetType() == d.GetType())
            {
                double dt = (double)value;
                if (dt == 0)
                    return "0:00";
                TimeSpan ts = TimeSpan.FromSeconds(dt);
                return String.Format("{0}:{1:D2}", ts.Minutes, ts.Seconds);
            }
            else
                return "0:00";

        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// double返回int
    /// </summary>
    public class ReturnInByDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double t = (double)value;
            return (int)t;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 是否在播放，返回颜色
    /// </summary>
    public class ReturnForegroundIsPlaying : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool t = (bool)value;
            if (t)
            //return new SolidColorBrush(Windows.UI.ViewManagement.UIColorType.Accent);
            {
                Windows.UI.ViewManagement.UISettings uISettings = new Windows.UI.ViewManagement.UISettings();
                return new SolidColorBrush(uISettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent));
            }
            else
                return new SolidColorBrush(Colors.White);
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 修改图片大小-48
    /// </summary>
    public class ReturnImageUriWithParam_48 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            string str = (string)value;
            return new BitmapImage(new Uri(str += "?param=48y48"));
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 修改图片大小-160
    /// </summary>
    public class ReturnImageUriWithParam_160 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            string str = (string)value;
            return new BitmapImage(new Uri(str += "?param=160y160"));
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 是否在播放，返回visibility
    /// </summary>
    public class GetPlayingIconVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility == false)
                return DependencyProperty.UnsetValue;
            if ((Visibility)value == Visibility.Visible)
                return true;
            else
                return false;
        }
    }


    public class GetFavoriteIconVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isF = (bool)value;
            if (!isF)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility == false)
                return DependencyProperty.UnsetValue;
            if ((Visibility)value == Visibility.Visible)
                return true;
            else
                return false;
        }
    }


    //搜索
    public class Artist
    {
        public int id { get; set; }
        public string name { get; set; }
        public string picUrl { get; set; }
        public List<string> @alias { get; set; }
        public int albumSize { get; set; }
        public long picId { get; set; }
        public string img1v1Url { get; set; }
        public long img1v1 { get; set; }
        public string trans { get; set; }

        public long img1v1Id { get; set; }
        public int topicPerson { get; set; }
        public string followed { get; set; }
        public int musicSize { get; set; }
        public string briefDesc { get; set; }
        public string picId_str { get; set; }
        public string img1v1Id_str { get; set; }

        public string info { get; set; }
        public int mvSize { get; set; }
    }
    public class Album
    {
        public long id { get; set; }
        public string name { get; set; }
        public Artist artist { get; set; }
        public long publishTime { get; set; }
        public int size { get; set; }
        public int copyrightId { get; set; }
        public int status { get; set; }
        public long picId { get; set; }
        public int mark { get; set; }

        public List<string> songs { get; set; }
        public string paid { get; set; }
        public string onSale { get; set; }
        public string tags { get; set; }
        public string commentThreadId { get; set; }
        public string description { get; set; }
        public List<string> @alias { get; set; }
        public List<Artist> artists { get; set; }
        public string briefDesc { get; set; }
        public string company { get; set; }
        public string picUrl { get; set; }
        public long pic { get; set; }
        public string subType { get; set; }
        public int companyId { get; set; }
        public string blurPicUrl { get; set; }
        public string type { get; set; }
        public string picId_str { get; set; }
        public Info info { get; set; }
        //public bool liked { get; set; }
        //public int likedCount { get; set; }
    }
    public class SongsItem : INotifyPropertyChanged
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Artist> artists { get; set; }
        public Album album { get; set; }
        public int duration { get; set; }
        public int copyrightId { get; set; }
        public int status { get; set; }
        public List<string> @alias { get; set; }
        public int rtype { get; set; }
        public int ftype { get; set; }
        public int mvid { get; set; }
        public int fee { get; set; }
        public string rUrl { get; set; }
        public long mark { get; set; }

        //歌曲详细增加
        public int pst { get; set; }
        public int t { get; set; }
        public List<ArItem> ar { get; set; }
        public List<string> alia { get; set; }
        public int pop { get; set; }
        public int st { get; set; }
        public string rt { get; set; }
        public int v { get; set; }
        public string crbt { get; set; }
        public string cf { get; set; }
        public Al al { get; set; }
        public int dt { get; set; }
        public H h { get; set; }
        public M m { get; set; }
        public L l { get; set; }
        public string a { get; set; }
        public string cd { get; set; }
        public int no { get; set; }
        public string rtUrl { get; set; }
        public List<string> rtUrls { get; set; }
        public int djId { get; set; }
        public int copyright { get; set; }
        public int s_id { get; set; }
        public int originCoverType { get; set; }
        public int single { get; set; }
        //public string noCopyrightRcmd { get; set; }
        public int mv { get; set; }
        public string rurl { get; set; }
        public int mst { get; set; }
        public int cp { get; set; }
        public long publishTime { get; set; }

        public Privilege privilege { get; set; }

        //是否为喜欢歌曲
        public bool _isFavorite=false;
        public bool isFavorite
        {
            get { return _isFavorite; }
            set
            {
                _isFavorite = value;
                NotifyPropertyChanged("isFavorite");
            }
        }
        //是否在播放
        public bool _isPlaying = false;
        public bool isPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                NotifyPropertyChanged("isPlaying");
            }
        }

        
        public string eq { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
    public class SearchResult
    {
        //歌曲
        public List<SongsItem> songs { get; set; }
        public string hasMore { get; set; }
        public int songCount { get; set; }

        //歌单
        public List<PlaylistItem> playlists { get; set; }
        public int playlistCount { get; set; }

        //歌手
        public int artistCount { get; set; }
        public List<Artist> artists { get; set; }

        //专辑
        public List<Album> albums { get; set; }
        public int albumCount { get; set; }
    }
    public class SearchRoot
    {
        public SearchResult result { get; set; }
        public int code { get; set; }
    }

    //歌单详细
    public class SubscribersItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string defaultAvatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int province { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int authStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string followed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int accountStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int gender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int city { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long birthday { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string signature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string detailDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long avatarImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long backgroundImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string backgroundUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int authority { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mutual { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string expertTags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string experts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int djStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int vipType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remarkName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarImgIdStr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string backgroundImgIdStr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarImgId_str { get; set; }
    }

    public class TracksItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pst { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int t { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ArItem> ar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> alia { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pop { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int st { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int v { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string crbt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Al al { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int dt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public H h { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public M m { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public L l { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string a { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int no { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rtUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ftype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> rtUrls { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int djId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int copyright { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int s_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long mark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int originCoverType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string noCopyrightRcmd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int rtype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rurl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int mst { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int mv { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long publishTime { get; set; }
    }

    public class TrackIdsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int v { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long at { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string alg { get; set; }
    }

    public class Playlist
    {
        /// <summary>
        /// 
        /// </summary>
        public List<SubscribersItem> subscribers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string subscribed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Creator creator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<TracksItem> tracks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<TrackIdsItem> trackIds { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updateFrequency { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long backgroundCoverId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string backgroundCoverUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long titleImage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string titleImageUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string englishTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string opRecommend { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int adType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long trackNumberUpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int subscribedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cloudTrackCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int privacy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long trackUpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int trackCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long createTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string highQuality { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long updateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long coverImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string newImported { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int specialType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string commentThreadId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string coverImgUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long playCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> tags { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ordered { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// binaryify喜欢的音乐
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long shareCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string coverImgId_str { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long commentCount { get; set; }
    }

    public class PrivilegesItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int payed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int st { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int dl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int subp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int maxbr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string toast { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int flag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string preSell { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int playMaxbr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int downloadMaxbr { get; set; }
    }

    public class PlayListDetailRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string relatedVideos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Playlist playlist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string urls { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PrivilegesItem> privileges { get; set; }
    }

    //歌曲详细
    public class ChargeInfoListItem
    {
        public int rate { get; set; }
        public string chargeUrl { get; set; }
        public string chargeMessage { get; set; }
        public int chargeType { get; set; }
    }
    public class MusicDetailRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<SongsItem> songs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PrivilegesItem> privileges { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }


    //歌曲url
    public class SongDataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int br { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string md5 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int expi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int gain { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string uf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int payed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int flag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string canExtend { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string freeTrialInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string level { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string encodeType { get; set; }
    }
    public class SongUrlRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<SongDataItem> data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    //专辑详细
    public class ResourceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userId { get; set; }
        /// <summary>
        /// 神的游戏
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string imgUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string creator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string encodedId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string subTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string webUrl { get; set; }
    }
    public class CommentThread
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ResourceInfo resourceInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int resourceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int commentCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int likedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int shareCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int hotCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string latestLikedUsers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int resourceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int resourceOwnerId { get; set; }
        /// <summary>
        /// 神的游戏
        /// </summary>
        public string resourceTitle { get; set; }
    }
    public class Info
    {
        /// <summary>
        /// 
        /// </summary>
        public CommentThread commentThread { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string latestLikedUsers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string liked { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string comments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int resourceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int resourceId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int commentCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int likedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int shareCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string threadId { get; set; }
    }
    public class AlbumRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<SongsItem> songs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Album album { get; set; }
    }

    //喜欢歌曲
    public class FavoriteSongsRoot
    {
        public List<int> ids { get; set; }
        public long checkPoint { get; set; }
        public int code { get; set; }
        public List<SongsItem> songs { get; set; }
    }

    //我的歌单
    public class PlaylistItem
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> subscribers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string subscribed { get; set; }//true为自己创建，false为收藏
        /// <summary>
        /// 
        /// </summary>
        public Creator creator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string artists { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string tracks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string updateFrequency { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long backgroundCoverId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string backgroundCoverUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long titleImage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string titleImageUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string englishTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string opRecommend { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string recommendInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int adType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long trackNumberUpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long coverImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string newImported { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string anonimous { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long createTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string highQuality { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long updateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int specialType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int totalDuration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string coverImgUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long trackCount { get; set; }//歌曲数
        /// <summary>
        /// 
        /// </summary>
        public string commentThreadId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int privacy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long trackUpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long playCount { get; set; }//播放次数
        /// <summary>
        /// 
        /// </summary>
        public long subscribedCount { get; set; }//订阅人数
        /// <summary>
        /// 
        /// </summary>
        public long cloudTrackCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ordered { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> tags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 异元骇客喜欢的音乐
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string coverImgId_str { get; set; }
    }

    public class MyPlaylistRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string more { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PlaylistItem> playlist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    //收藏的专辑

    public class CADataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public long subTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> alias { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<Artist> artists { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long picId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string picUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> transNames { get; set; }
    }

    public class MyCollectionfAlbumRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<CADataItem> data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hasMore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cover { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int paidCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    //收藏的歌手
    public class MyCollectionfArtistRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public List<Artist> data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hasMore { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    //歌手详情

    public class ArtistBaseDetailRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public Artist artist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<SongsItem> hotSongs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string more { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    //歌手专辑
    public class ArtistAllAlbumRoot
    {
        public Artist artist { get; set; }
        public List<Album> hotAlbums { get; set; }
        public string more { get; set; }
        public int code { get; set; }
    }

    //歌词
    public class TransUser
    {
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int demand { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long userid { get; set; }
        /// <summary>
        /// 只是安渚
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long uptime { get; set; }
    }

    public class Lrc
    {
        /// <summary>
        /// 
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lyric { get; set; }
    }

    public class Klyric
    {
        /// <summary>
        /// 
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lyric { get; set; }
    }

    public class Tlyric
    {
        /// <summary>
        /// 
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string lyric { get; set; }
    }

    public class LyricRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public string sgc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sfy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string qfy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TransUser transUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Lrc lrc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Klyric klyric { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Tlyric tlyric { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
    }

    public class LyricStr
    {
        public string Original { get; set; }
        public string Tran { get; set; }
        public DateTime DateTime { get; set; }
    }

    //评论
    public class Associator
    {
        /// <summary>
        /// 
        /// </summary>
        public int vipCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rights { get; set; }
    }

    public class VipRights
    {
        /// <summary>
        /// 
        /// </summary>
        public Associator associator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string musicPackage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int redVipAnnualCount { get; set; }
    }

    public class User
    {
        /// <summary>
        /// 
        /// </summary>
        public string locationInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string liveInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int anonym { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int authStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string experts { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public VipRights vipRights { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userType { get; set; }
        /// <summary>
        /// 蛋蛋是圆D
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int vipType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remarkName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string expertTags { get; set; }
    }

    public class CommentsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public User user { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public List<string> beReplied { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string pendantData { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string showFloorComment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long commentId { get; set; }
        public string content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int likedCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string expressionUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int commentLocationType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long parentCommentId { get; set; }

        public string repliedMark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string liked { get; set; }
    }

    public class CommentRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public string isMusician { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int cnum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> topComments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string moreHot { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CommentsItem> hotComments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string commentBanner { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CommentsItem> comments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string more { get; set; }
    }

    //相似歌曲
    public class SimiSongsRoot
    {
        public List<SongsItem> songs { get; set; }
        public int code { get; set; }
    }

    //登录
    public class Account
    {
        /// <summary>
        /// 
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int whitelistAuthority { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long createTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string salt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int tokenVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ban { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int baoyueVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int donateVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int vipType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long viptypeVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string anonimousUser { get; set; }
    }

    public class Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public long userId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long gender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int vipType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int accountStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long avatarImgId { get; set; }
        /// <summary>
        /// 异元骇客
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long birthday { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long city { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long backgroundImgId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long province { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string defaultAvatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int djStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mutual { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remarkName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string expertTags { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int authStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatarImgIdStr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string backgroundImgIdStr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string followed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string backgroundUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string detailDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string signature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long authority { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long followeds { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long follows { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long eventCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int playlistCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int playlistBeSubscribedCount { get; set; }
    }

    public class LoginRoot
    {
        /// <summary>
        /// 
        /// </summary>
        public int loginType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Account account { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Profile profile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cookie { get; set; }
    }


    /// <summary>
    /// 封装自维护cookie http
    /// </summary>
    public class Http
    {
        public static CookieContainer cookies = new CookieContainer();

        /// <summary>
        /// GET方法(自动维护cookie)
        /// </summary>
        public static string Get(string url, string referer = "", int timeout = 2000, Encoding encode = null)
        {
            string dat;
            HttpWebResponse res = null;
            HttpWebRequest req = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = cookies;
                req.AllowAutoRedirect = false;
                req.Timeout = timeout;
                req.Referer = referer;
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0;%20WOW64; rv:47.0) Gecko/20100101 Firefox/47.0";
                res = (HttpWebResponse)req.GetResponse();
                if (res.StatusCode != HttpStatusCode.OK)
                    return null;
                cookies.Add(res.Cookies);
                dat = new StreamReader(res.GetResponseStream(), encode ?? Encoding.UTF8).ReadToEnd();
                res.Close();
                req.Abort();
            }
            catch
            {
                return null;
            }
            return dat;
        }

        /// <summary>
        /// Post方法(自动维护cookie)
        /// </summary>
        public static string Post(string url, string postdata, CookieContainer cookie = null, string referer = "", int timeout = 2000, Encoding encode = null)
        {
            string html = null;
            HttpWebRequest request;
            HttpWebResponse response;
            if (encode == null) encode = Encoding.UTF8;
            try
            {
                byte[] byteArray = encode.GetBytes(postdata); // 转化
                request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                if (cookie == null) cookie = new CookieContainer();
                request.CookieContainer = cookie;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; InfoPath.1)";
                request.Method = "POST";
                request.Referer = referer;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                request.Timeout = timeout;
                Stream newStream = request.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);    //写入参数
                newStream.Close();
                response = (HttpWebResponse)request.GetResponse();
                cookie.Add(response.Cookies);
                StreamReader str = new StreamReader(response.GetResponseStream(), encode);
                html = str.ReadToEnd();
            }
            catch
            {
                return "";
            }
            return html;
        }
    }
}
