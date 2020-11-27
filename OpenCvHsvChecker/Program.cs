using OpenCvSharp;
using System;
using System.Collections.Generic;

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

        private static Mat _src = new Mat();
        private static Mat _hsv = new Mat();
        private static Mat _dst = new Mat();
        private static Mat _mask = new Mat();

        private const string WINDOW_NAME = "HSV Checker";
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string inputImagePath = "Images/gauge_2hand.jpg";

            _src = Cv2.ImRead(inputImagePath);
            if (_src is null)
                return;
            Cv2.ImShow("src", _src);

            Cv2.CvtColor(_src, _hsv, ColorConversionCodes.BGR2HSV);
            Cv2.ImShow("hsv", _hsv);

            //名前つきウィンドウを作成
            Cv2.NamedWindow(WINDOW_NAME);

            //ウィンドウ名を指定してスライダーを配置
            Cv2.CreateTrackbar("H_Min", WINDOW_NAME, 359, onChange: H_Min_Changed);
            Cv2.CreateTrackbar("H_Max", WINDOW_NAME, 359, onChange: H_Max_Changed);
            Cv2.CreateTrackbar("S_Min", WINDOW_NAME, 255, onChange: S_Min_Changed);
            Cv2.CreateTrackbar("S_Max", WINDOW_NAME, 255, onChange: S_Max_Changed);
            Cv2.CreateTrackbar("V_Min", WINDOW_NAME, 255, onChange: V_Min_Changed);
            Cv2.CreateTrackbar("V_Max", WINDOW_NAME, 255, onChange: V_Max_Changed);

            //初期画像を表示
            Cv2.ImShow(WINDOW_NAME, _src);

            Cv2.WaitKey();

            Cv2.ImWrite("Images/output.jpg", _mask);
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
            _h_max = pos - 179;
            Update();
        }

        private static void H_Min_Changed(int pos, IntPtr userData)
        {
            _h_min = pos - 179;
            Console.WriteLine(_h_min);
            Update();
        }


        static void Update()
        {
            //HSV画像とスライダーの値からマスクを生成
            var scalar_min = new Scalar(_h_min, _s_min, _v_min);
            var scalar_max = new Scalar(_h_max, _s_max, _v_max);

            _src.CopyTo(_mask);
            if (_h_min < 0)
            {
                var tempScalar1 = new Scalar(179, _s_max, _v_max);
                var tempScalar2 = new Scalar(0, _s_min, _s_min);
                scalar_min = new Scalar(_h_min + 180, _s_min, _v_min);

                var masks = new List<Mat>();
                masks.Add(CreateMaskImage(_src, scalar_min, tempScalar1));
                masks.Add(CreateMaskImage(_src, tempScalar2, scalar_max));

                Cv2.BitwiseOr(masks[0], masks[1], _mask);
            }
            else
            {
                Cv2.InRange(_hsv, scalar_min, scalar_max, _mask);
            }


            ////マスク画像を使って元画像にフィルタをかける
            //_dst = new Mat();
            ////src.CopyTo(src, mask);
            //Cv2.BitwiseAnd(_src, _src, _dst, _mask);

            //ウィンドウの画像を更新
            _src.CopyTo(_dst);

            int HueToDisplay(int hue)
            {
                return (hue + 180) ;
            }
            int SatValToDisplay(int aaa)
            {
                return (int)(aaa /2.55);
            }
            Cv2.PutText(_dst, $"Hue: {HueToDisplay(_h_min)} ~ {HueToDisplay(_h_max)}", new Point(20, 20), HersheyFonts.HersheyTriplex, fontScale: 1.5, Scalar.Red, thickness: 2);
            Cv2.PutText(_dst, $"Sat: {SatValToDisplay(_s_min)} ~ {SatValToDisplay(_s_max)}", new Point(20, 70), HersheyFonts.HersheyTriplex, fontScale: 1.5, Scalar.Red, thickness: 2);
            Cv2.PutText(_dst, $"Val: {SatValToDisplay(_v_min)} ~ {SatValToDisplay(_v_max)}", new Point(20, 120), HersheyFonts.HersheyTriplex, fontScale: 1.5, Scalar.Red, thickness: 2);


            Cv2.ImShow("src", _dst);
            Cv2.ImShow(WINDOW_NAME, _mask);
        }

        private static Mat CreateMaskImage(Mat image, Scalar hsv_min, Scalar hsv_max)
        {
            //HSVに変換
            Mat hsv = new Mat();
            Cv2.CvtColor(image, hsv, ColorConversionCodes.BGR2HSV);

            //マスクを作成
            Mat maskImage = new Mat();
            Cv2.InRange(hsv, hsv_min, hsv_max, maskImage);
            //Cv2.BitwiseNot(maskImage, maskImage);

            return maskImage;
        }
    }
}
