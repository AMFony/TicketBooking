using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ticket_booking
{
    public partial class EditPassenger : Form
    {
        string action = "Edit";
        Passenger passenger;
        public EditPassenger()
        {
            InitializeComponent();
        }
        public int PassengerToEditDelete { get; set; }
        public ICrossDataFormLoadSync FormToReloaded { get; set; }
        
        private void button1_Click(object sender, EventArgs e)
        {
            this.action = "Edit";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE  passenger  
                                            SET  pName=@n, pAddress=@a, fID=@fi 
                                            WHERE pID=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@a", textBox3.Text);
                        cmd.Parameters.AddWithValue("@fi", textBox4.Text);
                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                passenger = new Passenger
                                {
                                    pID = int.Parse(textBox1.Text),
                                    pName = textBox2.Text,
                                    pAddress = textBox3.Text,
                                    fID = int.Parse(textBox4.Text),
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
        private void EditPassenger_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.FormToReloaded.UpdatePassenger(passenger);
            else
                this.FormToReloaded.RemovePassenger(Int32.Parse(this.textBox1.Text));
        }

        private void EditPassenger_Load(object sender, EventArgs e)
        {
            ShowData();
        }
        private void ShowData()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM passenger WHERE pID =@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.PassengerToEditDelete);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        textBox3.Text = dr.GetDecimal(2).ToString("0.00");
                        textBox4.Text = dr.GetInt32(3).ToString();
                    }
                    con.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.action = "Delete";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"DELETE  passenger  
                                            WHERE pID=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
    }
}
