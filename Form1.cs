using System;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;


namespace SAD_2E_Project
{
    public partial class Form1 : Form
    {
        //private int Data;
        private SQLiteConnection sql_con;
        private SQLiteCommand sql_cmd;
        //private SQLiteDataAdapter DB;
        private DataSet DS = new DataSet();
        private DataTable DT = new DataTable();

        //private string tableName = "rfid_user_data_table";

        //private bool LoadImageStr = false;
        //private string IDRam;
        //private string IMG_FileNameInput;
        //private string StatusInput = "Save";
        //private string SqlCmdSearchStr;
        private byte[] imageTemp;


        public string StrSerialIn;
        public string IDtemp;
        private bool GetID = false;
        //private bool ViewUserData = false;

        private void SetConnection()
        {
            sql_con = new SQLiteConnection("Data Source=ville.db;Version=3;New=False;Compress=True;");
        }

        public Form1()
        {
            InitializeComponent();
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            PanelRegistrationEditUserData.Visible = false;
            PanelUserData.Visible = false;
            PanelConnection.Visible = true;
            PanelSessionLog.Visible = false;
            PanelTimeIn.Visible = false;

            TimerSerialIn.Enabled = false;
            SetConnection();
            sql_con.Open();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS
                [userData] (
                [ID]                INT      NOT NULL,
                [RFID]              CHAR(100) PRIMARY KEY           NOT NULL,
                [NameENG]           TEXT                NOT NULL,
                [NameCHI]           TEXT                NOT NULL,
                [Address]           TEXT                NOT NULL,
                [City]              TEXT                NOT NULL,
                [Birthday]          TEXT                NOT NULL,
                [Landline]          TEXT                NOT NULL,
                [Mobile]            TEXT                NOT NULL,
                [ContactPerson]     TEXT                NOT NULL,
                [CPRelationship]    TEXT                NOT NULL,
                [CPLandline]        TEXT                NOT NULL,
                [CPMobile]          TEXT                NOT NULL,
                [Image]             BLOB                NULL)";
            sql_cmd.ExecuteNonQuery();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS
                [userSession] (
                [RFID]              CHAR(100) PRIMARY KEY   NOT NULL,
                [TimeInDate]        TEXT                    NOT NULL)";
            sql_cmd.ExecuteNonQuery();
            sql_cmd = sql_con.CreateCommand();
            sql_cmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS
                [userSessionLogs] (
                [RFID]              CHAR(100)  NOT NULL,
                [ID]                INT                     NOT NULL,
                [NameENG]           TEXT                    NOT NULL,
                [TimeInDate]        TEXT                    NOT NULL,
                [TimeOut]           TEXT                    NOT NULL,
                [Hours]             DOUBLE                   NOT NULL)";
            sql_cmd.ExecuteNonQuery();
            sql_con.Close();
        }

        private void showData()
        {
            try
            {
                DT = new DataTable();
                sql_con.Open();
                sql_cmd = sql_con.CreateCommand();
                var sql_adapter = new SQLiteDataAdapter("SELECT * FROM userData ORDER BY NameENG", sql_con);
                var sql_cmdBuilder = new SQLiteCommandBuilder(sql_adapter);
                DataSet DS = new DataSet();
                int Data = sql_adapter.Fill(DS);
                sql_adapter.Fill(DT);
 
                if (Data > 0)
                {
                    dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dataGridView2.DataSource = DT;
                    dataGridView2.DefaultCellStyle.ForeColor = Color.Black;
                    dataGridView2.ClearSelection();
                }
                else
                    dataGridView2.DataSource = DT;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load database! DT \n" + ex.Message);
            }
            sql_con.Close();
        }

