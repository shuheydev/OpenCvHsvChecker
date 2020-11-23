using OpenCvSharp;
using System;

namespace OpenCvHsvChecker
{
    class Program
    {
        private static int _h_min = 0;
        private static int _h_max = 0;
        private static int _s_min = 0;
        private static int _s_max = 0;
        private static int _v_min = 0;
        private static int _v_max = 0;

        private static Mat src = new Mat();
        private static Mat hsv = new Mat();
        //private static Mat dst = new Mat();

        private const string WINDOW_NAME = "HSV Checker";
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string inputImagePath = "Images/gauge-2.jpg";

            Cv2.NamedWindow(WINDOW_NAME);
            Cv2.CreateTrackbar("H_Min", "HSV Checker", 179, onChange: H_Min_Changed);
            Cv2.CreateTrackbar("H_Max", "HSV Checker", 179, onChange: H_Max_Changed);
            Cv2.CreateTrackbar("S_Min", "HSV Checker", 255, onChange: S_Min_Changed);
            Cv2.CreateTrackbar("S_Max", "HSV Checker", 255, onChange: S_Max_Changed);
            Cv2.CreateTrackbar("V_Min", "HSV Checker", 255, onChange: V_Min_Changed);
            Cv2.CreateTrackbar("V_Max", "HSV Checker", 255, onChange: V_Max_Changed);

            src = Cv2.ImRead(inputImagePath);
            Cv2.CvtColor(src, hsv, ColorConversionCodes.BGR2HSV, 3);

            Cv2.ImShow(WINDOW_NAME, src);

            Cv2.WaitKey();
        }

        private static void V_Max_Changed(int pos, IntPtr userData)
        {
            _v_max = pos;
            Update();
        }

        private static void V_Min_Changed(int pos, IntPtr userData)
        {
            _v_min = pos;
            Update();
        }

        private static void S_Max_Changed(int pos, IntPtr userData)
        {
            _s_max = pos;
            Update();
        }

        private static void S_Min_Changed(int pos, IntPtr userData)
        {
            _s_min = pos;
            Update();
        }

        private static void H_Max_Changed(int pos, IntPtr userData)
        {
            _h_max = pos;
            Update();
        }

        private static void H_Min_Changed(int pos, IntPtr userData)
        {
            _h_min = pos;
            Console.WriteLine(_h_min);
            Update();
        }


        static void Update()
        {
            var scalar_min = new Scalar(_h_min, _s_min, _v_min);
            var scalar_max = new Scalar(_h_max, _s_max, _v_max);
            Mat mask = new Mat();
            Cv2.InRange(hsv, scalar_min, scalar_max, mask);

            Mat dst = new Mat();
            src.CopyTo(dst, mask);

            Cv2.ImShow(WINDOW_NAME, dst);
        }
    }
}
