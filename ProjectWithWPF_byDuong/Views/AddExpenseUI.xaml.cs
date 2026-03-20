using System;
using System.Linq;
using System.Windows;
using ProjectWithWPF_byDuong.Models;
using ProjectWithWPF_byDuong.Helpers;

namespace ProjectWithWPF_byDuong
{
    public partial class AddExpenseUI : Window
    {
        ExpenseManagementDbContext db = new ExpenseManagementDbContext();
        private const string AmountPlaceholder = "Nhập số tiền...";
        private const string NotePlaceholder = "Nhập ghi chú...";

        public AddExpenseUI()
        {
            InitializeComponent();
            LoadCategories();
            PlaceholderHelper.SetupPlaceholder(txtAmount, AmountPlaceholder);
            PlaceholderHelper.SetupPlaceholder(txtNote, NotePlaceholder);
        }

        void LoadCategories()
        {
            try
            {
                var categories = db.Categories
                    .Where(c => c.Type == "Expense")
                    .OrderBy(c => c.CategoryName)
                    .ToList();
                
                cbCategory.ItemsSource = categories;
                
                if (categories.Count > 0)
                {
                    cbCategory.SelectedIndex = 0;
                }
                else
                {
                    NotificationHelper.ShowError("Không có danh mục chi tiêu nào!");
                }
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError($"Lỗi tải danh mục: {ex.Message}");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            if (!AppContext.IsLoggedIn)
            {
                NotificationHelper.ShowError("Bạn cần đăng nhập để thêm chi tiêu.");
                return;
            }

            string amountText = PlaceholderHelper.GetRealText(txtAmount, AmountPlaceholder);
            
            if (!ValidationHelper.IsValidAmount(amountText, out decimal amount))
            {
                NotificationHelper.ShowError("Vui lòng nhập số tiền hợp lệ (lớn hơn 0)!");
                txtAmount.Focus();
                return;
            }

            if (cbCategory.SelectedValue == null || cbCategory.SelectedItem == null)
            {
                NotificationHelper.ShowError("Vui lòng chọn danh mục!");
                return;
            }

            try
            {
                var selectedCategory = cbCategory.SelectedItem as Category;
                if (selectedCategory == null)
                {
                    NotificationHelper.ShowError("Danh mục không hợp lệ!");
                    return;
                }
                
                int categoryId = selectedCategory.CategoryId;
                string? note = PlaceholderHelper.GetRealText(txtNote, NotePlaceholder);
                if (string.IsNullOrEmpty(note)) note = null;

                var expense = new Expense
                {
                    UserId = AppContext.CurrentUserId,
                    CategoryId = categoryId,
                    Amount = amount,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Note = note,
                    CreatedAt = DateTime.Now
                };

                db.Expenses.Add(expense);
                db.SaveChanges();

                NotificationHelper.ShowSuccess("Thêm chi tiêu thành công!");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void TxtAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            // Handled by PlaceholderHelper
        }

        private void TxtAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            // Handled by PlaceholderHelper
        }

        private void TxtNote_GotFocus(object sender, RoutedEventArgs e)
        {
            // Handled by PlaceholderHelper
        }

        private void TxtNote_LostFocus(object sender, RoutedEventArgs e)
        {
            // Handled by PlaceholderHelper
        }
    }
}