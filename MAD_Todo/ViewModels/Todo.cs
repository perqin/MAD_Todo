using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MAD_Todo.ViewModels {

    public class Todo : INotifyPropertyChanged {
        private string _id;
        private string title;
        private string detail;
        private DateTime dueDate;
        private ImageSource coverSource;
        private string coverImageExt;
        private bool? done;

        public void ReloadSource() {
            //TODO: Reload CoverSource using id and ext
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            string fileName = ID + "." + CoverImageExt;
            localFolder.GetFileAsync(fileName).Completed = new AsyncOperationCompletedHandler<StorageFile>((IAsyncOperation<StorageFile> o, AsyncStatus s) => {
                if (s == AsyncStatus.Completed) {
                    //TODO
                }
            });

        }

        public string ID {
            get {
                return _id;
            }
            private set {
                _id = value;
                OnPropertyChanged();
            }
        }
        public string Title {
            get {
                return title;
            }
            set {
                title = value;
                OnPropertyChanged();
            }
        }
        public string Detail {
            get {
                return detail;
            }
            set {
                detail = value;
                OnPropertyChanged();
            }
        }
        public DateTime DueDate {
            get {
                return dueDate;
            }
            set {
                dueDate = value;
                OnPropertyChanged();
            }
        }
        public ImageSource CoverSource {
            get {
                return coverSource;
            }
            set {
                coverSource = value;
                OnPropertyChanged();
            }
        }
        public string CoverImageExt {
            get {
                return coverImageExt;
            }
            set {
                coverImageExt = value;
            }
        }
        public bool? Done {
            get {
                return done;
            }
            set {
                done = value;
                OnPropertyChanged();
            }
        }

        public Todo(bool defaultProperties = true) {
            _id = Guid.NewGuid().ToString();
            if (defaultProperties) {
                title = "New Todo";
                detail = "Detail here...";
                done = false;
                //coverSource = new BitmapImage(new Uri("ms-appx://MAD_Todo/Assets/default.png"));
                coverImageExt = "";
                dueDate = DateTime.Today;
                ReloadSource();
            }
        }

        public void CloneFrom(Todo copy) {
            ID = copy.ID;
            Title = copy.Title;
            Detail = copy.Detail;
            DueDate = copy.DueDate;
            CoverImageExt = copy.CoverImageExt;
            Done = copy.Done;
            ReloadSource();
        }

        public override string ToString() {
            JsonObject j = new JsonObject();
            j.Add("ID", JsonValue.CreateStringValue(ID));
            j.Add("Title", JsonValue.CreateStringValue(Title));
            j.Add("Detail", JsonValue.CreateStringValue(Detail));
            j.Add("Done", JsonValue.CreateBooleanValue(Done ?? false));
            j.Add("DueDate", JsonValue.CreateNumberValue((int)((DueDate - new DateTime(1970, 1, 1)).TotalSeconds)));
            return j.Stringify();
        }

        public void FromString(string data) {
            if (data == null) return;
            JsonObject j = JsonObject.Parse(data);
            ID = j["ID"].GetString();
            Title = j["Title"].GetString();
            Detail = j["Detail"].GetString();
            Done = j["Done"].GetBoolean();
            DueDate = new DateTime(1970, 1, 1).AddSeconds(j["DueDate"].GetNumber());
            ReloadSource();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Singleton class for access of Todo data.
    /// </summary>
    public class TodoViewModel {
        private static TodoViewModel _instance;

        private ObservableCollection<Todo> todos = new ObservableCollection<Todo>();

        public ObservableCollection<Todo> Todos { get { return todos; } }

        public static TodoViewModel getInstance() {
            if (_instance == null) {
                _instance = new TodoViewModel();
            }
            return _instance;
        }

        public Todo GetTodoOfIndex(int index) {
            return index >= 0 && index < Todos.Count ? Todos[index] : null;
        }

        public void SaveToStorage() {
            //TODO: ---Save to storage
            //FIXME: Do nothing, since local storage has not been implemented.
        }

        public void LoadFromStorage() {
            //TODO: ---Load from storage
            //FIXME: Create fake Todo items, since local storage has not been implemented.
            _instance.addTodo(new Todo());
            _instance.addTodo(new Todo());
            _instance.addTodo(new Todo());
            _instance.addTodo(new Todo());
        }

        public void addTodo(Todo todo) {
            todos.Add(todo);
        }
    }
}
