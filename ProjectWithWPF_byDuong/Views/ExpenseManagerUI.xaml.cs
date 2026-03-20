using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ProjectWithWPF_byDuong.Models;
using ProjectWithWPF_byDuong.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace ProjectWithWPF_byDuong
{
    public partial class ExpenseManagerUI : UserControl
    {
        ExpenseManagementDbContext db = new ExpenseManagementDbContext();
        private const string SearchPlaceholder = "🔍 Tìm kiếm theo danh mục hoặc ghi chú...";

        public ExpenseManagerUI()
        {
            InitializeComponent();
            txtSearch.TextChanged -= TxtSearch_TextChanged; // if wired in XAML
            txtSearch.TextChanged += TxtSearch_TextChanged; // ensure attached after init
            PlaceholderHelper.SetupPlaceholder(txtSearch, SearchPlaceholder);
            LoadExpenses();
        }

        void LoadExpenses()
{
    if (!AppContext.IsLoggedIn) return;

    using (var db = new ExpenseManagementDbContext())
    {
        // Lấy danh sách chi tiêu, nếu không có thì trả về list rỗng
        var expenses = db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == AppContext.CurrentUserId)
            .OrderByDescending(e => e.Date)
            .ToList() ?? new List<Expense>();

        // Chuyển sang list hiển thị
        var list = expenses.Select((e, index) => new
        {
            Index = index + 1,
            e.ExpenseId,
            CategoryName = e.Category?.CategoryName ?? string.Empty,
            e.Amount,
            Date = e.Date.ToString("dd/MM/yyyy"),
            e.Note
        }).ToList();

        // Kiểm tra dgExpense có null không trước khi gán
        if (dgExpense != null)
        {
            dgExpense.ItemsSource = list;
        }

        // Tính tổng chi tiêu
        decimal total = db.Expenses
            .Where(e => e.UserId == AppContext.CurrentUserId)
            .Sum(e => (decimal?)e.Amount) ?? 0;

        if (txtTotalExpense != null)
        {
            txtTotalExpense.Text = "Tổng chi tiêu: " + total.ToString("N0") + " đ";
        }
    }
}


        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!AppContext.IsLoggedIn) return;
            if (dgExpense == null) return; // guard

            string keyword = PlaceholderHelper.GetRealText(txtSearch, SearchPlaceholder).ToLower();

            var query = db.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == AppContext.CurrentUserId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(e => 
                    (e.Note != null && e.Note.ToLower().Contains(keyword)) ||
                    (e.Category != null && e.Category.CategoryName != null && e.Category.CategoryName.ToLower().Contains(keyword)));
            }

            var expenses = query
                .OrderByDescending(e => e.Date)
                .ToList();

            var list = expenses.Select((e, index) => new
            {
                Index = index + 1,
                e.ExpenseId,
                CategoryName = e.Category != null ? e.Category.CategoryName : "",
                e.Amount,
                Date = e.Date.ToString("dd/MM/yyyy"),
                e.Note
            }).ToList();
            try
            {
                if (!list.IsNullOrEmpty())
                    dgExpense.ItemsSource = list;
            } catch (Exception ex)
            {
                MessageBox.Show("Lỗi truy cập quản lý chi tiêu");
            }
            
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgExpense.SelectedItem == null)
            {
                NotificationHelper.ShowError("Vui lòng chọn chi tiêu cần xóa!");
                return;
            }

            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa chi tiêu này?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                dynamic row = dgExpense.SelectedItem;
                var expense = db.Expenses.Find((int)row.ExpenseId);
                
                if (expense == null)
                {
                    NotificationHelper.ShowError("Không tìm thấy chi tiêu!");
                    return;
                }

                db.Expenses.Remove(expense);
                db.SaveChanges();
                
                NotificationHelper.ShowSuccess("Xóa chi tiêu thành công!");
                LoadExpenses();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError($"Lỗi xóa chi tiêu: {ex.Message}");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgExpense.SelectedItem == null)
            {
                NotificationHelper.ShowError("Vui lòng chọn chi tiêu cần sửa!");
                return;
            }

            try
            {
                dynamic row = dgExpense.SelectedItem;
                int expenseId = (int)row.ExpenseId;
                
                var expense = db.Expenses.Find(expenseId);
                if (expense == null)
                {
                    NotificationHelper.ShowError("Không tìm thấy chi tiêu!");
                    return;
                }

                var editForm = new EditExpenseUI(expense);
                editForm.Owner = Window.GetWindow(this);
                if (editForm.ShowDialog() == true)
                {
                    LoadExpenses();
                }
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            var form = new AddExpenseUI();
            form.Owner = Window.GetWindow(this);
            if (form.ShowDialog() == true)
            {
                LoadExpenses();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new DashboardUI());
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            // Handled by PlaceholderHelper
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            // Handled by PlaceholderHelper
        }

        private void dgExpense_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable/disable edit button based on selection
            btnEdit.IsEnabled = dgExpense.SelectedItem != null;
            btnDelete.IsEnabled = dgExpense.SelectedItem != null;
        }
    }
}
