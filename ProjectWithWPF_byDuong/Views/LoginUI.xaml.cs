using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ProjectWithWPF_byDuong.Helpers;
using ProjectWithWPF_byDuong.Models;
using ProjectWithWPF_byDuong.Views;

namespace ProjectWithWPF_byDuong
{
    public partial class LoginUI : UserControl
    {
        public LoginUI()
        {
            InitializeComponent();
            LoadRememberedAccount();
        }

        // ================= ERRORS =================
        private void ClearErrors()
        {
            lblEmailError.Visibility = Visibility.Collapsed;
            lblEmailError.Text = "";
            lblPasswordError.Visibility = Visibility.Collapsed;
            lblPasswordError.Text = "";
        }

        private void ShowEmailError(string message)
        {
            lblEmailError.Text = message;
            lblEmailError.Visibility = Visibility.Visible;
        }

        private void ShowPasswordError(string message)
        {
            lblPasswordError.Text = message;
            lblPasswordError.Visibility = Visibility.Visible;
        }

        private bool ValidateLogin(string email, string password)
        {
            ClearErrors();

            bool hasError = false;

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowEmailError("Vui lòng nhập email");
                hasError = true;
            }
            else if (!ValidationHelper.IsValidEmail(email))
            {
                ShowEmailError("Email không hợp lệ");
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowPasswordError("Vui lòng nhập mật khẩu");
                hasError = true;
            }

            return !hasError;
        }

        // ================= ENCRYPT =================
        private string Encrypt(string plainText)
        {
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        private string Decrypt(string encryptedText)
        {
            try
            {
                byte[] data = Convert.FromBase64String(encryptedText);
                byte[] decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch
            {
                return "";
            }
        }

        // ================= REMEMBER =================
        private void LoadRememberedAccount()
        {
            if (Properties.Settings.Default.IsRemember)
            {
                txtEmail.Text = Properties.Settings.Default.SavedUsername;
                txtPassword.Password = Decrypt(Properties.Settings.Default.SavedPassword);
                chkRemember.IsChecked = true;
                DangNhap_click(null, null);
            }
        }

        private void SaveRememberAccount(string email, string password)
        {
            if (chkRemember.IsChecked == true)
            {
                Properties.Settings.Default.SavedUsername = email;
                Properties.Settings.Default.SavedPassword = Encrypt(password);
                Properties.Settings.Default.IsRemember = true;
            }
            else
            {
                Properties.Settings.Default.SavedUsername = "";
                Properties.Settings.Default.SavedPassword = "";
                Properties.Settings.Default.IsRemember = false;
            }

            Properties.Settings.Default.Save();
        }

        // ================= LOGIN =================
        private async void DangNhap_click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            if (!ValidateLogin(email, password))
                return;

            try
            {
                using (var db = new ExpenseManagementDbContext())
                {
                    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (user == null)
                    {
                        ShowEmailError("Email hoặc mật khẩu không đúng");
                        return;
                    }

                    bool isPasswordCorrect = false;

                    if (user.Password.Length < 64)
                    {
                        isPasswordCorrect = user.Password == password;
                        if (isPasswordCorrect)
                        {
                            user.Password = PasswordHelper.HashPassword(password);
                            await db.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        isPasswordCorrect = PasswordHelper.VerifyPassword(password, user.Password);
                    }

                    if (isPasswordCorrect)
                    {
                        AppContext.CurrentUserId = user.UserId;
                        AppContext.CurrentUserEmail = user.Email;
                        SaveRememberAccount(email, password);
                        NotificationHelper.ShowSuccess("Đăng nhập thành công!");
                        await Task.Delay(500);
                        MainWindow.Instance.Navigate(new DashboardUI());
                    }
                    else
                    {
                        ShowPasswordError("Email hoặc mật khẩu không đúng");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowEmailError($"Lỗi đăng nhập: {ex.Message}");
            }
        }

        // ================= NAVIGATION =================
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new RegisterUI());
        }

        private void BtnForgot_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new ForgotPasswordUI());
        }
    }
}