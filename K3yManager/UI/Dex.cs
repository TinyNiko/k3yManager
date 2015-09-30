using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace K3yManager.UI
{
    public partial class Dex : Form
    {
        private string filepath; 
        public Dex(string path)
        {
            filepath = path; 
            InitializeComponent();
        }
    }
}
