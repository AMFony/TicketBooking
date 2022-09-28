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
    public partial class AddPassenger : Form
    {
        List<Passenger> passenger = new List<Passenger>();
        public AddPassenger()
        {
            InitializeComponent();
        }
        public ICrossDataFormLoadSync FormToSysnc { get; set; }
        
        

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO passenger 
                                            (pID, pName, pAddress, fID) VALUES
                                            (@i, @n, @a, @f)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@f", int.Parse(textBox4.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@a", textBox3.Text);
                        ;


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
        private void AddPassenger_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormToSysnc.ReloadPassenger(this.passenger);
        }

        private void AddPassenger_Load_1(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewPassengerId().ToString();
        }
        private int GetNewPassengerId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(pID), 0) FROM passenger", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    return id + 1;
                }
            }
        }

    }
}
