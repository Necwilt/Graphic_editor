using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LicManager;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Xml;
using LicenseKeyValidator;

namespace Kursach_OOP
{

    public partial class Form1 : Form, IFunctionalityManager
    {
        //public class LicenseManager
        //{
        //    private const string keyPath = "SOFTWARE\\Kursach_OOP";
        //    private const string valueName = "LicenseKey";

        //    public string LicenseKey { get; private set; }
        //    public bool IsLicenseValid { get; private set; }
        //    public bool IsAdditionalFunctionalityEnabled { get; private set; }

        //    public LicenseManager(Form1 myForm)
        //    {
        //        LicenseKey = ReadLicenseKeyFromRegistry();
        //        IsLicenseValid = ValidateLicenseKey(LicenseKey);
        //        EnableDisableFunctionality(myForm);
        //    }

        //    public void SetLicenseKey(string licenseKey, Form1 myForm)
        //    {
        //        LicenseKey = licenseKey;
        //        IsLicenseValid = ValidateLicenseKey(licenseKey);
        //        WriteLicenseKeyToRegistry(LicenseKey);
        //        EnableDisableFunctionality(myForm);
        //    }

        //    public void RemoveLicenseKey()
        //    {
        //        LicenseKey = null;
        //        IsLicenseValid = false;
        //        DeleteLicenseKeyFromRegistry();
        //    }

        //    public void CheckFunctionality(Form1 myForm)
        //    {
        //        EnableDisableFunctionality(myForm);
        //    }

        //    private string ReadLicenseKeyFromRegistry()
        //    {
        //        try
        //        {
        //            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath))
        //            {
        //                return key?.GetValue(valueName)?.ToString();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Ошибка при чтении ключа из реестра: " + ex.Message);
        //            return null;
        //        }
        //    }

        //    private int WriteLicenseKeyToRegistry(string licenseKey)
        //    {
        //        try
        //        {
        //            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath))
        //            {
        //                key.SetValue(valueName, licenseKey);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Ошибка при записи ключа в реестр: " + ex.Message);
        //        }
        //        return 1;
        //    }

        //    private bool ValidateLicenseKey(string licenseKey)
        //    {
        //        // Вызов функции библиотеки или внешнего компонента для проверки валидности ключа
        //        bool isValid = LicenseKeyValid.ValidateLicenseKey(licenseKey);

        //        return isValid;
        //    }

        //    private void DeleteLicenseKeyFromRegistry()
        //    {
        //        try
        //        {
        //            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath, true))
        //            {
        //                key?.DeleteValue(valueName, false);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Ошибка при удалении ключа из реестра: " + ex.Message);
        //        }
        //    }

        //    private bool EnableDisableFunctionality(Form1 myForm)
        //    {
        //        if (IsLicenseValid)
        //        {
        //            // Разблокировка элементов, если функционал разрешен
        //            myForm.градиентнаяToolStripMenuItem.Enabled = true;
        //            myForm.кисть3ToolStripMenuItem.Enabled = true;

        //            return true;
        //        }
        //        else
        //        {
        //            // Если лицензия недействительна, дополнительный функционал блокируется
        //            myForm.градиентнаяToolStripMenuItem.Enabled = false;
        //            myForm.кисть3ToolStripMenuItem.Enabled = false;

        //            return false;
        //        }
        //    }

        //}
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessageTimeout(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, out IntPtr lpdwResult);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, StringBuilder lParam);

        public Form1()
        {
            InitializeComponent();
            loadPictureBox();
            this.WindowState = FormWindowState.Maximized;
            selectedShapeButton = PenButton;
        }

        private bool isMouse = false;
        //private MyLicenseManager licenseManager;
        Point startPoint;
        Graphics graphics;
        Bitmap map;
        Button maincolor;
        Button eraserButton;
        Pen pen = new Pen(Color.Black, 3f);

