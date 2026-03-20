using System;
using System.Linq;
using System.Windows;
using ProjectWithWPF_byDuong.Models;
using ProjectWithWPF_byDuong.Helpers;

namespace ProjectWithWPF_byDuong
{
    public partial class AddIncomeUI : Window
    {
        ExpenseManagementDbContext db = new ExpenseManagementDbContext();

        public AddIncomeUI()
        {
            InitializeComponent();
            LoadCategories();
        }

        void LoadCategories()
        {
            try
            {
                var categories = db.Categories
                    .Where(c => c.Type == "Income")
                    .OrderBy(c => c.CategoryName)
                    .ToList();
                
                cbCategory.ItemsSource = categories;
                
                if (categories.Count > 0)
                {
                    cbCategory.SelectedIndex = 0;
                }
                else
                {
                    NotificationHelper.ShowError("Không có danh mục thu nhập nào!");
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

        private void BtnAddIncome_Click(object sender, RoutedEventArgs e)
        {
            if (!AppContext.IsLoggedIn)
            {
                NotificationHelper.ShowError("Bạn cần đăng nhập để thêm thu nhập.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAmount.Text) || txtAmount.Text == "Nhập số tiền...")
            {
                NotificationHelper.ShowError("Vui lòng nhập số tiền!");
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

                decimal amount = decimal.Parse(txtAmount.Text.Trim().Replace(",", ""));
                int categoryId = selectedCategory.CategoryId;
                string? note = txtNote.Text?.Trim();
                if (note == "Nhập ghi chú...") note = null;

                var income = new Income
                {
                    UserId = AppContext.CurrentUserId,
                    CategoryId = categoryId,
                    Amount = amount,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Note = string.IsNullOrEmpty(note) ? null : note,
                    CreatedAt = DateTime.Now
                };

                db.Incomes.Add(income);
                db.SaveChanges();

                NotificationHelper.ShowSuccess("Thêm thu nhập thành công!");
                DialogResult = true;
                Close();
            }
            catch (FormatException)
            {
                NotificationHelper.ShowError("Số tiền không hợp lệ!");
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError("Lỗi: " + ex.Message);
            }
        }

        private void TxtAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtAmount.Text == "Nhập số tiền...")
                txtAmount.Text = "";
        }

        private void TxtAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAmount.Text))
                txtAmount.Text = "Nhập số tiền...";
        }

        private void TxtNote_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtNote.Text == "Nhập ghi chú...")
                txtNote.Text = "";
        }

        private void TxtNote_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNote.Text))
                txtNote.Text = "Nhập ghi chú...";
        }
    }
}
