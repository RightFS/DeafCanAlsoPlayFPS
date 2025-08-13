using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace Common.Utils
{
    public static class Window
    {
        // Win32 常量和函数导入
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;  // 添加工具窗口样式常量
        private const int GWL_EXSTYLE = (-20);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public static void MakeWindowTransparent(System.Windows.Window window)
        {
            try
            {
                // 获取窗口句柄
                IntPtr hwnd = new WindowInteropHelper(window).Handle;
                // 获取当前窗口的扩展样式
                int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                // 设置窗口为透明
                SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"设置窗口透明失败: {ex.Message}");
            }
        }
        // 添加新方法：使窗口不出现在Alt+Tab列表中
        public static void HideFromAltTab(System.Windows.Window window)
        {
            try
            {
                // 获取窗口句柄
                IntPtr hwnd = new WindowInteropHelper(window).Handle;
                // 获取当前窗口的扩展样式
                int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                // 设置窗口为工具窗口 (不会出现在Alt+Tab列表和任务栏)
                SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TOOLWINDOW);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"隐藏窗口从Alt+Tab列表失败: {ex.Message}");
            }
        }
    }
}
