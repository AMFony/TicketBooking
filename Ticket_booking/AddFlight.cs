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
    public partial class AddFlight : Form
    {
        string filePath = "";
        string fileName = "";
        List<Flight> flight = new List<Flight>();
        public AddFlight()
        {
            InitializeComponent();
        }
        public ICrossDataFormLoadSync FormToReloaded { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.pictureBox1.Image = Image.FromFile(this.filePath);
                this.label6.Text = Path.GetFileName(this.filePath);
            }
        }
        private void AddFlight_Load_1(object sender, EventArgs e)
        {
            textBox1.Text = GetNewFlightId().ToString();
        }

        private object GetNewFlightId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(fID), 0) FROM flight", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    return id + 1;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO flight 
                                            (fID, airline_name, flight_date, leave_from, going_to, ticket_price, seat_avilable, airline_pic) VALUES
                                            (@i, @n, @f, @l, @g, @p, @a, @x)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@f", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@l", textBox3.Text);
                        cmd.Parameters.AddWithValue("@g", textBox5.Text);
                        cmd.Parameters.AddWithValue("@p", decimal.Parse(textBox4.Text));
                        cmd.Parameters.AddWithValue("@a", checkBox1.Checked);
                        string ext = Path.GetExtension(this.filePath);
                        fileName = $"{Guid.NewGuid()}{ext}";
                        string savePath = Path.Combine(Path.GetFullPath(@"..\..\pictures"), fileName);
                        File.Copy(filePath, savePath, true);
                        cmd.Parameters.AddWithValue("@x", fileName);

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                flight.Add(new Flight
                                {
                                    fID = int.Parse(textBox1.Text),
                                    airline_name = textBox2.Text,
                                    flight_date = dateTimePicker1.Value,
                                    leave_from = textBox3.Text,
                                    going_to = textBox5.Text,
                                    ticket_price = decimal.Parse(textBox4.Text),
                                    seat_available = checkBox1.Checked,
                                    airline_pic = fileName
                                });
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
        private void AddFlight_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormToReloaded.ReloadData(this.flight);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
