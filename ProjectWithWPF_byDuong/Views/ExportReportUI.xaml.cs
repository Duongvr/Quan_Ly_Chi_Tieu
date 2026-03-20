using System;
using System.Linq;
using ClosedXML.Excel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectWithWPF_byDuong.Models;

namespace ProjectWithWPF_byDuong.Views
{
    // Class phụ để gom chung Thu nhập và Chi tiêu
    public class TransactionItem
    {
        public DateOnly Date { get; set; }
        public string Type { get; set; }
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public string Note { get; set; }
    }

    public partial class ExportReportUI : UserControl
    {
        ExpenseManagementDbContext db = new ExpenseManagementDbContext();

        public ExportReportUI()
        {
            InitializeComponent();
            try
            {
                // Mặc định chọn 1 tháng gần nhất
                dpStartDate.SelectedDate = DateTime.Now.AddMonths(-1);
                dpEndDate.SelectedDate = DateTime.Now;

                LoadHistory(); // Tải dữ liệu ngay khi mở
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải giao diện: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadHistory()
        {
            if (!AppContext.IsLoggedIn) return;
            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null) return;

            DateOnly startDate = DateOnly.FromDateTime(dpStartDate.SelectedDate.Value);
            DateOnly endDate = DateOnly.FromDateTime(dpEndDate.SelectedDate.Value);

            // Lấy Thu nhập
            var incomes = db.Incomes
                .Where(i => i.UserId == AppContext.CurrentUserId && i.Date >= startDate && i.Date <= endDate)
                .Select(i => new TransactionItem
                {
                    Date = i.Date,
                    Type = "Thu nhập",
                    // Tránh lỗi null reference nếu giao dịch không có danh mục
                    CategoryName = i.Category != null ? i.Category.CategoryName : "Khác",
                    Amount = (decimal)i.Amount,
                    Note = i.Note
                }).ToList();

            // Lấy Chi tiêu
            var expenses = db.Expenses
                .Where(e => e.UserId == AppContext.CurrentUserId && e.Date >= startDate && e.Date <= endDate)
                .Select(e => new TransactionItem
                {
                    Date = e.Date,
                    Type = "Chi tiêu",
                    CategoryName = e.Category != null ? e.Category.CategoryName : "Khác",
                    Amount = (decimal)e.Amount,
                    Note = e.Note
                }).ToList();

            // Gộp lại và sắp xếp: Giao dịch mới nhất lên đầu
            var historyList = incomes.Concat(expenses).OrderByDescending(x => x.Date).ToList();

            // Đổ vào DataGrid
            dgHistory.ItemsSource = historyList;
        }

        private void BtnViewHistory_Click(object sender, RoutedEventArgs e)
        {
            LoadHistory();
        }

        private void BtnExportReport_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem DataGrid có dữ liệu không
            if (dgHistory.ItemsSource is not List<TransactionItem> data || data.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất trong khoảng thời gian này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mở hộp thoại lưu file (Đổi đuôi thành .xlsx)
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"LichSuGiaoDich_{DateTime.Now:ddMMyyyy}.xlsx"
            };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    // Bắt đầu tạo file Excel bằng ClosedXML
                    using (var workbook = new XLWorkbook())
                    {
                        // Tạo một Sheet có tên là "Lịch sử giao dịch"
                        var worksheet = workbook.Worksheets.Add("Lịch sử giao dịch");

                        // 1. TẠO DÒNG TIÊU ĐỀ (Dòng số 1)
                        worksheet.Cell(1, 1).Value = "Ngày";
                        worksheet.Cell(1, 2).Value = "Loại";
                        worksheet.Cell(1, 3).Value = "Danh mục";
                        worksheet.Cell(1, 4).Value = "Số tiền";
                        worksheet.Cell(1, 5).Value = "Ghi chú";

                        // Trang trí Tiêu đề: In đậm, nền xám nhạt, căn giữa
                        var headerRange = worksheet.Range("A1:E1");
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        // 2. ĐỔ DỮ LIỆU TỪ BẢNG VÀO (Bắt đầu từ dòng số 2)
                        int row = 2;
                        foreach (var item in data)
                        {
                            worksheet.Cell(row, 1).Value = item.Date.ToString("dd/MM/yyyy");
                            worksheet.Cell(row, 2).Value = item.Type;
                            worksheet.Cell(row, 3).Value = item.CategoryName;

                            // Điền số tiền và format có dấu phẩy phân cách hàng nghìn (VD: 1,000,000)
                            worksheet.Cell(row, 4).Value = item.Amount;
                            worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0";

                            worksheet.Cell(row, 5).Value = item.Note;

                            row++;
                        }

                        // 3. AUTO-FIT: Tự động giãn độ rộng tất cả các cột cho vừa khít với chữ
                        worksheet.Columns().AdjustToContents();

                        // 4. Lưu file lại
                        workbook.SaveAs(sfd.FileName);
                    }

                    MessageBox.Show("Xuất báo cáo Excel thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra khi xuất file (Lưu ý: Nếu file Excel trùng tên đang mở, hãy đóng nó lại trước nhé): \n" + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Navigate(new DashboardUI());
        }
    }
}