using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public int numberOfFilesRenamed;

        //files loaded into the program
        public string[] files;

        public List<string> fileList = new List<string>();
        public List<string> filesToRemove = new List<string>();
        public List<string> filesToAdd = new List<string>();

        private string invalidCharPattern = "[\\<>:/\"|?*]";

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Logic for loading multiple files that are dragged from Explorer onto the program window.
        /// </summary>
        private void WindowDrop(object sender, DragEventArgs e)
        {
            ClearFiles();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //Grab all files dropped onto the window frame
                string[] tempfiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                for (int i = 0; i < tempfiles.Length; i++)
                {
                    fileList.Add(tempfiles[i]);
                    //get rid of the directory name and just display the actual file name with extension
                    string displayName = System.IO.Path.GetFileName(tempfiles[i]);
                    File_Display_Box.Items.Add(displayName);
                }

                SetExampleFile();
            }
        }

        /// <summary>
        /// Sets the Example File and updates the Example_Text with the example file's name.
        /// </summary>
        public void SetExampleFile()
        {
            if (fileList != null & fileList.Count != 0)
            {
                exampleFile = System.IO.Path.GetFileNameWithoutExtension(fileList[0]);
                exampleFileExtension = System.IO.Path.GetExtension(fileList[0]);
                Example_Text.Text = exampleFile + exampleFileExtension;
            }
        }

        /// <summary>
        /// Updates the text shown in the Example_Text field.
        /// </summary>
        public void RefreshExampleText()
        {
            if (exampleFile != null)
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

                    if (textToReplace != "")
                    {
                        exampleString = exampleFile.Replace(textToReplace, replaceTextWith);
                        Example_Text.Text = exampleString + exampleFileExtension;
                    }
                }
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            //Clear all loaded files, and clear the example text under the file box.
            ClearFiles();
        }

        //Toggles the grid visibility options for Add Text or Replace Text
        private void RenameSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToggleGrids(RenameSelectionBox.SelectedIndex);
            RefreshExampleText();
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
                    if (fileList != null && fileList.Count != 0)
                    {
                        if (!ContainsInvalidChars())
                        {
                            foreach (string file in fileList)
                            {
                                //Add text to end of file name.
                                if (AfterName.IsChecked == true)
                                {
                                    fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                                    fileDirectory = System.IO.Path.GetDirectoryName(file) + "\\";
                                    fileExtension = System.IO.Path.GetExtension(file);
                                    string newName = fileDirectory + fileName + TextToAdd_Box.Text + fileExtension;

                                    try
                                    {
                                        System.IO.File.Move(file, newName);
                                    } catch (Exception ex)
                                    {
                                        NotifyException(ex);
                                    }
                                    filesToRemove.Add(file);
                                    filesToAdd.Add(newName);
                                    numberOfFilesRenamed++;
                                }
                                //Add text to beginning of file name.
                                if (BeforeName.IsChecked == true)
                                {
                                    fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                                    fileDirectory = System.IO.Path.GetDirectoryName(file) + "\\";
                                    fileExtension = System.IO.Path.GetExtension(file);
                                    string newName = fileDirectory + TextToAdd_Box.Text + fileName + fileExtension;

                                    try
                                    {
                                        System.IO.File.Move(file, newName);
                                    }
                                    catch (Exception ex)
                                    {
                                        NotifyException(ex);
                                    }
                                    filesToRemove.Add(file);
                                    filesToAdd.Add(newName);
                                    numberOfFilesRenamed++;
                                }
                            }
                            RefreshFileList();
                            NotifyAmountRenamed();
                        }
                        else
                        {
                            NotifyInvalidFileName();
                        }
                    }
                    else
                    {
                        NotifyNoFilesSelected();
                    }
                    break;
                //Replace Text
                case 1:
                    if (fileList != null && fileList.Count != 0)
                    {
                        textToReplace = TextToReplace_Box.Text;
                        replaceTextWith = ReplaceTextWith_Box.Text;
                        string newFileName;

                        if (textToReplace != null && replaceTextWith != null)
                        {
                            foreach (string file in fileList)
                            {
                                if (file.Contains(textToReplace))
                                {
                                    newFileName = file.Replace(textToReplace, replaceTextWith);

                                    try
                                    {
                                        System.IO.File.Move(file, newFileName);
                                    }
                                    catch (Exception ex)
                                    {
                                        NotifyException(ex);
                                    }

                                    filesToRemove.Add(file);
                                    filesToAdd.Add(newFileName);
                                    numberOfFilesRenamed++;
                                }
                            }

                            RefreshFileList();
                            NotifyAmountRenamed();
                        }
                    } else
                    {
                        NotifyNoFilesSelected();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Checks for invalid characters when trying to rename files.
        /// </summary>
        public bool ContainsInvalidChars()
        {
            //TODO: Add more checks depending on what box the user is typing into.
            if (!Regex.IsMatch(TextToAdd_Box.Text, invalidCharPattern))
            {
                return false;
            }
            else {
                return true;
            }
        }
          
        public void NotifyException(Exception ex)
        {
            string msg = ex.ToString();
            MessageBoxResult result = MessageBox.Show(this, ex.GetType().ToString());
        }

        public void NotifyInvalidFileName()
        {
            MessageBoxResult result = MessageBox.Show(this, "That file name contains invalid characters, try again.");
        }
        public void NotifyNoFilesSelected()
        {
            MessageBoxResult result = MessageBox.Show(this, "You have not selected any files to rename...");
        }


        public void NotifyAmountRenamed()
        {
            if (numberOfFilesRenamed > 1)
            {
                MessageBoxResult result = MessageBox.Show(this, $"{numberOfFilesRenamed} files have been renamed!");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show(this, $"{numberOfFilesRenamed} file has been renamed!");
            }

            numberOfFilesRenamed = 0;
        }

        /// <summary>
        /// Removes old file names from the fileList List, then adds the newly renamed file names back to the list, and finally refreshes the File Box display.
        /// </summary>
        public void RefreshFileList()
        {
            foreach (string file in filesToRemove)
            {
                fileList.Remove(file);
            }

            foreach (string file in filesToAdd)
            {
                fileList.Add(file);
            }

            filesToRemove.Clear();
            filesToAdd.Clear();

            RefreshDisplayBox();
            RefreshExampleText();
        }

        /// <summary>
        /// Logic for refreshing the File Box display.
        /// </summary>
        public void RefreshDisplayBox()
        {
            File_Display_Box.Items.Clear();
            if (fileList != null)
            {
                foreach (string file in fileList)
                {
                    File_Display_Box.Items.Add(System.IO.Path.GetFileName(file));
                }
            }
        }

        /// <summary>
        /// Clears all files in the fileList and removes any displays.
        /// </summary>
        public void ClearFiles()
        {
            exampleFile = null;
            exampleFileExtension = null;
            exampleString = null;

            fileName = null;
            fileExtension = null;
            fileDirectory = null;

            textToAddAfter = null;
            textToAddBefore = null;

            textToReplace = null;
            replaceTextWith = null;

            File_Display_Box.Items.Clear();
            Example_Text.Text = "";

            if (fileList != null && fileList.Count != 0)
            {
                fileList.Clear();
            }
            numberOfFilesRenamed = 0;
        }

        //text changes in the text to add box
        private void AddText_Changed(object sender, TextChangedEventArgs args)
        {
            RefreshExampleText();
        }

        //after name radio dial selected
        private void AfterName_Checked(object sender, RoutedEventArgs e)
        {
            RefreshExampleText();
        }

        //before name radio dial selected
        private void BeforeName_Checked(object sender, RoutedEventArgs e)
        {
            RefreshExampleText();
        }

        //text changes in the text to replace box
        private void TextToReplace_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshExampleText();
        }

        //text changes in the replace text with box
        private void ReplaceTextWith_Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshExampleText();
        }
    }
}
