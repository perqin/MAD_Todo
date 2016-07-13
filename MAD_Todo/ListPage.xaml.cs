using MAD_Todo.Utils;
using MAD_Todo.ViewModels;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Popups;
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

        private async void SearchButton_Click(object sender, RoutedEventArgs e) {
            string[] searchKeys = SearchTextBox.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (searchKeys.Length == 0) {
                return;
            }
            using (var connection = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), C.DB_PATH)) {
                var cmd = connection.CreateCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{typeof(Todo).Name}'");
                if (cmd.ExecuteScalar<string>() == null) {
                    connection.CreateTable<Todo>(SQLite.Net.Interop.CreateFlags.None);
                }
                StringBuilder sb = new StringBuilder()
                    .Append("SELECT * FROM ")
                    .Append(typeof(Todo).Name)
                    .Append(" WHERE 0 = 1");
                foreach (string k in searchKeys) {
                    sb.Append(" OR title LIKE '%").Append(k).Append("%'");
                    sb.Append(" OR detail LIKE '%").Append(k).Append("%'");
                    sb.Append(" OR due_date LIKE '%").Append(k).Append("%'");
                }
                List<Todo> result = connection.Query<Todo>(sb.ToString());
                StringBuilder resultStringBuilder = new StringBuilder();
                foreach (Todo r in result) {
                    resultStringBuilder.Append("_ID: ").Append(r.ID);
                    resultStringBuilder.Append("; title: ").Append(r.Title);
                    resultStringBuilder.Append("; detail: ").Append(r.Detail);
                    resultStringBuilder.Append("; due_date: ").Append(r.DB_DueDate);
                    resultStringBuilder.Append("; done: ").Append(r.Done.ToString()).Append('\n');
                }
                MessageDialog dialog = new MessageDialog(resultStringBuilder.ToString());
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }
    }
}
