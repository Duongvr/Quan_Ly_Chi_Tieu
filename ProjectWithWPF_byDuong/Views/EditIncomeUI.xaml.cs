using System;
using System.Linq;
using System.Windows;
using ProjectWithWPF_byDuong.Models;
using ProjectWithWPF_byDuong.Helpers;

namespace ProjectWithWPF_byDuong
{
    public partial class EditIncomeUI : Window
    {
        ExpenseManagementDbContext db = new ExpenseManagementDbContext();
        private Income currentIncome;

        public EditIncomeUI(Income income)
        {
            InitializeComponent();
            currentIncome = income;
            LoadData();
        }

        void LoadData()
        {
            try
            {
                // Load categories
                var categories = db.Categories
                    .Where(c => c.Type == "Income")
                    .OrderBy(c => c.CategoryName)
                    .ToList();
                
                cbCategory.ItemsSource = categories;
                cbCategory.SelectedValue = currentIncome.CategoryId;
                cbCategory.SelectedIndex = 0;
                // Load income data
                txtAmount.Text = currentIncome.Amount.ToString();
                txtNote.Text = currentIncome.Note ?? "";
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

                currentIncome.Amount = amount;
                currentIncome.CategoryId = selectedCategory.CategoryId;
                currentIncome.Note = string.IsNullOrWhiteSpace(txtNote.Text) ? null : txtNote.Text.Trim();

                db.Incomes.Update(currentIncome);
                db.SaveChanges();

                NotificationHelper.ShowSuccess("Cập nhật thu nhập thành công!");
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
