using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using DataMatrix.net;
using OpenCvSharp;
using ZXing;
using ZXing.Common;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace samrtCheckPF.utils
{
 public  class QRDecodeUtils
    {
        private int _Brightness;
        private int _kernel;
        public Thread QRDecodeUtilsThread = null;
        public bool m_bThreadQRDecodeUtilsThreadLife = false;
        public bool m_bThreadQRDecodeUtilsThreadTerminate = false;
        private int _StartIndex;
        private int _ZoomNumber;
        private int _Degree;
        private int _Blocksize;
        private Bitmap _BitmapOrigin;
        private double _ContrastValue;
        private readonly BarcodeReader barcodeReader;
        public  IList<Result> results;
        public static OpenCvSharp.XFeatures2D.SURF SurfObj = OpenCvSharp.XFeatures2D.SURF.Create(10000);
        public static KeyPoint[] GoldenkeyPoints;
        public static Mat GoldenDescripit;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="n_Brightness">增强亮度参数，原始数值累加</param>
        /// <param name="n_kernel">锐化参数</param>
        /// <param name="Contrast">增强对比度参数，原始数值相乘</param>
        /// <param name="Codetype">二维码类型</param>
        /// <param name="degree">图像翻转角度</param>
        /// <param name="blocksize">自适应阈值参数</param>
        public QRDecodeUtils(int n_Brightness, int n_kernel, double Contrast,string Codetype, int degree, int blocksize)
        {
            barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true,
                Options = new DecodingOptions { TryHarder = true }
            };

            switch(Codetype)
            {
                case "0":
                    barcodeReader.Options.PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE };
                    break;
                case "1":
                    barcodeReader.Options.PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.DATA_MATRIX };
                    break;
                case "2":
                    barcodeReader.Options.PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.CODE_128 };
                    break;
                case "3":
                    barcodeReader.Options.PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.CODE_39 };
                    break;
            }
            //barcodeReader.Options.PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE, BarcodeFormat.DATA_MATRIX, BarcodeFormat.CODE_128};

            Brightness = n_Brightness;
            kernel = n_kernel;
            ContrastValue = Contrast;
            Degree = degree;
            Blocksize = blocksize;
        }

        /// <summary>
        /// 增强亮度
        /// </summary>
        /// <param name="_Brightness"></param>
        public int Brightness
        {
            get { return _Brightness; }
            set { _Brightness = value; }
        }

        /// <summary>
        /// 增强对比度
        /// </summary>
        /// <param name="_ContrastValue"></param>
        public double ContrastValue
        {
            get { return _ContrastValue; }
            set { _ContrastValue = value; }
        }


        /// <summary>
        /// 锐化参数
        /// </summary>
        /// <param name="_kernel"></param>
        public int kernel
        {
            get { return _kernel; }
            set { _kernel = value; }
        }

        /// <summary>
        /// 线程启动参数
        /// </summary>
        /// <param name="_StartIndex"></param>
        public int StartIndex
        {
            get { return _StartIndex; }
            set { _StartIndex = value; }
        }

        /// <summary>
        /// 图像resize参数
        /// </summary>
        /// <param name="_ZoomNumber"></param>
        public int ZoomNumber
        {
            get { return _ZoomNumber; }
            set { _ZoomNumber = value; }
        }

        /// <summary>
        /// 图像旋转角度
        /// </summary>
        /// <param name="_Degree"></param>
        public int Degree
        {
            get { return _Degree; }
            set { _Degree = value; }
        }

        /// <summary>
        /// 自适应阈值调整参数
        /// </summary>
        /// <param name="_Blocksize"></param>
        public int Blocksize
        {
            get { return _Blocksize; }
            set { _Blocksize = value; }
        }


        /// <summary>
        /// 线程传入原始图像
        /// </summary>
        /// <param name="_Blocksize"></param>
        public Bitmap BitmapOrigin
        {
            get { return _BitmapOrigin; }
            set { _BitmapOrigin = value; }
        }

        /// <summary>
        /// 线程解码结果
        /// </summary>
        /// <param name="results"></param>
        public IList<Result> Result
        {
            get { return results; }
            set { results = value; }
        }


        #region 线程处理

        public void QRDecodeUtilsFunction()
        {
            // Thread Loop
            while (m_bThreadQRDecodeUtilsThreadLife)
            {
                DoQRDecodeUtilsThreadJob();
                Thread.Sleep(1);
            }
            m_bThreadQRDecodeUtilsThreadTerminate = true;
        }
        public void QRDecodeUtilsRun()
        {
            if (m_bThreadQRDecodeUtilsThreadLife)
            {
                QRDecodeUtilsStop();
                Thread.Sleep(100);
            }
            m_bThreadQRDecodeUtilsThreadLife = true;
            m_bThreadQRDecodeUtilsThreadTerminate = false;
            QRDecodeUtilsThread = new Thread(new ThreadStart(QRDecodeUtilsFunction));
            QRDecodeUtilsThread.IsBackground = false;
            QRDecodeUtilsThread.Start();
        }
        public void QRDecodeUtilsStop()
        {
            m_bThreadQRDecodeUtilsThreadLife = false;

            if (null != QRDecodeUtilsThread)
            {
                QRDecodeUtilsThread.Abort();
            }

            QRDecodeUtilsThread = null;
        }
        public void DoQRDecodeUtilsThreadJob()
        {
            if (StartIndex == 1)
            {
                OpenCVToResize(BitmapOrigin, ZoomNumber);
                StartIndex = 2;
            }
        }
        #endregion

        /// <summary>
        /// 线程读码函数
        /// </summary>
        /// <param name="ImageOriginal">传入原始图像</param>
        /// <param name="ZoomNumber">resize尺寸</param>
        public void OpenCVToResize(Bitmap ImageOriginal, int ZoomNumber)
        {
            try
            {
                if (ZoomNumber <= 0)
                {
                    return;
                }

                //zxing解码
                {
                    System.Drawing.Bitmap ImageBaseOriginal = new System.Drawing.Bitmap(ImageOriginal);
                    switch (Degree)
                    {
                        case 0:
                            ImageBaseOriginal.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                            break;
                        case 90:
                            ImageBaseOriginal.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 180:
                            ImageBaseOriginal.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 270:
                            ImageBaseOriginal.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                    }
                    Mat mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(ImageBaseOriginal);

                    OpenCvSharp.Size size = new OpenCvSharp.Size(ImageBaseOriginal.Width * ZoomNumber, ImageBaseOriginal.Height * ZoomNumber);
                    Mat SizeMat = new Mat();
                    Cv2.Resize(mat, SizeMat, size);
                    //灰度图
                    Mat mask = SizeMat.CvtColor(ColorConversionCodes.BGR2GRAY);
                    Mat FilterMat = new Mat();
                    Mat ContrastMat = new Mat();
                    //增强对比度
                    mask.ConvertTo(ContrastMat, -1, ContrastValue, Brightness);
                    //滤波锐化
                    FilterMethod(FilterMat, ContrastMat);
                    //mat转bitmap
                    ImageBaseOriginal = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(FilterMat);
                    //解码
                    results = barcodeReader.DecodeMultiple(ImageBaseOriginal);

                    if (results == null)
                    {
                        OpenCvSharp.Size size1 = new OpenCvSharp.Size(300, 300);
                        Mat SizeMat1 = new Mat();
                        Cv2.Resize(mat, SizeMat1, size1);
                        Mat SizeMatThreshold = new Mat();
                        //灰度图
                        Mat mask1 = SizeMat1.CvtColor(ColorConversionCodes.BGR2GRAY);
                        Cv2.AdaptiveThreshold(mask1, SizeMatThreshold, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, Blocksize, 2);
                        Bitmap bitmap12 = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(SizeMatThreshold); // mat 转 bitmap
                        results = barcodeReader.DecodeMultiple(bitmap12);
                        SizeMat1.Dispose();
                        SizeMatThreshold.Dispose();
                        bitmap12.Dispose();
                        mask1.Dispose();
                    }

                    mat.Dispose();
                    SizeMat.Dispose();
                    ImageBaseOriginal.Dispose();
                    mask.Dispose();
                    FilterMat.Dispose();
                    ContrastMat.Dispose();
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 锐化函数
        /// </summary>
        /// <param name="FilterMat">传出锐化图</param>
        /// <param name="ContrastMat">传入对比图</param>
        private void FilterMethod(Mat FilterMat, Mat ContrastMat)
        {
            switch (_kernel)
            {
                case 0:
                    InputArray kernel1 = InputArray.Create<float>(new float[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } });
                    //滤波
                    Cv2.Filter2D(ContrastMat, FilterMat, -1, kernel1, new OpenCvSharp.Point(-1, -1), 0);
                    break;
                case 1:
                    InputArray kernel2 = InputArray.Create<float>(new float[3, 3] { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } });
                    //滤波
                    Cv2.Filter2D(ContrastMat, FilterMat, -1, kernel2, new OpenCvSharp.Point(-1, -1), 0);
                    break;
                case 2:
                    InputArray kernel3 = InputArray.Create<float>(new float[3, 3] { { 1, -2, 1 }, { -2, 5, -2 }, { 1, -2, 1 } });
                    //滤波
                    Cv2.Filter2D(ContrastMat, FilterMat, -1, kernel3, new OpenCvSharp.Point(-1, -1), 0);
                    break;
                default:
                    InputArray kernel4 = InputArray.Create<float>(new float[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } });
                    //滤波
                    Cv2.Filter2D(ContrastMat, FilterMat, -1, kernel4, new OpenCvSharp.Point(-1, -1), 0);
                    break;
            }
        }


        #region  //不同工站使用定位函数
        /// <summary>
        /// 定位LeftBoard站点二维码函数
        /// </summary>
        /// <param name="SrcImage">原始图像</param>
        public static Bitmap LocateLeftDataMatric(Bitmap SrcImage)
        {
            Bitmap BitmapLabel = LocateLeftBoardLabel(SrcImage);
            Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(BitmapLabel);  // 待处理图像
            Mat Rotate_Image = OpenCvSharp.Extensions.BitmapConverter.ToMat(BitmapLabel);  // 待旋转图像

            int height = Srcmat.Rows;
            int width = Srcmat.Cols;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    byte b = Srcmat.At<Vec3b>(i, j).Item0;            //这里AT函数和GET一样只能获取值
                    byte g = Srcmat.At<Vec3b>(i, j).Item1;
                    byte r = Srcmat.At<Vec3b>(i, j).Item2;
                    if (b > 130 && g > 130 && r > 130)
                    {
                        //  Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(0), (byte)(0), (byte)(0)));
                    }
                    else
                    {
                        Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(0), (byte)(0), (byte)(0)));
                    }
                }
            }

            //转成灰度图
            Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);

            //膨胀
            Mat Erode = new Mat();
            //获取自定义核
            Mat element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(15, 15));
            Mat element1 = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(8, 8));

            //开，闭运算
            Cv2.MorphologyEx(src_gray, Erode, MorphTypes.Erode, element1);


            //Canny边缘检测
            Mat canny = new Mat();
            Cv2.Canny(Erode, canny, 130, 255);
            //Cv2.ImShow("Canny", canny);

            //获得轮廓
            Point[][] contours;
            List<Point[]> contours2 = new List<Point[]>();
            HierarchyIndex[] hierarchly;

            //寻找轮廓 
            //第一个参数是输入图像 2值化的
            //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
            //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
            //第四个参数是类型，采用树结构
            //第五个参数是节点拟合模式，这里是全部寻找
            Cv2.FindContours(canny, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

            List<Point> CenterPoint = new List<Point>();
            RotatedRect RR;
            List<float> angle = new List<float>();
            List<OpenCvSharp.Rect> Rectangle = new List<OpenCvSharp.Rect>();
            //轮廓筛选
            for (int i = 0; i < contours.Length; i++)
            {
                double X = Cv2.ArcLength(contours[i], true);
                if (X > 200)
                {
                    double X1 = 0.01 * X;
                    Point[] approx = Cv2.ApproxPolyDP(contours[i], X1, true);
                    int corners = approx.Count();
                    RR = Cv2.MinAreaRect(approx);
                    //Rect R = Cv2.BoundingRect(approx);
                    Rect R = RR.BoundingRect();
                    if (R.Width > 110 && R.Height > 110 && R.Width < 180 && R.Height < 180)
                    {
                        Point p;
                        p.X = R.X + (int)(R.Width / 2);
                        p.Y = R.Y + (int)(R.Height / 2);
                        CenterPoint.Add(p);
                        contours2.Add(contours[i]);
                        angle.Add(RR.Angle);
                        Rectangle.Add(R);
                    }
                }
            }

            ////将结果画出并返回结果
            //Mat dst_Image = Srcmat.Clone();

            //Random rnd = new Random();
            ////填充定位点
            //for (int i = 0; i < contours2.Count; i++)
            //{
            //    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            //    Cv2.DrawContours(dst_Image, contours2, i, color, 2, LineTypes.Link8);
            //    Cv2.ImShow("dst_Image", dst_Image);
            //}

            if (angle.Count > 0)
            {
                Rect[] arr = Rectangle.ToArray();
                int max = arr[0].Height;
                int index = 0;//把假设的最大值索引赋值非index
                for (int i = 1; i < arr.Length; i++)
                {
                    if (arr[i].Height >= max)
                    {
                        max = arr[i].Height;
                        index = i;//把较大值的索引赋值非index
                    }
                }

                //旋转
                Mat affine_matrix = Cv2.GetRotationMatrix2D(CenterPoint[index], angle[index], 1.0);
                Cv2.WarpAffine(Rotate_Image, Rotate_Image, affine_matrix, Rotate_Image.Size());
                //Cv2.ImShow("Rotate", Rotate_Image);

                //Scalar color1 = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //Cv2.Rectangle(dst_Image, Rectangle[index], color1);

                //Mat CropImg = new Mat();
                //////画圆 cvPoint:确定圆的坐标  200：圆的半径 CV_RGB：圆的颜色 3：线圈的粗细
                //foreach (var p in CenterPoint)
                //{
                //    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //    Cv2.Circle(dst_Image, p.X, p.Y, 5, color, 2, LineTypes.Link8, 0);
                //}
                //Cv2.ImShow("center", dst_Image);

                //裁剪图片
                Rectangle Rect1 = new Rectangle(CenterPoint[index].X - 70, CenterPoint[index].Y - 70, 140, 140);
                Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Rotate_Image); // mat 转 bitmap
                Bitmap m_BitmapCrop = crop(bitmap, Rect1);
                //CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                //Cv2.ImShow("CropImg", CropImg);
                return m_BitmapCrop;
            }
            else
                return BitmapLabel;
        }

        /// <summary>
        /// 定位LeftBoard站点二维码标签函数
        /// </summary>
        /// <param name="SrcImage">原始图像</param>
        public static Bitmap LocateLeftBoardLabel(Bitmap SrcImage)
        {
            Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  // 待处理图像
            int height = Srcmat.Rows;
            int width = Srcmat.Cols;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    byte b = Srcmat.At<Vec3b>(i, j).Item0;            //这里AT函数和GET一样只能获取值
                    byte g = Srcmat.At<Vec3b>(i, j).Item1;
                    byte r = Srcmat.At<Vec3b>(i, j).Item2;
                    if (b > 130 && g > 130 && r > 130)
                    {

                    }
                    else
                    {
                        Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(0), (byte)(0), (byte)(0)));
                    }
                }
            }
            //Cv2.ImShow("Srcmat", Srcmat);

            //转成灰度图
            Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);
            //Cv2.ImShow("gray", src_gray);


            //膨胀
            Mat Dilate = new Mat();
            Mat Erode = new Mat();
            //获取自定义核
            Mat element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(10, 10));
            Mat element1 = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(4, 4));

            ////开，闭运算
            Cv2.MorphologyEx(src_gray, Dilate, MorphTypes.Dilate, element);
            //Cv2.ImShow("Dilate", Dilate);

            ////开，闭运算
            //Cv2.MorphologyEx(src_gray, Erode, MorphTypes.Erode, element1);
            //Cv2.ImShow("Erode", Erode);


            //Canny边缘检测
            Mat canny = new Mat();
            Cv2.Canny(Dilate, canny, 130, 255);
            //Cv2.ImShow("Canny", canny);

            //获得轮廓
            Point[][] contours;
            List<Point[]> contours2 = new List<Point[]>();
            HierarchyIndex[] hierarchly;

            //寻找轮廓 
            //第一个参数是输入图像 2值化的
            //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
            //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
            //第四个参数是类型，采用树结构
            //第五个参数是节点拟合模式，这里是全部寻找
            Cv2.FindContours(canny, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

            List<Point> CenterPoint = new List<Point>();
            RotatedRect RR;
            List<float> angle = new List<float>();
            List<OpenCvSharp.Rect> Rectangle = new List<OpenCvSharp.Rect>();
            //轮廓筛选
            for (int i = 0; i < contours.Length; i++)
            {
                double X = Cv2.ArcLength(contours[i], true);
                if (X > 250)
                {
                    double X1 = 0.01 * X;
                    Point[] approx = Cv2.ApproxPolyDP(contours[i], X1, true);
                    int corners = approx.Count();
                    RR = Cv2.MinAreaRect(approx);
                    //Rect R = Cv2.BoundingRect(approx);
                    Rect R = RR.BoundingRect();
                    if (R.Width > 290)
                    {
                        Point p;
                        p.X = R.X + (int)(R.Width / 2);
                        p.Y = R.Y + (int)(R.Height / 2);
                        CenterPoint.Add(p);
                        contours2.Add(contours[i]);
                        angle.Add(RR.Angle);
                        Rectangle.Add(R);
                    }
                }
            }

            ////将结果画出并返回结果
            //Mat dst_Image = Srcmat.Clone();

            //Random rnd = new Random();
            //////填充定位点
            ////for (int i = 0; i < contours2.Count; i++)
            ////{
            ////    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            ////    Cv2.DrawContours(dst_Image, contours2, i, color, 2, LineTypes.Link8);
            ////    Cv2.ImShow("dst_Image", dst_Image);
            ////}

            if (angle.Count > 0)
            {
                //Scalar color1 = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //Cv2.Rectangle(dst_Image, Rectangle[0], color1);

                //Mat CropImg = new Mat();

                //裁剪图片
                Rectangle Rect1 = new Rectangle(CenterPoint[0].X - Rectangle[0].Width / 2, CenterPoint[0].Y - Rectangle[0].Height / 2, Rectangle[0].Width, Rectangle[0].Height);
                Bitmap m_BitmapCrop = crop(SrcImage, Rect1);
                //CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                //Cv2.ImShow("CropImg1", CropImg);
                return m_BitmapCrop;
            }
            else
                return SrcImage;
        }

        /// <summary>
        /// 定位RightBoard站点二维码函数
        /// </summary>
        /// <param name="SrcImage">原始图像</param>
        public static Bitmap LocateRightDataMatric(Bitmap SrcImage)
        {
            Bitmap BitmapLabel = LocateRightBoardLabel(SrcImage);
            Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(BitmapLabel);  // 待处理图像
            Mat Rotate_Image = OpenCvSharp.Extensions.BitmapConverter.ToMat(BitmapLabel);  // 待旋转图像

            int height = Srcmat.Rows;
            int width = Srcmat.Cols;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    byte b = Srcmat.At<Vec3b>(i, j).Item0;            //这里AT函数和GET一样只能获取值
                    byte g = Srcmat.At<Vec3b>(i, j).Item1;
                    byte r = Srcmat.At<Vec3b>(i, j).Item2;
                    if (b < 120 && g < 120 && r < 120)
                    {
                        // Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(255), (byte)(255), (byte)(255)));
                    }
                    else
                    {
                        Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(255), (byte)(255), (byte)(255)));
                    }
                }
            }

            //转成灰度图
            Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);
            //Cv2.ImShow("gray", src_gray);

            //膨胀
            Mat Dilate = new Mat();
            Mat Erode = new Mat();
            //获取自定义核
            Mat element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(15, 15));
            Mat element1 = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(4, 4));

            //开，闭运算
            Cv2.MorphologyEx(src_gray, Erode, MorphTypes.Erode, element1);

            //Canny边缘检测
            Mat canny = new Mat();
            Cv2.Canny(Erode, canny, 130, 255);

            //获得轮廓
            Point[][] contours;
            List<Point[]> contours2 = new List<Point[]>();
            HierarchyIndex[] hierarchly;

            //寻找轮廓 
            //第一个参数是输入图像 2值化的
            //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
            //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
            //第四个参数是类型，采用树结构
            //第五个参数是节点拟合模式，这里是全部寻找
            Cv2.FindContours(canny, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

            List<Point> CenterPoint = new List<Point>();
            RotatedRect RR;
            List<float> angle = new List<float>();
            List<OpenCvSharp.Rect> Rectangle = new List<OpenCvSharp.Rect>();
            //轮廓筛选
            for (int i = 0; i < contours.Length; i++)
            {
                double X = Cv2.ArcLength(contours[i], true);
                if (X > 250)
                {
                    double X1 = 0.01 * X;
                    Point[] approx = Cv2.ApproxPolyDP(contours[i], X1, true);
                    int corners = approx.Count();
                    RR = Cv2.MinAreaRect(approx);
                    //Rect R = Cv2.BoundingRect(approx);
                    Rect R = RR.BoundingRect();
                    if (Math.Abs(RR.Size.Width - RR.Size.Height) <= 10 && RR.Size.Width > 100 && RR.Size.Width < 120 && RR.Size.Height > 100 && RR.Size.Height < 120)
                    {
                        Point p;
                        p.X = R.X + (int)(R.Width / 2);
                        p.Y = R.Y + (int)(R.Height / 2);
                        CenterPoint.Add(p);
                        contours2.Add(contours[i]);
                        angle.Add(RR.Angle);
                        Rectangle.Add(R);
                    }
                }
            }

            ////将结果画出并返回结果
            //Mat dst_Image = Srcmat.Clone();

            //Random rnd = new Random();
            //////填充定位点
            ////for (int i = 0; i < contours2.Count; i++)
            ////{
            ////    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            ////    Cv2.DrawContours(dst_Image, contours2, i, color, 2, LineTypes.Link8);
            ////    Cv2.ImShow("dst_Image", dst_Image);
            ////}

            if (angle.Count > 0)
            {
                Rect[] arr = Rectangle.ToArray();
                double max = Cv2.ArcLength(contours2[0], true);
                int index = 0;//把假设的最大值索引赋值非index
                for (int i = 1; i < arr.Length; i++)
                {
                    if (Cv2.ArcLength(contours2[i], true) >= max)
                    {
                        max = Cv2.ArcLength(contours2[i], true);
                        index = i;
                    }
                }

                //旋转
                Mat affine_matrix = Cv2.GetRotationMatrix2D(CenterPoint[index], angle[index], 1.0);
                Cv2.WarpAffine(Rotate_Image, Rotate_Image, affine_matrix, Rotate_Image.Size());
                //Cv2.ImShow("Rotate", Rotate_Image);

                //Scalar color1 = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //Cv2.Rectangle(dst_Image, Rectangle[index], color1);

                //Mat CropImg = new Mat();
                ////画圆 cvPoint:确定圆的坐标  200：圆的半径 CV_RGB：圆的颜色 3：线圈的粗细
                //foreach (var p in CenterPoint)
                //{
                //    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //    Cv2.Circle(dst_Image, p.X, p.Y, 5, color, 2, LineTypes.Link8, 0);
                //}
                //Cv2.ImShow("center", dst_Image);

                //裁剪图片
                Rectangle Rect1 = new Rectangle(CenterPoint[index].X - 65, CenterPoint[index].Y - 65, 130, 130);
                Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Rotate_Image); // mat 转 bitmap
                Bitmap m_BitmapCrop = crop(bitmap, Rect1);
                //CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                //Cv2.ImShow("CropImg", CropImg);
                return m_BitmapCrop;
            }
            else
            {
                Rectangle Rect1 = new Rectangle(0, BitmapLabel.Height / 2, BitmapLabel.Width, BitmapLabel.Height / 2);
                Bitmap m_BitmapCrop = crop(BitmapLabel, Rect1);
                //Mat CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                //Cv2.ImShow("CropImg2", CropImg);
                return m_BitmapCrop;
            }
        }

        /// <summary>
        /// 定位RightBoard站点二维码标签函数
        /// </summary>
        /// <param name="SrcImage">原始图像</param>
        public static Bitmap LocateRightBoardLabel(Bitmap SrcImage)
        {
            Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  // 待处理图像

            int height = Srcmat.Rows;
            int width = Srcmat.Cols;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    byte b = Srcmat.At<Vec3b>(i, j).Item0;            //这里AT函数和GET一样只能获取值
                    byte g = Srcmat.At<Vec3b>(i, j).Item1;
                    byte r = Srcmat.At<Vec3b>(i, j).Item2;
                    if (b > 130 && g > 130 && r > 130)
                    {

                    }
                    else
                    {
                        Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(0), (byte)(0), (byte)(0)));
                    }
                }
            }

            //转成灰度图
            Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);

            //膨胀
            Mat Dilate = new Mat();
            Mat Erode = new Mat();
            //获取自定义核
            Mat element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(10, 10));
            Mat element1 = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(4, 4));

            ////开，闭运算
            Cv2.MorphologyEx(src_gray, Dilate, MorphTypes.Dilate, element);


            //Canny边缘检测
            Mat canny = new Mat();
            Cv2.Canny(Dilate, canny, 130, 255);

            //获得轮廓
            Point[][] contours;
            List<Point[]> contours2 = new List<Point[]>();
            HierarchyIndex[] hierarchly;

            //寻找轮廓 
            //第一个参数是输入图像 2值化的
            //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
            //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
            //第四个参数是类型，采用树结构
            //第五个参数是节点拟合模式，这里是全部寻找
            Cv2.FindContours(canny, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

            List<Point> CenterPoint = new List<Point>();
            RotatedRect RR;
            List<float> angle = new List<float>();
            List<OpenCvSharp.Rect> Rectangle = new List<OpenCvSharp.Rect>();
            //轮廓筛选
            for (int i = 0; i < contours.Length; i++)
            {
                double X = Cv2.ArcLength(contours[i], true);
                if (X > 250)
                {
                    double X1 = 0.01 * X;
                    Point[] approx = Cv2.ApproxPolyDP(contours[i], X1, true);
                    int corners = approx.Count();
                    RR = Cv2.MinAreaRect(approx);
                    //Rect R = Cv2.BoundingRect(approx);
                    Rect R = RR.BoundingRect();
                    if (R.Height > 290)
                    {
                        Point p;
                        p.X = R.X + (int)(R.Width / 2);
                        p.Y = R.Y + (int)(R.Height / 2);
                        CenterPoint.Add(p);
                        contours2.Add(contours[i]);
                        angle.Add(RR.Angle);
                        Rectangle.Add(R);
                    }
                }
            }

            ////将结果画出并返回结果
            //Mat dst_Image = Srcmat.Clone();

            //Random rnd = new Random();
            //////填充定位点
            ////for (int i = 0; i < contours2.Count; i++)
            ////{
            ////    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            ////    Cv2.DrawContours(dst_Image, contours2, i, color, 2, LineTypes.Link8);
            ////    Cv2.ImShow("dst_Image", dst_Image);
            ////}

            if (angle.Count > 0)
            {
                //Scalar color1 = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //Cv2.Rectangle(dst_Image, Rectangle[0], color1);

                //Mat CropImg = new Mat();

                //裁剪图片
                Rectangle Rect1 = new Rectangle(CenterPoint[0].X - Rectangle[0].Width / 2, CenterPoint[0].Y - Rectangle[0].Height / 2, Rectangle[0].Width, Rectangle[0].Height);
                Bitmap m_BitmapCrop = crop(SrcImage, Rect1);
                //CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                //Cv2.ImShow("CropImg1", CropImg);
                return m_BitmapCrop;
            }
            else
                return SrcImage;
        }

        /// <summary>
        /// 定位MainBoard站点二维码函数
        /// </summary>
        /// <param name="SrcImage">原始图像</param>
        public static Bitmap LocateMainBoardDataMatric(Bitmap SrcImage)
        {
            Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  // 待处理图像
            Mat Rotate_Image = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  //带旋转图像

            int height = Srcmat.Rows;
            int width = Srcmat.Cols;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    byte b = Srcmat.At<Vec3b>(i, j).Item0;            //这里AT函数和GET一样只能获取值
                    byte g = Srcmat.At<Vec3b>(i, j).Item1;
                    byte r = Srcmat.At<Vec3b>(i, j).Item2;
                    if (b > 200 && g > 200 && r > 200)
                    {
                        //  Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(0), (byte)(0), (byte)(0)));
                    }
                    else
                    {
                        Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(0), (byte)(0), (byte)(0)));
                    }
                }
            }

            //转成灰度图
            Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);

            //膨胀
            Mat Erode = new Mat();
            //获取自定义核
            Mat element1 = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(1, 1));

            //开，闭运算
            Cv2.MorphologyEx(src_gray, Erode, MorphTypes.Erode, element1);


            //Canny边缘检测
            Mat canny = new Mat();
            Cv2.Canny(Erode, canny, 130, 255);

            //获得轮廓
            Point[][] contours;
            List<Point[]> contours2 = new List<Point[]>();
            HierarchyIndex[] hierarchly;

            //寻找轮廓 
            //第一个参数是输入图像 2值化的
            //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
            //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
            //第四个参数是类型，采用树结构
            //第五个参数是节点拟合模式，这里是全部寻找
            Cv2.FindContours(canny, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

            List<Point> CenterPoint = new List<Point>();
            RotatedRect RR;
            List<float> angle = new List<float>();
            List<OpenCvSharp.Rect> Rectangle = new List<OpenCvSharp.Rect>();
            //轮廓筛选
            for (int i = 0; i < contours.Length; i++)
            {
                double X = Cv2.ArcLength(contours[i], true);
                if (X > 200)
                {
                    double X1 = 0.01 * X;
                    Point[] approx = Cv2.ApproxPolyDP(contours[i], X1, true);
                    int corners = approx.Count();
                    RR = Cv2.MinAreaRect(approx);
                    //Rect R = Cv2.BoundingRect(approx);
                    Rect R = RR.BoundingRect();
                    if (R.Width > 110 && R.Height > 110 && R.Width < 180 && R.Height < 180)
                    {
                        Point p;
                        p.X = R.X + (int)(R.Width / 2);
                        p.Y = R.Y + (int)(R.Height / 2);
                        CenterPoint.Add(p);
                        contours2.Add(contours[i]);
                        angle.Add(RR.Angle);
                        Rectangle.Add(R);
                    }
                }
            }

            ////将结果画出并返回结果
            //Mat dst_Image = Srcmat.Clone();

            //Random rnd = new Random();
            //////填充定位点
            ////for (int i = 0; i < contours2.Count; i++)
            ////{
            ////    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            ////    Cv2.DrawContours(dst_Image, contours2, i, color, 2, LineTypes.Link8);
            ////    Cv2.ImShow("dst_Image", dst_Image);
            ////}

            if (angle.Count > 0)
            {
                Rect[] arr = Rectangle.ToArray();
                int max = arr[0].Height;
                int index = 0;//把假设的最大值索引赋值非index
                for (int i = 1; i < arr.Length; i++)
                {
                    if (arr[i].Height >= max)
                    {
                        max = arr[i].Height;
                        index = i;//把较大值的索引赋值非index
                    }
                }

                //旋转
                Mat affine_matrix = Cv2.GetRotationMatrix2D(CenterPoint[index], angle[index], 1.0);
                Cv2.WarpAffine(Rotate_Image, Rotate_Image, affine_matrix, Rotate_Image.Size());
                //Cv2.ImShow("Rotate", Rotate_Image);

                //Scalar color1 = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //Cv2.Rectangle(dst_Image, Rectangle[index], color1);

                //Mat CropImg = new Mat();
                //////画圆 cvPoint:确定圆的坐标  200：圆的半径 CV_RGB：圆的颜色 3：线圈的粗细
                ////foreach (var p in CenterPoint)
                ////{
                ////    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                ////    Cv2.Circle(dst_Image, p.X, p.Y, 5, color, 2, LineTypes.Link8, 0);
                ////}
                //Cv2.ImShow("center", dst_Image);

                //裁剪图片
                Rectangle Rect1 = new Rectangle(CenterPoint[index].X - 80, CenterPoint[index].Y - 80, 160, 160);
                Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Rotate_Image); // mat 转 bitmap
                Bitmap m_BitmapCrop = crop(bitmap, Rect1);
                //CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                //Cv2.ImShow("CropImg", CropImg);
                return m_BitmapCrop;
            }
            else
                return SrcImage;
        }


        /// <summary>
        /// 定位FlipChassis站点二维码函数
        /// </summary>
        /// <param name="SrcImage">原始图像</param>
        public static Bitmap LocateFlipChassisFirst(Bitmap SrcImage)
        {
            Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  // 待处理图像
            Mat Rotate_Image = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  //带旋转图像

            int height = Srcmat.Rows;
            int width = Srcmat.Cols;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    byte b = Srcmat.At<Vec3b>(i, j).Item0;            //这里AT函数和GET一样只能获取值
                    byte g = Srcmat.At<Vec3b>(i, j).Item1;
                    byte r = Srcmat.At<Vec3b>(i, j).Item2;
                    if (b > 245 && g > 245 && r > 245)
                    {
                        //  Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(0), (byte)(0), (byte)(0)));
                    }
                    else
                    {
                        Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(0), (byte)(0), (byte)(0)));
                    }
                }
            }
            //Cv2.ImShow("Srcmat", Srcmat);

            //转成灰度图
            Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);

            //膨胀
            Mat Dilate = new Mat();
            Mat element1 = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(20, 20));
            Cv2.MorphologyEx(src_gray, Dilate, MorphTypes.Dilate, element1);


            //Canny边缘检测
            Mat canny = new Mat();
            Cv2.Canny(Dilate, canny, 120, 255);

            //粗定位
            //获得轮廓
            Point[][] contours;
            List<Point[]> contours2 = new List<Point[]>();
            HierarchyIndex[] hierarchly;
            List<Point> CenterPoint = new List<Point>();

            //寻找轮廓 
            //第一个参数是输入图像 2值化的
            //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
            //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
            //第四个参数是类型，采用树结构
            //第五个参数是节点拟合模式，这里是全部寻找
            Cv2.FindContours(Dilate, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

            RotatedRect RR;
            List<float> angle = new List<float>();
            List<OpenCvSharp.Rect> Rectangle = new List<OpenCvSharp.Rect>();

            //轮廓筛选
            for (int i = 0; i < contours.Length; i++)
            {
                double X = Cv2.ArcLength(contours[i], true);
                if (X > 100)
                {
                    double X1 = 0.01 * X;
                    Point[] approx = Cv2.ApproxPolyDP(contours[i], X1, true);
                    int corners = approx.Count();
                    RR = Cv2.MinAreaRect(approx);
                    //Rect R = Cv2.BoundingRect(approx);
                    Rect R = RR.BoundingRect();
                    if (R.Width > 200 || R.Height > 200)
                    {
                        Point p;
                        p.X = R.X + (int)(R.Width / 2);
                        p.Y = R.Y + (int)(R.Height / 2);
                        CenterPoint.Add(p);
                        contours2.Add(contours[i]);
                        angle.Add(RR.Angle);
                        Rectangle.Add(R);
                    }
                }
            }

            ////将结果画出并返回结果
            //Mat dst_Image = Srcmat.Clone();
            //Random rnd = new Random();
            ////填充定位点
            //for (int i = 0; i < contours2.Count; i++)
            //{
            //    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            //    Cv2.DrawContours(dst_Image, contours2, i, color, 2, LineTypes.Link8);
            //    Cv2.NamedWindow("dst_Image", WindowMode.KeepRatio);
            //    Cv2.ResizeWindow("dst_Image", 500, 500);
            //    Cv2.ImShow("dst_Image", dst_Image);
            //}

            //for (int i = 0; i < contours2.Count; i++)
            //{
            //    Point point = Center_cal(contours2, i);
            //    CenterPoint.Add(point);
            //}

            //画圆 cvPoint:确定圆的坐标  200：圆的半径 CV_RGB：圆的颜色 3：线圈的粗细
            //if (CenterPoint.Count > 0)
            //{
            //    foreach (var p in CenterPoint)
            //    {
            //        Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            //        Cv2.Circle(dst_Image, p.X, p.Y, 20, color, 2, LineTypes.Link8, 0);
            //    }
            //}
            //Cv2.NamedWindow("dst", WindowMode.KeepRatio);
            //Cv2.ResizeWindow("dst", 500, 500);
            //Cv2.ImShow("dst", dst_Image);

            if (CenterPoint.Count > 0)
            {
                //旋转
                Mat affine_matrix = Cv2.GetRotationMatrix2D(CenterPoint[0], angle[0], 1.0);
                Cv2.WarpAffine(Rotate_Image, Rotate_Image, affine_matrix, Rotate_Image.Size());
                //Cv2.NamedWindow("Rotate", WindowMode.KeepRatio);
                //Cv2.ResizeWindow("Rotate", 500, 500);
                //Cv2.ImShow("Rotate", Rotate_Image);

                //裁剪图片
                Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Rotate_Image); // mat 转 bitmap
                //Rectangle Rect1 = new Rectangle((int)(CenterPoint[0].X - (Rectangle[0].Width / 2)), (int)(CenterPoint[0].Y - (Rectangle[0].Height / 2)), Rectangle[0].Width, Rectangle[0].Height);
                Rectangle Rect1 = new Rectangle(CenterPoint[0].X - 120, CenterPoint[0].Y - 120, 240, 240);
                Bitmap m_BitmapCrop = crop(bitmap, Rect1);
                //Mat CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                //Cv2.ImShow("CropImg1", CropImg);
                return m_BitmapCrop;
            }
            else
            {
                return SrcImage;
            }
        }

        /// <summary>
        ///定位二维码(thresold OTSU)
        /// </summary>
        /// <param name="SrcImage">Bitmap裁剪数据</param>
        /// <param name="IsLargeCode">区分不同站点参数</param>
        public static Bitmap LocateQRcodeOTSU(Bitmap SrcImage, string IsLargeCode)
        {
            //粗定位
            Bitmap m_bitmap = LocateQRcodeFirst(SrcImage);

            Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_bitmap);  // 待处理图像
            Mat Rotate_Image = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_bitmap);  //带旋转图像

            //转成灰度图
            Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);

            //二值化
            Mat threshold_output = new Mat();
            Cv2.AdaptiveThreshold(src_gray, threshold_output, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 17, 2);
            //Cv2.ImShow("Threshold", threshold_output);

            //Canny边缘检测
            Mat canny = new Mat();
            Cv2.Canny(threshold_output, canny, 130, 255);
            //Cv2.ImShow("Canny", canny);

            //获得轮廓
            Point[][] contours;
            List<Point[]> contours2 = new List<Point[]>();
            HierarchyIndex[] hierarchly;

            //寻找轮廓 
            //第一个参数是输入图像 2值化的
            //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
            //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
            //第四个参数是类型，采用树结构
            //第五个参数是节点拟合模式，这里是全部寻找
            Cv2.FindContours(canny, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

            List<Point> CenterPoint = new List<Point>();
            List<float> angle = new List<float>();
            List<OpenCvSharp.RotatedRect> Rectangle = new List<OpenCvSharp.RotatedRect>();

            //轮廓筛选
            int c = 0, ic = 0;
            int parentIdx = -1;
            for (int i = 0; i < contours.Length; i++)
            {
                //hierarchy[i][2] != -1 表示不是最外面的轮廓
                if (hierarchly[i].Child != -1 && ic == 0)
                {
                    parentIdx = i;
                    ic++;
                }
                else if (hierarchly[i].Child != -1)
                {
                    ic++;
                }
                //最外面的清0
                else if (hierarchly[i].Child == -1)
                {
                    ic = 0;
                    parentIdx = -1;
                }
                //找到定位点信息
                if (ic >= 2)
                {
                    double X = Cv2.ArcLength(contours[parentIdx], true);
                    double X1 = 0.01 * X;
                    Point[] approx = Cv2.ApproxPolyDP(contours[parentIdx], X1, true);
                    int corners = approx.Count();
                    RotatedRect RR = Cv2.MinAreaRect(approx);
                    if (corners >= 4 && corners <= 8)
                    {
                        if (90 < X && X < 230)
                        {
                            contours2.Add(contours[parentIdx]);
                            ic = 0;
                            parentIdx = -1;
                            Point p;
                            p.X = (int)RR.Center.X;
                            p.Y = (int)RR.Center.Y;
                            CenterPoint.Add(p);
                            angle.Add(RR.Angle);
                        }
                    }
                }
            }

            //将结果画出并返回结果
            Mat dst_Image = Srcmat.Clone();
            Random rnd = new Random();
            //for (int i = 0; i < contours2.Count; i++)
            //{
            //    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            //    Cv2.DrawContours(dst_Image, contours2, i, color, 2, LineTypes.Link8);
            //    Cv2.NamedWindow("dst_Image", WindowMode.KeepRatio);
            //    Cv2.ResizeWindow("dst_Image", 500, 500);
            //    Cv2.ImShow("dst_Image", dst_Image);
            //}

            if (angle.Count > 0)
            {
                List<Point> CP = new List<Point>();
                CP.Clear();
                CP.Add(CenterPoint[0]);
                Point PTemp = CenterPoint[0];
                foreach (Point p in CenterPoint)
                {
                    if (GetDistance(p, PTemp) >= 20)
                    {
                        CP.Add(p);
                        PTemp = p;
                    }
                }
                Point Centerp;
                if (CP.Count == 3)
                {
                    Centerp = GetCircleCenter(CP[0], CP[1], CP[2]);
                }
                else
                {
                    return m_bitmap;
                }

                //旋转
                Mat affine_matrix = Cv2.GetRotationMatrix2D(Centerp, angle[0], 1.0);
                Cv2.WarpAffine(Rotate_Image, Rotate_Image, affine_matrix, Rotate_Image.Size());
                //Cv2.NamedWindow("Rotate", WindowMode.KeepRatio);
                //Cv2.ResizeWindow("Rotate", 500, 500);
                //Cv2.ImShow("Rotate", Rotate_Image);

                //Scalar color1 = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //Cv2.Rectangle(dst_Image, Rectangle[0], color1);

                Mat CropImg = new Mat();
                //////画圆 cvPoint:确定圆的坐标  200：圆的半径 CV_RGB：圆的颜色 3：线圈的粗细
                //foreach (var p in CenterPoint)
                //{
                //    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //    Cv2.Circle(dst_Image, p.X, p.Y, 5, color, 2, LineTypes.Link8, 0);
                //}
                //Cv2.NamedWindow("center", WindowMode.KeepRatio);
                //Cv2.ResizeWindow("center", 500, 500);
                //Cv2.ImShow("center", dst_Image);

                //裁剪图片
                Rectangle Rect1 = new Rectangle(Centerp.X - 80, Centerp.Y - 80, 160, 160);
                Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Rotate_Image); // mat 转 bitmap
                Bitmap m_BitmapCrop = crop(bitmap, Rect1);
                CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                //Cv2.ImShow("CropImg", CropImg);
                return m_BitmapCrop;
            }
            else
            {
                return m_bitmap;
            }
        }




        /// <summary>
        ///定位二维码(thresold OTSU)
        /// </summary>
        /// <param name="SrcImage">Bitmap裁剪数据</param>
        /// <param name="IsLargeCode">区分不同站点参数</param>
        public static Bitmap LocateGuamQRcode(Bitmap SrcImage, string IsLargeCode)
        {

            Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  // 待处理图像
            Mat Rotate_Image = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  //带旋转图像

            //转成灰度图
            Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);

            //二值化
            Mat threshold_output = new Mat();
            Cv2.AdaptiveThreshold(src_gray, threshold_output, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 17, 2);

            //Canny边缘检测
            Mat canny = new Mat();
            Cv2.Canny(threshold_output, canny, 130, 255);
            //Cv2.ImShow("Canny", canny);

            //获得轮廓
            Point[][] contours;
            List<Point[]> contours2 = new List<Point[]>();
            HierarchyIndex[] hierarchly;

            //寻找轮廓 
            //第一个参数是输入图像 2值化的
            //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
            //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
            //第四个参数是类型，采用树结构
            //第五个参数是节点拟合模式，这里是全部寻找
            Cv2.FindContours(canny, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

            List<Point> CenterPoint = new List<Point>();
            List<float> angle = new List<float>();
            List<OpenCvSharp.RotatedRect> Rectangle = new List<OpenCvSharp.RotatedRect>();

            //轮廓筛选
            int c = 0, ic = 0;
            int parentIdx = -1;
            for (int i = 0; i < contours.Length; i++)
            {
                //hierarchy[i][2] != -1 表示不是最外面的轮廓
                if (hierarchly[i].Child != -1 && ic == 0)
                {
                    parentIdx = i;
                    ic++;
                }
                else if (hierarchly[i].Child != -1)
                {
                    ic++;
                }
                //最外面的清0
                else if (hierarchly[i].Child == -1)
                {
                    ic = 0;
                    parentIdx = -1;
                }
                //找到定位点信息
                if (ic >= 2)
                {
                    double X = Cv2.ArcLength(contours[parentIdx], true);
                    double X1 = 0.01 * X;
                    Point[] approx = Cv2.ApproxPolyDP(contours[parentIdx], X1, true);
                    int corners = approx.Count();
                    RotatedRect RR = Cv2.MinAreaRect(approx);
                    if (corners >= 4 && corners <= 8)
                    {
                        if (90 < X && X < 230)
                        {
                            contours2.Add(contours[parentIdx]);
                            ic = 0;
                            parentIdx = -1;
                            Point p;
                            p.X = (int)RR.Center.X;
                            p.Y = (int)RR.Center.Y;
                            CenterPoint.Add(p);
                            angle.Add(RR.Angle);
                        }
                    }
                }
            }

            //将结果画出并返回结果
            Mat dst_Image = Srcmat.Clone();
            Random rnd = new Random();

            if (angle.Count > 0)
            {
                List<Point> CP = new List<Point>();
                CP.Clear();
                CP.Add(CenterPoint[0]);
                Point PTemp = CenterPoint[0];
                foreach (Point p in CenterPoint)
                {
                    if (GetDistance(p, PTemp) >= 20)
                    {
                        CP.Add(p);
                        PTemp = p;
                    }
                }
                Point Centerp;
                if (CP.Count == 3)
                {
                    Centerp = GetCircleCenter(CP[0], CP[1], CP[2]);
                    //旋转
                    Mat affine_matrix = Cv2.GetRotationMatrix2D(Centerp, angle[2], 1.0);
                    Cv2.WarpAffine(Rotate_Image, Rotate_Image, affine_matrix, Rotate_Image.Size());

                    Mat CropImg = new Mat();
                    //裁剪图片
                    Rectangle Rect1 = new Rectangle(Centerp.X - 100, Centerp.Y - 100, 200, 200);
                    Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Rotate_Image); // mat 转 bitmap
                    Bitmap m_BitmapCrop = crop(bitmap, Rect1);
                    CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                    return m_BitmapCrop;
                }
                else
                {
                    Mat CropImg = new Mat();
                    //裁剪图片
                    Rectangle Rect1 = new Rectangle(CP[0].X - 150, CP[0].Y - 150, 300, 300);
                    Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Rotate_Image); // mat 转 bitmap
                    Bitmap m_BitmapCrop = crop(bitmap, Rect1);
                    CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                    return m_BitmapCrop;
                }
            }
            else
            {
                return SrcImage;
            }
        }

        /// <summary>
        ///定位二维码标签纸
        /// </summary>
        /// <param name="SrcImage">Bitmap裁剪数据</param>
        public static Bitmap LocateQRcodeFirst(Bitmap SrcImage)
        {
            Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  // 待处理图像
            Mat Rotate_Image = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  //带旋转图像
            //Cv2.NamedWindow("origin", WindowMode.KeepRatio);
            //Cv2.ResizeWindow("origin", 500, 500);
            //Cv2.ImShow("origin", Srcmat);

            //转成灰度图
            Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);
            //Cv2.ImShow("gray", src_gray);

            ////锐化
            //InputArray kernel1 = InputArray.Create<float>(new float[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } });
            ////滤波
            //Cv2.Filter2D(src_gray, src_gray, -1, kernel1, new OpenCvSharp.Point(-1, -1), 0);

            //二值化
            //////二值化
            Mat threshold_output = new Mat();
            Cv2.Threshold(src_gray, threshold_output, 220, 255, ThresholdTypes.Binary);
            //Cv2.NamedWindow("Threshold", WindowMode.KeepRatio);
            //Cv2.ResizeWindow("Threshold", 500, 500);
            //Cv2.ImShow("Threshold", threshold_output);

            //膨胀
            Mat Dilate = new Mat();
            Mat Open = new Mat();
            //获取自定义核
            Mat element1 = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(5, 5));
            Mat element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(10, 10));

            //开，闭运算
            Cv2.MorphologyEx(threshold_output, Dilate, MorphTypes.Dilate, element1);
            //Cv2.NamedWindow("Dilate", WindowMode.KeepRatio);
            //Cv2.ResizeWindow("Dilate", 500, 500);
            //Cv2.ImShow("Dilate", Dilate);

            //粗定位
            //获得轮廓
            Point[][] contours;
            List<Point[]> contours2 = new List<Point[]>();
            HierarchyIndex[] hierarchly;
            List<Point> CenterPoint = new List<Point>();

            //寻找轮廓 
            //第一个参数是输入图像 2值化的
            //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
            //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
            //第四个参数是类型，采用树结构
            //第五个参数是节点拟合模式，这里是全部寻找
            Cv2.FindContours(Dilate, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

            RotatedRect RR;
            List<float> angle = new List<float>();
            List<OpenCvSharp.Rect> Rectangle = new List<OpenCvSharp.Rect>();

            //轮廓筛选
            for (int i = 0; i < contours.Length; i++)
            {
                double X = Cv2.ArcLength(contours[i], true);
                if (X > 1000)
                {
                    double X1 = 0.01 * X;
                    Point[] approx = Cv2.ApproxPolyDP(contours[i], X1, true);
                    int corners = approx.Count();
                    RR = Cv2.MinAreaRect(approx);
                    //Rect R = Cv2.BoundingRect(approx);
                    Rect R = RR.BoundingRect();
                    if (R.Width > 500 || R.Height > 500)
                    {
                        Point p;
                        p.X = R.X + (int)(R.Width / 2);
                        p.Y = R.Y + (int)(R.Height / 2);
                        CenterPoint.Add(p);
                        contours2.Add(contours[i]);
                        angle.Add(RR.Angle);
                        Rectangle.Add(R);
                    }
                }
            }

            //将结果画出并返回结果
            Mat dst_Image = Srcmat.Clone();
            Random rnd = new Random();
            //填充定位点
            for (int i = 0; i < contours2.Count; i++)
            {
                Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                Cv2.DrawContours(dst_Image, contours2, i, color, 2, LineTypes.Link8);
                //Cv2.NamedWindow("dst_Image", WindowMode.KeepRatio);
                //Cv2.ResizeWindow("dst_Image", 500, 500);
                //Cv2.ImShow("dst_Image", dst_Image);
            }

            for (int i = 0; i < contours2.Count; i++)
            {
                Point point = Center_cal(contours2, i);
                CenterPoint.Add(point);
            }

            //画圆 cvPoint:确定圆的坐标  200：圆的半径 CV_RGB：圆的颜色 3：线圈的粗细
            if (CenterPoint.Count > 0)
            {
                foreach (var p in CenterPoint)
                {
                    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                    Cv2.Circle(dst_Image, p.X, p.Y, 20, color, 2, LineTypes.Link8, 0);
                }
            }
            //Cv2.NamedWindow("dst", WindowMode.KeepRatio);
            //Cv2.ResizeWindow("dst", 500, 500);
            //Cv2.ImShow("dst", dst_Image);

            if (CenterPoint.Count > 0)
            {
                ////旋转
                //Mat affine_matrix = Cv2.GetRotationMatrix2D(CenterPoint[0], angle[0], 1.0);
                //Cv2.WarpAffine(Rotate_Image, Rotate_Image, affine_matrix, Rotate_Image.Size());
                //Cv2.NamedWindow("Rotate", WindowMode.KeepRatio);
                //Cv2.ResizeWindow("Rotate", 500, 500);
                //Cv2.ImShow("Rotate", Rotate_Image);

                //裁剪图片
                //Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Rotate_Image); // mat 转 bitmap
                Rectangle Rect1 = new Rectangle((int)(CenterPoint[0].X - (Rectangle[0].Width / 2)), (int)(CenterPoint[0].Y - (Rectangle[0].Height / 2)), Rectangle[0].Width, Rectangle[0].Height);
                Bitmap m_BitmapCrop = crop(SrcImage, Rect1);
                Mat CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                //Cv2.ImShow("CropImg", CropImg);
                return m_BitmapCrop;
            }
            else
            {
                return SrcImage;
            }


            //if (angle.Count > 0)
            //{
            //    Rect[] arr = Rectangle.ToArray();
            //    int max = arr[0].Height;
            //    int index = 0;//把假设的最大值索引赋值非index
            //    for (int i = 1; i < arr.Length; i++)
            //    {
            //        if (arr[i].Height >= max)
            //        {
            //            max = arr[i].Height;
            //            index = i;//把较大值的索引赋值非index
            //        }
            //    }

            //    //旋转
            //    Mat affine_matrix = Cv2.GetRotationMatrix2D(CenterPoint[index], angle[index], 1.0);
            //    Cv2.WarpAffine(Rotate_Image, Rotate_Image, affine_matrix, Rotate_Image.Size());
            //    Cv2.NamedWindow("Rotate", WindowMode.KeepRatio);
            //    Cv2.ResizeWindow("Rotate", 500, 500);
            //    Cv2.ImShow("Rotate", Rotate_Image);

            //    Scalar color1 = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            //    Cv2.Rectangle(dst_Image, Rectangle[index], color1);

            //    Mat CropImg = new Mat();
            //    ////画圆 cvPoint:确定圆的坐标  200：圆的半径 CV_RGB：圆的颜色 3：线圈的粗细
            //    //foreach (var p in CenterPoint)
            //    //{
            //    //    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            //    //    Cv2.Circle(dst_Image, p.X, p.Y, 5, color, 2, LineTypes.Link8, 0);
            //    //}
            //    Cv2.NamedWindow("center", WindowMode.KeepRatio);
            //    Cv2.ResizeWindow("center", 500, 500);
            //    Cv2.ImShow("center", dst_Image);

            //    //裁剪图片
            //    Rectangle Rect1 = new Rectangle(CenterPoint[index].X - 90, CenterPoint[index].Y - 90, 180, 180);
            //    Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Rotate_Image); // mat 转 bitmap
            //    Bitmap m_BitmapCrop = crop(bitmap, Rect1);
            //    CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
            //    Cv2.ImShow("CropImg", CropImg);
            //    return m_BitmapCrop;
            //}
            //else
            //    return SrcImage;
        }


        ///// <summary>
        /////surf方式实现图像配准(surf+C#)
        ///// </summary>
        ///// <param name="SrcImage">Bitmap裁剪数据</param>
        ///// <param name="IsLargeCode">区分不同站点参数</param>
        //public static Bitmap MatchPicBySurf(Bitmap imgSrc, Bitmap imgSub, double threshold =10000,double Scale = 1)
        //{
        //    int MIN_MATCH_COUNT = 4;
        //    Mat matSrc = OpenCvSharp.Extensions.BitmapConverter.ToMat(imgSrc);
        //    Mat matTo = OpenCvSharp.Extensions.BitmapConverter.ToMat(imgSub);
        //    int h = matSrc.Rows;
        //    int w = matSrc.Cols;


        //    Mat matSrcScale = new Mat();
        //    Mat matToScale = new Mat();
        //    matSrc.CopyTo(matSrcScale);
        //    matTo.CopyTo(matToScale);

        //    if (matTo.Rows >1000 || matTo.Cols > 1000)
        //    {
        //        Cv2.Resize(matTo, matToScale, new Size(0, 0), fx: 1.0 / Scale, fy: 1.0 / Scale);
        //    }
        //    if (matSrc.Rows > 1000 || matSrc.Cols > 1000)
        //    {
        //        Cv2.Resize(matSrc, matSrcScale, new Size(0, 0), fx: 1.0 / Scale, fy: 1.0 / Scale);
        //    }

        //    using (Mat matSrcRet = new Mat())
        //    using (Mat matToRet = new Mat())
        //    {
        //        KeyPoint[] keyPointsSrc, keyPointsTo;
        //        using (var surf = OpenCvSharp.XFeatures2D.SURF.Create(threshold))
        //        {
        //            surf.DetectAndCompute(matSrcScale, null, out keyPointsSrc, matSrcRet);
        //            surf.DetectAndCompute(matToScale, null, out keyPointsTo, matToRet);
        //        }

        //        OpenCvSharp.Flann.KDTreeIndexParams kDTreeIndexParams = new OpenCvSharp.Flann.KDTreeIndexParams(trees: 5);
        //        OpenCvSharp.Flann.SearchParams searchParams = new OpenCvSharp.Flann.SearchParams(checks :50);

        //        using (var flnMatcher = new OpenCvSharp.FlannBasedMatcher(kDTreeIndexParams, searchParams))
        //        {
        //            var matches = flnMatcher.KnnMatch(matSrcRet, matToRet, 2);

        //            var pointsSrc = new List<Point2f>();
        //            var pointsDst = new List<Point2f>();
        //            //筛选较好的匹配点
        //            var goodMatches = new List<DMatch>();
        //            var coff = 0.9; // 0.1 0.7  0.8  参数可以自己修改进行测试

        //            foreach (DMatch[] items in matches.Where(x => x.Length > 1))
        //            {
        //                if (items[0].Distance < coff * items[1].Distance)
        //                {
        //                    pointsSrc.Add(keyPointsSrc[items[0].QueryIdx].Pt * Scale);
        //                    pointsDst.Add(keyPointsTo[items[0].TrainIdx].Pt * Scale);
        //                    goodMatches.Add(items[0]);
        //                }
        //            }

        //            var outMat = new Mat();
        //            Mat H = new Mat(); //配准使用的H值
        //            // 算法RANSAC对匹配的结果做过滤
        //            var pSrc = pointsSrc.ConvertAll(Point2fToPoint2d);
        //            var pDst = pointsDst.ConvertAll(Point2fToPoint2d);
        //            var outMask = new Mat();
        //            // 如果原始的匹配结果为空, 则跳过过滤步骤
        //            if (pSrc.Count > MIN_MATCH_COUNT && pDst.Count > MIN_MATCH_COUNT)
        //                H = Cv2.FindHomography(pSrc, pDst, HomographyMethods.Ransac,5.0, mask: outMask);

        //            if (H.Cols == 0)
        //            {
        //                return null;
        //            }

        //            var pts = new List<Point2f>();
        //            pts.Add(new Point2f(0, 0));
        //            pts.Add(new Point2f(0, h - 1));
        //            pts.Add(new Point2f(w - 1, h - 1));
        //            pts.Add(new Point2f(w- 1, 0));
        //            var dst = Cv2.PerspectiveTransform(pts, H);

        //            //根据原始宽高计算
        //            var dst1 = new List<Point2f>();
        //            dst1.Add(dst[0]);
        //            dst1.Add(dst[3]);
        //            dst1.Add(dst[1]);
        //            dst1.Add(dst[2]);

        //            var pts1 = new List<Point2f>();
        //            pts1.Add(new Point2f(0, 0));
        //            pts1.Add(new Point2f(w - 1, 0));
        //            pts1.Add(new Point2f(0, h - 1));
        //            pts1.Add(new Point2f(w - 1, h - 1));

        //            Mat M = Cv2.GetPerspectiveTransform(dst1, pts1);
        //            Cv2.WarpPerspective(matTo, outMat, M, new Size(w, h));

        //            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(outMat);
        //        }
        //    }
        //}

        /// <summary>
        ///surf方式实现图像配准(surf+C#)
        /// </summary>
        /// <param name="SrcImage">Bitmap裁剪数据</param>
        /// <param name="IsLargeCode">区分不同站点参数</param>
        public static Bitmap MatchPicBySurf(Bitmap imgSrc, Bitmap imgSub,  double S = 1)
        {
            Mat matSrc = OpenCvSharp.Extensions.BitmapConverter.ToMat(imgSrc);
            Mat matTo = OpenCvSharp.Extensions.BitmapConverter.ToMat(imgSub);
            int h = matSrc.Rows;
            int w = matSrc.Cols;

            KeyPoint[] keyPoints;
            Mat matToRet = new Mat();
            matToRet = SurfDetect(imgSub, out keyPoints, S);

            Mat H = new Mat();
            H = SurfKnnMatch(GoldenDescripit, matToRet, GoldenkeyPoints, keyPoints, Scale: S);

            if (H.Cols == 0)
            {
                return null;
            }

            Mat outMat = new Mat();
            var pts = new List<Point2f>();
            pts.Add(new Point2f(0, 0));
            pts.Add(new Point2f(0, h - 1));
            pts.Add(new Point2f(w - 1, h - 1));
            pts.Add(new Point2f(w - 1, 0));
            var dst = Cv2.PerspectiveTransform(pts, H);

            //根据原始宽高计算
            var dst1 = new List<Point2f>();
            dst1.Add(dst[0]);
            dst1.Add(dst[3]);
            dst1.Add(dst[1]);
            dst1.Add(dst[2]);

            var pts1 = new List<Point2f>();
            pts1.Add(new Point2f(0, 0));
            pts1.Add(new Point2f(w - 1, 0));
            pts1.Add(new Point2f(0, h - 1));
            pts1.Add(new Point2f(w - 1, h - 1));

            Mat M = Cv2.GetPerspectiveTransform(dst1, pts1);
            Cv2.WarpPerspective(matTo, outMat, M, new Size(w, h));

            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(outMat);
        }

        /// <summary>
        /// Surf特征点查找
        /// </summary>
        /// <param name="imgSrc">原始输入图像/param>
        /// <param name="Scale">缩放比例尺寸/param>
        public  static Mat SurfDetect(Bitmap imgSrc,out KeyPoint[] keyPoints, double Scale = 1)
        {
            Mat matSrc = OpenCvSharp.Extensions.BitmapConverter.ToMat(imgSrc);

            if (matSrc.Rows > 1000 || matSrc.Cols > 1000)
            {
                Cv2.Resize(matSrc, matSrc, new Size(0, 0), fx: 1.0 / Scale, fy: 1.0 / Scale);
            }

            Mat matSrcRet = new Mat();
            SurfObj.DetectAndCompute(matSrc, null, out keyPoints, matSrcRet);
            return matSrcRet;
        }

        /// <summary>
        /// Surf透视变换矩阵计算 H计算
        /// </summary>
        /// <param name="matSrcRet">原始文件特征点描述符/param>
        /// <param name="matDstRet">待配准文件特征点描述符/param>
        /// <param name="coff">特征点比例/param>
        /// <param name="k">KNNMatch参数/param>
        private static Mat SurfKnnMatch(Mat matSrcRet, Mat matDstRet, KeyPoint[] GoldenkeyPoints, KeyPoint[] keyPoints,double coff =0.5,int k = 2, double Scale = 1)
        {
            OpenCvSharp.Flann.KDTreeIndexParams kDTreeIndexParams = new OpenCvSharp.Flann.KDTreeIndexParams(trees: 5);
            OpenCvSharp.Flann.SearchParams searchParams = new OpenCvSharp.Flann.SearchParams(checks: 50);

            using (var flnMatcher = new OpenCvSharp.FlannBasedMatcher(kDTreeIndexParams, searchParams))
            {
                var matches = flnMatcher.KnnMatch(matSrcRet, matDstRet, 2);

                var pointsSrc = new List<Point2f>();
                var pointsDst = new List<Point2f>();
                //筛选较好的匹配点
                var goodMatches = new List<DMatch>();

                foreach (DMatch[] items in matches.Where(x => x.Length > 1))
                {
                    if (items[0].Distance < coff * items[1].Distance)
                    {
                        pointsSrc.Add(GoldenkeyPoints[items[0].QueryIdx].Pt * Scale);
                        pointsDst.Add(keyPoints[items[0].TrainIdx].Pt * Scale);
                        goodMatches.Add(items[0]);
                    }
                }

                var outMat = new Mat();
                Mat H = new Mat(); //配准使用的H值

                // 算法RANSAC对匹配的结果做过滤
                var pSrc = pointsSrc.ConvertAll(Point2fToPoint2d);
                var pDst = pointsDst.ConvertAll(Point2fToPoint2d);
                var outMask = new Mat();
                // 如果原始的匹配结果为空, 则跳过过滤步骤
                if (pSrc.Count > 4 && pDst.Count > 4)
                    H = Cv2.FindHomography(pSrc, pDst, HomographyMethods.Ransac, 5.0, mask: outMask);

                return H;
            }
        }

        /// <summary>
        /// 检测SIM产品有无
        /// </summary>
        /// <param name="SrcImage">原始图像</param>
        public static bool CheckDUTExist(Bitmap SrcImage)
        {
            Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  // 待处理图像
            //Cv2.ImShow("Srcmat", Srcmat);

            //转成灰度图
            Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);
            //Cv2.ImShow("gray", src_gray);

            ////二值化
            Mat threshold_output = new Mat();
            Cv2.Threshold(src_gray, threshold_output, 100, 255, ThresholdTypes.Otsu);
            //Cv2.ImShow("Threshold", threshold_output);

            //Canny边缘检测
            Mat canny = new Mat();
            Cv2.Canny(threshold_output, canny, 130, 255);
            //Cv2.ImShow("Canny", canny);

            //获得轮廓
            Point[][] contours;
            List<Point[]> contours2 = new List<Point[]>();
            HierarchyIndex[] hierarchly;

            //寻找轮廓 
            //第一个参数是输入图像 2值化的
            //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
            //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
            //第四个参数是类型，采用树结构
            //第五个参数是节点拟合模式，这里是全部寻找
            Cv2.FindContours(canny, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

            List<Point> CenterPoint = new List<Point>();
            RotatedRect RR;
            List<float> angle = new List<float>();
            List<OpenCvSharp.Rect> Rectangle = new List<OpenCvSharp.Rect>();
            //轮廓筛选
            for (int i = 0; i < contours.Length; i++)
            {
                double X = Cv2.ArcLength(contours[i], true);
                if (X > 250)
                {
                    double X1 = 0.01 * X;
                    Point[] approx = Cv2.ApproxPolyDP(contours[i], X1, true);
                    int corners = approx.Count();
                    RR = Cv2.MinAreaRect(approx);
                    Rect R = RR.BoundingRect();
                    if (R.Width > 300 && R.Height > 150 && corners >= 4)
                    {
                        contours2.Add(contours[i]);
                        angle.Add(RR.Angle);
                    }
                }
            }

            //将结果画出并返回结果
            //Mat dst_Image = Srcmat.Clone();

            //Random rnd = new Random();
            ////填充定位点
            //for (int i = 0; i < contours2.Count; i++)
            //{
            //    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            //    Cv2.DrawContours(dst_Image, contours2, i, color, 2, LineTypes.Link8);
            //    Cv2.ImShow("dst_Image", dst_Image);
            //}

            if (angle.Count > 0)
            {
                return true;
            }
            else
                return false;
        }




        /// <summary>
        /// 浮点数转为整形数
        /// </summary>
        /// <param name="input">原始输入数组</param>
        private static Point2d Point2fToPoint2d(Point2f input)
        {
            Point2d p2 = new Point2d(input.X, input.Y);
            return p2;
        }


        /// <summary>
        /// 定位Images站点二维码函数
        /// </summary>
        /// <param name="SrcImage">原始图像</param>
        public static Bitmap LocateImagersMatric(Bitmap SrcImage)
        {
            Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  // 待处理图像
            Mat Rotate_Image = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);  //带旋转图像

            int height = Srcmat.Rows;
            int width = Srcmat.Cols;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    byte b = Srcmat.At<Vec3b>(i, j).Item0;            //这里AT函数和GET一样只能获取值
                    byte g = Srcmat.At<Vec3b>(i, j).Item1;
                    byte r = Srcmat.At<Vec3b>(i, j).Item2;
                    if (b > 130 && g > 130 && r > 130)
                    {
                        //  Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(0), (byte)(0), (byte)(0)));
                    }
                    else
                    {
                        Srcmat.Set<Vec3b>(i, j, new Vec3b((byte)(0), (byte)(0), (byte)(0)));
                    }
                }
            }

            //转成灰度图
            Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);

            //膨胀
            Mat Erode = new Mat();
            Mat element1 = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(1, 1));


            //开，闭运算
            Cv2.MorphologyEx(src_gray, Erode, MorphTypes.Erode, element1);


            //Canny边缘检测
            Mat canny = new Mat();
            Cv2.Canny(Erode, canny, 130, 255);

            //获得轮廓
            Point[][] contours;
            List<Point[]> contours2 = new List<Point[]>();
            HierarchyIndex[] hierarchly;

            //寻找轮廓 
            //第一个参数是输入图像 2值化的
            //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
            //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
            //第四个参数是类型，采用树结构
            //第五个参数是节点拟合模式，这里是全部寻找
            Cv2.FindContours(canny, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

            List<Point> CenterPoint = new List<Point>();
            RotatedRect RR;
            List<float> angle = new List<float>();
            List<OpenCvSharp.RotatedRect> Rectangle = new List<OpenCvSharp.RotatedRect>();
            //轮廓筛选
            for (int i = 0; i < contours.Length; i++)
            {
                double X = Cv2.ArcLength(contours[i], true);
                if (X > 200)
                {
                    double X1 = 0.01 * X;
                    Point[] approx = Cv2.ApproxPolyDP(contours[i], X1, true);
                    int corners = approx.Count();
                    RR = Cv2.MinAreaRect(approx);
                    //Rect R = Cv2.BoundingRect(approx);
                    //Rect R = RR.BoundingRect();
                    if (Math.Abs(RR.Size.Width - RR.Size.Height) < 10 && RR.Size.Width > 170 && RR.Size.Width < 210)
                    {
                        Point p;
                        p.X = (int)RR.Center.X;
                        p.Y = (int)RR.Center.Y;
                        CenterPoint.Add(p);
                        contours2.Add(contours[i]);
                        angle.Add(RR.Angle);
                        Rectangle.Add(RR);
                    }
                }
            }

            ////将结果画出并返回结果
            //Mat dst_Image = Srcmat.Clone();

            //Random rnd = new Random();
            ////填充定位点
            //for (int i = 0; i < contours2.Count; i++)
            //{
            //    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            //    Cv2.DrawContours(dst_Image, contours2, i, color, 2, LineTypes.Link8);
            //    //Cv2.ImShow("dst_Image", dst_Image);
            //}

            if (angle.Count > 0)
            {
                RotatedRect[] arr = Rectangle.ToArray();
                float max = arr[0].Size.Height;
                int index = 0;//把假设的最大值索引赋值非index
                for (int i = 1; i < arr.Length; i++)
                {
                    if (arr[i].Size.Height >= max)
                    {
                        max = arr[i].Size.Height;
                        index = i;//把较大值的索引赋值非index
                    }
                }

                //旋转
                Mat affine_matrix = Cv2.GetRotationMatrix2D(CenterPoint[index], angle[index], 1.0);
                Cv2.WarpAffine(Rotate_Image, Rotate_Image, affine_matrix, Rotate_Image.Size());
                //Cv2.ImShow("Rotate", Rotate_Image);


                //画出旋转矩形
                //Scalar color1 = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //Cv2.Rectangle(dst_Image, Rectangle[index].Center.X, color1);
                Point2f[] vertices = Rectangle[index].Points();
                Point[] verticesPoint = new Point[4];
                for (int i = 0; i < 4; i++)
                {
                    verticesPoint[i].X = (int)vertices[i].X;
                    verticesPoint[i].Y = (int)vertices[i].Y;
                }

                //for (int i = 0; i < 4; i++)//画矩形
                //{
                //    Cv2.Line(dst_Image, verticesPoint[i], verticesPoint[(i + 1) % 4], color1);
                //}

                //Mat CropImg = new Mat();
                ////画圆 cvPoint:确定圆的坐标  200：圆的半径 CV_RGB：圆的颜色 3：线圈的粗细
                //foreach (var p in CenterPoint)
                //{
                //    Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //    Cv2.Circle(dst_Image, p.X, p.Y, 5, color, 2, LineTypes.Link8, 0);
                //}
                //Cv2.ImShow("center", dst_Image);

                //裁剪图片
                Rectangle Rect1 = new Rectangle(CenterPoint[index].X - 110, CenterPoint[index].Y - 110, 220, 220);
                Bitmap bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(Rotate_Image); // mat 转 bitmap
                Bitmap m_BitmapCrop = crop(bitmap, Rect1);
                //CropImg = OpenCvSharp.Extensions.BitmapConverter.ToMat(m_BitmapCrop);
                //Cv2.ImShow("CropImg", CropImg);
                return m_BitmapCrop;
            }
            else
                return SrcImage;
        }

        /// <summary>
        ///裁剪Bitmap
        /// </summary>
        /// <param name="oriBmp">Bitmap待裁剪数据</param>
        /// <param name="rect">ROI区域</param>
        public static Bitmap crop(Bitmap oriBmp, Rectangle rect)
        {

            Bitmap target = new Bitmap(rect.Width, rect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(oriBmp, new Rectangle(0, 0, target.Width, target.Height),
                      rect,
                      GraphicsUnit.Pixel);
            }
            return target;
        }

        /// <summary>
        ///定位二位码位置并裁剪出来
        /// </summary>
        /// <param name="StationName">工站名称</param>
        /// <param name="SrcImage">Bitmap裁剪数据</param>
        /// <param name="IsLargeCode">区分不同站点参数</param>
        public static Bitmap LocateBarcode(string StationName,Bitmap SrcImage,string IsLargeCode)
        {
            switch(StationName)
            {
                case "MainBoard":
                    return LocateMainBoardDataMatric(SrcImage);
                case "BoardinHSG":
                    return LocateQRcodeOTSU(SrcImage, IsLargeCode);
                case "BaseBracket":
                    return LocateQRcodeOTSU(SrcImage, IsLargeCode);
                case "ChassisScrews":
                    return LocateRightDataMatric(SrcImage);
                case "FlipChassis":
                    return LocateFlipChassisFirst(SrcImage);
                case "LeftBoard":
                    return LocateLeftDataMatric(SrcImage);
                case "RightBoard":
                    return LocateRightDataMatric(SrcImage);
                case "Cosmetic":
                    return LocateGuamQRcode(SrcImage, IsLargeCode);
                case "UnitsClosed":
                    return LocateQRcodeOTSU(SrcImage, IsLargeCode);
                default:
                    return LocateGuamQRcode(SrcImage, IsLargeCode);
            }
        }
        #endregion

        public static Point GetCircleCenter(Point p1, Point p2, Point p3)  //求圆心
        {
            double a, b, c, d, e, f;
            Point p;
            
            a = 2*(p2.X-p1.X);
            b = 2*(p2.Y-p1.Y);
            c = p2.X * p2.X + p2.Y * p2.Y - p1.X * p1.X - p1.Y * p1.Y;
            d = 2*(p3.X - p2.X);
            e = 2*(p3.Y - p2.Y);
            f = p3.X * p3.X + p3.Y * p3.Y - p2.X * p2.X - p2.Y * p2.Y;
            p.X = (int)((b*f-e*c)/(b*d-e*a));
            p.Y = (int)((d*c-a*f)/(b*d-e*a)); 
            return p;
        }

        public static  Point Center_cal(List<Point[]> contours, int i)
        {
            int centerx = 0, centery = 0, n = contours[i].Length;
            centerx = (contours[i][n / 4].X + contours[i][n * 2 / 4].X + contours[i][3 * n / 4].X + contours[i][n - 1].X) / 4;
            centery = (contours[i][n / 4].Y + contours[i][n * 2 / 4].Y + contours[i][3 * n / 4].Y + contours[i][n - 1].Y) / 4;
            Point point1 = new Point(centerx, centery);
            return point1;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }


        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        public static Bitmap CropBitmap(BitmapImage _image, Rectangle rect)
        {
            // BitmapImage image = getBitmapImage(imagePath);

            Bitmap oriBmp = new Bitmap(5472, 3648);
            oriBmp = BitmapImage2Bitmap(_image);
            Bitmap target = new Bitmap(rect.Width, rect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(oriBmp, new Rectangle(0, 0, target.Width, target.Height),
                      rect,
                      GraphicsUnit.Pixel);
            }

            return target;
        }



        /// <summary>
        /// 把内存里的BitmapImage数据保存到硬盘中
        /// </summary>
        /// <param name="bitmapImage">BitmapImage数据</param>
        /// <param name="filePath">输出的文件路径</param>
        public static void SaveBitmapImageIntoFile(BitmapImage bitmapImage, string filePath)

        {

            BitmapEncoder encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));



            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))

            {

                encoder.Save(fileStream);

            }

        }

        ////先模糊在锐化 Factor= 1   Threshold=30  针对边缘
        //public static Mat ImproveUSM(Mat src, int Threshold = 30, float Factor = 1)
        //{
        //    Mat dst = new Mat();
        //    Mat DiffMask = src.Clone();
        //    Mat BlurImg = new Mat();
        //    Cv2.Blur(src, BlurImg, new Size(7, 7));
        //  //  Cv2.ImShow("Blur", BlurImg);

        //    if(src.Channels() == 1)
        //    {

        //        int height = src.Rows; //行 获取图片的行叠加起来就是高度
        //        int width = src.Cols;  //列  ... ...     宽度
        //                               //1.at 方式  速度较慢
        //                               //for (int row = 0; row < height; row++)
        //                               //{
        //                               //    for (int col = 0; col < width; col++)
        //                               //    {
        //                               //        byte p1 = src.At<byte>(row, col); //获对应矩阵坐标的取像素
        //                               //        byte p = BlurImg.At<byte>(row, col); //获对应矩阵坐标的取像素
        //                               //        int x = Math.Abs(p1 - p);

        //        //        byte value1 = byte.Parse((1).ToString()); //设置像素值
        //        //        byte value2 = byte.Parse((0).ToString()); //设置像素值

        //        //        if (x<Threshold)
        //        //        {
        //        //            DiffMask.Set<byte>(row, col, value1);
        //        //        }
        //        //        else
        //        //        {
        //        //            DiffMask.Set<byte>(row, col, value2);
        //        //        }
        //        //    }
        //        //}
        //        //2.指针 方式  速度较快
        //        unsafe
        //        {
        //            for (int row = 1; row < height-1; row++)
        //            {
        //                IntPtr a = src.Ptr(row);
        //                byte* b = (byte*)a.ToPointer();

        //                IntPtr a1 = BlurImg.Ptr(row);
        //                byte* b1 = (byte*)a1.ToPointer();

        //                IntPtr a2 = DiffMask.Ptr(row);
        //                byte* b2 = (byte*)a2.ToPointer();

        //                for (int col = 1; col < width-1; col++)
        //                {                            
        //                    int x = Math.Abs(b[col] - b1[col]);                     
        //                    if (x < Threshold)
        //                    {
        //                        b2[col] = 1;
        //                    }
        //                    else
        //                    {
        //                        b2[col] = 0;
        //                    }
        //                }
        //            }
        //        }


        //    }


        //    Cv2.AddWeighted(src, 1 + Factor, BlurImg, -Factor, 0, dst);
        //    src.CopyTo(dst, DiffMask);
        //    return dst;
        //}

        ////定位二维码(BLOB)
        //public static void LocateQRcodeBLOB(Bitmap SrcImage)
        //{
        //    //找到所提取轮廓的中心点
        //    //在提取的中心小正方形的边界上每隔周长个像素提取一个点的坐标，求所提取四个点的平均坐标（即为小正方形的大致中心）
        //    Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);
        //    Cv2.ImShow("Srcmat", Srcmat);
        //    //    Cv2.WaitKey();

        //    //转成灰度图
        //    Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);
        //    Cv2.ImShow("gray", src_gray);
        //    //   Cv2.WaitKey();

        //    Mat sharpMat = new Mat();
        //    InputArray kernel1 = InputArray.Create<float>(new float[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } });
        //    //滤波
        //    Cv2.Filter2D(src_gray, sharpMat, -1, kernel1, new OpenCvSharp.Point(-1, -1), 0);
        //    Cv2.ImShow("USM", sharpMat);


        //    Mat dst = ImproveUSM(src_gray,20);
        //    Cv2.ImShow("ImproveUSM", dst);
        //  //  Cv2.ImWrite(@"C:\Users\86156\Desktop\out.png", dst);//保存到桌面



        //}


        //定位二维码(thresold OTSU)
        //public static Bitmap LocateQRcodeOTSU(Bitmap SrcImage, string IsLargeCode)
        //{
        //    //找到所提取轮廓的中心点
        //    //在提取的中心小正方形的边界上每隔周长个像素提取一个点的坐标，求所提取四个点的平均坐标（即为小正方形的大致中心）
        //    Mat Srcmat = OpenCvSharp.Extensions.BitmapConverter.ToMat(SrcImage);

        //    //转成灰度图
        //    Mat src_gray = Srcmat.CvtColor(ColorConversionCodes.BGR2GRAY);

        //    //获得轮廓
        //    Point[][] contours;
        //    List<Point[]> contours2 = new List<Point[]>();
        //    HierarchyIndex[] hierarchly;

        //    //预处理
        //    //模糊
        //    //Cv2.Blur(src_gray, src_gray, new Size(3, 3));
        //    //Cv2.ImShow("Blur", src_gray);
        //    //锐化
        //    InputArray kernel1 = InputArray.Create<float>(new float[3, 3] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } });
        //    //滤波
        //    Cv2.Filter2D(src_gray, src_gray, -1, kernel1, new OpenCvSharp.Point(-1, -1), 0);

        //    ////二值化
        //    Mat threshold_output = new Mat();
        //    Cv2.Threshold(src_gray, threshold_output, 128, 255, ThresholdTypes.Binary);
        //    //   Cv2.ImShow("Threshold", threshold_output);

        //    //膨胀
        //    Mat Dilate = new Mat();
        //    //获取自定义核
        //    Mat element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(2, 2));
        //    //第一个参数MORPH_RECT表示矩形的卷积核，当然还可以选择椭圆形的、交叉型的
        //    //膨胀操作
        //    Cv2.Erode(threshold_output, Dilate, element);


        //    //寻找轮廓 
        //    //第一个参数是输入图像 2值化的
        //    //第二个参数是内存存储器，FindContours找到的轮廓放到内存里面。
        //    //第三个参数是层级，**[Next, Previous, First_Child, Parent]** 的vector
        //    //第四个参数是类型，采用树结构
        //    //第五个参数是节点拟合模式，这里是全部寻找
        //    Cv2.FindContours(Dilate, out contours, out hierarchly, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new Point(0, 0));

        //    //轮廓筛选
        //    int c = 0, ic = 0;
        //    int parentIdx = -1;
        //    for (int i = 0; i < contours.Length; i++)
        //    {
        //        //hierarchy[i][2] != -1 表示不是最外面的轮廓
        //        if (hierarchly[i].Child != -1 && ic == 0)
        //        {
        //            parentIdx = i;
        //            ic++;
        //        }
        //        else if (hierarchly[i].Child != -1)
        //        {
        //            ic++;
        //        }
        //        //最外面的清0
        //        else if (hierarchly[i].Child == -1)
        //        {
        //            ic = 0;
        //            parentIdx = -1;
        //        }
        //        //找到定位点信息
        //        if (ic >= 2)
        //        {
        //            double X = Cv2.ArcLength(contours[parentIdx], true);
        //            double X1 = 0.01 * X;
        //            Point[] approx = Cv2.ApproxPolyDP(contours[parentIdx], X1, true);
        //            int corners = approx.Count();
        //            if (corners >= 4 && corners <= 10)
        //            {
        //                if (IsLargeCode == "1")
        //                {
        //                    if (170 < X && X < 250)
        //                    {
        //                        contours2.Add(contours[parentIdx]);
        //                        ic = 0;
        //                        parentIdx = -1;
        //                    }
        //                }
        //                else
        //                {
        //                    if (135 < X && X < 170)
        //                    {
        //                        contours2.Add(contours[parentIdx]);
        //                        ic = 0;
        //                        parentIdx = -1;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    List<Point> CenterPoint = new List<Point>();
        //    for (int i = 0; i < contours2.Count; i++)
        //    {
        //        Point point = Center_cal(contours2, i);
        //        CenterPoint.Add(point);
        //    }

        //    Rectangle Rect1;
        //    if (CenterPoint.Count > 0)
        //    {
        //        if (CenterPoint.Count == 1)
        //        {
        //            Rect1 = new System.Drawing.Rectangle(CenterPoint[0].X - 140, CenterPoint[0].Y - 140, 200, 200);
        //        }
        //        else if (CenterPoint.Count == 2)
        //        {
        //            OpenCvSharp.Point point1;
        //            point1.X = (int)((CenterPoint[0].X + CenterPoint[1].X) / 2);
        //            point1.Y = (int)((CenterPoint[0].Y + CenterPoint[1].Y) / 2);
        //            Rect1 = new System.Drawing.Rectangle(point1.X - 150, point1.Y - 150, 300, 300);
        //        }
        //        else
        //        {
        //            OpenCvSharp.Point CenterP = GetCircleCenter(CenterPoint[0], CenterPoint[1], CenterPoint[2]);
        //            Rect1 = new System.Drawing.Rectangle(CenterP.X - 150, CenterP.Y - 150, 300, 300);
        //        }
        //        Bitmap m_BitmapCrop = crop(SrcImage, Rect1);
        //        return m_BitmapCrop;
        //    }

        //    else
        //        return SrcImage;
        //}

        /// <summary>
        /// 判断输入的字符串是否只包含数字和英文字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumAndEnCh(string input)
        {
            string pattern = @"^[A-Z0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 计算两点距离
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <returns></returns>
        public static double GetDistance(Point startPoint, Point endPoint)
        {
            int x = System.Math.Abs(endPoint.X - startPoint.X);
            int y = System.Math.Abs(endPoint.Y - startPoint.Y);
            return Math.Sqrt(x * x + y * y);
        }

    }


}
