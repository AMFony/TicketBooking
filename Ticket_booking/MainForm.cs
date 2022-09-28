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
using Ticket_booking.Reports;

namespace Ticket_booking
{
    public partial class MainForm : Form, ICrossDataFormLoadSync
    {
        DataSet ds;
        BindingSource bsFlight = new BindingSource();
        BindingSource bsPassenger = new BindingSource();

        public MainForm()
        {
            InitializeComponent();
        }
        public void LoadData()
        {
            ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM flight", con))
                {
                    da.Fill(ds, "flight");
                    ds.Tables["flight"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["flight"].Rows.Count; i++)
                    {
                        ds.Tables["flight"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\pictures"), ds.Tables["flight"].Rows[i]["airline_pic"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM passenger";
                    da.Fill(ds, "passenger");
                    DataRelation rel = new DataRelation("FK_FLIGHT_PASSENGER",
                        ds.Tables["flight"].Columns["fID"],
                        ds.Tables["passenger"].Columns["fID"]);
                    ds.Relations.Add(rel);
                    ds.AcceptChanges();
                }
            }
        }
        private void BindData()
        {
            bsFlight.DataSource = ds;
            bsFlight.DataMember = "flight";
            bsPassenger.DataSource = bsFlight;
            bsPassenger.DataMember = "FK_FLIGHT_PASSENGER";
            this.dataGridView1.DataSource = bsPassenger;
            lblName.DataBindings.Add(new Binding("Text", bsFlight, "airline_name"));
            labelDate.DataBindings.Add(new Binding("Text", bsFlight, "flight_date"));
            lblFrom.DataBindings.Add(new Binding("Text", bsFlight, "leave_from"));
            lblTo.DataBindings.Add(new Binding("Text", bsFlight, "going_to"));
            lblPrice.DataBindings.Add(new Binding("Text", bsFlight, "ticket_price"));
            checkBox1.DataBindings.Add(new Binding("Checked", bsFlight, "seat_avilable"));
            pictureBox1.DataBindings.Add(new Binding("Image", bsFlight, "image", true));
        }

        public void ReloadData(List<Flight> flight)
        {
            foreach (var e in flight)
            {
                DataRow dr = ds.Tables["flight"].NewRow();
                dr[0] = e.fID;
                dr["airline_name"] = e.airline_name;
                dr["flight_date"] = e.flight_date;
                dr["leave_from"] = e.leave_from;
                dr["going_to"] = e.going_to;
                dr["ticket_price"] = e.ticket_price;
                dr["seat_avilable"] = e.seat_available;
                dr["airline_pic"] = e.airline_pic;
                dr["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\pictures"), e.airline_pic));
                ds.Tables["flight"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            bsFlight.MoveLast();
        }

        public void ReloadPassenger(List<Passenger> passenger)
        {
            foreach (var e in passenger)
            {
                DataRow dr = ds.Tables["passenger"].NewRow();
                dr[0] = e.pID;
                dr["pName"] = e.pName;
                dr["pAddress"] = e.pAddress;
                dr["fID"] = e.fID;
                ds.Tables["pssenger"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            bsFlight.MoveLast();
        }

        public void RemovePassenger(int id)
        {
            for (var i = 0; i < ds.Tables["flight"].Rows.Count; i++)
            {
                if ((int)ds.Tables["flight"].Rows[i]["fID"] == id)
                {
                    ds.Tables["flight"].Rows.RemoveAt(i);
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void UpdatePassenger(Passenger p)
        {
            for (var i = 0; i < ds.Tables["passenger"].Rows.Count; i++)
            {
                if ((int)ds.Tables["passenger"].Rows[i]["pID"] == p.pID)
                {
                    ds.Tables["flight"].Rows[i]["pName"] = p.pName;
                    ds.Tables["flight"].Rows[i]["pAddress"] = p.pAddress;
                    ds.Tables["flight"].Rows[i]["fID"] = p.fID;
                    break;
                }
            }
            ds.AcceptChanges();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (bsFlight.Position < bsFlight.Count - 1)
            {
                bsFlight.MoveNext();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bsFlight.Position > 0)
            {
                bsFlight.MovePrevious();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            bsFlight.MoveFirst();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            bsFlight.MoveLast();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddFlight() { FormToReloaded = this }.ShowDialog();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new AddPassenger { FormToSysnc = this }.ShowDialog();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = (int)(this.bsPassenger.Current as DataRowView).Row[0];
            new EditPassenger { PassengerToEditDelete = id, FormToReloaded = this }.ShowDialog();
        }
       
        private void button6_Click(object sender, EventArgs e)
        {
            new AddFlight() { FormToReloaded = this }.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int id = (int)(this.bsFlight.Current as DataRowView).Row[0];
            new EditFlight { FlightToEditDelete = id, FormToReload = this }.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadData();
            BindData();
        }

        public void UpdateFlight(Flight flight)
        {
            for (var i = 0; i < ds.Tables["flight"].Rows.Count; i++)
            {
                if ((int)ds.Tables["flight"].Rows[i]["fID"] == flight.fID)
                {
                    ds.Tables["flight"].Rows[i]["airline_name"] = flight.airline_name;
                    ds.Tables["flight"].Rows[i]["flight_date"] = flight.flight_date;
                    ds.Tables["flight"].Rows[i]["leave_from"] = flight.leave_from;
                    ds.Tables["flight"].Rows[i]["going_to"] = flight.going_to;
                    ds.Tables["flight"].Rows[i]["ticket_price"] = flight.ticket_price;
                    ds.Tables["flight"].Rows[i]["airline_pic"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\pictures"), flight.airline_pic));
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void RemoveFlight(int id)
        {
            for (var i = 0; i < ds.Tables["flight"].Rows.Count; i++)
            {
                if ((int)ds.Tables["flight"].Rows[i]["fID"] == id)
                {
                    ds.Tables["flight"].Rows.RemoveAt(i);
                    break;
                }
            }
            ds.AcceptChanges();
        }


        private void flightRptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormGroupRpt().ShowDialog();
        }

        private void editDeleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int id = (int)(this.bsFlight.Current as DataRowView).Row[0];
            new EditFlight { FlightToEditDelete = id, FormToReload = this }.ShowDialog();
        }

        private void flightToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new FormFlightRpt().ShowDialog();
        }
    }
}
