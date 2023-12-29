using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;

namespace ConsoleImageViewer{
    public struct Rect{
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    class Program{
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr ho);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreatePatternBrush(IntPtr hbm);

        [DllImport("user32.dll")]
        static extern int FillRect(IntPtr hdc, ref Rect lprc, IntPtr hbr);

        public static void Main(string[] args){
            Console.Clear();
            Thread.Sleep(50);

            if(args.Length == 0 || args[0] == "/?" || args[0] == "--help"){
                Console.WriteLine("Использование: civ <файл> [длина] [ширина] [-p]\n\nПросмотр изображений в консоли.\n\n        [длина]     Длина изображения. Если равна 0, использовать оригинальную длину.\n        [ширина]    Ширина изображения. Если равна 0, использовать оригинальную ширину.\n        [-p]        Ждать нажатия клавиши после завершения (чтобы изображение не затерлось приглашением консоли)");
                Environment.Exit(0);
            }

            if(!File.Exists(args[0])){
                Console.WriteLine($"ОШИБКА: файл \"{args[0]}\" не найден");
                Environment.Exit(1);
            }

            int width = 0, height = 0;
            Image img = Image.FromFile(args[0]);
            Bitmap bm = new Bitmap(img);

            if(args.Length > 1){
                if(!int.TryParse(args[1], out width) || !int.TryParse(args[2], out height)){
                    Console.WriteLine($"ОШИБКА: размер задан некорректно");
                    Environment.Exit(1);
                }
            }

            if(width == 0 || height == 0){
                width = img.Width;
                height = img.Height;
            }

            if(args.Length > 3 && args[3] == "-p"){
                Console.SetCursorPosition(Console.WindowWidth - 1, Console.WindowHeight - 1);
            }

            IntPtr hWnd = GetConsoleWindow();
            IntPtr hdc = GetDC(hWnd);
            IntPtr hbm = bm.GetHbitmap();
            IntPtr hbr = CreatePatternBrush(hbm);

            Rect rect = new Rect(){ Left = 0, Top = 0, Right = width, Bottom = height };

            FillRect(hdc, ref rect, hbr);

            DeleteObject(hbm);
            DeleteObject(hbr);
            ReleaseDC(hWnd, hdc);

            if(args.Length > 3 && args[3] == "-p"){
                Console.ReadKey(false);
            }
        }
    }
}