        private void loadPictureBox()
        {
            Rectangle rectangle = Screen.PrimaryScreen.Bounds;
            map = new Bitmap(rectangle.Width, rectangle.Height);
            graphics = Graphics.FromImage(map);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            pictureBox1.Image = map;

            pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            pictureBox1.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
            pictureBox1.MouseUp += new MouseEventHandler(pictureBox1_MouseUp);
            maincolor = button13;
        }

        Bitmap workingImage;
        Graphics workingGraphics;
        Point lastPoint;

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;

        }
        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVTYP_VOLUME = 0x00000002;
        private static string[] usbInfo;
        public List<string> usbInfoCopy = new List<string>();
        public const int WM_COPYDATA = 0x4A;
        List<string> usbLiteral = new List<string>();
        List<string> usbSereal = new List<string>();
        public bool license = false;
        public string name { get; private set; }
        public DateTime startDate { get; private set; }
        public DateTime updateTo { get; private set; }

        private bool LoadLicense(string literal)
        {
            try
            {
                string File = literal + "license.xml";
                if (!System.IO.File.Exists(File))
                {
                    throw new ApplicationException("Копия программы не лицензирована! Не найден файл лицензии License.xml.\n");
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(File);
                string sig1;
                string Signature;
                try
                {
                    string Name = doc.ChildNodes[0].SelectSingleNode(@"/license/Name", null).InnerText;
                    string StartDate = doc.ChildNodes[0].SelectSingleNode(@"/license/Date", null).InnerText;
                    string UpdateTo = doc.ChildNodes[0].SelectSingleNode(@"/license/UpdateTo", null).InnerText;
                    Signature = doc.ChildNodes[0].SelectSingleNode(@"/license/Signature", null).InnerText;
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(Name + StartDate + UpdateTo + "HaLoh");
                    byte[] hash = md5.ComputeHash(data);
                    sig1 = Convert.ToBase64String(hash);
                    this.name = Name;
                    this.startDate = Convert.ToDateTime(StartDate);
                    this.updateTo = Convert.ToDateTime(UpdateTo);
                }
                catch (Exception)
                {
                    throw new ApplicationException("Копия программы не лицензирована!\nОшибка чтения файла лицензии!\nОбратитесь к автору.");
                }

                if (sig1 != Signature)
                {
                    throw new ApplicationException("Копия программы не лицензирована!\nОшибка чтения файла лицензии!\nОбратитесь к автору.");
                }
                if (DateTime.Now < this.startDate)
                {
                    throw new ApplicationException(string.Format("Копия программы не лицензирована!\nСрок действия лицензии еще не начался. Начало {0}\n.", startDate.ToShortDateString()));
                }
                if (DateTime.Now > this.updateTo)
                {
                    throw new ApplicationException(string.Format("Копия программы не лицензирована!\nСрок действия лицензии истёк. Окончание {0}\n.", updateTo.ToShortDateString()));
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == WM_DEVICECHANGE) //WM_DEVICECHANGE
            {
                switch ((int)msg.WParam)
                {
                    case DBT_DEVICEARRIVAL:

                        LoadUSB();

                        usbInfoCopy = usbInfo.ToList();

                        GoToLog("", (IntPtr)2);
                        usbInfo = GetUsbInfo();
                        foreach (string s in usbInfo)
                            GoToLog(s, (IntPtr)0);

                        CreateLog();

                        break;

                    case DBT_DEVICEREMOVECOMPLETE:

                        license = false;
                        LoadUSB();

                        usbInfoCopy = usbInfo.ToList();

                        GoToLog("", (IntPtr)2);
                        usbInfo = GetUsbInfo();
                        foreach (string s in usbInfo)
                            GoToLog(s, (IntPtr)0);

                        CreateLog();

                        break;

                    default:
                        //
                        break;
                }
            }
            else base.WndProc(ref msg);
        }

        private void устройствоНеПодключеноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuStrip1.Items[3].Text == "Устройство подключено")
            {
                Form2 form2 = new Form2(name, startDate, updateTo);
                form2.ShowDialog();
            }
        }

        private void GetLiteraUSB()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if ((d.DriveType) == DriveType.Removable)
                    usbLiteral.Add(d.Name);
            }
        }

        public void GetUSBSerialNumber()
        {
            //mutex.WaitOne();
            try
            {
                string query = "SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

                List<string> usbDevdata = new List<string>();

                foreach (ManagementObject diskDrive in searcher.Get().Cast<ManagementObject>())
                {
                    // Получаем строку идентификации устройства 
                    string DeviceIDstr = (string)diskDrive["PNPDeviceID"];
                    // получив строку

                    string Serial = Retrieve_serial(DeviceIDstr);

                    usbSereal.Add(Serial);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //mutex.ReleaseMutex();
        }

        private static string[] GetUsbInfo()
        {
            string query = "SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            List<string> usbDevdata = new List<string>();

            foreach (ManagementObject diskDrive in searcher.Get().Cast<ManagementObject>())
            {
                // Получаем строку идентификации устройства 
                string DeviceIDstr = (string)diskDrive["PNPDeviceID"];
                // получив строку

                string vendor = Retrieve_FleshDRVVendor(DeviceIDstr);
                string name = Retrieve_FleshDRVName(DeviceIDstr);
                string Serial = Retrieve_serial(DeviceIDstr);


                string usbData = "Производитель:" + vendor + "   Название:" + name + "   Серийный номер:" + Serial;

                if (usbDevdata.IndexOf(usbData) < 0)
                    usbDevdata.Add(usbData);
            }

            return usbDevdata.ToArray();
        }

        public static string Retrieve_serial(string strSource)
        {
            // USBSTOR\DISK&VEN_KINGSTON&PROD_DATATRAVELER_2.0&REV_PMAP\5B661B004102&0
            // Находим фрагмент отвечающий за серийный номер типично находится
            // в самом конце, после &REV_ или еще дополнительной информации
            // поэтому выполняем реверсирование строки и 
            string strStart = "\\";
            int Start, End;
            Start = strSource.LastIndexOf(strStart) + strStart.Length;
            End = strSource.IndexOf("&0", Start);
            string serial = strSource.Substring(Start, End - Start);
            return serial;
        }

        public static string Retrieve_FleshDRVVendor(string strSource)
        {
            // получив строку
            // USBSTOR\DISK&VEN_&PROD_DATATRAVELER_2.0&REV_PMAP\5B661B004102&0
            // Находим фрагмент отвечающий за производителя &VEN_
            string strStart = "&VEN_";
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf("&", Start);
            string vendor = strSource.Substring(Start, End - Start);
            if (vendor.Length < 1)
                vendor = "Unknown";
            return vendor;
        }

        public static string Retrieve_FleshDRVName(string strSource)
        {
            // получив строку
            // USBSTOR\DISK&VEN_KINGSTON&PROD_DATATRAVELER_2.0&REV_PMAP\5B661B004102&0
            // Находим фрагмент отвечающий за идентификатор &PROD_
            string strStart = "&PROD_";
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf("&", Start);
            string Name = strSource.Substring(Start, End - Start);
            if (Name.Length < 1)
                Name = "Unknown";
            return Name;
        }

        private void LoadUSB()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            if (allDrives != null)
            {
                GetUSBSerialNumber();
                GetLiteraUSB();

                for (int i = 0; i < usbSereal.Count; i++)
                    if (usbSereal[i] == "04I7DFD36H3VC1J1")
                    {
                        license = LoadLicense(usbLiteral[i]);
                        break;
                    }
            }

            if (license)
                menuStrip1.Items[3].Text = "Устройство подключено";
            else
                menuStrip1.Items[3].Text = "Устройство отключено";
        }

        private void GoToLog(string str, IntPtr lb)
        {
            COPYDATASTRUCT copyData = new COPYDATASTRUCT();

            copyData.dwData = lb;

            byte[] txt = Encoding.UTF8.GetBytes(str);
            copyData.cbData = txt.Length;
            copyData.lpData = Marshal.AllocHGlobal(copyData.cbData);

            Marshal.Copy(txt, 0, copyData.lpData, txt.Length);

            try
            {
                IntPtr handl = FindWindow(null, "Logs");
                if (handl != null)
                {
                    int res;

                    SendMessage(handl, WM_COPYDATA, IntPtr.Zero, ref copyData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateLog()
        {
            if (usbInfo.Length > usbInfoCopy.Count)
            {
                for (int i = 0; i < usbInfo.Length; i++)
                {
                    if (usbInfoCopy.Count - 1 < i)
                    {
                        GoToLog(DateTime.Now.ToString() + " устройство вставлено " + usbInfo[i], (IntPtr)1);
                        break;
                    }
                    else if (usbInfoCopy[i] != usbInfo[i])
                    {
                        GoToLog(DateTime.Now.ToString() + " устройство вставлено " + usbInfo[i], (IntPtr)1);
                        break;
                    }
                }
            }
            else if (usbInfo.Length < usbInfoCopy.Count)
            {
                for (int i = 0; i < usbInfoCopy.Count; i++)
                {
                    if (usbInfo.Length - 1 < i)
                    {
                        GoToLog(DateTime.Now.ToString() + " устройство удалено " + usbInfoCopy[i], (IntPtr)1);
                        break;
                    }
                    else if (usbInfoCopy[i] != usbInfo[i])
                    {
                        GoToLog(DateTime.Now.ToString() + " устройство удалено " + usbInfoCopy[i], (IntPtr)1);
                        break;
                    }
                }
            }
        }

        private void DrawShapeInWorkingImage(Point currentPoint)
        {
            workingImage = new Bitmap(map);
            workingGraphics = Graphics.FromImage(workingImage);

            int startPointX = lastPoint.X < currentPoint.X ? lastPoint.X : currentPoint.X;
            int startPointY = lastPoint.Y < currentPoint.Y ? lastPoint.Y : currentPoint.Y;

            int shapeWidth = (lastPoint.X > currentPoint.X ? lastPoint.X : currentPoint.X) - startPointX;
            int shapeHeight = (lastPoint.Y > currentPoint.Y ? lastPoint.Y : currentPoint.Y) - startPointY;

            switch (selectedShapeButton.Name)
            {
                case "RectangleButton":
                    if (!FillColorcheckBox.Checked)
                        workingGraphics.DrawRectangle(pen, startPointX, startPointY, shapeWidth, shapeHeight);
                    else
                        workingGraphics.FillRectangle(pen.Brush, startPointX, startPointY, shapeWidth, shapeHeight);
                    break;
                case "LineButton":
                    workingGraphics.DrawLine(pen, lastPoint, currentPoint);
                    break;
                case "CircleButton":
                    if (!FillColorcheckBox.Checked)
                        workingGraphics.DrawEllipse(pen, startPointX, startPointY, shapeWidth, shapeHeight);
                    else
                        workingGraphics.FillEllipse(pen.Brush, startPointX, startPointY, shapeWidth, shapeHeight);
                    break;
                case "TriangleButton":
                    Point point1 = new Point() { X = startPointX, Y = startPointY + shapeHeight };
                    Point point2 = new Point() { X = startPointX + (shapeWidth / 2), Y = startPointY };
                    Point point3 = new Point() { X = startPointX + shapeWidth, Y = startPointY + shapeHeight };

                    if (!FillColorcheckBox.Checked)
                        workingGraphics.DrawPolygon(pen, new Point[] { point1, point2, point3 });
                    else
                        workingGraphics.FillPolygon(pen.Brush, new Point[] { point1, point2, point3 });
                    break;
            }

            pictureBox1.Image = workingImage;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (maincolor == button13)
                pen.Color = button13.BackColor;
            else
                if (maincolor == button14)
                pen.Color = button14.BackColor;

            if (eraserButton == button15)
                pen.Color = Color.White;

            lastPoint = e.Location;
            isMouse = true;

            if (selectedToolStripButton == градиентнаяToolStripMenuItem && eraserButton != button15)
            {
                brush = new LinearGradientBrush(this.startPoint, new Point(e.X, e.Y), button13.BackColor, button14.BackColor);
                pen.Brush = brush;
            }

            if (selectedToolStripButton == кистьToolStripMenuItem1 && eraserButton != button15)
            {
                if (maincolor == button13)
                    brush = new SolidBrush(button13.BackColor);
                else
                    if (maincolor == button14)
                    brush = new SolidBrush(button14.BackColor);
                pen.DashStyle = DashStyle.Solid;
                pen.Brush = brush;
            }
            if (selectedToolStripButton == кисть3ToolStripMenuItem && eraserButton != button15)
            {
                HatchBrush myHatchBrush = new HatchBrush(HatchStyle.Vertical, button13.BackColor, button14.BackColor);
                pen.Brush = myHatchBrush;
            }
            if (selectedToolStripButton == пунктирToolStripMenuItem && eraserButton != button15)
            {
                pen.DashStyle = DashStyle.DashDot;
            }
        }

        Brush brush;

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouse = false;

            if (selectedShapeButton != PenButton)
            {
                map = new Bitmap(workingImage);

                graphics = Graphics.FromImage(map);
                pictureBox1.Image = map;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isMouse) { return; }

            if (selectedShapeButton == PenButton)
            {
                DrawLines(e.Location);
            }
            else
            {
                DrawShapeInWorkingImage(e.Location);
            }
        }

        private void DrawLines(Point currentPoint)
        {
            {
                graphics.DrawLine(pen, lastPoint, currentPoint);
                lastPoint = currentPoint;
                pictureBox1.Refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (maincolor == button13)
                button13.BackColor = ((Button)sender).BackColor;
            else
                if (maincolor == button14)
                button14.BackColor = ((Button)sender).BackColor;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                if (maincolor == button13)
                {
                    button13.BackColor = colorDialog1.Color;
                    ((Button)sender).BackColor = colorDialog1.Color;
                }
                else
                    if (maincolor == button14)
                {
                    button14.BackColor = colorDialog1.Color;
                    ((Button)sender).BackColor = colorDialog1.Color;
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphics.Clear(pictureBox1.BackColor);
            pictureBox1.Image = map;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            pen.Width = trackBar1.Value;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!license)
            {
                MessageBox.Show("Отсутствие лицензии", "ошибка");
                return;
            }

            saveFileDialog1.Filter = "PNG(*.PNG)|*.png";
            if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Save(saveFileDialog1.FileName);
                }
            }
            else return;
        }

    private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                if (pictureBox1.Image != null)
                {
                    graphics.Clear(Color.White);
                    graphics.DrawImage(Image.FromFile(openFileDialog1.FileName), new Point(0, 0));
                    pictureBox1.Invalidate();
                }
            }
            else return;
        }

        private void LineButton_Click(object sender, EventArgs e)
        {
            selectedShapeButton = LineButton;
        }

        Button selectedShapeButton;
        ToolStripMenuItem selectedToolStripButton;

        private void PenButton_Click(object sender, EventArgs e)
        {
            selectedShapeButton = PenButton;
            eraserButton = null;
            if (maincolor == button13)
                pen.Color = button13.BackColor;
            else
                if (maincolor == button14)
                pen.Color = button14.BackColor;
        }

        private void RectangleButton_Click(object sender, EventArgs e)
        {
            selectedShapeButton = RectangleButton;
        }

        private void CircleButton_Click(object sender, EventArgs e)
        {
            selectedShapeButton = CircleButton;
        }

        private void TriangleButton_Click(object sender, EventArgs e)
        {
            selectedShapeButton = TriangleButton;
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 a = new AboutBox1();
            a.ShowDialog();
        }

        private void кистьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            selectedToolStripButton = кистьToolStripMenuItem1;
        }

        private void пунктирToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedToolStripButton = пунктирToolStripMenuItem;
        }

        private void кисть3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedToolStripButton = кисть3ToolStripMenuItem;
        }

        private void градиентнаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedToolStripButton = градиентнаяToolStripMenuItem;

        }

        private void button13_Click(object sender, EventArgs e)
        {
            maincolor = button13;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            maincolor = button14;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            eraserButton = button15;
        }

        bool UpdateInForm;
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            UpdateInForm = true;
        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (UpdateInForm)
            {
                DialogResult result = MessageBox.Show("Сохранить изменения?", "Внимание", MessageBoxButtons.YesNoCancel);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    saveFileDialog1.Filter = "PNG(*.PNG)|*.png";
                    if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)
                    {
                        if (pictureBox1.Image != null)
                        {
                            pictureBox1.Image.Save(saveFileDialog1.FileName);
                        }
                    }
                }
                else if (result == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
                else e.Cancel = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadUSB();

            GoToLog("", (IntPtr)2);
            usbInfo = GetUsbInfo();
            foreach (string s in usbInfo)
                GoToLog(s, (IntPtr)0);

            //licenseManager = new MyLicenseManager(this);
            //string licenseKey = licenseManager.LicenseKey;
            //bool isLicenseValid = licenseManager.IsLicenseValid;
            //bool isLicenseValid = LicenseKeyValid.ValidateLicenseKey(licenseKey);
        }

        private void ввестиЛицензионныйКлючToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Создание и отображение диалогового окна для ввода ключа
            //using (var dialog = new LicenseKeyInputDialog())
            //{
            //    if (dialog.ShowDialog() == DialogResult.OK)
            //    {
            //        string licenseKey = dialog.LicenseKey;

            //        licenseManager.SetLicenseKey(licenseKey);
            //        if (licenseManager.IsLicenseValid)
            //            MessageBox.Show("Лицензионный ключ успешно введен!", "Успех");
            //        else
            //            MessageBox.Show("Неверный лицензионный ключ.", "Ошибка");
            //    }
            //}
        }

        private void удалитьЛицензионныйКлючToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //licenseManager.RemoveLicenseKey();
        }

        public void EnableFunctionality()
        {
            градиентнаяToolStripMenuItem.Enabled = true;
            кисть3ToolStripMenuItem.Enabled = true;
        }

        public void DisableFunctionality()
        {
            градиентнаяToolStripMenuItem.Enabled = false;
            кисть3ToolStripMenuItem.Enabled = false;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (menuStrip1.Items[3].Text == "Устройство подключено")
            {
                Form2 form2 = new Form2(name, startDate, updateTo);
                form2.ShowDialog();
            }
        }
    }

    public class LicenseKeyInputDialog : Form
    {
        private TextBox licenseKeyTextBox;
        private Button okButton;
        private Button cancelButton;

        public string LicenseKey { get; private set; }

        public LicenseKeyInputDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.licenseKeyTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // licenseKeyTextBox
            // 
            this.licenseKeyTextBox.Location = new System.Drawing.Point(30, 30);
            this.licenseKeyTextBox.Name = "licenseKeyTextBox";
            this.licenseKeyTextBox.Size = new System.Drawing.Size(240, 22);
            this.licenseKeyTextBox.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(61, 70);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(157, 70);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Отмена";
            // 
            // LicenseKeyInputDialog
            // 
            this.ClientSize = new System.Drawing.Size(300, 120);
            this.Controls.Add(this.licenseKeyTextBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LicenseKeyInputDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ввод лицензионного ключа";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            LicenseKey = licenseKeyTextBox.Text;
        }
    }
}
