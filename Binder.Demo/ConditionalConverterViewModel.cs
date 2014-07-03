using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight;

namespace Binder.Demo
{
    public class ConditionalConverterViewModel : ViewModelBase
    {
        private readonly BindingList<object> _firstCollection;
        private readonly BindingList<object> _secondCollection;
        private readonly BindingList<object> _thirdCollection;

        public ConditionalConverterViewModel()
        {
            _firstCollection = new BindingList<object>(new object[] { false, true });
            _secondCollection = new BindingList<object>(Enumerable.Range(1, 10).Cast<object>().ToArray());
            _thirdCollection = new BindingList<object>(new object[] { "String 1", "String 2", "String 3"});
        }

        public ICollectionView FirstCollection
        {
            get { return CollectionViewSource.GetDefaultView(_firstCollection); }
        }

        public ICollectionView SecondCollection
        {
            get { return CollectionViewSource.GetDefaultView(_secondCollection); }
        }

        public ICollectionView ThirdCollection
        {
            get { return CollectionViewSource.GetDefaultView(_thirdCollection); }
        }
    }
}