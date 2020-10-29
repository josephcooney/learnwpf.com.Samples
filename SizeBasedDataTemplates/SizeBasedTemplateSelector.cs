namespace SizeBasedDataTemplates
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public class SizeBasedTemplateSelector : DataTemplateSelector
    {
        public ObservableCollection<TemplateSize> TemplateSizes { get; set; }
        private Dictionary<ContentPresenter, TemplateSize> elementSizeDictionary = new Dictionary<ContentPresenter, TemplateSize>(); 

        public SizeBasedTemplateSelector()
        {
            TemplateSizes = new ObservableCollection<TemplateSize>();
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var contentPresenter = container as ContentPresenter;
            if (contentPresenter != null)
            {
                var templateWithSize = FindTemplateBySize(contentPresenter.ActualWidth, contentPresenter.ActualHeight);

                if (!elementSizeDictionary.ContainsKey(contentPresenter))
                {
                    contentPresenter.SizeChanged += TemplateContainerSizeChanged;
                    contentPresenter.Unloaded += RemoveContentControl;
                    contentPresenter.DataContextChanged += (s, args) => UpdateTemplate(s); 
                    elementSizeDictionary.Add(contentPresenter, templateWithSize);
                }
                else
                {
                    elementSizeDictionary[contentPresenter] = templateWithSize;
                }

                return templateWithSize.DataTemplate;
            }

            return null;
        }


        private void RemoveContentControl(object sender, RoutedEventArgs e)
        {
            var presenter = sender as ContentPresenter;
            if (presenter != null)
            {
                presenter.SizeChanged -= TemplateContainerSizeChanged;
                presenter.Unloaded -= RemoveContentControl;
                elementSizeDictionary.Remove(presenter);
            }
        }

        private TemplateSize FindTemplateBySize(double actualWidth, double actualHeight)
        {
            foreach (var templateWithSize in TemplateSizes)
            {
                if (templateWithSize.IsRightSize(actualWidth, actualHeight))
                {
                    System.Diagnostics.Debug.WriteLine("An appropriately sized template was found for width {0} and height {1}", actualWidth, actualHeight);
                    return templateWithSize;
                }
            }
            System.Diagnostics.Debug.WriteLine("No appropriately sized template was found for {0} {1} - using the first template instead", actualWidth, actualHeight);
            return TemplateSizes.First();
        }

        void TemplateContainerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTemplate(sender);
        }

        private void UpdateTemplate(object sender)
        {
            var contentControl = sender as ContentPresenter;
            if (contentControl != null)
            {
                var templateWithSize = elementSizeDictionary[contentControl];
                if (!templateWithSize.IsRightSize(contentControl.ActualWidth, contentControl.ActualHeight))
                {
                    contentControl.ContentTemplateSelector = null;
                    contentControl.ContentTemplate = SelectTemplate(contentControl.DataContext, contentControl);
                }
            }
        }
    }

    public class TemplateSize
    {
        public DataTemplate DataTemplate { get; set; }
        public Size? MinimumSize { get; set; }
        public Size? MaximumSize { get; set; }
        public bool IsRightSize(double width, double height)
        {
            return ((!MinimumSize.HasValue || (MinimumSize.Value.Width < width && MinimumSize.Value.Height < height)) && (!MaximumSize.HasValue || (MaximumSize.Value.Width > width && MaximumSize.Value.Height > height)));
        }
    }
}
