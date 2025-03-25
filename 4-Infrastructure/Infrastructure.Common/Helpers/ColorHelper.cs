using System.Drawing;

namespace Infrastructure.Common.Helpers
{
    public class ColorHelper
    {
        //取得开始-结束渐变色的每一个值共255个
        public static List<Color> getGradientColorArr(Color startColor, Color endColor)
        {
            List<Color> listColor = new List<Color>();

            int redSpace = endColor.R - startColor.R;
            int greenSpace = endColor.G - startColor.G;
            int blueSpace = endColor.B - startColor.B;
            for (int i = 0; i <= 255; i++)
            {
                Color vColor = Color.FromArgb(
                    startColor.R + (int)((double)i / 255 * redSpace),
                    startColor.G + (int)((double)i / 255 * greenSpace),
                    startColor.B + (int)((double)i / 255 * blueSpace)
                );
                listColor.Add(vColor);
            }
            return listColor;
        }
        public static List<Color> getGradientColorArr(Color color1, Color color2, Color color3)
        {
            List<Color> listColor1 = getGradientColorArr(color1, color2);
            List<Color> listColor2 = getGradientColorArr(color2, color3);
            //  List<Color> listColor3 = getColorArr(color3, color4);

            //var arrcounts = listColor1.Concat(listColor2).Concat(listColor3).ToList();
            var arrcounts = listColor1.Concat(listColor2).ToList();
            return arrcounts;
        }
    }
}
