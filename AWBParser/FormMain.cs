using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AWBParser
{
    public partial class FormMain : Form
    {
        private Button ButtonChoose;
        private Button ButtonLocation;
        private Label label1;
        private TextBox TxtBoxFilename;
        private Label LabelFullOutputDetail;
        private Button ButtonGenerate;
        private Label LabelOutputLocation;
        private Label LabelTargetFiles;

        private Regex unspupportedRegex = new Regex("(^(PRN|AUX|NUL|CON|COM[1-9]|LPT[1-9]|(\\.+)$)(\\..*)?$)|(([‌​\\x00-\\x1f\\\\?*:\"‌​;‌​|/<>])+)|([\\. ]+)", RegexOptions.IgnoreCase);

        private OpenFileDialog ofd = new OpenFileDialog();
        private FolderBrowserDialog fbd = new FolderBrowserDialog();

        private static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private string outputPath = desktopPath;
        private static string defaultOutputFilename = DateTime.Now.ToString("yy.MM.dd HH.mm.ss tt");
        private string outPutFile = defaultOutputFilename;
        private static string defaultFileExt = ".csv";
        private string currentFile = "";
        private string report = "";

        string outputContent = "";
        string PACKAGE_TYPE_DOC = "DOX";
        string PACKAGE_TYPE_PAK = "WPX";

        public FormMain()
        {
            InitializeComponent();

            ofd.Filter = "PDFs|*.pdf";
            ofd.Multiselect = true;
            ofd.Title = "Browse AWB PDFs";

            fbd.RootFolder = Environment.SpecialFolder.Desktop;

            SetLocationPath();

            TxtBoxFilename.Text = outPutFile;
            TxtBoxFilename.Enter += new EventHandler(this.TxtBoxFilename_Enter);
            TxtBoxFilename.Click += new EventHandler(this.TxtBoxFilename_Click);

            SetFullLocationText();

            TxtBoxFilename.TextChanged += new EventHandler(this.TxtBoxFilename_TxtChanged);
        }

        private void ButtonChoose_Click(object sender, EventArgs e)
        {
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                LabelTargetFiles.Text = ofd.FileNames.Length + "PDF(s) selected.";
            }
        }

        private void SetFullLocationText()
        {
            LabelFullOutputDetail.Text = outputPath + "\\" + outPutFile + defaultFileExt;
        }

        private void SetLocationPath()
        {
            LabelOutputLocation.Text = outputPath;
        }

        private void ShowWarningMessage(string message)
        {
            MessageBox.Show(null, message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(null, message, "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        private void ShowInfoMessage(string message)
        {
            MessageBox.Show(null, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        private void TxtBoxFilename_Click(object sender, EventArgs e)
        {
            TxtBoxFilename.SelectAll();
        }

        private void TxtBoxFilename_Enter(object sender, EventArgs e)
        {
            TxtBoxFilename.SelectAll();
        }

        private void TxtBoxFilename_TxtChanged(object sender, EventArgs e)
        {
            if (CheckFilename(TxtBoxFilename.Text))
            {
                outPutFile = TxtBoxFilename.Text;
                SetFullLocationText();
            }
            else
            {
                ShowErrorMessage("Invalid character for a file name found.");
                TxtBoxFilename.Text = Regex.Replace(TxtBoxFilename.Text, unspupportedRegex.ToString(), "");
                TxtBoxFilename.Focus();
                TxtBoxFilename.Select(TxtBoxFilename.Text.Length, 0);
            }
        }

        private bool CheckFilename(string filename)
        {
            if (unspupportedRegex.IsMatch(filename))
            {
                return false;
            }
            return true;
        }

        private void ButtonLocation_Click(object sender, EventArgs e)
        {
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                outputPath = fbd.SelectedPath;
                SetLocationPath();
                SetFullLocationText();
            }
        }

        private bool ReadFromPDF(string fullFileName)
        {
            currentFile = "";
            try
            {
                // Create a reader for the given PDF file
                PdfReader reader = new PdfReader(fullFileName);
                currentFile += PdfTextExtractor.GetTextFromPage(reader, reader.NumberOfPages);
                reader.Close();
                // SHOW THE PAGE
                //ShowInfoMessage(fullFileName + Environment.NewLine + currentFile);
                bool noError = Parser(currentFile);
                if (!noError)
                {
                    report = report + fullFileName + Environment.NewLine;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Parser(string fileContent)
        {
            string docType = "TYPE MISSING";
            string awbNumber = "AWB MISSING";
            string refNumber = "REF MISSING";
            //string weightNumber = "KG MISSING";
            //string totalPiece = "PCS MISSING";
            //string dateString = "DATE MISSING";
            string destString = "DEST MISSING";
            bool noError = true;

            List<string> Lines = new List<string>();
            using (StringReader reader = new StringReader(fileContent))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Lines.Add(line);
                }

                for (int i = 0; i < Lines.Count; i++)
                {

                    line = Lines[i];
                    if (String.Compare(line, PACKAGE_TYPE_DOC, true) == 0)
                    {
                        docType = "Doc";
                    }
                    else if (String.Compare(line, PACKAGE_TYPE_PAK, true) == 0)
                    {
                        docType = "Non-Doc";
                    }
                    else if (line.Contains("Ref:"))
                    {
                        string[] refLine = line.Split(' ');
                        if (refLine.Length >= 2)
                        {
                            string[] refNumberArray = refLine[0].Split(':');
                            if (refNumberArray.Length == 2)
                            {
                                refNumber = refNumberArray[1];
                            }
                            // if ref is missing
                            else if (refNumberArray.Length == 1)
                            {
                                noError = false;
                            }
                            // if there is nothing after Ref:
                            else
                            {
                                noError = false;
                            }
                        }
                        // if there is nothing after Ref:
                        else
                        {
                            noError = false;
                        }

                        /*
                        // go to the next line, should be kgs
                        if ((line = reader.ReadLine()) != null)
                        {
                            // if this is the kg line
                            if (line.ToLower().Contains("kg"))
                            {
                                string[] weightArray = line.Split(' ');
                                if (weightArray.Length == 2)
                                {
                                    weightNumber = weightArray[0];
                                }
                                // if there is error in reading weight number
                                else
                                {
                                    noError = false;
                                }

                            }
                            // if there is no kg
                            else
                            {
                                noError = false;
                            }

                            // go to pcs line
                            if ((line = reader.ReadLine()) != null)
                            {
                                try
                                {
                                    int m = Int32.Parse(line);
                                    totalPiece = line;
                                }
                                // if the number cannot be parse
                                catch
                                {
                                    noError = false;
                                }

                                // go to date line
                                if ((line = reader.ReadLine()) != null)
                                {
                                    try
                                    {
                                        DateTime.TryParseExact(line, "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime re);
                                        dateString = re.ToString("dd-MM-yyyy");
                                    }
                                    // if the number cannot be parse
                                    catch
                                    {
                                        noError = false;
                                    }
                                }
                                // if there is no line afetr kg line
                                else
                                {
                                    noError = false;
                                }
                            }
                            // there is no next line after ref line
                            else
                            {
                                noError = false;
                            }
                        }
                        */
                    }
                    else if (line.Contains("WAYBILL "))
                    {
                        string[] waybillLine = line.Split(" ".ToCharArray(), 2);
                        if (waybillLine.Length == 2)
                        {
                            awbNumber = waybillLine[1];
                            //ShowInfoMessage(awbNumber);
                        }
                        else
                        {
                            noError = false;
                        }
                    }
                    if (line[0] == '.' && i > 0)
                    {
                        destString = Lines[i - 1];
                    }
                }
            }
            outputContent = outputContent + refNumber + ", , " + awbNumber + ", " + destString + ", , " + docType + Environment.NewLine;
            //outputContent = outputContent + awbNumber + "," + docType + "," + refNumber + "," + dateString + "," + weightNumber + "," + totalPiece + Environment.NewLine;
            return noError;
        }

        private void WriteToFile()
        {
            using (StreamWriter writer = new StreamWriter(LabelFullOutputDetail.Text))
            {
                writer.Write(outputContent);
                ShowInfoMessage(LabelFullOutputDetail.Text + " was created successfuly.");
            }
        }

        private void ButtonGenerate_Click(object sender, EventArgs e)
        {
            outputContent = "Ref, , AWB, Destination, , Goods" + Environment.NewLine;
            report = "";
            if (ofd.FileNames.Length < 1)
            {
                ShowErrorMessage("Please select PDFs to parse data!");
                return;
            }
            // Read the files
            foreach (String file in ofd.FileNames)
            {
                try
                {
                    ReadFromPDF(file);
                }
                catch (SecurityException ex)
                {
                    // The user lacks appropriate permissions to read files, discover paths, etc.
                    ShowErrorMessage("Security error. Please contact your administrator for details.\n\n" +
                        "Error message: " + ex.Message + "\n\n" +
                        "Details (send to Support):\n\n" + ex.StackTrace
                    );
                }
                catch (Exception ex)
                {
                    // Could not load the pdf - probably related to Windows file system permissions.
                    ShowErrorMessage("Cannot parse the PDF: " + file.Substring(file.LastIndexOf('\\'))
                        + ". You may not have permission to read the file, or " +
                        "it may be corrupt.\n\nReported error: " + ex.Message);
                }
            }

            bool createFile = false;
            if (report.Length == 0)
            {
                ShowInfoMessage("All files was parsed successfully.");
                createFile = true;
            }
            else
            {
                string dialog = "The following files were not parsed correctly:" + Environment.NewLine + report + Environment.NewLine;
                dialog = dialog + "Do you still want to create the file anyway?";
                DialogResult dialogResult = MessageBox.Show(dialog, "Please select", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dialogResult == DialogResult.Yes)
                {
                    createFile = true;
                }
            }
            if (createFile)
            {
                WriteToFile();
            }
            //ShowInfoMessage(outputContent);
        }
    }
}