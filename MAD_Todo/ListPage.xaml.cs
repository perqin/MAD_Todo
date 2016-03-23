using MAD_Todo.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MAD_Todo {
    public sealed partial class ListPage : Page {
        private TodoViewModel todoVM;
        private MainAdaptiveViewModel MainAdaptiveVM;
        
        public ListPage() {
            InitializeComponent();
            todoVM = TodoViewModel.getInstance();
            MainAdaptiveVM = MainAdaptiveViewModel.getInstance();
        }
    }
}
