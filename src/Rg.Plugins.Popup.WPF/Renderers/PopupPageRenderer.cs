using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Windows.Renderers;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.WPF;

[assembly: ExportRenderer(typeof(PopupPage), typeof(PopupPageRenderer))]
namespace Rg.Plugins.Popup.Windows.Renderers
{
    [Preserve(AllMembers = true)]
    public class PopupPageRenderer : PageRenderer
    {
        private System.Windows.Controls.Grid grid;

        private PopupPage CurrentElement => (PopupPage)Element;

        [Preserve]
        public PopupPageRenderer()
        { }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (string.Equals(e.PropertyName, Page.IsVisibleProperty.PropertyName, StringComparison.Ordinal))
            {
                Control.Visibility = CurrentElement.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        internal void Prepare()
        {
            grid = GetTopGridFromWindow();
            if (!grid.Children.Contains(Control))
            {
                grid.SizeChanged += OnGridSizeChanged;
                System.Windows.Controls.Panel.SetZIndex(Control, 10000);
                System.Windows.Controls.Grid.SetColumn(Control, 0);
                System.Windows.Controls.Grid.SetColumnSpan(Control, grid.ColumnDefinitions.Count + 1);
                System.Windows.Controls.Grid.SetRow(Control, 0);
                System.Windows.Controls.Grid.SetRowSpan(Control, grid.RowDefinitions.Count + 1);

                grid.Children.Add(Control);

                //grid.UpdateLayout();
                //CurrentElement.ForceLayout();
                Control.MouseMove += OnMouseMove;
                Control.LayoutUpdated += OnLayoutUpdated;
                Control.MouseDown += OnBackgroundClick;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentElement.IsResizeble)
                Resizeble(e);
        }

        private void Resizeble(MouseEventArgs e)
        {
            if (CurrentElement.WidthMin == 0)
                CurrentElement.WidthMin = CurrentElement.Content.Width;
            var mousePosition = e.GetPosition(Control);
            if (Control.Cursor == Cursors.SizeWE && e.LeftButton == MouseButtonState.Pressed)
                CurrentElement.Content.WidthRequest = GetWidth(mousePosition);
            else
                OnHoverBorder(mousePosition.X, mousePosition.Y);
        }

        private double GetWidth(System.Windows.Point mousePosition)
        {
            double width = 0;
            if (mousePosition.X > (CurrentElement.Content.X + CurrentElement.Content.Width / 2))
                width = CurrentElement.Content.Width + (mousePosition.X - (CurrentElement.Content.X + CurrentElement.Content.Width));
            else if (mousePosition.X < (CurrentElement.Content.X + CurrentElement.Content.Width / 2))
                width = CurrentElement.Content.Width + CurrentElement.Content.X - mousePosition.X;
            if (CurrentElement.WidthMin > width)
                return CurrentElement.WidthMin;
            return width;
        }

        private void OnHoverBorder(double x, double y)
        {
            var view = CurrentElement.Content;
            if (view.X < x && view.Y < y && view.X + view.Width > x && view.Y + view.Height > y)
            {
                if (Math.Abs(view.X - x) < 3 || Math.Abs((view.X + view.Width) - x) < 3)
                    Control.Cursor = Cursors.SizeWE;
                else
                    Control.Cursor = Cursors.Arrow;
            }
            else
                Control.Cursor = Cursors.Arrow;
        }

        private void OnGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Control.InvalidateMeasure();
            OnLayoutUpdated(null, e);
        }

        internal void Destroy()
        {
            if (grid != null)
            {
                CurrentElement.IsVisible = false;
                grid.Children.Remove(Control);
                grid.SizeChanged -= OnGridSizeChanged;
                grid = null;
                Control.MouseDown -= OnBackgroundClick;
                Control.LayoutUpdated -= OnLayoutUpdated;
            }
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (CurrentElement.Width != grid.ActualWidth
                || CurrentElement.Height != grid.ActualHeight)
            {
                CurrentElement.Layout(new Rectangle(0, 0, grid.ActualWidth, grid.ActualHeight));
            }
        }

        private void OnBackgroundClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == Control
                || (e.OriginalSource is System.Windows.Controls.Border border && border.Parent == null))
            {
                CurrentElement.SendBackgroundClick();
            }
        }

        private System.Windows.Controls.Grid GetTopGridFromWindow()
        {
            return GetChildGrid(System.Windows.Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive) ?? System.Windows.Application.Current.MainWindow);
        }

        private System.Windows.Controls.Grid GetChildGrid(DependencyObject content)
        {
            if (content is System.Windows.Controls.Grid grid)
                return grid;
            var count = VisualTreeHelper.GetChildrenCount(content);
            for (int i = 0; i < count; i++)
            {
                grid = GetChildGrid(VisualTreeHelper.GetChild(content, i));
                if (grid != null)
                    return grid;
            }
            return null;
        }


    }
}
