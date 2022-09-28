using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ticket_booking
{
    public partial class EditFlight : Form
    {
        string filePath, oldFile, fileName;
        string action = "Edit";
        Flight flight;
        public EditFlight()
        {
            InitializeComponent();
        }
        public int FlightToEditDelete { get; set; }
        public ICrossDataFormLoadSync FormToReload { get; set; }

        private void button4_Click(object sender, EventArgs e)
        {
            this.action = "Delete";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"DELETE  flight  
                                            WHERE fID=@i DELETE FROM [flight]
                                    WHERE fID = @i;", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Deleted", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                tran.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }

                    }
                }

            }
        }

        private void EditFlight_Load(object sender, EventArgs e)
        {
            ShowData();
        }
        private void ShowData()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM flight WHERE fID =@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.FlightToEditDelete);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        dateTimePicker1.Value = dr.GetDateTime(2);
                        textBox3.Text = dr.GetString(3);
                        textBox4.Text = dr.GetString(4);
                        textBox5.Text = dr.GetDecimal(5).ToString("0.00");
                        checkBox1.Checked = dr.GetBoolean(6);
                        oldFile = dr.GetString(7).ToString();
                        pictureBox1.Image = Image.FromFile(Path.Combine(@"..\..\pictures", dr.GetString(7).ToString()));
                    }
                    con.Close();
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label7.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.action = "Edit";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE  flight  
                                            SET  airline_name=@n, flight_date=@d, leave_from= @l, going_to=@g, ticket_price=@p,seat_avilable=@a, airline_pic=@x 
                                            WHERE fID=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@l", textBox3.Text);
                        cmd.Parameters.AddWithValue("@g", textBox4.Text);
                        cmd.Parameters.AddWithValue("@p", decimal.Parse(textBox5.Text));
                        cmd.Parameters.AddWithValue("@a", checkBox1.Checked);
                        if (!string.IsNullOrEmpty(this.filePath))
                        {
                            string ext = Path.GetExtension(this.filePath);
                            fileName = $"{Guid.NewGuid()}{ext}";
                            string savePath = Path.Combine(Path.GetFullPath(@"..\..\pictures"), fileName);
                            File.Copy(filePath, savePath, true);
                            cmd.Parameters.AddWithValue("@x", fileName);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@x", oldFile);
                        }


                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                flight = new Flight
                                {
                                    fID = int.Parse(textBox1.Text),
                                    airline_name = textBox2.Text,
                                    flight_date = dateTimePicker1.Value,
                                    leave_from = textBox3.Text,
                                    going_to = textBox4.Text,
                                    ticket_price = decimal.Parse(textBox5.Text),
                                    seat_available = checkBox1.Checked,
                                    airline_pic = filePath == "" ? oldFile : fileName
                                };
                                tran.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }

                    }
                }

            }
        }

        private void EditFlight_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.FormToReload.UpdateFlight(flight);
            else
                this.FormToReload.RemoveFlight(Int32.Parse(this.textBox1.Text));
        }
    }
}