        private void sessionLogs()
        {
            try
            {
                sql_con.Open();
                DT = new DataTable();
                sql_cmd = sql_con.CreateCommand();
                var sql_adapter = new SQLiteDataAdapter("SELECT * FROM userSessionLogs", sql_con);
                var sql_cmdBuilder = new SQLiteCommandBuilder(sql_adapter);
                DataSet DS = new DataSet();
                int Data = sql_adapter.Fill(DS);
                sql_adapter.Fill(DT);

                if (Data > 0)
                {
                    dataGridSessionLog.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    dataGridSessionLog.DataSource = DT;
                    dataGridSessionLog.DefaultCellStyle.ForeColor = Color.Black;
                    dataGridSessionLog.ClearSelection();
                }
                else
                    dataGridSessionLog.DataSource = DT;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load database! DT\n" + ex.Message, "Error");
            }
            sql_con.Close();
        }

        private void showDataUser() //REGEDIT PANEL
        {
            try
            {
                sql_con.Open();
                sql_cmd = sql_con.CreateCommand();
                var commandText = "SELECT * FROM userData WHERE rfid='" + IDtemp.Substring(0, 8) + "'";
                var sql_adapter = new SQLiteDataAdapter(commandText, sql_con);
                var sql_cmdBuilder = new SQLiteCommandBuilder(sql_adapter);
                DataSet DS = new DataSet();
                int Data = sql_adapter.Fill(DS);
                sql_adapter.Fill(DT);

                if (Data > 0)
                {
                    PictureBoxImageInput.Invalidate();
                    byte[] imgArray = (byte[])DT.Rows[0]["Image"];
                    imageTemp = (byte[])DT.Rows[0]["Image"];
                    System.IO.MemoryStream imgStr = new System.IO.MemoryStream(imgArray);
                    PictureBoxImageInput.Image = System.Drawing.Image.FromStream(imgStr);
                    PictureBoxImageInput.SizeMode = PictureBoxSizeMode.Zoom;
                    imgStr.Close();

                    PictureBoxImageInput.Paint += new PaintEventHandler((sender, e) =>
                    {
                        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        string Text = "";
                        SizeF textSize = e.Graphics.MeasureString(Text, Font);
                        PointF locationToDraw = new PointF();
                        locationToDraw.X = (PictureBoxImageInput.Width / 2) - (textSize.Width / 2);
                        locationToDraw.Y = (PictureBoxImageInput.Height / 2) - (textSize.Height / 2);
                        e.Graphics.DrawString(Text, Font, Brushes.Black, locationToDraw);

                    });


                    LabelGetID.Text = (string)DT.Rows[0]["RFID"];
                    TextBoxName.Text = (string)DT.Rows[0]["NameENG"];
                    TextBoxChiName.Text = (string)DT.Rows[0]["NameCHI"];
                    TextBoxAddress.Text = (string)DT.Rows[0]["Address"];
                    TextBoxCity.Text = (string)DT.Rows[0]["City"];
                    TextBoxMobile.Text = (string)DT.Rows[0]["Mobile"];
                    TextBoxLandline.Text = (string)DT.Rows[0]["Landline"];
                    TextBoxBirthday.Text = (string)DT.Rows[0]["Birthday"];
                    TextBoxContactPerson.Text = (string)DT.Rows[0]["ContactPerson"];
                    TextBoxRelationship.Text = (string)DT.Rows[0]["CPRelationship"];
                    TextBoxContactLandline.Text = (string)DT.Rows[0]["CPLandline"];
                    TextBoxContactMobile.Text = (string)DT.Rows[0]["CPMobile"];

                    int idTemp = (int)DT.Rows[0]["ID"];
                    TextBoxID.Text = idTemp.ToString();
                }
                else
                {
                    MessageBox.Show("RFID Is not registered!");
                }
                DT.Clear();
                DS.Clear();
            }

            catch (SQLiteException ex)
            {
                int errorCode = ex.ErrorCode;
                MessageBox.Show("Failed to load database!\n" + errorCode.ToString(), errorCode.ToString());
                sql_con.Close();
            }
        }

