using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging; // Thêm thư viện này để load ảnh từ URL
using ProjectWithWPF_byDuong.Models;

namespace ProjectWithWPF_byDuong.Views
{
    public partial class StatisticsUI : UserControl
    {
        ExpenseManagementDbContext db = new ExpenseManagementDbContext();

        public StatisticsUI()
        {
            InitializeComponent();
            try
            {
                LoadStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thống kê: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void LoadStatistics()
        {
            if (!AppContext.IsLoggedIn) return;

            // 1. Tính toán tổng
            decimal totalIncome = db.Incomes.Where(i => i.UserId == AppContext.CurrentUserId).Sum(i => (decimal?)i.Amount) ?? 0;
            decimal totalExpense = db.Expenses.Where(e => e.UserId == AppContext.CurrentUserId).Sum(e => (decimal?)e.Amount) ?? 0;
            decimal balance = totalIncome - totalExpense;

            txtTotalIncome.Text = totalIncome.ToString("N0") + " đ";
            txtTotalExpense.Text = totalExpense.ToString("N0") + " đ";
            txtBalance.Text = balance.ToString("N0") + " đ";
            txtBalance.Foreground = balance < 0 ? Brushes.Red : Brushes.Green;

            // 2. Tính danh mục Chi tiêu lớn nhất
            var topExpense = db.Expenses
                               .Where(e => e.UserId == AppContext.CurrentUserId)
                               .GroupBy(e => e.Category) // Sửa lại 'Category' nếu tên cột của bạn khác
                               .Select(g => new { Name = g.Key, Total = g.Sum(e => e.Amount) })
                               .OrderByDescending(x => x.Total)
                               .FirstOrDefault();

            if (topExpense != null && totalExpense > 0)
            {
                txtTopExpenseName.Text = topExpense.Name.CategoryName;
                txtExpenseCategory.Text = topExpense.Total.ToString("N0") + " đ";
                progressExpense.Value = (double)((topExpense.Total / totalExpense) * 100);
            }

            // 3. Tính danh mục Thu nhập lớn nhất
            var topIncome = db.Incomes
                              .Where(i => i.UserId == AppContext.CurrentUserId)
                              .GroupBy(i => i.Category) // Sửa lại 'Category' nếu tên cột của bạn khác
                              .Select(g => new { Name = g.Key, Total = g.Sum(i => i.Amount) })
                              .OrderByDescending(x => x.Total)
                              .FirstOrDefault();

            if (topIncome != null && totalIncome > 0)
            {
                txtTopIncomeName.Text = topIncome.Name.CategoryName;
                txtIncomeCategory.Text = topIncome.Total.ToString("N0") + " đ";
                progressIncome.Value = (double)((topIncome.Total / totalIncome) * 100);
            }

            // 4. Gọi hàm vẽ biểu đồ
            LoadChartFromAPI(totalIncome, totalExpense);
        }

        private void LoadChartFromAPI(decimal income, decimal expense)
        {
            try
            {
                // Tránh lỗi hiển thị nếu cả thu và chi đều chưa có dữ liệu
                if (income == 0 && expense == 0) return;

                // Sử dụng InvariantCulture để đảm bảo dấu thập phân chuẩn trong JSON, tránh lỗi API
                string incomeStr = income.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string expenseStr = expense.ToString(System.Globalization.CultureInfo.InvariantCulture);

                // Cấu hình chuỗi JSON cho QuickChart
                string chartConfig = $@"{{
                    type: 'doughnut',
                    data: {{
                        labels: ['Thu nhập', 'Chi tiêu'],
                        datasets: [{{
                            data: [{incomeStr}, {expenseStr}],
                            backgroundColor: ['rgb(34, 197, 94)', 'rgb(239, 68, 68)']
                        }}]
                    }},
                    options: {{
                        plugins: {{
                            datalabels: {{
                                color: '#fff',
                                font: {{ weight: 'bold', size: 14 }}
                            }}
                        }}
                    }}
                }}";

                // Encode chuỗi cấu hình thành URL (Thêm &w=400&h=250 để giới hạn kích thước ảnh tải về)
                string apiUrl = "https://quickchart.io/chart?c=" + Uri.EscapeDataString(chartConfig) + "&w=400&h=250";

                // Hiển thị hình ảnh từ URL lên thẻ Image (ApiChartImage) trong XAML
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(apiUrl, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Đảm bảo tải ảnh hoàn tất trước khi hiển thị
                bitmap.EndInit();

                ApiChartImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tải biểu đồ: " + ex.Message);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new DashboardUI());
        }

    }
}