using MAD_Todo.Utils;
using SQLite.Net;
using SQLite.Net.Attributes;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;
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

        public async void ReloadSource() {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file;
            string fileName = ID + "." + CoverImageExt;
            try {
                file = await localFolder.GetFileAsync(fileName);
            } catch (Exception e) {
                StorageFile defaultFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/default.png"));
                CoverImageExt = defaultFile.FileType;
                file = await defaultFile.CopyAsync(localFolder, ID + "." + CoverImageExt, NameCollisionOption.ReplaceExisting);
            }
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read)) {
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);
                CoverSource = bitmapImage;
            }
        }

        [Column("_id"), MaxLength(64), PrimaryKey]
        public string ID {
            get {
                return _id;
            }
            private set {
                _id = value;
                OnPropertyChanged();
            }
        }

        [Column("title"), MaxLength(1024)]
        public string Title {
            get {
                return title;
            }
            set {
                title = value;
                OnPropertyChanged();
            }
        }

        [Column("detail"), MaxLength(1073741824)]
        public string Detail {
            get {
                return detail;
            }
            set {
                detail = value;
                OnPropertyChanged();
            }
        }

        [Ignore]
        public DateTime DueDate {
            get {
                return dueDate;
            }
            set {
                dueDate = value;
                OnPropertyChanged();
            }
        }

        [Ignore]
        public ImageSource CoverSource {
            get {
                return coverSource;
            }
            set {
                coverSource = value;
                OnPropertyChanged();
            }
        }

        [Column("cover_image_ext"), MaxLength(16)]
        public string CoverImageExt {
            get {
                return coverImageExt;
            }
            set {
                coverImageExt = value;
            }
        }

        [Column("done")]
        public bool? Done {
            get {
                return done;
            }
            set {
                done = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// For SQLite attribute
        /// </summary>
        [Column("due_date"), MaxLength(16)]
        public string DB_DueDate {
            get {
                return DueDate.ToString(C.YMD_FORMAT);
            }
            set {
                DateTime dt;
                if (DateTime.TryParseExact(value, C.YMD_FORMAT, null, DateTimeStyles.None, out dt)) {
                    DueDate = dt;
                } else {
                    DueDate = new DateTime(1970, 1, 1);
                }
            }
        }

        public Todo() {
            _id = Guid.NewGuid().ToString();
            title = "";
            detail = "";
            done = false;
            coverImageExt = "";
            dueDate = new DateTime(1970, 1, 1);
        }

        public Todo(bool init) {
            _id = Guid.NewGuid().ToString();
            if (init) {
                title = "New Todo";
                detail = "Detail here...";
                done = false;
                coverImageExt = "null";
                dueDate = DateTime.Today;
                ReloadSource();
            } else {
                title = "";
                detail = "";
                done = false;
                coverImageExt = "";
                dueDate = new DateTime(1970, 1, 1);
            }
        }

        public void CloneFrom(Todo copy, bool copyID = true) {
            if (copyID) {
                ID = copy.ID;
            }
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

        //public void SaveToStorage() {
            //TODO: ---Save to storage
            //FIXME: Do nothing, since local storage has not been implemented.
        //}

        public void AddTodo(Todo todo) {
            Todos.Add(todo);
            using (var connection = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), C.DB_PATH)) {
                var cmd = connection.CreateCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{typeof(Todo).Name}'");
                if (cmd.ExecuteScalar<string>() == null) {
                    connection.CreateTable<Todo>(SQLite.Net.Interop.CreateFlags.None);
                }
                int count = connection.InsertOrReplace(todo, typeof(Todo));
            }
        }

        public void LoadFromStorage() {
            using (var connection = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), C.DB_PATH)) {
                var cmd = connection.CreateCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{typeof(Todo).Name}'");
                if (cmd.ExecuteScalar<string>() == null) {
                    connection.CreateTable<Todo>(SQLite.Net.Interop.CreateFlags.None);
                }
                TableQuery<Todo> q = connection.Table<Todo>();
                _instance.Todos.Clear();
                for (int i = 0; i < q.Count(); ++i) {
                    Todos.Add(q.ElementAt(i));
                }
            }
        }

        public void UpdateTodo(int index, Todo todo) {
            //string oid = Todos[index].ID;
            Todos[index].CloneFrom(todo, false);
            using (var connection = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), C.DB_PATH)) {
                var cmd = connection.CreateCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{typeof(Todo).Name}'");
                if (cmd.ExecuteScalar<string>() == null) {
                    connection.CreateTable<Todo>(SQLite.Net.Interop.CreateFlags.None);
                }
                int count = connection.Update(Todos[index], typeof(Todo));
            }
        }

        public void DeleteTodo(int index) {
            string oid = Todos[index].ID;
            Todos.RemoveAt(index);
            using (var connection = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), C.DB_PATH)) {
                var cmd = connection.CreateCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{typeof(Todo).Name}'");
                if (cmd.ExecuteScalar<string>() == null) {
                    connection.CreateTable<Todo>(SQLite.Net.Interop.CreateFlags.None);
                }
                int count = connection.Delete<Todo>(oid);
            }
        }
    }
}
