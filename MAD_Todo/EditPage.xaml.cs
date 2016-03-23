using MAD_Todo.ViewModels;
using System;
using Windows.ApplicationModel;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MAD_Todo {
    public sealed partial class EditPage : Page {
        private Todo displayTodo = new Todo();

        public Todo DisplayTodo {
            get {
                return displayTodo;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            JsonObject parameters = JsonObject.Parse(e.Parameter as string);
            if (parameters != null) {
                if (parameters.ContainsKey("EditingTodoData"))
                    displayTodo.FromString(parameters["EditingTodoData"].GetString());
            }
        }

        public EditPage() {
            InitializeComponent();
            Application.Current.Resuming += App_Resuming;
            Application.Current.Suspending += App_Suspending;
        }

        /// <summary>
        /// Change editing todo data without changing ID
        /// </summary>
        /// <param name="todo"></param>
        public void ChangeEditingTodoData(Todo todo) {
            //TODO: Remove old image file
            displayTodo = new Todo(false);
            //TODO: Copy new file from passed todo id
            displayTodo.Title = todo == null ? "" : todo.Title;
            displayTodo.Detail = todo == null ? "" : todo.Detail;
            displayTodo.DueDate = todo == null ? DateTime.Today : todo.DueDate;
            displayTodo.Done = todo == null ? false : todo.Done;
            displayTodo.CoverImageExt = todo == null ? "" : todo.CoverImageExt;
            displayTodo.CoverSource = todo == null ? null : todo.CoverSource;
        }

        private async void SelectCoverButton_Click(object sender, RoutedEventArgs e) {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".png");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null) {
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read)) {
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    displayTodo.CoverSource = bitmapImage;
                }
            }
        }

        private void App_Resuming(object sender, object e) {
            displayTodo.FromString(ApplicationData.Current.LocalSettings.Values["EditingTodoData"] as string);
        }

        private void App_Suspending(object sender, SuspendingEventArgs e) {
            ApplicationData.Current.LocalSettings.Values["EditingTodoData"] = displayTodo.ToString();
        }
    }
}
