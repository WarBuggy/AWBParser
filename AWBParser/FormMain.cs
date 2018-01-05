using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        string PACKAGE_TYPE_DOC = "DOC";
        string PACKAGE_TYPE_PAK = "DOX";


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
                currentFile += PdfTextExtractor.GetTextFromPage(reader, 1);
                reader.Close();
                //showInfoMessage(currentFile);
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
            string docType = "";
            string awbNumber = "";
            string refNumber = "";
            string weightNumber = "";
            string totalPiece = "";
            bool noError = true;

            using (StringReader reader = new StringReader(fileContent))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (String.Compare(line, PACKAGE_TYPE_DOC, true) == 0 || String.Compare(line, PACKAGE_TYPE_PAK, true) == 0)
                    {
                        docType = line;
                    }
                    else if (line.Contains("Ref:"))
                    {
                        string[] refLine = line.Split(':');
                        if (refLine.Length == 2)
                        {
                            refNumber = refLine[1];
                        }
                        else
                        {
                            noError = false;
                        }

                        if ((line = reader.ReadLine()) != null)
                        {
                            string[] weightLine = line.Split('/');
                            int requiredIndex = weightLine.Length - 1;
                            if (requiredIndex == 1 || requiredIndex == 0)
                            {
                                string[] weight = weightLine[requiredIndex].Split(' ');
                                if (weight.Length == 2)
                                {
                                    weightNumber = weight[0];
                                    if ((line = reader.ReadLine()) != null)
                                    {
                                        string[] pcsLine = line.Split('/');
                                        if (pcsLine.Length == 2)
                                        {
                                            totalPiece = pcsLine[1];
                                        }
                                        else
                                        {
                                            noError = false;
                                        }
                                    }
                                }
                                else
                                {
                                    noError = false;
                                }
                            }
                            else
                            {
                                noError = false;
                            }
                        }
                    }
                    else if (line.Contains("WAYBILL "))
                    {
                        string[] waybillLine = line.Split(" ".ToCharArray(), 2);
                        if (waybillLine.Length == 2)
                        {
                            awbNumber = waybillLine[1];
                            //showInfoMessage(awbNumber);
                        }
                        else
                        {
                            noError = false;
                        }
                    }
                }
                outputContent = outputContent + awbNumber + "," + docType + "," + refNumber + "," + weightNumber + "," + totalPiece + Environment.NewLine;
            }

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
            outputContent = "AWB,Type,Ref,Weight,Pieces" + Environment.NewLine;
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
            //showInfoMessage(outputContent);
        }
    }
}