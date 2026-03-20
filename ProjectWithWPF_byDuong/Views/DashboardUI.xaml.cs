using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProjectWithWPF_byDuong.Models;
using ProjectWithWPF_byDuong.Views;

namespace ProjectWithWPF_byDuong
{
    public partial class DashboardUI : UserControl
    {
        ExpenseManagementDbContext db = new ExpenseManagementDbContext();

        public DashboardUI()
        {
            InitializeComponent();
            if (!AppContext.IsLoggedIn)
            {
                MainWindow.Instance.Navigate(new LoginUI());
                return;
            }
            if (!string.IsNullOrEmpty(AppContext.CurrentUserEmail))
                txtWelcomeUser.Text = "Đăng nhập: " + AppContext.CurrentUserEmail;
            txtWelcomeUser.Visibility = Visibility.Visible;
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void LoadData()
        {
            if (!AppContext.IsLoggedIn) return;

            decimal totalIncome = db.Incomes
                .Where(i => i.UserId == AppContext.CurrentUserId)
                .Sum(i => (decimal?)i.Amount) ?? 0;
            decimal totalExpense = db.Expenses
                .Where(e => e.UserId == AppContext.CurrentUserId)
                .Sum(e => (decimal?)e.Amount) ?? 0;

            decimal balance = totalIncome - totalExpense;

            txtIncome.Text = "+ " + totalIncome.ToString("N0") + " đ";
            txtExpense.Text = "- " + totalExpense.ToString("N0") + " đ";
            txtBalance.Text = balance.ToString("N0") + " đ";
        }

        void OpenIncome(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try { MainWindow.Instance.Navigate(new IncomeManagerUI()); }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void OpenExpense(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try { MainWindow.Instance.Navigate(new ExpenseManagerUI()); }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void OpenStatistic(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try { MainWindow.Instance.Navigate(new Views.StatisticsUI()); }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void OpenReport(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try { MainWindow.Instance.Navigate(new ExportReportUI()); }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            AppContext.Logout();
            MainWindow.Instance.Navigate(new LoginUI());
        }
    }
}