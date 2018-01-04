using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        private OpenFileDialog ofd;

        public FormMain()
        {
            InitializeComponent();
        }
        
        private void ButtonChoose_Click(object sender, EventArgs e)
        {
            
        }
    }
}
