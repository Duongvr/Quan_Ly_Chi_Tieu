using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProjectWithWPF_byDuong.Helpers
{
    /// <summary>
    /// Helper class để xử lý placeholder text cho TextBox
    /// </summary>
    public static class PlaceholderHelper
    {
        private static readonly Brush PlaceholderBrush = new SolidColorBrush(Color.FromRgb(156, 163, 175));
        private static readonly Brush NormalBrush = new SolidColorBrush(Color.FromRgb(31, 41, 55));

        /// <summary>
        /// Setup placeholder cho TextBox
        /// </summary>
        public static void SetupPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.Foreground = PlaceholderBrush;

            textBox.GotFocus += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = string.Empty;
                    textBox.Foreground = NormalBrush;
                }
            };

            textBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.Foreground = PlaceholderBrush;
                }
            };
        }

        /// <summary>
        /// Lấy text thực (không bao gồm placeholder)
        /// </summary>
        public static string GetRealText(TextBox textBox, string placeholder)
        {
            if (textBox.Text == placeholder)
                return string.Empty;
            return textBox.Text?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Check xem TextBox có đang hiển thị placeholder không
        /// </summary>
        public static bool IsShowingPlaceholder(TextBox textBox, string placeholder)
        {
            return textBox.Text == placeholder;
        }
    }
}
