using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PrinterMonitoring.Reports
{
    public partial class ReportGeneral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pv_cust_generateReport(Request.QueryString["s_str_reportName"], Request.QueryString["s_str_parameter"], Request.QueryString["s_str_param_value"]);
            }
        }


        private void pv_cust_generateReport(string s_str_reportName, string s_str_parameter, string s_str_param_value)
        {
            int i_int_totParam = s_str_parameter.Split(';').Count();
            string[] i_arr_param = s_str_parameter.Split(';').ToArray();
            string[] i_arr_param_value = s_str_param_value.Split(';').ToArray();

            Microsoft.Reporting.WebForms.ReportParameter[] i_cls_rParam = new Microsoft.Reporting.WebForms.ReportParameter[i_int_totParam];

            //for (int i = 0; i < i_int_totParam; i++)
            //{
            //    i_cls_rParam[i] = new Microsoft.Reporting.WebForms.ReportParameter(i_arr_param[i], i_arr_param_value[i]);
            //}


            //if (s_str_parameter != "" && s_str_param_value != "")
            //{
            //    Report.ServerReport.SetParameters(i_cls_rParam);
            //}

            Report.Height = 1200;

            Report.ProcessingMode = ProcessingMode.Remote;
            string i_str_reportName = s_str_reportName;
            Report.ServerReport.ReportServerUrl = new Uri(System.Configuration.ConfigurationManager.AppSettings["ReportServerUrl"].ToString());
            Report.ServerReport.ReportPath = System.Configuration.ConfigurationManager.AppSettings["ReportPath"].ToString() + i_str_reportName;
            Report.AsyncRendering = false;
            Report.ServerReport.Refresh();

        }

    }
}