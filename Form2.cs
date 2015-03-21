using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace testdm
{
    public partial class form2 : Form
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);
        public form2()
        {
            InitializeComponent();
        }
        private void form2_Load(object sender, EventArgs e)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(Form1.strSec, "loginpassword", "", temp, 255, Form1.strFilePath);
            textBox_key.Text = temp.ToString();
        }
        private void textBox_key_TextChanged(object sender, EventArgs e)
        {
            Form1.key = textBox_key.Text;
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            if (Form1.a.Bind(Form1.key))
            {
                MessageBox.Show("成功");
                Form1.loginSuccess = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(Form1.a.LastError);
                Form1.loginSuccess = false;
            }
        }

    }
}