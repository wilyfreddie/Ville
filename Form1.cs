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

        private void ClearInputUpdateData()
        {
            TextBoxName.Text = "";
            TextBoxAge.Text = "";
            TextBoxSex.Text = "";
            textBoxContactPerson.Text = "";
            TextBoxBirthday.Text = "";
            TextBoxMedCon.Text = "";
            LabelGetID.Text = "";
            PictureBoxImageInput.Paint += new PaintEventHandler((sender, e) =>
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                string Text = "Click here \n to \n browse image.";
                SizeF textSize = e.Graphics.MeasureString(Text, Font);
                PointF locationToDraw = new PointF();
                locationToDraw.X = (PictureBoxImageInput.Width / 2) - (textSize.Width / 2);
                locationToDraw.Y = (PictureBoxImageInput.Height / 2) - (textSize.Height/2);
                e.Graphics.DrawString(Text, Font, Brushes.Black, locationToDraw);
     
            });


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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                PictureBoxImageInput.Image = new Bitmap(openFileDialog.FileName);
                PictureBoxImageInput.ImageLocation = openFileDialog.FileName;      
            }
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
            //showData();
            ClearInputUpdateData();
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

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            System.IO.MemoryStream mstream = new System.IO.MemoryStream();

            byte[] arrImage;

            if(TextBoxName.Text == "")
            {
                MessageBox.Show("Name cannot be empty!");
                return;
            }
            if (TextBoxAge.Text == "")
            {
                MessageBox.Show("Age cannot be empty!");
                return;
            }
            if (TextBoxSex.Text == "")
            {
                MessageBox.Show("Sex cannot be empty!");
                return;
            }
            if (textBoxContactPerson.Text == "")
            {
                MessageBox.Show("Contact Person cannot be empty!");
                return;
            }
            if (TextBoxBirthday.Text == "")
            {
                MessageBox.Show("Birthday cannot be empty!");
                return;
            }
            if (TextBoxMedCon.Text == "")
            {
                MessageBox.Show("Medical Conditions cannot be empty!");
                return;
            }

            if(StatusInput == "Save")
            {
                if(PictureBoxImageInput.ImageLocation != "")
                {
                    PictureBoxImageInput.Image.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    arrImage = mstream.GetBuffer();
                }
                else
                {
                    MessageBox.Show("Image has not been selected!");
                }

                try
                {
                    Connection.Open();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Connection failed! \n Please check server is turned on!");
                    return;
                }

                try
                {
                    MySqlCommand command = new MySqlCommand();
                    command.CommandText = "INSERT INTO " + tableName + "(Name, ID, Age, Sex, Contact_Person, Birthday, Medical Condition, Images) VALUES (@name, @ID, @Age, @Sex, @Contact_Person, @Birthday, @Medical_Conditions, @Images)";
                    command.Connection = Connection;
                    command.Parameters.AddWithValue("@Name", TextBoxName.Text);
                    command.Parameters.AddWithValue("@ID", LabelGetID.Text);
                    command.Parameters.AddWithValue("@Age", TextBoxAge.Text);
                    command.Parameters.AddWithValue("@Sex", TextBoxSex.Text);
                    command.Parameters.AddWithValue("@Contact_Person", textBoxContactPerson.Text);
                    command.Parameters.AddWithValue("@Birthday", TextBoxBirthday.Text);
                    command.Parameters.AddWithValue("@Medical_Conditions", TextBoxMedCon.Text);
                    //command.Parameters.AddWithValue("@Images", arrImage);
                    command.ExecuteNonQuery();

                    MessageBox.Show("Data saved successfully!");
                    PictureBoxImageInput.ImageLocation = "";
                    ClearInputUpdateData();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Data failed to save!");
                    Connection.Close();
                }

            }
            else
            {

            }

        }

        private void TextBoxName_TextChanged(object sender, EventArgs e)
        {

        }

        private void ButtonScanID_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelDateTime.Text = "Time " + DateTime.Now.ToString("HH:mm:ss") + "     Date " + DateTime.Now.ToString("dd MMM, yyyy");
        }

        private void timerSerialIn_Tick(object sender, EventArgs e)
        {

        }
    }
}
