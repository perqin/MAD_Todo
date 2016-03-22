using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Data.Json;

namespace MAD_Todo.ViewModels {
    /// <summary>
    /// Store view state data for MainPage.
    /// </summary>
    public class MainAdaptiveViewModel : INotifyPropertyChanged {
        public bool ShowNewButton {
            get {
                return showNewButton;
            }
            set {
                showNewButton = value;
                OnPropertyChanged();
            }
        }
        public bool ShowBackButton {
            get {
                return showBackButton;
            }
            set {
                showBackButton = value;
                OnPropertyChanged();
            }
        }
        public bool ShowSaveResetDeleteButton {
            get {
                return showSaveResetDeleteButton;
            }
            set {
                showSaveResetDeleteButton = value;
                OnPropertyChanged();
            }
        }
        public bool ShowEditFrame {
            get {
                return showEditFrame;
            }
            set {
                showEditFrame = value;
                OnPropertyChanged();
            }
        }
        public ScreenWidthEnum ScreenWidth {
            get {
                return screenWidth;
            }
            set {
                screenWidth = value;
                adapt();
            }
        }
        public int SelectedItemIndex {
            get {
                return selectedItemIndex;
            }
            set {
                selectedItemIndex = value;
                OnPropertyChanged();
                adapt();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static MainAdaptiveViewModel getInstance() {
            if (instance == null) {
                instance = new MainAdaptiveViewModel(); 
            }
            return instance;
        }

        private static MainAdaptiveViewModel instance = null;
        private bool showNewButton = true;
        private bool showBackButton = false;
        private bool showSaveResetDeleteButton = false;
        private bool showEditFrame = false;
        private ScreenWidthEnum screenWidth = ScreenWidthEnum.Wide;
        private int selectedItemIndex = -1;

        private MainAdaptiveViewModel() {
            selectedItemIndex = -1;
            screenWidth = ScreenWidthEnum.Wide;
        }

        public override string ToString() {
            JsonObject j = new JsonObject();
            j.Add("ScreenWidth", JsonValue.CreateNumberValue(ScreenWidth == ScreenWidthEnum.Narrow ? 0 : 1));
            j.Add("SelectedItemIndex", JsonValue.CreateNumberValue(SelectedItemIndex));
            return j.Stringify();
        }

        public void FromString(string data) {
            if (data != null) {
                JsonObject j = JsonObject.Parse(data);
                ScreenWidth = ((int)j["ScreenWidth"].GetNumber()) == 0 ? ScreenWidthEnum.Narrow : ScreenWidthEnum.Wide;
                SelectedItemIndex = ((int)j["SelectedItemIndex"].GetNumber());
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// State table
        /// 	                narrow		                wide	
        ///             selected    no selected     selected    no selected
        /// new btn	        -	        +	            +	        +
        /// save btn        +	        -	            +	        -
        /// reset btn       +	        -	            +	        -
        /// delete btn      +	        -	            +	        -
        /// back button     +	        -	            -	        -
        private void adapt() {
            ShowNewButton = ScreenWidth == ScreenWidthEnum.Wide || SelectedItemIndex == -1;
            ShowSaveResetDeleteButton = SelectedItemIndex != -1;
            ShowBackButton = ScreenWidth == ScreenWidthEnum.Narrow && SelectedItemIndex != -1;
            ShowEditFrame = SelectedItemIndex != -1;
        }
    }

    public enum ScreenWidthEnum { Narrow, Wide };
}
