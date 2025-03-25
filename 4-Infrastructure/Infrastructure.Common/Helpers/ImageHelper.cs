using SkiaSharp;

namespace Infrastructure.Common.Helpers
{
    //**若在Linux出现依赖问题，可以使用包SkiaSharp.NativeAssets.Linux.NoDependencies
    public class ImageHelper
    {
        // <summary>
        /// 创建图像的缩略图。使用SkiaSharp，以支持跨平台。
        /// </summary>
        /// <param name="orgPath">原图文件全路径</param>
        /// <param name="thuPath">将生成的缩略图文件全路径</param>
        public static void MakeThumb(string orgPath, string thuPath)
        {
            const int width = 180, height = 135;
            const int quality = 80; //质量为[SKFilterQuality.Medium]结果的100%
            using var input = File.OpenRead(orgPath);
            using var inputStream = new SKManagedStream(input);
            using var original = SKBitmap.Decode(inputStream);
            if (original.Width <= width && original.Height <= height)//如果宽度和高度都小于缩率图值，则直接复制文件
            {
                File.Copy(orgPath, thuPath);
            }
            else
            {
                using (var resized = original.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium))
                {
                    if (resized != null)
                    {
                        using var image = SKImage.FromBitmap(resized);
                        using var output = File.OpenWrite(thuPath);
                        image.Encode(SKEncodedImageFormat.Png, quality).SaveTo(output);
                    }
                }

            }
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="base64Str">图片文件的base64Str</param>
        /// <param name="targetWidth">缩略图宽度</param>
        public static string CreateThumbnail(string base64Str, int targetWidth = 480)
        {

            byte[] filebytes = Convert.FromBase64String(base64Str);

            const int quality = 60; //质量为[SKFilterQuality.Medium]结果的100%

            using var ms = new MemoryStream(filebytes);

            using var inputStream = new SKManagedStream(ms);
            using var original = SKBitmap.Decode(inputStream);
            if (original == null)
            {
                //无法生成缩略图,返回空，留给前端处理
                return string.Empty;
            }
            // 计算缩放后的目标宽度，保持纵横比
            float aspectRatio = (float)original.Width / original.Height;
            int targetHeight = (int)(targetWidth / aspectRatio);

            //if (original.Width > width && original.Height > height) //去掉判断，尺寸一样也要压缩质量
            using (var resized = original.Resize(new SKImageInfo(targetWidth, targetHeight), SKFilterQuality.Low))
            {
                if (resized != null)
                {
                    using var image = SKImage.FromBitmap(resized);
                    var p = image.Encode(SKEncodedImageFormat.Jpeg, quality);
                    filebytes = p.ToArray();
                    base64Str = Convert.ToBase64String(filebytes);
                }
            }

            return base64Str;
        }

        public static string CreateThumbnail(byte[] imagesBytes, int targetWidth = 480)
        {
            string base64Str = string.Empty;
            const int quality = 60; //质量为[SKFilterQuality.Medium]结果的100%

            using var ms = new MemoryStream(imagesBytes);

            using var inputStream = new SKManagedStream(ms);
            using var original = SKBitmap.Decode(inputStream);
            if (original == null)
            {
                //无法生成缩略图,返回空，留给前端处理
                return string.Empty;
            }
            // 计算缩放后的目标宽度，保持纵横比
            float aspectRatio = (float)original.Width / original.Height;
            int targetHeight = (int)(targetWidth / aspectRatio);

            //if (original.Width > width && original.Height > height) //去掉判断，尺寸一样也要压缩质量
            using (var resized = original.Resize(new SKImageInfo(targetWidth, targetHeight), SKFilterQuality.Low))
            {
                if (resized != null)
                {
                    using var image = SKImage.FromBitmap(resized);
                    var p = image.Encode(SKEncodedImageFormat.Jpeg, quality);
                    imagesBytes = p.ToArray();
                    base64Str = Convert.ToBase64String(imagesBytes);
                }
            }

            return base64Str;
        }
    }
}
