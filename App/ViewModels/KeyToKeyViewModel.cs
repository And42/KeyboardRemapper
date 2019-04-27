using MVVM_Tools.Code.Classes;

namespace App.ViewModels
{
    public class KeyToKeyViewModel : BindableBase
    {
        public int SourceKey
        {
            get => _sourceKey;
            set => SetProperty(ref _sourceKey, value);
        }

        public int MappedKey
        {
            get => _mappedKey;
            set => SetProperty(ref _mappedKey, value);
        }

        private int _sourceKey;
        private int _mappedKey;
    }
}
