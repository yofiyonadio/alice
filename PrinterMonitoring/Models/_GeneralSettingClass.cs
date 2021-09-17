using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Configuration;

namespace PrinterMonitoring.Models
{
    public class _GeneralSettingClass
    {
        public enum Action { Edit, Delete, Read, Insert }
        DtClassAppsDataContextDataContext db_;

        public bool IsValidPermission(int sPrimer, int sGPID, Action sAction, ref string sStrRemarks)
        {
            bool iBlReturn = false;
            string iActionDesc = string.Empty;

            db_ = new DtClassAppsDataContextDataContext();
            var Tbl_ = db_.Menu_GPs.Where(f => f.Primer == sPrimer && f.GP_ID == sGPID).FirstOrDefault();

            if (Tbl_ != null)
            {
                switch (sAction)
                {
                    case Action.Insert:
                        iBlReturn = (bool)Tbl_.A;
                        iActionDesc = "Insert";
                        break;
                    case Action.Delete:
                        iBlReturn = (bool)Tbl_.D;
                        iActionDesc = "Delete";
                        break;
                    case Action.Edit:
                        iBlReturn = (bool)Tbl_.E;
                        iActionDesc = "Update";
                        break;
                    case Action.Read:
                        iBlReturn = (bool)Tbl_.R;
                        iActionDesc = "Read";
                        break;
                    default:
                        iBlReturn = false;
                        break;
                }
            }

            sStrRemarks = iBlReturn ? "Validation Ok !" : string.Format("Anda tidak memiliki otorisasi {0} di form ini !", iActionDesc);
            return iBlReturn;
        }

        public DataTable ProcessCSV(string fileName, string sSessUpload)
        {
            //Set up our variables
            string Feedback = string.Empty;
            string line = string.Empty;
            string[] strArray;
            List<string> strList;

            DataTable dt = new DataTable();
            DataRow row;
            // work out where we should split on comma, but not in a sentence
            Regex r = new Regex(";(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            //Set the filename in to our stream
            StreamReader sr = new StreamReader(fileName);

            //Read the first line and split the string at , with our regular expression in to an array
            line = sr.ReadLine();
            strList = r.Split(line).ToList();
            strList.Insert(0, "pid");
            strList.Insert(0, "add1");
            strList.Add("add2");
            strList.Add("add3");
            strList.Add("add4");
            //strArray = r.Split(line);
            strArray = strList.ToArray();

            //For each item in the new split array, dynamically builds our Data columns. Save us having to worry about it.
            Array.ForEach(strArray, s => dt.Columns.Add(new DataColumn()));

            //Read each line in the CVS file until it’s empty
            while ((line = sr.ReadLine()) != null)
            {
                row = dt.NewRow();

                //add our current value to our data row
                line = string.Format(";{0};{1};;;{2}", sSessUpload, line, DateTime.Now);
                row.ItemArray = r.Split(line);
                dt.Rows.Add(row);
            }

            //Tidy Streameader up
            sr.Dispose();
            //return a the new DataTable
            return dt;
        }

        public String ProcessBulkCopy(DataTable dt, string sConnetionString, string sTblTempName)
        {
            string Feedback = string.Empty;
            string connString = ConfigurationManager.ConnectionStrings[sConnetionString].ConnectionString;

            //make our connection and dispose at the end
            using (SqlConnection conn = new SqlConnection(connString))
            {
                //make our command and dispose at the end
                using (var copy = new SqlBulkCopy(conn))
                {
                    //Open our connection
                    conn.Open();
                    ///Set target table and tell the number of rows
                    copy.DestinationTableName = sTblTempName;
                    copy.BatchSize = dt.Rows.Count;
                    try
                    {
                        //Send it to the server
                        copy.WriteToServer(dt);
                        Feedback = "Upload complete";
                    }
                    catch (Exception ex)
                    {
                        Feedback = ex.Message;
                    }
                }
            }
            return Feedback;
        }
    }
}