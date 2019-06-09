using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using System.Drawing;
using Emgu;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;
using Emgu.CV.Features2D;
using System.Windows.Forms;

namespace InvoiceExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            Mat src = new Mat(@"Invoices/invoice1.png", Emgu.CV.CvEnum.ImreadModes.AnyColor);
            Mat template = new Mat(@"Images/t1.jpg", Emgu.CV.CvEnum.ImreadModes.AnyColor);
            Mat t2 = new Mat(@"Images/t2.jpg", Emgu.CV.CvEnum.ImreadModes.AnyColor);
            Mat t3 = new Mat(@"Images/t4.jpg", Emgu.CV.CvEnum.ImreadModes.AnyColor);
            Rectangle rectangle = program.CVsearchImg(src, template);
            Rectangle r2 = program.CVsearchImg(src, t2);
            Rectangle r3 = program.CVsearchImg(src, t3);
            Console.WriteLine(program.Img2Txt(rectangle));
            Console.WriteLine(program.Img2Txt(r3));
            Console.WriteLine(r2.Location.X + ";" + r2.Location.Y);
            Console.WriteLine(program.Img2Txt(r2));


        }

        public string Img2Txt(string path)
        {
            TesseractEngine te = new TesseractEngine(@"","eng",EngineMode.Default);
            string result;
            Pix p = Pix.LoadFromFile(path);
            try
            {
                p.Deskew();
                p.RemoveLines();            
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            result = te.Process(p, PageSegMode.Auto).GetText();
            return result;
        }

        public string Img2Txt(Rectangle rectangle)
        {
            TesseractEngine te = new TesseractEngine("tessdata","eng", EngineMode.Default);
            string result;
            Rect rect = new Rect(rectangle.X,rectangle.Y,rectangle.Width,rectangle.Height);
           
            Pix p = Pix.LoadFromFile(@"Invoices/invoice1.png");
            try
            {
                p.Deskew();
                p.RemoveLines();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            result = te.Process(p,rect, PageSegMode.Auto).GetText();
            return result;
        }

        public Rectangle CVsearchImg(Mat Src, Mat Template)
        {
            
            Mat MatchResult = new Mat();//匹配结果
            CvInvoke.MatchTemplate(Src, Template, MatchResult, Emgu.CV.CvEnum.TemplateMatchingType.CcorrNormed);//使用相关系数法匹配
            Point max_loc = new Point();
            Point min_loc = new Point();
            double max = 0, min = 0;
            CvInvoke.MinMaxLoc(MatchResult, ref min, ref max, ref min_loc, ref max_loc);//获得极值信息           
            Console.WriteLine("\r\nX:" + max_loc.X + " Y:" + max_loc.Y + " 最大相似度:" + max + " 最小相似度:" + min);
            return new Rectangle(max_loc, Template.Size);
        }
        
    }
}
