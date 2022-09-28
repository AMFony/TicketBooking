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
    public partial class FormGroupRpt : Form
    {
        public FormGroupRpt()
        {
            InitializeComponent();
        }

        private void FormGroupRpt_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM flight", con))
                {
                    da.Fill(ds, "flighti");
                    ds.Tables["flighti"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["flighti"].Rows.Count; i++)
                    {
                        ds.Tables["flighti"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\pictures"), ds.Tables["flighti"].Rows[i]["airline_pic"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM passenger";
                    da.Fill(ds, "passenger");
                    TicketGroupRpt rpt = new TicketGroupRpt();
                    rpt.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = rpt;
                    rpt.Refresh();
                    crystalReportViewer1.Refresh();
                }
            }
        }
    }
    
}