        private void ClearInputUpdateData()
        {
            LabelGetID.Text = "";
            TextBoxID.Text = "";
            TextBoxName.Text = "";
            TextBoxChiName.Text = "";
            TextBoxAddress.Text = "";
            TextBoxCity.Text = "";
            TextBoxBirthday.Text = "";
            TextBoxMobile.Text = "";
            TextBoxLandline.Text = "";
            TextBoxContactPerson.Text = "";
            TextBoxRelationship.Text = "";
            TextBoxContactLandline.Text = "";
            TextBoxContactMobile.Text = "";
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            sql_con.Close();
            PanelRegistrationEditUserData.Visible = false;
            PanelUserData.Visible = false;
            PanelConnection.Visible = true;
            PanelSessionLog.Visible = false;
            PanelTimeIn.Visible = false;
        }

        private void buttonUData_Click(object sender, EventArgs e)
        {
            StrSerialIn = "";
            sql_con.Close();
            //ViewUserData = true;
            PanelRegistrationEditUserData.Visible = false;
            PanelConnection.Visible = false;
            PanelUserData.Visible = true;
            PanelSessionLog.Visible = false;
            PanelTimeIn.Visible = false;
            showData();

        }

        private void buttonRegedit_Click(object sender, EventArgs e)
        {
            StrSerialIn = "";
            sql_con.Close();
            if (DT != null)
            {
                DT.Clear();
            }
            PictureBoxImageInput.SizeMode = PictureBoxSizeMode.Zoom;
            PanelRegistrationEditUserData.Visible = true;
            PanelUserData.Visible = false;
            PanelConnection.Visible = false;
            PanelTimeIn.Visible = false;
            //ViewUserData = false;
            //showDataUser();
            ClearInputUpdateData();

        }

        private void PanelUserData_Paint(object sender, PaintEventArgs e)
        {

        } // Might be unneeded

