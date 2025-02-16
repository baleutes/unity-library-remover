using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.IO;
using System.Threading.Tasks;

namespace UnityLibraryRemover;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Width = 400;
        Height = 200;
    }

    public async void SelectFolder_Click(object sender, RoutedEventArgs args)
    {
        var dialog = new OpenFolderDialog();
        var folder = await dialog.ShowAsync(this);
        int deletedFoldersCount = 0;

        if (folder != null)
        {
            var subfolders = Directory.GetDirectories(folder);
            int libraryFolderCount = 0;

            foreach (var subfolder in subfolders)
            {
                var libraryFolder = Path.Combine(subfolder, "Library");

                if (Directory.Exists(libraryFolder))
                {
                    libraryFolderCount++;
                }
            }

            ProgressBar.Maximum = libraryFolderCount;

            if (libraryFolderCount == 0)
            {
                StatusTextBlock.Text = "No 'Library' folders found in the subfolders.";
                
                return;
            }

            ProgressBar.IsVisible = true;
            SelectionButton.IsEnabled = false;

            StatusTextBlock.Text = $"Found {libraryFolderCount} 'Library' folder(s). Starting deletion process...";
            await Task.Delay(2000);

            await Task.Run(async () =>
            {
                foreach (var subfolder in subfolders)
                {
                    var libraryFolder = Path.Combine(subfolder, "Library");

                    if (Directory.Exists(libraryFolder))
                    {
                        Directory.Delete(libraryFolder, true);
                        deletedFoldersCount++;

                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            ProgressBar.Value = deletedFoldersCount;
                            StatusTextBlock.Text = $"Deletion in progress...\n{deletedFoldersCount} folder(s) deleted, {libraryFolderCount - deletedFoldersCount} folder(s) left.";
                        });
                    }
                }
            });

            SelectionButton.IsEnabled = true;
            StatusTextBlock.Text = "Deletion process finished. Deleted " + deletedFoldersCount.ToString() + " folder(s).";
        }
    }
}
