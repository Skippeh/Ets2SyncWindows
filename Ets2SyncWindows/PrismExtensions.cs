using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Pfim;
using PrismLibrary;

namespace Ets2SyncWindows
{
    public static class PrismExtensions
    {
        public static ImageSource TryLoadThumbnailImage(this GameSave gameSave)
        {
            if (!File.Exists(gameSave.ThumbnailPath))
                return null;

            try
            {
                IImage image = Pfim.Pfim.FromFile(gameSave.ThumbnailPath);
                GCHandle pinnedArray = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                IntPtr addr = pinnedArray.AddrOfPinnedObject();

                BitmapSource imageSource = null;

                void CreateImageSource()
                {
                    imageSource = BitmapSource.Create(image.Width, image.Height, 96, 96, GetPixelFormat(image), null, addr, image.DataLen, image.Stride);
                }
                
                // Make sure to create the image on the dispatcher thread or the application will crash
                if (Application.Current.Dispatcher.Thread == Thread.CurrentThread)
                {
                    CreateImageSource();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(CreateImageSource);
                }

                pinnedArray.Free();

                return imageSource;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        private static PixelFormat GetPixelFormat(IImage image)
        {
            switch (image.Format)
            {
                case ImageFormat.Rgb24:
                    return PixelFormats.Bgr24;
                case ImageFormat.Rgba32:
                    return PixelFormats.Bgr32;
                case ImageFormat.Rgb8:
                    return PixelFormats.Gray8;
                case ImageFormat.R5g5b5a1:
                case ImageFormat.R5g5b5:
                    return PixelFormats.Bgr555;
                case ImageFormat.R5g6b5:
                    return PixelFormats.Bgr565;
                default:
                    throw new Exception($"Unable to convert {image.Format} to WPF PixelFormat");
            }
        }
    }
}