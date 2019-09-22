using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TimedScreenshot
{
    public partial class Form1 : Form
    {

        public String execDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        public String directory;

        // From https://stackoverflow.com/questions/362986/capture-the-screen-into-a-bitmap (22-63)
        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "[^0-9]"))
            {
                MessageBox.Show("Please only enter integers.");
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "[^0-9]"))
            {
                MessageBox.Show("Please only enter integers.");
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "[^0-9]"))
            {
                MessageBox.Show("Please only enter integers.");
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
                if(folderBrowserDialog1.SelectedPath != null)
                {
                    directory = folderBrowserDialog1.SelectedPath;
                }
        }


        // Save to directory of executable
        // Check if data is valid
        private void button2_Click(object sender, EventArgs e)
        {
            if ((int.Parse(textBox1.Text) <= 24) && (int.Parse(textBox2.Text) <= 60) && (int.Parse(textBox3.Text) <= 60) && (directory != null))
            {
                String[] inputs = { textBox1.Text, textBox2.Text, textBox3.Text, directory };
                File.WriteAllLines(path: execDir + "/TS_Settings.cfg", contents: inputs);
                checkBox1.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please ensure that you've:\nSelected a save directory\nEntered a number of hours within 24\nEntered a number of minutes within 60\nAnd entered a number of seconds within 60");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkBox1.Enabled = false;
            if (File.Exists(execDir + "/TS_Settings.cfg")) {
                String[] inputs = File.ReadAllLines(execDir + "/TS_Settings.cfg");
                textBox1.Text = inputs[0];
                textBox2.Text = inputs[1];
                textBox3.Text = inputs[2];
                directory = inputs[3];
                checkBox1.Enabled = true;
            }
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
                if (checkBox1.Checked)
                {
                    timer1.Enabled = true;
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                }
                else
                {
                    timer1.Enabled = false;
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    MessageBox.Show("The program is now disabled. To re-enable, retick the checkbox.");
                }
            }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((System.DateTime.Now.Hour == int.Parse(textBox1.Text)) && (System.DateTime.Now.Minute == int.Parse(textBox2.Text)) && (System.DateTime.Now.Second == int.Parse(textBox3.Text)))
            {
                try
                {
                    String filename = directory + @"\Capture" + DateTime.UtcNow.Hour.ToString() + "_" + DateTime.UtcNow.Minute.ToString() + "_" + DateTime.UtcNow.Second.ToString();

                    //From https://stackoverflow.com/questions/362986/capture-the-screen-into-a-bitmap (184-194)
                    foreach (Screen screen in Screen.AllScreens)
                    {
                        DEVMODE dm = new DEVMODE();
                        dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
                        EnumDisplaySettings(screen.DeviceName, -1, ref dm);

                        using (Bitmap bmp = new Bitmap(dm.dmPelsWidth, dm.dmPelsHeight))
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.CopyFromScreen(dm.dmPositionX, dm.dmPositionY, 0, 0, bmp.Size);
                            bmp.Save(filename + "_" + screen.DeviceName.Split('\\').Last() + ".png");
                            //MessageBox.Show("Captured to " + filename + "_" + screen.DeviceName.Split('\\').Last() + ".png");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        // From https://stackoverflow.com/questions/4410717/c-sharp-programmatically-unminimize-form (207-233)
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);


        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        public void RestoreFromMinimzied(Form form)
        {
            const int WPF_RESTORETOMAXIMIZED = 0x2;
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(form.Handle, ref placement);

            if ((placement.flags & WPF_RESTORETOMAXIMIZED) == WPF_RESTORETOMAXIMIZED)
                form.WindowState = FormWindowState.Maximized;
            else
                form.WindowState = FormWindowState.Normal;
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreFromMinimzied(this);
            this.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            System.Environment.Exit(1);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.ShowInTaskbar = false;
            this.Hide();
        }
    }
}
