using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;


namespace SAD_2E_Project
{
    public partial class Form1 : Form
    {
        // server=localhost; user=yout_database_user; password=your_database_password; database=your_database_name
        SerialPort serialPort = new SerialPort();

        string connectionString = "server=localhost; user=root; password=Cestlavie1!; port:3306; database=rfid_user_data";
        //MySqlConnection Connection;
        MySqlConnection Connection = new MySqlConnection("server=localhost; user=root; password=Cestlavie1!; port=3306; database=rfid_user_data");
        MySqlCommand MySQLCMD = new MySqlCommand();
        private MySqlDataAdapter MySQLDA = new MySqlDataAdapter();
        private DataTable DT = new DataTable();
        private int Data;

        private string tableName = "rfid_user_data_table";

        private bool LoadImageStr = false;
        private string IDRam;
        private string IMG_FileNameInput;
        private string StatusInput = "Save";
        private string SqlCmdSearchStr;
        DataGridView dataGridView1;
        

        public string StrSerialIn;
        public string IDtemp;
        private bool GetID = false;
        private bool ViewUserData = false;

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
                if (Connection.State.ToString() == "Closed")
                {
                    Connection.Open();
                }
                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Failed! \nPlease check if server is turned on!");
            }

            try
            {
                if (LoadImageStr == false)
                {
              
                    MySqlCommand command = new MySqlCommand();
                    command.CommandType = CommandType.Text;
                    command.CommandText = "Select Name, Age, Sex, Contact_Person, Birthday, Medical_Conditions FROM " + tableName + " ORDER BY Name";
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command.CommandText, Connection);
                    int Data = dataAdapter.Fill(DT);
                    DataSet DS = new DataSet();
                    dataAdapter.Fill(DS);
                    if (Data > 0)
                    {
                        dataGridView1.DataSource = DT;
                        dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                        dataGridView1.ClearSelection();
                    }
                    else
                        dataGridView1.DataSource = DT;
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
                MessageBox.Show("Failed to load database! DT");
            }
            Connection.Close();
        }

        private void showDataUser()
        {
            try
            {
                if (Connection.State.ToString() == "Closed")
                {
                    Connection.Open();
                }
                else
                {
                    return;
                }

            }
            catch
            {
                MessageBox.Show("Connection Failed! \n Please check if server is turned on!");
            }

            try
            {
                MySqlCommand command = new MySqlCommand();
                command.CommandType = CommandType.Text;
                //command.CommandText = "Select * FROM " + tableName + " WHERE ID LIKE '" + LabelGetID.Text.Substring(5, LabelGetID.Text.Length -5) + "'";
                command.CommandText = "Select * FROM " + tableName + " WHERE ID LIKE '" + IDtemp + "'";
                //MessageBox.Show(command.CommandText);
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command.CommandText, Connection);
                
                DataTable DT = new DataTable();
                int Data = dataAdapter.Fill(DT);
                //MessageBox.Show("Data is: " + Data.ToString());
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
                    MessageBox.Show("ID not found! \nPlease Register your ID!");
            }

            catch 
            {
                MessageBox.Show("Failed to load database!");
                Connection.Close();
            }
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
                locationToDraw.Y = (PictureBoxImageInput.Height / 2) - (textSize.Height / 2);
                e.Graphics.DrawString(Text, Font, Brushes.Black, locationToDraw);

            });


        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            PanelRegistrationEditUserData.Visible = false;
            PanelUserData.Visible = false;
            PanelConnection.Visible = true;
        }

        private void buttonUData_Click(object sender, EventArgs e)
        {
            if (timerSerialIn.Enabled == false)                                                      //turn to false after debug
            {
                MessageBox.Show("Failed to open User Data!");
                return;
            }
            else
            {
                StrSerialIn = "";
                ViewUserData = true;
                PanelRegistrationEditUserData.Visible = false;
                PanelConnection.Visible = false;
                PanelUserData.Visible = true;
            }

        }

        private void buttonRegedit_Click(object sender, EventArgs e)
        {
            StrSerialIn = "";
            PanelRegistrationEditUserData.Visible = true;
            PanelUserData.Visible = false;
            PanelConnection.Visible = false;
            ViewUserData = false;
            showData();
            ClearInputUpdateData();

        }

        private void PanelUserData_Paint(object sender, PaintEventArgs e)
        {

        } //might be unneeded

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

        private void button1_Click(object sender, EventArgs e)
        {
            if (buttonConnectDevice.Text == "Connect")
            {
                try
                {
                    serialPort.PortName = comboBoxPorts.SelectedItem.ToString();
                    serialPort.BaudRate = 115200;
                    serialPort.Parity = Parity.None;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = StopBits.One;
                    //MessageBox.Show("Connecting to Serial Port " + comboBoxPorts.SelectedItem.ToString());
                    serialPort.Open();

                    //MessageBox.Show("Connecting to Serial Port hello" + comboBoxPorts.SelectedItem.ToString());
                    buttonConnectDevice.Text = "Disconnect";
                    labelConnectionStatus.Text = "CONNECTED";
                    labelConnectionStatus.ForeColor = Color.Green;
                    //MessageBox.Show("Connecting to Serial Port bye" + comboBoxPorts.SelectedItem.ToString());
                    timerSerialIn.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to connect!");
                    labelConnectionStatus.Text = "DISCONNECTED";
                    labelConnectionStatus.ForeColor = Color.Red;
                    //timerSerialIn.Start();
          
                }
                return;

            }

            else if (buttonConnectDevice.Text == "Disconnect")
            {
                serialPort.Close();
                labelConnectionStatus.Text = "DISCONNECTED";
                labelConnectionStatus.ForeColor = Color.Red;
                //timerSerialIn.Stop();
                buttonConnectDevice.Text = "Connect";
            }
            PanelRegistrationEditUserData.Visible = false;
            PanelUserData.Visible = false;
            PanelConnection.Visible = true;
        }   /// Connect Device Button

        private void button1_Click_1(object sender, EventArgs e)
        {
            labelName.Text = "Waiting...";
            labelAge.Text = "Waiting...";
            labelSex.Text = "Waiting...";
            labelCont.Text = "Waiting...";
            labelBDay.Text = "Waiting...";
            labelMedical.Text = "Waiting...";
            pictureBox1.Image = null;           //Not Sure
        }  /// Clear Button

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
            MessageBox.Show(StatusInput);

            if(StatusInput == "Save" || StatusInput == "Update")
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
                    if (Connection.State.ToString() == "Closed")
                    {
                        Connection.Open();
                    }
                    else
                    {
                        return;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection failed! \n Please check server is turned on!");
                    return;
                }

                try
                {
                    MySqlCommand command = new MySqlCommand();
                    command.CommandText = "INSERT INTO " + tableName + "(Name, ID, Age, Sex, Contact_Person, Birthday, Medical_Conditions, Images) VALUES (@name, @ID, @Age, @Sex, @Contact_Person, @Birthday, @Medical_Conditions, @Images)";
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

        private void ButtonClearForm_Click(object sender, EventArgs e)
        {
            ClearInputUpdateData();
        }

        private void ButtonScanID_Click(object sender, EventArgs e)
        {
            if (timerSerialIn.Enabled == true)
            {
                PanelReadingTagProcess.Visible = true;
                GetID = true;
                ButtonScanID.Enabled = false;
            }
            else
                MessageBox.Show("Failed to open User Data!");
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

        private void CheckBoxByName_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxByName.Checked == true)
                CheckBoxByID.Checked = false;
            if (CheckBoxByName.Checked == false)
                CheckBoxByID.Checked = true;
        }

        private void CheckBoxByID_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBoxByID.Checked == true)
                CheckBoxByName.Checked = false;
            if (CheckBoxByID.Checked == false)
                CheckBoxByName.Checked = true;
        }

        private void TextBoxSearch_TextChanged(object sender, EventArgs e)
        {
            if (CheckBoxByID.Checked == true)
            {
                if (TextBoxSearch.Text == null/* TODO Change to default(_) if this is not a reference type */ )
                    SqlCmdSearchStr = "SELECT `Name`, `ID`, `Age`, `Sex`, `Birthday`, `Contact_Person`, `Medical_Conditions`, `Images` FROM" + tableName + " ORDER BY Name";
                else
                    SqlCmdSearchStr = "SELECT `Name`, `ID`, `Age`, `Sex`, `Birthday`, `Contact_Person`, `Medical_Conditions`, `Images` FROM " + tableName + "' WHERE 'ID' LIKE'" + TextBoxSearch.Text + "%'";
            }
            if (CheckBoxByName.Checked == true)
            {
                if (TextBoxSearch.Text == null/* TODO Change to default(_) if this is not a reference type */ )
                    SqlCmdSearchStr = "SELECT `Name`, `ID`, `Age`, `Sex`, `Birthday`, `Contact_Person`, `Medical_Conditions`, `Images` FROM " + tableName + " ORDER BY Name";
                else
                    SqlCmdSearchStr = "SELECT `Name`, `ID`, `Age`, `Sex`, `Birthday`, `Contact_Person`, `Medical_Conditions`, `Images` FROM " + tableName + " WHERE Name LIKE'" + TextBoxSearch.Text + "%'";
            }

            try
            {
                if (Connection.State.ToString() == "Closed")
                {
                    Connection.Open();
                }
                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed !!!");
                return;
            }

            try
            {
                MySQLDA = new MySqlDataAdapter(SqlCmdSearchStr, Connection);
                DT = new DataTable();
                Data = MySQLDA.Fill(DT);
                if (Data > 0)
                {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = DT;
                    dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                    dataGridView1.ClearSelection();
                }
                else
                    dataGridView1.DataSource = DT;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to search");
                Connection.Close();
            }
            Connection.Close();
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (AllCellsSelected(dataGridView1) == false)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        dataGridView1.CurrentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
                        int i;
                        {
                            var withBlock = dataGridView1;
                            if (e.RowIndex >= 0)
                            {
                                i = withBlock.CurrentRow.Index;
                                LoadImageStr = true;
                                IDRam = withBlock.Rows[i].Cells["ID"].Value.ToString();
                                showData();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private bool AllCellsSelected(DataGridView dgv)
        {
            //AllCellsSelected = (dataGridView1.SelectedCells.Count == (dataGridView1.RowCount * dataGridView1.Columns.GetColumnCount(DataGridViewElementStates.Visible)));
            return true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelDateTime.Text = "Time " + DateTime.Now.ToString("HH:mm:ss") + "     Date " + DateTime.Now.ToString("dd MMM, yyyy");
        }  //date time timer

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount == 0)
            {
                MessageBox.Show("Cannot delete, table data is empty");
                return;
            }

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Cannot delete, select the table data to be deleted");
                return;
            }

            if (MessageBox.Show("Do you want to delete the record?", "Delete record?", MessageBoxButtons.YesNo)==DialogResult.Yes)
                return;

            try
            {
                if (Connection.State.ToString() == "Closed")
                {
                    Connection.Open();
                }
                else
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed !!!");
                return;
            }

            try
            {
                if (AllCellsSelected(dataGridView1) == true)
                {
                    MySQLCMD.CommandType = CommandType.Text;
                    MySQLCMD.CommandText = "DELETE FROM " + tableName;
                    MySQLCMD.Connection = Connection;
                    MySQLCMD.ExecuteNonQuery();
                }

                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (row.Selected == true)
                    {
                        MySQLCMD.CommandType = CommandType.Text;
                        MySQLCMD.CommandText = "DELETE FROM " + tableName + " WHERE ID='" + row.DataBoundItem.ToString() + "'";
                        MySQLCMD.Connection = Connection;
                        MySQLCMD.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete");
                Connection.Close();
            }
            PictureBoxImagePreview.Image = null;
            Connection.Close();
            showData();
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.SelectAll();
        }

        private void clearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            PictureBoxImagePreview.Image = null;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showData();
        }

        private void timerSerialIn_Tick(object sender, EventArgs e)
        {
            try
            {
                StrSerialIn = serialPort.ReadExisting();
                //labelConnectionStatus.Text = "CONNECTED";
                Console.WriteLine(StrSerialIn);

                if (StrSerialIn != "")
                {
                    IDtemp = StrSerialIn;
                    label7.Text = StrSerialIn;
                    //MessageBox.Show(StrSerialIn);
                    if (GetID == true)
                    {
                        LabelGetID.Text = StrSerialIn;
                        label7.Text = StrSerialIn;
                        IDtemp = StrSerialIn;
                        GetID = false;
                        if (LabelGetID.Text != "________")
                        {
                            PanelReadingTagProcess.Visible = false;
                            IDCheck();
                            label7.Text = StrSerialIn;
                        }
                    }
                    if (ViewUserData == true)
                        showDataUser();
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show(StrSerialIn);
                timerSerialIn.Stop();
                serialPort.Close();
                MessageBox.Show("Failed to connect !!!");
                button1_Click(sender, e);
                return;
            }

            //if (labelConnectionStatus.Text == "DISCONNECTED")
            //{
            //    labelConnectionStatus.ForeColor = System.Drawing.Color.Green;
            //    labelConnectionStatus.Text = "CONNECTED";
            //}
            //else if (labelConnectionStatus.Text == "CONNECTED")
            //{
            //    labelConnectionStatus.ForeColor = System.Drawing.Color.Red;
            //    labelConnectionStatus.Text = "DISCONNECTED";
            //}
        }

        private void IDCheck()
        {
            try
            {
                if(Connection.State.ToString() == "Closed")
                {
                    Connection.Open();
                }
                else
                {
                    return;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed !!!" + Connection.State.ToString());
                return;
            }

            try
            {
                MySQLCMD.CommandType = CommandType.Text;
                MySQLCMD.CommandText = "SELECT * FROM " + tableName + " WHERE ID LIKE '" + LabelGetID.Text + "'";
                MySQLDA = new MySqlDataAdapter(MySQLCMD.CommandText, Connection);
                DT = new DataTable();
                Data = MySQLDA.Fill(DT);
                if (Data > 0)
                {
                    if (MessageBox.Show("Do you want to edit the data ?", "ID registered !", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        DT = null;
                        Connection.Close();
                        ButtonScanID.Enabled = true;
                        GetID = false;
                        LabelGetID.Text = "________";
                        return;
                    }
                    else
                    {
                        byte[] imgArray = (byte[])DT.Rows[0]["Images"];
                        System.IO.MemoryStream imgStr = new System.IO.MemoryStream(imgArray);
                        pictureBox1.Image = System.Drawing.Image.FromStream(imgStr);
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        imgStr.Close();

                        LabelGetID.Text = (string)DT.Rows[0]["ID"];
                        TextBoxName.Text = (string)DT.Rows[0]["Name"];
                        TextBoxAge.Text = (string)DT.Rows[0]["Age"];
                        TextBoxSex.Text = (string)DT.Rows[0]["Sex"];
                        textBoxContactPerson.Text = (string)DT.Rows[0]["Contact_Person"];
                        TextBoxBirthday.Text = (string)DT.Rows[0]["Birthday"];
                        TextBoxMedCon.Text = (string)DT.Rows[0]["Medical_Conditions"];
                        StatusInput = "Update";
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Database !!!");
                Connection.Close();
                return;
            }

            DT = null;
            Connection.Close();

            ButtonScanID.Enabled = true;
            GetID = false;
        }

        private void ViewData()
        {
            label7.Text = StrSerialIn;
            if (label7.Text == "_________")
                ViewData();
            else
                showDataUser();
        }
        private void ButtonCloseReadingTag_Click(object sender, EventArgs e)
        {
            PanelReadingTagProcess.Visible = false;
            ButtonScanID.Enabled = true;
        }

        private void PanelRegistrationEditUserData_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }
    }
}
