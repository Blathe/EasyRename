﻿using System;
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

        public int numberOfFilesRenamed;

        //files loaded into the program
        public string[] files;

        public List<string> fileList = new List<string>();
        public List<string> filesToRemove = new List<string>();
        public List<string> filesToAdd = new List<string>();

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

        public void SetExampleFile()
        {
            if (fileList != null & fileList.Count != 0)
            {
                exampleFile = System.IO.Path.GetFileNameWithoutExtension(fileList[0]);
                exampleFileExtension = System.IO.Path.GetExtension(fileList[0]);
                Example_Text.Text = exampleFile + exampleFileExtension;
            }
        }

        public void RefreshExampleText()
        {
            SetExampleFile();
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
                        foreach (string file in fileList)
                        {
                            //Add text to end of file name.
                            if (AfterName.IsChecked == true)
                            {
                                fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                                fileDirectory = System.IO.Path.GetDirectoryName(file) + "\\";
                                fileExtension = System.IO.Path.GetExtension(file);
                                string newName = fileDirectory + fileName + TextToAdd_Box.Text + fileExtension;

                                System.IO.File.Move(file, newName);
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

                                System.IO.File.Move(file, newName);
                                filesToRemove.Add(file);
                                filesToAdd.Add(newName);
                                numberOfFilesRenamed++;
                            }
                        }

                        RefreshFileList();
                        NotifyAmountRenamed();
                    } else
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
                                    System.IO.File.Move(file, newFileName);

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

        //clear all files and the fileList if it's populated.
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
