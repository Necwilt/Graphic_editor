using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace Kursach_OOP
{
    public partial class Form2 : Form
    {
        private static string[] usbdrvdata;
        string name;
        DateTime startDate, ToDate;

        public Form2(string n, DateTime dt, DateTime odt)
        {
            InitializeComponent();

            name = n; startDate = dt; ToDate = odt;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if ((d.DriveType) == DriveType.Removable)
                    listBox1.Items.Add("Буква диска: " + d.Name + " назначена на съемный USB накопитель \n");
            }

            usbdrvdata = GetUSBSerialNumber();
            foreach (string str in usbdrvdata)
                listBox1.Items.Add(str);

            listBox1.Items.Add("Имя: " + name + "\tДата активации: " + startDate.ToShortDateString() + "\tДата окончания: " + ToDate.ToShortDateString());
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

        public static string[] GetUSBSerialNumber()
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
    }
}
