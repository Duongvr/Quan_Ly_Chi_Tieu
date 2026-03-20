using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ProjectWithWPF_byDuong.Helpers
{
    public static class NotificationHelper
    {
        public static void ShowSuccess(string message, double duration = 3)
        {
            ShowNotification(message, "#4CAF50", "✓", duration);
        }

        public static void ShowError(string message, double duration = 3)
        {
            ShowNotification(message, "#F44336", "✕", duration);
        }

        public static void ShowInfo(string message, double duration = 3)
        {
            ShowNotification(message, "#2196F3", "ℹ", duration);
        }

        private static void ShowNotification(string message, string color, string icon, double duration)
        {
            try
            {
                var mainWindow = Application.Current?.MainWindow;
                if (mainWindow == null)
                {
                    MessageBox.Show(message);
                    return;
                }

                // Tìm Grid chính trong MainWindow
                Grid grid = null;
                if (mainWindow.Content is Grid g)
                {
                    grid = g;
                }
                else if (mainWindow.Content is Border b && b.Child is Grid g2)
                {
                    grid = g2;
                }

                if (grid == null)
                {
                    MessageBox.Show(message);
                    return;
                }

                var border = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(20, 15, 20, 15),
                    Margin = new Thickness(0, 30, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top
                };

                var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

                var iconBlock = new TextBlock
                {
                    Text = icon,
                    Foreground = Brushes.White,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                var textBlock = new TextBlock
                {
                    Text = message,
                    Foreground = Brushes.White,
                    FontSize = 14,
                    FontWeight = FontWeights.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center
                };

                stackPanel.Children.Add(iconBlock);
                stackPanel.Children.Add(textBlock);
                border.Child = stackPanel;

                // Add to grid
                grid.Children.Add(border);
                Panel.SetZIndex(border, 1000);

                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(duration) };
                timer.Tick += (s, e) =>
                {
                    try
                    {
                        grid.Children.Remove(border);
                    }
                    catch { }
                    timer.Stop();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Notification error: {ex.Message}");
                try
                {
                    MessageBox.Show(message);
                }
                catch { }
            }
        }
    }
}
