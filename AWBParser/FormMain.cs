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
                // Read the files
                foreach (String file in ofd.FileNames)
                {
                    try
                    {

                    }
                    catch (SecurityException ex)
                    {
                        // The user lacks appropriate permissions to read files, discover paths, etc.
                        showErrorMessage("Security error. Please contact your administrator for details.\n\n" +
                            "Error message: " + ex.Message + "\n\n" +
                            "Details (send to Support):\n\n" + ex.StackTrace
                        );
                    }
                    catch (Exception ex)
                    {
                        // Could not load the pdf - probably related to Windows file system permissions.
                        showErrorMessage("Cannot parse the PDF: " + file.Substring(file.LastIndexOf('\\'))
                            + ". You may not have permission to read the file, or " +
                            "it may be corrupt.\n\nReported error: " + ex.Message);
                    }
                }

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

        private void showWarningMessage(string message)
        {
            MessageBox.Show(null, message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }

        private void showErrorMessage(string message)
        {
            MessageBox.Show(null, message, "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        private void showInfoMessage(string message)
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
                showErrorMessage("Invalid character for a file name found.");
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
    }
}