        private void buttonScan_Click(object sender, EventArgs e)
        {
            try
            {
                comboBoxPorts.Items.Clear();
                string[] ports = SerialPort.GetPortNames();
                comboBoxPorts.DataSource = ports;
            }

            catch (Exception ex)
            {
                MessageBox.Show("COM Port not detected!\n" + ex.Message, "Error");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (buttonConnectDevice.Text == "Connect")
            {
                try
                {
                    SerialPort1.PortName = comboBoxPorts.SelectedItem.ToString();
                    SerialPort1.BaudRate = 115200;
                    SerialPort1.Parity = Parity.None;
                    SerialPort1.DataBits = 8;
                    SerialPort1.StopBits = StopBits.One;

                    SerialPort1.Open();


                    buttonConnectDevice.Text = "Disconnect";
                    labelConnectionStatus.Text = "CONNECTED";
                    labelConnectionStatus.ForeColor = Color.Green;

                    TimerSerialIn.Enabled = true;
                    TimerSerialIn.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to connect!\n" + ex.Message, "Error");
                    labelConnectionStatus.Text = "DISCONNECTED";
                    labelConnectionStatus.ForeColor = Color.Red;
                    //TimerSerialIn.Start();

                }
                return;

            }

            else if (buttonConnectDevice.Text == "Disconnect")
            {
                SerialPort1.Close();
                labelConnectionStatus.Text = "DISCONNECTED";
                labelConnectionStatus.ForeColor = Color.Red;
                //TimerSerialIn.Stop();
                buttonConnectDevice.Text = "Connect";
            }
            PanelRegistrationEditUserData.Visible = false;
            PanelUserData.Visible = false;
            PanelConnection.Visible = true;
            PanelConnection.Visible = true;
            PanelSessionLog.Visible = false;
            PanelTimeIn.Visible = false;
        }   /// Connect Device Button

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            System.IO.MemoryStream mstream = new System.IO.MemoryStream();

            byte[] arrImage;

            if (TextBoxID.Text == "")
            {
                MessageBox.Show("ID cannot be empty!", "Error");
                return;
            }
            if (TextBoxName.Text == "")
            {
                MessageBox.Show("English Name cannot be empty!", "Error");
                return;
            }
            if (TextBoxChiName.Text == "")
            {
                MessageBox.Show("Chinese Name cannot be empty!", "Error");
                return;
            }
            if (TextBoxAddress.Text == "")
            {
                MessageBox.Show("Address cannot be empty!", "Error");
                return;
            }
            if (TextBoxCity.Text == "")
            {
                MessageBox.Show("City cannot be empty!", "Error");
                return;
            }
            if (TextBoxBirthday.Text == "")
            {
                MessageBox.Show("Birthday cannot be empty!", "Error");
                return;
            }
            if (TextBoxContactPerson.Text == "")
            {
                MessageBox.Show("Contact Person cannot be empty!", "Error");
                return;
            }
            if (TextBoxRelationship.Text == "")
            {
                MessageBox.Show("Contact Person Relationship cannot be empty!", "Error");
                return;
            }
            if (TextBoxContactLandline.Text == "")
            {
                MessageBox.Show("Contact Person Landline cannot be empty!", "Error");
                return;
            }
            if (TextBoxContactMobile.Text == "")
            {
                MessageBox.Show("Contact Person Mobile cannot be empty!", "Error");
                return;
            }

            //MessageBox.Show(StatusInput);

            if (ButtonSave.Text == "Save")
            {
                //bool isNullOrEmpty = PictureBoxImageInput.Image == null;
                if (PictureBoxImageInput.Image != null)
                {
                    //PictureBoxImageInput.Image.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    if(PictureBoxImageInput.ImageLocation == null)
                    {
                        arrImage = imageTemp;
                    }
                    else {
                        PictureBoxImageInput.Image.Save(mstream, System.Drawing.Imaging.ImageFormat.Png);
                        arrImage = mstream.GetBuffer();
                    }
                    
                }
                else
                {
                    MessageBox.Show("Image has not been selected!");
                    return;
                }

                sql_con.Open();
                try
                {
                    sql_cmd = sql_con.CreateCommand();
                    sql_cmd.CommandText = "INSERT INTO userData (ID, RFID, NameENG, NameCHI, Address, City, Birthday, Landline, Mobile, ContactPerson, CPRelationship, CPLandline, CPMobile, Image) " +
                        "VALUES (@ID, @RFID, @NameENG, @NameCHI, @Address, @City, @Birthday, @Landline, @Mobile, @ContactPerson, @CPRelationship, @CPLandline, @CPMobile, @Image)";
                    sql_cmd.Parameters.Add(new SQLiteParameter("@ID", TextBoxID.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@RFID", IDtemp.Substring(0, 8)));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@NameENG", TextBoxName.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@NameCHI", TextBoxChiName.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@Address", TextBoxAddress.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@City", TextBoxCity.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@Birthday", TextBoxBirthday.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@Landline", TextBoxLandline.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@Mobile", TextBoxMobile.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@ContactPerson", TextBoxContactPerson.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@CPRelationship", TextBoxRelationship.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@CPLandline", TextBoxContactLandline.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@CPMobile", TextBoxContactMobile.Text));
                    sql_cmd.Parameters.Add(new SQLiteParameter("@Image", arrImage));
                    sql_cmd.ExecuteNonQuery();

                    MessageBox.Show("Data saved successfully!");
                    PictureBoxImageInput.ImageLocation = "";
                    sql_con.Close();
                    ClearInputUpdateData();
                }
                catch (SQLiteException ex)
                {
                    int errorCode = ex.ErrorCode;
                    MessageBox.Show("Data failed to save! " + errorCode.ToString(), errorCode.ToString());
                    sql_con.Close();
                    PictureBoxImageInput.Enabled = false;
                }

            }
            if (ButtonSave.Text == "Update")
            {
                DT = new DataTable();
                //bool isNullOrEmpty = PictureBoxImageInput.Image == null;
                //if (PictureBoxImageInput.ImageLocation == null)
                //{
                //    arrImage = imageTemp;
                //}
                //else
                //{
                //    PictureBoxImageInput.Image.Save(mstream, System.Drawing.Imaging.ImageFormat.Png);
                //    arrImage = mstream.GetBuffer();
                //}

                sql_con.Open();
                try
                {
                    sql_cmd = sql_con.CreateCommand();
                    sql_cmd.CommandText =
                        "UPDATE userData SET" +
                        " ID = '" + TextBoxID.Text + "'," +
                        " NameENG = '" + TextBoxName.Text + "'," +
                        " NameCHI = '" + TextBoxChiName.Text + "'," +
                        " Address = '" + TextBoxAddress.Text + "'," +
                        " City = '" + TextBoxCity.Text + "'," +
                        " Birthday = '" + TextBoxBirthday.Text + "'," +
                        " Landline = '" + TextBoxLandline.Text + "'," +
                        " Mobile = '" + TextBoxMobile.Text + "'," +
                        " ContactPerson = '" + TextBoxContactPerson.Text + "'," +
                        " CPRelationship = '" + TextBoxRelationship.Text + "'," +
                        " CPLandline = '" + TextBoxContactLandline.Text + "'," +
                        " CPMobile = '" + TextBoxContactMobile.Text + "'" +
                        //" Image = '" + arrImage + "'" +
                        " WHERE RFID= '" + IDtemp.Substring(0, 8) + "'";
                    sql_cmd.ExecuteNonQuery();

                    MessageBox.Show("Data saved successfully!");
                    PictureBoxImageInput.ImageLocation = "";
                    sql_con.Close();
                    ClearInputUpdateData();
                }
                catch (SQLiteException ex)
                {
                    int errorCode = ex.ErrorCode;
                    MessageBox.Show("Data failed to save! " + errorCode.ToString(), errorCode.ToString());
                    sql_con.Close();
                }

            }

        }

