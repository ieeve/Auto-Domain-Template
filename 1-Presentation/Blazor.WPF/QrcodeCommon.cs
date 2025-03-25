using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace Blazor.WPF
{
    public class QrcodeCommon
    {
        public void FindQrPoint(string imagePath)
        {
            // var src = Cv2.ImRead(Environment.CurrentDirectory + "\\12345.jpg", ImreadModes.Color);
            var src = Cv2.ImRead("D:\\123456.jpg", ImreadModes.Color);
            var src_gray = new Mat();
            Mat src_all = src.Clone();

            //彩色图转灰度图
            Cv2.CvtColor(src, src_gray, ColorConversionCodes.BGR2GRAY);

            //对图像进行平滑处理
            Cv2.Blur(src_gray, src_gray, new OpenCvSharp.Size(3, 3));

            Cv2.ImShow("src_gray", src_gray);


            Scalar color = new Scalar(1, 1, 255);

            Mat threshold_output = new Mat();
            OpenCvSharp.Point[][] contours;
            List<OpenCvSharp.Point[]> contours2 = new List<OpenCvSharp.Point[]>();
            HierarchyIndex[] hierarchy;
            Mat drawing = Mat.Zeros(src.Size(), MatType.CV_8UC3);
            Mat drawing2 = Mat.Zeros(src.Size(), MatType.CV_8UC3);
            Mat drawingAllContours = Mat.Zeros(src.Size(), MatType.CV_8UC3);

            //指定112阈值进行二值化
            Cv2.Threshold(src_gray, threshold_output, 112, 255, ThresholdTypes.Binary);
            Cv2.ImShow("Threshold_output", threshold_output);

            Cv2.FindContours(threshold_output, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, new OpenCvSharp.Point(0, 0));
            int c = 0, ic = 0, k = 0, area = 0;
            RNG rng = new RNG(255);
            //通过黑色定位角作为父轮廓，有两个子轮廓的特点，筛选出三个定位角
            int parentIdx = -1;
            for (int i = 0; i < contours.Length; i++)
            {
                //画出所有轮廓图
                Cv2.DrawContours(drawingAllContours, contours, parentIdx, new Scalar(255, 255, 255));
                if (hierarchy[i].Child != -1 && ic == 0)
                {
                    parentIdx = i;
                    ic++;
                }
                else if (hierarchy[i].Child != -1)
                {
                    ic++;
                }
                else if (hierarchy[i].Child == -1)
                {
                    ic = 0;
                    parentIdx = -1;
                }

                //有两个子轮廓
                if (ic >= 2)
                {
                    //保存找到的三个黑色定位角
                    contours2.Add(contours[parentIdx]);
                    //画出三个黑色定位角的轮廓
                    Cv2.DrawContours(drawing, contours, parentIdx, new Scalar(rng.Next(), rng.Next(), rng.Next()), 1, LineTypes.Link8);
                    ic = 0;
                    parentIdx = -1;
                }

            }

            //填充的方式画出三个黑色定位角的轮廓
            for (int i = 0; i < contours2.Count; i++)
            {
                Cv2.DrawContours(drawing2, contours2, i, new Scalar(rng.Next(), rng.Next(), rng.Next()), -1, LineTypes.Link4, hierarchy, 0, new OpenCvSharp.Point());
            }

            //获取三个定位角的中心坐标
            OpenCvSharp.Point[] point = new OpenCvSharp.Point[3];
            for (int i = 0; i < contours2.Count; i++)
            {
                point[i] = Center_cal(contours2, i);
            }

            //计算轮廓的面积，计算定位角的面积，从而计算出边长
            area = (int)Cv2.ContourArea(contours2[1]);
            int area_side = (int)Math.Round(Math.Sqrt((double)area));
            for (int i = 0; i < contours2.Count; i++)
            {
                //画出三个定位角的中心连线
                Cv2.Line(drawing2, point[i % contours2.Count], point[(i + 1) % contours2.Count], color, area_side / 2, LineTypes.Link8);
            }

            Cv2.ImShow("DrawingAllContours", drawingAllContours);
            Cv2.ImShow("Drawing2", drawing2);
            Cv2.ImShow("Drawing", drawing);


            //接下来要框处整个二维码
            Mat gray_all = new Mat();
            Mat threshold_output_all = new Mat();
            OpenCvSharp.Point[][] contours_all;
            HierarchyIndex[] hierarchy_all;
            Cv2.CvtColor(drawing2, gray_all, ColorConversionCodes.BGR2GRAY);


            Cv2.Threshold(gray_all, threshold_output_all, 45, 255, ThresholdTypes.Binary);
            Cv2.FindContours(threshold_output_all, out contours_all, out hierarchy_all, RetrievalModes.External, ContourApproximationModes.ApproxNone, new OpenCvSharp.Point(0, 0)); //External表示只寻找最外层轮廓


            Point2f[] fourPoint2f = new Point2f[4];
            //求最小包围矩形
            RotatedRect rectPoint = Cv2.MinAreaRect(contours_all[0]);

            //将rectPoint变量中存储的坐标值放到fourPoint的数组中
            fourPoint2f = rectPoint.Points();

            for (int i = 0; i < 4; i++)
            {

                Cv2.Line(src_all, new OpenCvSharp.Point(fourPoint2f[i % 4].X, fourPoint2f[i % 4].Y), new OpenCvSharp.Point(fourPoint2f[(i + 1) % 4].X, fourPoint2f[(i + 1) % 4].Y), new Scalar(20, 21, 237), 3);
            }

            Cv2.ImShow("Src_all", src_all);


        }

        public static Point Center_cal(List<Point[]> contours, int i)
        {
            int centerx = 0, centery = 0, n = contours[i].Length;
            centerx = (contours[i][n / 4].X + contours[i][n * 2 / 4].X + contours[i][3 * n / 4].X + contours[i][n - 1].X) / 4;
            centery = (contours[i][n / 4].Y + contours[i][n * 2 / 4].Y + contours[i][3 * n / 4].Y + contours[i][n - 1].Y) / 4;
            Point point1 = new Point(centerx, centery);
            return point1;
        }
    }
}
