using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProjectWithWPF_byDuong.Models;
using ProjectWithWPF_byDuong; // for LoginUI, MainWindow

namespace ProjectWithWPF_byDuong.Views
{
    public partial class ForgotPasswordUI : UserControl
    {
        ExpenseManagementDbContext db = new ExpenseManagementDbContext();

        public ForgotPasswordUI()
        {
            InitializeComponent();
        }

        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string newPassword = txtNewPassword.Password.Trim();

            if (string.IsNullOrEmpty(email) || email == "Nhập email đã đăng ký..." ||
                string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            var user = db.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                MessageBox.Show("Email không tồn tại trong hệ thống!");
                return;
            }

            user.Password = newPassword;

            db.SaveChanges();

            MessageBox.Show("Đổi mật khẩu thành công!");
            MainWindow.Instance.Navigate(new LoginUI());
        }

        private void BtnBackLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Instance.Navigate(new LoginUI());
        }

        private void TxtEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtEmail.Text == "Nhập email đã đăng ký...")
                txtEmail.Text = "";
        }

        private void TxtEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtEmail.Text == "")
                txtEmail.Text = "Nhập email đã đăng ký...";
        }
    }
}