        private void ButtonClearForm_Click(object sender, EventArgs e)
        {
            ClearInputUpdateData();
        }

        private void ButtonScanID_Click(object sender, EventArgs e)
        {
            if (DT != null)
            {
                DT.Clear();
            }
            TimerSerialIn.Enabled = true;
            TimerSerialIn.Start();
            if (TimerSerialIn.Enabled == true)
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
                PictureBoxImageInput.Invalidate();
                PictureBoxImageInput.Text = "";
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
            //if (dataGridView1.RowCount == 0)
            //{
            //    MessageBox.Show("Cannot delete, table data is empty");
            //    return;
            //}

            //if (dataGridView1.SelectedRows.Count == 0)
            //{
            //    MessageBox.Show("Cannot delete, select the table data to be deleted");
            //    return;
            //}

            //if (MessageBox.Show("Do you want to delete the record?", "Delete record?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    return;

            //try
            //{
            //    if (Connection.State.ToString() == "Closed")
            //    {
            //        Connection.Open();
            //    }
            //    else
            //    {
            //        return;
            //    }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Connection failed !!!");
            //    return;
            //}

            //try
            //{
            //    if (AllCellsSelected(dataGridView1) == true)
            //    {
            //        MySQLCMD.CommandType = CommandType.Text;
            //        MySQLCMD.CommandText = "DELETE FROM " + tableName;
            //        MySQLCMD.Connection = Connection;
            //        MySQLCMD.ExecuteNonQuery();
            //    }

            //    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            //    {
            //        if (row.Selected == true)
            //        {
            //            MySQLCMD.CommandType = CommandType.Text;
            //            MySQLCMD.CommandText = "DELETE FROM " + tableName + " WHERE `ID` = '" + row.DataBoundItem.ToString() + "'";
            //            MySQLCMD.Connection = Connection;
            //            MySQLCMD.ExecuteNonQuery();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Failed to delete");
            //    Connection.Close();
            //}
            //PictureBoxImagePreview.Image = null;
            //Connection.Close();
            //showData();
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //dataGridView1.SelectAll();
        }

