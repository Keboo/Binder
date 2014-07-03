
namespace Binder.Demo
{
    /// <summary>
    /// Interaction logic for ConditionalConverterExample.xaml
    /// </summary>
    public partial class ConditionalConverterExample
    {
        public ConditionalConverterExample()
        {
            ViewModel = new ConditionalConverterViewModel();
            InitializeComponent();
        }

        public ConditionalConverterViewModel ViewModel { get; private set; }
    }
}
