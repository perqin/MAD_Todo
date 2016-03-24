using MAD_Todo.ViewModels;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MAD_Todo {
    public sealed partial class MainPage : Page {
        private MainAdaptiveViewModel MainAdaptiveVM;

        public MainPage() {
            InitializeComponent();
            MainAdaptiveVM = MainAdaptiveViewModel.getInstance();
            Application.Current.Resuming += App_Resuming;
            Application.Current.Suspending += App_Suspending;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(360, 120));
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
            // Bind view adapter
            MainAdaptiveViewModel.getInstance().PropertyChanged += MainAdaptiveVM_PropertyChanged;
        }

        private void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args) {
            DataRequest request = args.Request;
            Todo shared = TodoViewModel.getInstance().GetTodoOfIndex(MainAdaptiveViewModel.getInstance().SelectedItemIndex);
            if (shared == null) {
                request.FailWithDisplayText("You have not selected any todo item");
            } else {
                request.Data.Properties.Title = "Todo: " + shared.Title;
                request.Data.SetText(shared.Detail);
            }
        }

        private void MainAdaptiveVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            // Bind back button
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = MainAdaptiveViewModel.getInstance().ShowBackButton
                ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            // Pass data to EditPage
            if (e.PropertyName == "SelectedItemIndex") {
                EditPage editPage = EditFrame.Content as EditPage;
                if (editPage != null) {
                    if (MainAdaptiveViewModel.getInstance().SelectedItemIndex != -1) {
                        editPage.ChangeEditingTodoData(TodoViewModel.getInstance().Todos[(sender as MainAdaptiveViewModel).SelectedItemIndex]);
                    } else {
                        editPage.ChangeEditingTodoData(null);
                    }
                }
            }
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e) {
            GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs ne) {
            ListFrame.Navigate(typeof(ListPage));
            (ListFrame.Content as ListPage).Loaded += (object sender, RoutedEventArgs re) => {
                JsonObject parameters = JsonObject.Parse(ne.Parameter as string);
                if (parameters != null) {
                    if (parameters.ContainsKey("MainAdaptiveState"))
                        MainAdaptiveViewModel.getInstance().FromString(parameters["MainAdaptiveState"].GetString());
                }
                EditFrame.Navigate(typeof(EditPage), ne.Parameter);
            };
        }

        private void App_Resuming(object sender, object e) {
            // Restore view states
            MainAdaptiveViewModel.getInstance().FromString(ApplicationData.Current.LocalSettings.Values["MainAdaptiveState"] as string);
        }

        private void App_Suspending(object sender, SuspendingEventArgs e) {
            // Store Todo list to storage
            TodoViewModel.getInstance().SaveToStorage();
            // Store view states
            ApplicationData.Current.LocalSettings.Values["MainAdaptiveState"] = MainAdaptiveViewModel.getInstance().ToString();
        }

        private void GoBack() {
            MainAdaptiveViewModel.getInstance().SelectedItemIndex = -1;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e) {
            MainAdaptiveViewModel.getInstance().ScreenWidth = e.NewSize.Width > 720 ? ScreenWidthEnum.Wide : ScreenWidthEnum.Narrow;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e) {
            // Prevent creating new Todo item, since local storage has not been implemented.
            return;
            /*
            TodoViewModel.getInstance().Todos.Add(new Todo());

            selectedItemIndex = TodoViewModel.getInstance().Todos.Count - 1;
            selectedItem = TodoViewModel.getInstance().Todos[selectedItemIndex];

            ListPage listPage = ListFrame.Content as ListPage;
            listPage.setSelected(selectedItemIndex);

            //Frame rootFrame = Window.Current.Content as Frame;
            //Grid.SetColumn(EditFrame, rootFrame.ActualWidth > 720 ? 1 : 0);
            //EditFrame.Visibility = Visibility.Visible;

            EditPage editPage = EditFrame.Content as EditPage;
            editPage.UpdateView(selectedItem);*/
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            TodoViewModel.getInstance().Todos[MainAdaptiveViewModel.getInstance().SelectedItemIndex].CloneFrom((EditFrame.Content as EditPage).DisplayTodo);
            if (MainAdaptiveViewModel.getInstance().ScreenWidth == ScreenWidthEnum.Narrow) {
                GoBack();
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e) {
            (EditFrame.Content as EditPage).ChangeEditingTodoData(TodoViewModel.getInstance().Todos[MainAdaptiveViewModel.getInstance().SelectedItemIndex]);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e) {
            // Prevent creating new Todo item, since local storage has not been implemented.
            return;
            /*
            EditFrame.Visibility = Visibility.Collapsed;
            GoBack();
            if (selectedItemIndex >= 0)
            {
                TodoViewModel.getInstance().Todos.RemoveAt(selectedItemIndex);
            }*/
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e) {
            DataTransferManager.ShowShareUI();
        }
    }
}