        private void clearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //dataGridView1.ClearSelection();
            //PictureBoxImagePreview.Image = null;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //showData();
        }

        private void TimerSerialIn_Tick(object sender, EventArgs e)
        {
            try
            {
                StrSerialIn = SerialPort1.ReadExisting();
                //labelConnectionStatus.Text = "CONNECTED";
                Console.WriteLine(StrSerialIn);

                if (StrSerialIn != "")
                {
                    IDtemp = StrSerialIn;
                    LabelGetID.Text = StrSerialIn;
                    //label7.Text = StrSerialIn;
                    //
                    //MessageBox.Show(StrSerialIn);
                    if (PanelRegistrationEditUserData.Visible == true)
                    {
                        if (GetID == true)
                        {
                            //label7.Text = StrSerialIn;
                            IDtemp = StrSerialIn;
                            //MessageBox.Show("LabelGetID is " + LabelGetID.Text);
                            //GetID = false;
                            //if (LabelGetID.Text != "_________")
                            //{

                                PanelReadingTagProcess.Visible = false;
                                //MessageBox.Show("TEST");
                                //LabelGetID.Text = StrSerialIn;
                                TimerSerialIn.Enabled = false;
                                TimerSerialIn.Stop();
                                IDCheck();
                                //label7.Text = StrSerialIn;


                            //}
                        //}
                        //if (ViewUserData == true)
                        //{
                        //    TimerSerialIn.Enabled = false;
                        //    TimerSerialIn.Stop();
                        //    showDataUser();
                        }
                    }
                    if(PanelTimeIn.Visible == true)
                    {
                        TimeInTimeOut();
                    }

                }
            }
            catch (Exception ex)
            {

                // MessageBox.Show(StrSerialIn);
                TimerSerialIn.Stop();
                SerialPort1.Close();
                MessageBox.Show("Failed to connect. \n" + ex.Message, "Error");
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
                sql_con.Open();
                sql_cmd = sql_con.CreateCommand();
                var commandText = "SELECT * FROM userData WHERE rfid='" + IDtemp.Substring(0, 8) + "'";
                var sql_adapter = new SQLiteDataAdapter(commandText, sql_con);
                var sql_cmdBuilder = new SQLiteCommandBuilder(sql_adapter);
                DataSet DS = new DataSet();
                int Data = sql_adapter.Fill(DS);
                sql_adapter.Fill(DT);

                if (Data > 0)
                {
                    //MessageBox.Show("RFID Card registered!");
                    sql_con.Close();
                    ButtonSave.Text = "Update";
                    showDataUser();
                }
                else if (Data == 0)
                {
                    //LabelGetID.Text = StrSerialIn;
                    MessageBox.Show("RFID Card not registered!");
                    PictureBoxImageInput.Enabled = true;
                    ButtonSave.Text = "Save";

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


            }
            catch (SQLiteException ex)
            {
                int errorCode = ex.ErrorCode;
                MessageBox.Show("Failed to load Database!", errorCode.ToString());
                sql_con.Close();
                return;
            }

            DT = null;
            sql_con.Close();

            ButtonScanID.Enabled = true;
            GetID = false;
        }

        private void ViewData()
        {
            label7.Text = StrSerialIn;
            if (label7.Text == "_________")
            {
                ViewData();
            }
            else
                showDataUser();
        }

        private async void TimeInTimeOut()
        {
            var welcomeSound = new System.Media.SoundPlayer(SAD_2E_Project.Properties.Resources.welcome);
            var exitSound = new System.Media.SoundPlayer(SAD_2E_Project.Properties.Resources.exit);
            var errorSound = new System.Media.SoundPlayer(SAD_2E_Project.Properties.Resources.error);
            try
            {
                sql_con.Open();
                sql_cmd = sql_con.CreateCommand();
                var commandText = "SELECT * FROM userData WHERE rfid='" + IDtemp.Substring(0, 8) + "'";
                var sql_adapter = new SQLiteDataAdapter(commandText, sql_con);
                var sql_cmdBuilder = new SQLiteCommandBuilder(sql_adapter);
                DataSet DS = new DataSet();
                int Data = sql_adapter.Fill(DS);
                sql_adapter.Fill(DT);

                if (Data > 0)
                {
                                        byte[] imgArray = (byte[])DT.Rows[0]["Image"];
                    System.IO.MemoryStream imgStr = new System.IO.MemoryStream(imgArray);
                    pictureBox3.Image = System.Drawing.Image.FromStream(imgStr);
                    pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                    imgStr.Close();

                    TimeInName.Text = (string)DT.Rows[0]["NameENG"];
                    int idTemp = (int)DT.Rows[0]["ID"];
                    TimeInID.Text = idTemp.ToString();
                    TimeInTime.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt");

                    sql_cmd = sql_con.CreateCommand();
                    commandText = "SELECT * FROM userSession WHERE rfid='" + IDtemp.Substring(0, 8) + "'";
                    sql_adapter = new SQLiteDataAdapter(commandText, sql_con);
                    sql_cmdBuilder = new SQLiteCommandBuilder(sql_adapter);
                    DataSet DS1 = new DataSet();
                    DataTable DT1 = new DataTable();
                    int Data1 = sql_adapter.Fill(DS1);
                    sql_adapter.Fill(DT1);

                    if (Data1 == 0)
                    {
                        welcomeSound.Play();
                        sql_cmd = sql_con.CreateCommand();
                        sql_cmd.CommandText = "INSERT INTO userSession (RFID, TimeInDate) VALUES (@RFID, @TimeInDate)";
                        sql_cmd.Parameters.Add(new SQLiteParameter("@RFID", IDtemp.Substring(0, 8)));
                        sql_cmd.Parameters.Add(new SQLiteParameter("@TimeInDate", TimeInTime.Text));
                        sql_cmd.ExecuteNonQuery();

                        LabelTime.Text = "Time In";
                        LabelTime.ForeColor = Color.Green;
                    }
                    else
                    {
                        exitSound.Play();
                        DateTime time_in;
                        DateTime time_out;
                        string timeInString = (string)DT1.Rows[0]["TimeInDate"];
                        DateTime.TryParse(timeInString, out time_in);
                        DateTime.TryParse(TimeInTime.Text, out time_out);

                        double totalHours = Math.Truncate((time_out - time_in).TotalHours * 100)/100;
                        LabelRole2.Visible = true;
                        LabelRole2.Text = totalHours.ToString() + " Hours";
                        //MessageBox.Show(totalHours.ToString(), "Total Hours:");

                        sql_cmd = sql_con.CreateCommand();
                        sql_cmd.CommandText = "DELETE FROM userSession WHERE RFID='" + IDtemp.Substring(0, 8) + "'";
                        sql_cmd.ExecuteNonQuery();

                        sql_cmd = sql_con.CreateCommand();
                        sql_cmd = sql_con.CreateCommand();
                        sql_cmd.CommandText = "INSERT INTO userSessionLogs (RFID, ID, NameENG, TimeInDate, TimeOut, Hours) " +
                            "VALUES (@RFID, @ID, @NameENG, @TimeInDate, @TimeOut, @Hours)";
                        sql_cmd.Parameters.Add(new SQLiteParameter("@RFID", IDtemp.Substring(0, 8)));
                        sql_cmd.Parameters.Add(new SQLiteParameter("@ID", DT.Rows[0]["ID"]));
                        sql_cmd.Parameters.Add(new SQLiteParameter("@NameENG", DT.Rows[0]["NameEng"]));
                        sql_cmd.Parameters.Add(new SQLiteParameter("@TimeInDate", DT1.Rows[0]["TimeInDate"]));
                        sql_cmd.Parameters.Add(new SQLiteParameter("@TimeOut", TimeInTime.Text));
                        sql_cmd.Parameters.Add(new SQLiteParameter("@Hours", totalHours));
                        sql_cmd.ExecuteNonQuery();
                        LabelTime.Text = "Time Out";
                        LabelTime.ForeColor = Color.Red;
                    }
                    await Task.Delay(2000);


                    TimeInName.Text = "";
                    TimeInID.Text = "";
                    TimeInTime.Text = "";
                    LabelTime.Text = "";
                    LabelRole2.Text = "";
                    LabelRole2.Visible = false;
                    pictureBox3.Image = null;
                    DT.Clear();
                    DT1.Clear();
                    DS.Clear();
                    DS1.Clear();
                }
                else
                {
                    errorSound.Play();
                }
                sql_con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
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

        private void button1_Click_2(object sender, EventArgs e)
        {

        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }


        private void ButtonCalculation_Click(object sender, EventArgs e)
        {
            sql_con.Close();
            if (DT != null)
            {
                DT.Clear();
            }
            PanelRegistrationEditUserData.Visible = false;
            PanelUserData.Visible = false;
            PanelConnection.Visible = false;
            PanelSessionLog.Visible = true;
            PanelTimeIn.Visible = false;
            sessionLogs();
        }

        private void ButtonTimeInOut_Click(object sender, EventArgs e)
        {
            sql_con.Close();
            if(DT != null)
            {
                DT.Clear();
            }            
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            PanelRegistrationEditUserData.Visible = false;
            PanelUserData.Visible = false;
            PanelConnection.Visible = false;
            PanelSessionLog.Visible = false;
            PanelTimeIn.Visible = true;

            TimerSerialIn.Enabled = true;
            TimerSerialIn.Start();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBoxLandline_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridSessionLog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void TextBoxSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void buttonExportSL_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            saveFileDialog1.Filter = "All files (*.*)|*.*|Excel files (*.xls)|*.xls";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            string cs = "URI=file:ville.db";
            string data = String.Empty;

            int i = 0;
            int j = 0;

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(cs))
                {
                    con.Open();

                    string stm = "SELECT * FROM userSessionLogs";

                    using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read()) // Reading Rows
                            {
                                for (j = 0; j <= rdr.FieldCount - 1; j++) // Looping throw colums
                                {
                                    data = rdr.GetValue(j).ToString();
                                    xlWorkSheet.Cells[i + 1, j + 1] = data;
                                }
                                i++;
                            }
                        }
                    }
                    con.Close();
                }
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    xlWorkBook.SaveAs(saveFileDialog1.FileName, Excel.XlFileFormat.xlWorkbookNormal);
                    xlWorkBook.Close(true, saveFileDialog1.FileName);
                }
                else
                    xlWorkBook.Close(false);

                // xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                MessageBox.Show("User Session Logs exported!");
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        private void ExportUD_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;
            object misValue = System.Reflection.Missing.Value;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            saveFileDialog1.Filter = "All files (*.*)|*.*|Excel files (*.xls)|*.xls";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            string cs = "URI=file:ville.db";
            string data = String.Empty;

            int i = 0;
            int j = 0;

            try
            {
                using (SQLiteConnection con = new SQLiteConnection(cs))
                {
                    con.Open();

                    string stm = "SELECT * FROM userData";

                    using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read()) // Reading Rows
                            {
                                for (j = 0; j <= rdr.FieldCount - 1; j++) // Looping throw colums
                                {
                                    data = rdr.GetValue(j).ToString();
                                    xlWorkSheet.Cells[i + 1, j + 1] = data;
                                }
                                i++;
                            }
                        }
                    }
                    con.Close();
                }
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    xlWorkBook.SaveAs(saveFileDialog1.FileName, Excel.XlFileFormat.xlWorkbookNormal);
                    xlWorkBook.Close(true, saveFileDialog1.FileName);
                }
                else
                    xlWorkBook.Close(false);

                // xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                MessageBox.Show("User Session Logs exported!");
                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
