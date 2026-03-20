using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ProjectWithWPF_byDuong;
using ProjectWithWPF_byDuong.Models;

namespace ProjectWithWPF_byDuong.Views
{
    public partial class RegisterUI : UserControl
    {
        public RegisterUI()
        {
            InitializeComponent();
        }

        private void ShowSuccessNotification(string message)
        {
            txtSuccessMessage.Text = message;
            successNotification.Visibility = Visibility.Visible;

            var timer = new DispatcherTimer
            {
                Interval = System.TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, e) =>
            {
                successNotification.Visibility = Visibility.Collapsed;
                timer.Stop();
                MainWindow.Instance.Navigate(new LoginUI());
            };
            timer.Start();
        }

        private void DangKy_Click(object sender, RoutedEventArgs e)
        {
            string name = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            if (name == "" || email == "" || password == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!email.Contains("@"))
            {
                MessageBox.Show("Email không hợp lệ!",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải >= 6 ký tự!",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            using (var db = new ExpenseManagementDbContext())
            {
                var existUser = db.Users.FirstOrDefault(u => u.Email == email);

                if (existUser != null)
                {
                    MessageBox.Show("Email đã được đăng ký!",
                        "Thông báo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                User newUser = new User
                {
                    Email = email,
                    Password = password
                };

                db.Users.Add(newUser);
                db.SaveChanges();
            }

            ShowSuccessNotification("Đăng ký thành công! Đang chuyển đến trang đăng nhập...");
        }

        private void GoLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow.Instance.Navigate(new LoginUI());
        }
    }
}