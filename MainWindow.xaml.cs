using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //The file we will be using in the Example under the file box.
        public string exampleFile;
        public string exampleFileExtension;

        //files loaded into the program
        public string[] files;

        public MainWindow()
        {
            InitializeComponent();
        }

        //Drag and Drop functionality for multiple files
        private void WindowDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //Grab all files dropped onto the window frame
                files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    string fileName = System.IO.Path.GetFileName(file);
                    //store file names in the File_Display_Box.
                    File_Display_Box.Items.Add(fileName).ToString();
                }

                if (files != null)
                {
                    exampleFile = System.IO.Path.GetFileNameWithoutExtension(files[0]);
                    exampleFileExtension = System.IO.Path.GetExtension(files[0]);
                    Example_Text.Text = exampleFile + exampleFileExtension;
                }
            }
        }

        //Fired any time the text changes in the AddText box.
        private void AddText_Changed(object sender, TextChangedEventArgs args)
        {
            if (Example_Text.Text != null)
            {
                if (AfterName.IsChecked == true)
                {
                    //Reset the example text, then add the text input after the file name.
                    Example_Text.Text = "";
                    string fileName = exampleFile;
                    string addingToFileName = TextToAdd_Box.Text;
                    string exampleString = fileName + addingToFileName + exampleFileExtension;
                    Example_Text.Text = exampleString;
                }
                if (BeforeName.IsChecked == true)
                {
                    //Reset the example text, then add the text input before the file name.
                    Example_Text.Text = "";
                    string fileName = exampleFile;
                    string addingToFileName = TextToAdd_Box.Text;
                    string exampleString = addingToFileName + fileName + exampleFileExtension;
                    Example_Text.Text = exampleString;
                }
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            //Clear all loaded files, and clear the example text under the file box.
            File_Display_Box.Items.Clear();
            Example_Text.Text = "";
        }

        //Toggles the grid visibility options for Add Text or Replace Text
        private void RenameSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToggleGrids(RenameSelectionBox.SelectedIndex);
        }

        private void ToggleGrids(int selection)
        {
            //Toggle visibility based on selection (0 = Add Text, 1 = Replace Text)
            switch (selection)
            {
                case 0:
                    AddText_Grid.Visibility = Visibility.Visible;
                    ReplaceWith_Grid.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    AddText_Grid.Visibility = Visibility.Hidden;
                    ReplaceWith_Grid.Visibility = Visibility.Visible;
                    break;
                default:
                    AddText_Grid.Visibility = Visibility.Hidden;
                    ReplaceWith_Grid.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}
