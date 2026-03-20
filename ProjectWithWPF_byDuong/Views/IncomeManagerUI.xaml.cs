using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ProjectWithWPF_byDuong.Helpers;
using ProjectWithWPF_byDuong.Models;
using ProjectWithWPF_byDuong.Views;

namespace ProjectWithWPF_byDuong
{
    public partial class IncomeManagerUI : UserControl
    {
        ExpenseManagementDbContext db = new ExpenseManagementDbContext();
        private const string SearchPlaceholder = "🔍 Tìm kiếm theo danh mục hoặc ghi chú...";

        public IncomeManagerUI()
        {
            InitializeComponent();
            PlaceholderHelper.SetupPlaceholder(txtSearch, SearchPlaceholder);
            
            try
            {
                LoadIncome();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        void LoadIncome()
        {
            if (!AppContext.IsLoggedIn) return;

            var incomes = db.Incomes
                .Include(x => x.Category)
                .Where(x => x.UserId == AppContext.CurrentUserId)
                .OrderByDescending(x => x.Date)
                .ToList();

            var list = incomes.Select((x, index) => new
            {
                STT = index + 1,
                x.IncomeId,
                x.Amount,
                Date = x.Date.ToString("dd/MM/yyyy"),
                x.Note,
                CategoryName = x.Category != null ? x.Category.CategoryName : ""
            })
            .ToList();

            incomeGrid.ItemsSource = list;

            decimal total = db.Incomes
                .Where(i => i.UserId == AppContext.CurrentUserId)
                .Sum(i => (decimal?)i.Amount) ?? 0;

            txtTotalIncome.Text = "Tổng thu nhập: " + total.ToString("N0") + " đ";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!AppContext.IsLoggedIn) return;

            string keyword = PlaceholderHelper.GetRealText(txtSearch, SearchPlaceholder).ToLower();

            var query = db.Incomes
                .Include(i => i.Category)
                .Where(i => i.UserId == AppContext.CurrentUserId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(i => 
                    (i.Note != null && i.Note.ToLower().Contains(keyword)) ||
                    (i.Category != null && i.Category.CategoryName != null && i.Category.CategoryName.ToLower().Contains(keyword)));
            }

            var incomes = query
                .OrderByDescending(i => i.Date)
                .ToList();

            var list = incomes.Select((i, index) => new
            {
                STT = index + 1,
                i.IncomeId,
                i.Amount,
                Date = i.Date.ToString("dd/MM/yyyy"),
                i.Note,
                CategoryName = i.Category != null ? i.Category.CategoryName : ""
            }).ToList();

            incomeGrid.ItemsSource = list;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (incomeGrid.SelectedItem == null)
            {
                NotificationHelper.ShowError("Vui lòng chọn thu nhập cần xóa!");
                return;
            }

            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa thu nhập này?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                dynamic row = incomeGrid.SelectedItem;
                var income = db.Incomes.Find((int)row.IncomeId);
                
                if (income == null)
                {
                    NotificationHelper.ShowError("Không tìm thấy thu nhập!");
                    return;
                }

                db.Incomes.Remove(income);
                db.SaveChanges();
                
                NotificationHelper.ShowSuccess("Xóa thu nhập thành công!");
                LoadIncome();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError($"Lỗi xóa thu nhập: {ex.Message}");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (incomeGrid.SelectedItem == null)
            {
                NotificationHelper.ShowError("Vui lòng chọn thu nhập cần sửa!");
                return;
            }

            try
            {
                dynamic row = incomeGrid.SelectedItem;
                int incomeId = (int)row.IncomeId;
                
                var income = db.Incomes.Find(incomeId);
                if (income == null)
                {
                    NotificationHelper.ShowError("Không tìm thấy thu nhập!");
                    return;
                }

                var editForm = new EditIncomeUI(income);
                editForm.Owner = Window.GetWindow(this);
                if (editForm.ShowDialog() == true)
                {
                    LoadIncome();
                }
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new DashboardUI());
        }

        private void BtnAddIncome_Click(object sender, RoutedEventArgs e)
        {
            var form = new AddIncomeUI();
            form.Owner = Window.GetWindow(this);
            if (form.ShowDialog() == true)
            {
                LoadIncome();
            }
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            // Handled by PlaceholderHelper
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            // Handled by PlaceholderHelper
        }

        private void incomeGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEdit.IsEnabled = incomeGrid.SelectedItem != null;
            btnDelete.IsEnabled = incomeGrid.SelectedItem != null;
        }
    }
}