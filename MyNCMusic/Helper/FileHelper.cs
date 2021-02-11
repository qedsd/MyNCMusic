using MyNCMusic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace MyNCMusic.Helper
{
    public static class FileHelper
    {
        public static async Task<BitmapImage> DownloadFile(Uri uri)
        {
            Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient();
            try
            {
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
            finally
            {
                http.Dispose();
            }
        }

        public static async Task<byte[]> ConvertIRandomAccessStreamByte(IRandomAccessStream stream)
        {
            DataReader read = new DataReader(stream.GetInputStreamAt(0));
            await read.LoadAsync((uint)stream.Size);
            byte[] temp = new byte[stream.Size];
            read.ReadBytes(temp);
            read.Dispose();
            return temp;
        }
        public static async Task<BitmapImage> ReadLoaclBitmapImage(string name)
        {
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(name) is StorageFile)
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(name);
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage img = new BitmapImage();
                    await img.SetSourceAsync(stream);
                    return img;
                }
            }
            else
                return new BitmapImage();
        }

        public static async Task<byte[]> StorageImageFolder(IRandomAccessStream stream, Uri uri)
        {
            StorageFile file = await ConfigService.Folder.CreateFileAsync(ConfigService.ImageFilename, CreationCollisionOption.ReplaceExisting);
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
                catch (Exception) { return new WriteableBitmap(200, 200); }
            }
        }
    }
}
