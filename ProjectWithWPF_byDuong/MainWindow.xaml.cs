using System.Windows;
using ProjectWithWPF_byDuong.Views;

namespace ProjectWithWPF_byDuong
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance = null!;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            //Trang mặc định khi mở app: màn hình Đăng nhập
            Navigate(new LoginUI());
        }

        //Hàm chuyển trang
        public void Navigate(object page)
        {
            MainContent.Content = page;
        }
    }
}