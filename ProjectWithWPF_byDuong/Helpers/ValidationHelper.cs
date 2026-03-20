using System;
using System.Text.RegularExpressions;

namespace ProjectWithWPF_byDuong.Helpers
{
    /// <summary>
    /// Helper class để validate input
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validate email format
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate số tiền (phải > 0)
        /// </summary>
        public static bool IsValidAmount(string amountText, out decimal amount)
        {
            amount = 0;
            
            if (string.IsNullOrWhiteSpace(amountText))
                return false;

            // Loại bỏ dấu phẩy
            amountText = amountText.Replace(",", "").Trim();

            if (!decimal.TryParse(amountText, out amount))
                return false;

            return amount > 0;
        }

        /// <summary>
        /// Validate mật khẩu (tối thiểu 6 ký tự)
        /// </summary>
        public static bool IsValidPassword(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Mật khẩu không được để trống";
                return false;
            }

            if (password.Length < 6)
            {
                errorMessage = "Mật khẩu phải có ít nhất 6 ký tự";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check xem text có phải là placeholder không
        /// </summary>
        public static bool IsPlaceholder(string text, string placeholder)
        {
            return string.IsNullOrWhiteSpace(text) || text.Trim() == placeholder.Trim();
        }
    }
}
