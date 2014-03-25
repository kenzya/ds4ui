using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlsLibrary
{
    /// <summary>
    /// CustomControl used to replace the standard expander
    /// </summary>
    public partial class CustomExpander : UserControl
    {
        #region Dependency Properties

        public FrameworkElement Header
        {
            get
            {
                return (FrameworkElement)GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(FrameworkElement), typeof(CustomExpander),
            new UIPropertyMetadata(null));

        public FrameworkElement Container
        {
            get
            {
                return (FrameworkElement)GetValue(ContainerProperty);
            }
            set
            {
                SetValue(ContainerProperty, value);
            }
        }
        public static readonly DependencyProperty ContainerProperty = DependencyProperty.Register("Container", typeof(FrameworkElement), typeof(CustomExpander),
            new UIPropertyMetadata(null, OnControlChanged));

        #endregion // Dependency Properties

        #region ctor

        public CustomExpander()
        {
            InitializeComponent();
        }

        #endregion // ctor

        #region PropertyChange Handlers

        private static void OnControlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            CustomExpander c = dependencyObject as CustomExpander;
            if (c != null)
            {
                c.ConstructExpander();
            }
        }

        private void HeaderMouseClick(object sender, MouseButtonEventArgs e)
        {
            if (toggle.IsChecked == false)
            {
                toggle.IsChecked = true;
                e.Handled = true; // block the bubbling of the event!
            }
            else
            {
                toggle.IsChecked = false;
                e.Handled = true; // block the bubbling of the event!
            }
        }

        #endregion // PropertyChange Handlers

        #region methods

        private void ConstructExpander()
        {
            Header.MouseLeftButtonUp += new MouseButtonEventHandler(HeaderMouseClick);
            header.Content = Header;
            container.Content = Container;
        }

        #endregion // methods
    }
}
