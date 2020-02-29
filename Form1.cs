using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using MySql.Data.MySqlClient;

namespace SAD_2E_Project
{
    public partial class Form1 : Form
    {
        SerialPort serialPort = new SerialPort();

        string connectionString = @"server-localhost; user=; password=; database=";
        MySqlConnection Connection;
        string tableName = "";

        bool LoadImageStr = false;
        string IDRam;
        string IMG_FileNameInput;
        string StatusInput = "Save";
        string SqlCmdSearchStr;
        DataGridView DataGridView1;
        

        public string StrSerialIn;
        bool GetID = false;
        bool ViewUserData = false;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            PanelRegistrationEditUserData.Visible = false;
            PanelUserData.Visible = false;
            PanelConnection.Visible = true;
        }

        private void showData()
        {
            try
            {
                Connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Failed! \n Please check if server is turned on!");
            }

            try
            {
                if (LoadImageStr == false)
                {
                    MySqlCommand command = new MySqlCommand();
                    command.CommandType = CommandType.Text;
                    command.CommandText = "Select Name, Age, Sex, Contact_Person, Birthday, Medical_Condition FROM" + tableName + " ORDER BY Name";
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command.CommandText, Connection);

                    DataTable DT = new DataTable();
                    int Data = dataAdapter.Fill(DT);
                    if (Data > 0)
                    {
                        DataGridView1.DataSource = null;
                        DataGridView1.DataSource = DT;
                        // DataGridView1.Columns(2).DefaultCellStyle.Format = "c"
                        DataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                        dataGridView1.ClearSelection();
                    }
                    else
                        DataGridView1.DataSource = DT;
                }
                else
                {
                    MySqlCommand command = new MySqlCommand();
                    command.CommandType = CommandType.Text;
                    command.CommandText = "Select Images FROM" + tableName + "WHERE ID LIKE '" + IDRam + "'";
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command.CommandText, Connection);

                    DataTable DT = new DataTable();
                    int Data = dataAdapter.Fill(DT);
                    if (Data > 0)
                    {
                        byte[] imgArray = (byte[])DT.Rows[0]["Images"];
                        System.IO.MemoryStream imgStr = new System.IO.MemoryStream(imgArray);
                        pictureBox1.Image = System.Drawing.Image.FromStream(imgStr);
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        imgStr.Close();
                    }
                    LoadImageStr = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load database!");
            }
            Connection.Close();
        }

        private void showDataUser()
        {
            try
            {
                Connection.Open();
            }
            catch
            {
                MessageBox.Show("Connection Failed! \n Please check if server is turned on!");
            }

            try
            {
                MySqlCommand command = new MySqlCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "Select * FROM " + tableName + " WHERE ID LIKE '" + LabelGetID.Text.Substring(5, LabelGetID.Text.Length -5) + "'";
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command.CommandText, Connection);

                DataTable DT = new DataTable();
                int Data = dataAdapter.Fill(DT);
                if (Data > 0)
                {
                    byte[] imgArray = (byte[])DT.Rows[0]["Images"];
                    System.IO.MemoryStream imgStr = new System.IO.MemoryStream(imgArray);
                    pictureBox1.Image = System.Drawing.Image.FromStream(imgStr);
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    imgStr.Close();

                    LabelGetID.Text = (string)DT.Rows[0]["ID"];
                    labelName.Text = (string)DT.Rows[0]["Name"];
                    labelAge.Text = (string)DT.Rows[0]["Age"];
                    labelSex.Text = (string)DT.Rows[0]["Sex"];
                    labelCont.Text = (string)DT.Rows[0]["Contact_Person"];
                    labelBDay.Text = (string)DT.Rows[0]["Birthday"];
                    labelMedical.Text = (string)DT.Rows[0]["Medical_Conditions"];
                }
                else
                    MessageBox.Show("ID not found! \n Please Register your ID!");
            }

            catch 
            {
                MessageBox.Show("Failed to load database!");
                Connection.Close();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

   

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (buttonConnect.Text == "Connect")
            {
       
                try
                {
                    SerialPort serialPort = new SerialPort(comboBoxPorts.SelectedItem.ToString(),
                                                       115200,
                                                       Parity.None,
                                                       8,
                                                       StopBits.One);
                    serialPort.Open();
                    buttonConnect.Text = "Disconnect";
                    labelConnectionStatus.Text = "CONNECTED";
                    labelConnectionStatus.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to connect!");
                    labelConnectionStatus.Text = "DISCONNECTED";
                    labelConnectionStatus.ForeColor = Color.Red;
                }

            }

            if (buttonConnect.Text == "Disconnect")
            {
                labelConnectionStatus.Text = "DISCONNECTED";
                labelConnectionStatus.ForeColor = Color.Red;
                buttonConnect.Text = "Connect";
                serialPort.Close();
            }
            PanelRegistrationEditUserData.Visible = false;
            PanelUserData.Visible = false;
            PanelConnection.Visible = true;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)    /// Clear Button
        {
            labelName.Text  = "Waiting...";
            labelAge.Text = "Waiting...";
            labelSex.Text = "Waiting...";
            labelCont.Text = "Waiting...";
            labelBDay.Text = "Waiting...";
            labelMedical.Text = "Waiting...";
            pictureBox1.Image = null;           //Not Sure
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void PictureBoxImageInput_Click(object sender, EventArgs e)
        {

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            PanelRegistrationEditUserData.Visible = false;
            PanelUserData.Visible = false;
            PanelConnection.Visible = true;
        }

        private void buttonUData_Click(object sender, EventArgs e)
        {
            PanelRegistrationEditUserData.Visible = false;
            PanelUserData.Visible = true;
            PanelConnection.Visible = false;
        }

        private void buttonRegedit_Click(object sender, EventArgs e)
        {
            StrSerialIn = "";

            PanelRegistrationEditUserData.Visible = true;
            PanelUserData.Visible = false;
            PanelConnection.Visible = false;
            showData();
        }

        private void PanelUserData_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            try
            {
                comboBoxPorts.Items.Clear();
                string[] ports = SerialPort.GetPortNames();
                comboBoxPorts.DataSource = ports;
            }
            
            catch(Exception ex)
            {
                MessageBox.Show("COM Port not detected");
                //comboBoxPorts.Items.Clear();
            }
        }

        private void labelName_Click(object sender, EventArgs e)
        {

        }

        private void labelAge_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
