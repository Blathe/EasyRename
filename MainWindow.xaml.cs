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
        //Save the file name + extension separately so we can format text easier.
        public string exampleFile;
        public string exampleFileExtension;
        public string exampleString;

        //file name strings
        public string fileName;
        public string fileExtension;
        public string fileDirectory;

        public string textToAddAfter;
        public string textToAddBefore;

        public string textToReplace;
        public string replaceTextWith;

        //files loaded into the program
        public string[] files;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void RefreshFiles()
        {
            if (files != null)
            {
                foreach (string file in files)
                {
                    fileName = System.IO.Path.GetFileName(file);
                    //store file names in the File_Display_Box.
                    File_Display_Box.Items.Add(fileName);
                }

                exampleFile = System.IO.Path.GetFileNameWithoutExtension(files[0]);
                exampleFileExtension = System.IO.Path.GetExtension(files[0]);
                Example_Text.Text = exampleFile + exampleFileExtension;
            }
        }

        public void UpdateText()
        {
            if (Example_Text != null && exampleFile != null)
            {
                if (AfterName.IsChecked == true)
                {
                    //Reset the example text, then add the text input after the file name.
                    Example_Text.Text = "";
                    fileName = exampleFile;
                    textToAddAfter = TextToAdd_Box.Text;
                    exampleString = fileName + textToAddAfter + exampleFileExtension;
                    Example_Text.Text = exampleString;
                }

                if (BeforeName.IsChecked == true)
                {
                    //Reset the example text, then add the text input before the file name.
                    Example_Text.Text = "";
                    fileName = exampleFile;
                    textToAddBefore = TextToAdd_Box.Text;
                    exampleString = textToAddBefore + fileName + exampleFileExtension;
                    Example_Text.Text = exampleString;
                }

                if (RenameSelectionBox.SelectedIndex == 1)
                {
                    Example_Text.Text = "";
                    textToReplace = TextToReplace_Box.Text;
                    replaceTextWith = ReplaceTextWith_Box.Text;
                    Example_Text.Text = exampleFile + exampleFileExtension;
                    if (textToReplace != "" && replaceTextWith != "")
                    {
                        exampleString = exampleFile.Replace(textToReplace, replaceTextWith);
                        Example_Text.Text = exampleString + exampleFileExtension;
                    }
                }
            }
        }

        //Drag and Drop functionality for multiple files
        private void WindowDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //Grab all files dropped onto the window frame
                files = (string[])e.Data.GetData(DataFormats.FileDrop);

                RefreshFiles();
            }
        }

        //Fired any time the text changes in the AddText box.
        private void AddText_Changed(object sender, TextChangedEventArgs args)
        {
            UpdateText();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            //Clear all loaded files, and clear the example text under the file box.
            ClearFiles();
            File_Display_Box.Items.Clear();
            Example_Text.Text = "";
        }

        //Toggles the grid visibility options for Add Text or Replace Text
        private void RenameSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToggleGrids(RenameSelectionBox.SelectedIndex);
            UpdateText();
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

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            switch (RenameSelectionBox.SelectedIndex)
            {
                //Add Text
                case 0:
                    if (files != null && files.Length != 0)
                    {
                        foreach (string file in files)
                        {
                            //Add text to end of file name.
                            if (AfterName.IsChecked == true)
                            {
                                fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                                fileDirectory = System.IO.Path.GetDirectoryName(file) + "\\";
                                fileExtension = System.IO.Path.GetExtension(file);
                                string newName = fileDirectory + fileName + TextToAdd_Box.Text + fileExtension;

                                System.IO.File.Move(file, newName);
                            }
                            //Add text to beginning of file name.
                            if (BeforeName.IsChecked == true)
                            {
                                fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                                fileDirectory = System.IO.Path.GetDirectoryName(file) + "\\";
                                fileExtension = System.IO.Path.GetExtension(file);
                                string newName = fileDirectory + TextToAdd_Box.Text + fileName + fileExtension;

                                System.IO.File.Move(file, newName);
                            }
                        }
                        ClearFiles();
                    } else
                    {
                        return;
                    }
                    break;
                //Replace Text
                case 1:
                    if (files != null && files.Length != 0)
                    {
                        textToReplace = TextToReplace_Box.Text;
                        replaceTextWith = ReplaceTextWith_Box.Text;
                        string newFileName = "";

                        if (textToReplace != null && replaceTextWith != null)
                        {
                            foreach (string file in files)
                            {
                                if (file.Contains(textToReplace))
                                {
                                    newFileName = file.Replace(textToReplace, replaceTextWith);
                                    System.IO.File.Move(file, newFileName);
                                }
                            }
                        }
                        ClearFiles();
                    }
                    break;
                default:
                    break;
            }
        }

        public void ClearFiles()
        {
            if (files != null && files.Length != 0)
            {
                Array.Clear(files, 0, files.Length);
            }
            exampleFile = "";
            exampleFileExtension = "";
            exampleString = "";

            fileName = "";
            fileExtension = "";
            fileDirectory = "";

            textToAddAfter = "";
            textToAddBefore = "";

            textToReplace = "";
            replaceTextWith = "";

    }

        private void AfterName_Checked(object sender, RoutedEventArgs e)
        {
            UpdateText();
        }

        private void BeforeName_Checked(object sender, RoutedEventArgs e)
        {
            UpdateText();
        }

        private void TextToReplace_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateText();
        }

        private void ReplaceTextWith_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateText();
        }
    }
}
