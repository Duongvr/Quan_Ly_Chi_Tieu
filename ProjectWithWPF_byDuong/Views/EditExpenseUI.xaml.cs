using System;
using System.Linq;
using System.Windows;
using ProjectWithWPF_byDuong.Models;
using ProjectWithWPF_byDuong.Helpers;

namespace ProjectWithWPF_byDuong
{
    public partial class EditExpenseUI : Window
    {
        ExpenseManagementDbContext db = new ExpenseManagementDbContext();
        private Expense currentExpense;

        public EditExpenseUI(Expense expense)
        {
            InitializeComponent();
            currentExpense = expense;
            LoadData();
        }

        void LoadData()
        {
            try
            {
                // Load categories
                var categories = db.Categories
                    .Where(c => c.Type == "Expense")
                    .OrderBy(c => c.CategoryName)
                    .ToList();
                
                cbCategory.ItemsSource = categories;
                cbCategory.SelectedValue = currentExpense.CategoryId;
                cbCategory.SelectedIndex = 0;
                // Load expense data
                txtAmount.Text = currentExpense.Amount.ToString();
                txtNote.Text = currentExpense.Note ?? "";
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAmount.Text))
            {
                NotificationHelper.ShowError("Vui lòng nhập số tiền!");
                return;
            }

            if (cbCategory.SelectedValue == null)
            {
                NotificationHelper.ShowError("Vui lòng chọn danh mục!");
                return;
            }

            try
            {
                if (!decimal.TryParse(txtAmount.Text.Trim(), out decimal amount) || amount <= 0)
                {
                    NotificationHelper.ShowError("Số tiền phải lớn hơn 0!");
                    return;
                }

                var selectedCategory = cbCategory.SelectedItem as Category;
                if (selectedCategory == null)
                {
                    NotificationHelper.ShowError("Danh mục không hợp lệ!");
                    return;
                }

                currentExpense.Amount = amount;
                currentExpense.CategoryId = selectedCategory.CategoryId;
                currentExpense.Note = string.IsNullOrWhiteSpace(txtNote.Text) ? null : txtNote.Text.Trim();

                db.Expenses.Update(currentExpense);
                db.SaveChanges();

                NotificationHelper.ShowSuccess("Cập nhật chi tiêu thành công!");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
