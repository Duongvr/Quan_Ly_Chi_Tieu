namespace ProjectWithWPF_byDuong;

/// <summary>
/// Lưu thông tin user đang đăng nhập (dùng sau khi Login).
/// </summary>
public static class AppContext
{
    public static int CurrentUserId { get; set; }
    public static string? CurrentUserEmail { get; set; }

    public static bool IsLoggedIn => CurrentUserId > 0;

    public static void Logout()
    {
        CurrentUserId = 0;
        CurrentUserEmail = null;
        Properties.Settings.Default.SavedUsername = "";
        Properties.Settings.Default.SavedPassword = "";
        Properties.Settings.Default.IsRemember = false;
    }
}
