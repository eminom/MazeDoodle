using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MazeDoodle
{
    public partial class NewForm : Form
    {
        public NewForm()
        {
            InitializeComponent();
            CenterToParent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            row_ = int.Parse(rowTextBox_.Text);
            column_ = int.Parse(columnTextBox_.Text);
            this.DialogResult = DialogResult.OK;
            Close();
        }

        public int RowCount
        {
            get
            {
                return row_;
            }
        }

        public int ColumnCount
        {
            get
            {
                return column_;
            }
        }

        private int row_=0;

        private int column_ = 0;
    }
}
