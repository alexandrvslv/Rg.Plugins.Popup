using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Windows.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;
using Size = Windows.Foundation.Size;
using WinPopup = Windows.UI.Xaml.Controls.Primitives.Popup;

[assembly: ExportRenderer(typeof(PopupPage), typeof(PopupPageRenderer))]
namespace Rg.Plugins.Popup.Windows.Renderers
{
    [Preserve(AllMembers = true)]
    public class PopupPageRenderer : PageRenderer
    {
        //private Rect _keyboardBounds;
        private Canvas grid;
        private PopupPage CurrentElement => (PopupPage)Element;

        [Preserve]
        public PopupPageRenderer()
        {
        }

        internal void Prepare()
        {
            grid = GetTopGridFromWindow();
            if (!grid.Children.Contains(ContainerElement))
            {
                grid.SizeChanged += OnGridSizeChanged;
                Canvas.SetZIndex(ContainerElement, 10000);
                Canvas.SetTop(ContainerElement, 0);
                Canvas.SetLeft(ContainerElement, 0);

                grid.Children.Add(ContainerElement);

                //grid.UpdateLayout();
                //CurrentElement.ForceLayout();

                ContainerElement.LayoutUpdated += OnLayoutUpdated;
                ContainerElement.PointerPressed += OnBackgroundClick;
            }
        }

        private void OnGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Control.InvalidateMeasure();
            OnLayoutUpdated(null, e);
        }

        private void OnLayoutUpdated(object sender, object e)
        {
            if (CurrentElement.Width != grid.ActualWidth)
            {
                CurrentElement.Layout(new Rectangle(0, 0, grid.ActualWidth, grid.ActualHeight));
            }
        }


        internal void Destroy()
        {
            if (grid != null)
            {
                CurrentElement.IsVisible = false;
                grid.Children.Remove(ContainerElement);
                grid.SizeChanged -= OnGridSizeChanged;
                grid = null;
                ContainerElement.PointerPressed -= OnBackgroundClick;
                ContainerElement.LayoutUpdated -= OnLayoutUpdated;
            }
        }

        private void OnBackgroundClick(object sender, PointerRoutedEventArgs e)
        {
            if (e.OriginalSource == this)
            {
                CurrentElement.SendBackgroundClick();
            }
        }

        private Canvas GetTopGridFromWindow()
        {
            return GetChildGrid(Window.Current.Content);
        }

        private Canvas GetChildGrid(DependencyObject content)
        {
            if (content is Canvas grid)
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
