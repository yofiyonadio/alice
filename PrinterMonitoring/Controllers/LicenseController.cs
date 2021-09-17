using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrinterMonitoring.Models;
using Kendo.DynamicLinq;
using System.IO;

namespace PrinterMonitoring.Controllers
{
    public class LicenseController : Controller
    {

        private DtClassAliceContextDataContext db;
        private DtClassAliceContextDataContext db1 = new DtClassAliceContextDataContext();
        private LicenseDataContext db2 = new LicenseDataContext();

        // GET: License
        public ActionResult RegisterLicense()
        {
            return View();
        }

        private Dictionary<dynamic, dynamic> JsonToDict(string datas)
        {
            Dictionary<dynamic, dynamic> body = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(datas);
            return body;
        }

        private string DictToJson(Dictionary<string, string> datas)
        {
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(datas);
            return body;
        }

        private List<dynamic> JsonToList(string datas)
        {
            List<dynamic> body = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(datas);
            return body;
        }

        private string ListToJson(List<dynamic> datas)
        {
            string body = Newtonsoft.Json.JsonConvert.SerializeObject(datas);
            return body;
        }



        [HttpPost]
        public JsonResult Read(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            try
            {
                var users = Session["NRP"]; // Get User
                
                var tbl = db2.TBL_LICENSE_REGISTERs;
                var data = tbl.OrderByDescending(c => c.timestamps).ToDataSourceResult(take, skip, sort, filter);
                //System.Diagnostics.Debug.WriteLine(data);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }

        }

        [HttpPost]
        public JsonResult ReadRenewal(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter, string datas)
        {
            try
            {
                var users = Session["NRP"]; // Get User

                var tbl = db2.TBL_LICENSE_RENEWALs;
                var data = tbl.Where(c => c.license_number == datas).OrderByDescending(c => c.id).ToDataSourceResult(take, skip, sort, filter);
                //System.Diagnostics.Debug.WriteLine(decode);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }

        }

        [HttpPost]
        public JsonResult Gets(string datas)
        {
            if (datas == null || datas == "undefined")
            {
                datas = "";
            }
            try
            {
                
                //Tbl_Pengajuans
                var tbl = db1.VW_SOFTWARE_USER_INSTALLEDs.Where(c => c.Device_Name.Contains(datas) || c.wDisplayName.Contains(datas)).Skip(0).Take(50).ToList();
                return Json(new { result = "SUCCESS", data = tbl });
                
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }

        }

        [HttpPost]
        public JsonResult GetPO(string datas)
        {
            if (datas == null || datas == "undefined")
            {
                datas = "";
            }
            try
            {
                Dictionary<string, string> body = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(datas);
                var tbl = db2.VW_PR_PO_ELLIPSE_REVIEWs.Where(c => c.PO.Equals(body["po"]) && c.PO_ITEM.Equals(body["item"])).Take(1).ToList();
                //System.Threading.Thread.Sleep(1000);
                return Json(new { result = "SUCCESS", data = tbl });
               
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult GetAllPO(string datas)
        {
            if (datas == null || datas == "undefined")
            {
                datas = "";
            }
            try
            {
                var tbl = db2.VW_PR_PO_ELLIPSE_REVIEWs.Where(c => c.PO.Contains(datas)).Skip(0).Take(50).OrderBy(c => c.PO).ToList();
                return Json(tbl);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }

        public Dictionary<string, string> Uploads(dynamic file)
        {
            try
            {
                var count = Request.Files.Count;
                if (count > 0)
                {
                    var fileContent = Request.Files[file]; // get file dari form
                    string fileName = fileContent.FileName; // get file name
                    string iGenerateID = Guid.NewGuid().ToString().Substring(1, 3); // generate id unik
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/FileUpload/License"));
                    var path = Path.Combine(Server.MapPath("~/FileUpload/License"), iGenerateID + "_" + fileName); // path untuk upload file
                    var iPath = "/FileUpload/License/" + iGenerateID + "_" + fileName; // path untuk database
                    fileContent.SaveAs(path); // save upload
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("res", "SUCCESS");
                    result.Add("val", iPath);
                    return result;
                }
                else
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("res", "ERROR");
                    result.Add("val", null);
                    return result;
                }
            }
            catch (Exception e)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                result.Add("res", "ERROR");
                result.Add("val", null);
                return result;
            }
        }

        [HttpPost]
        public JsonResult AddLicense(string datas)
        {
            try
            {
       
                // System.Diagnostics.Debug.WriteLine(Session["NRP"]);
                // System.Diagnostics.Debug.WriteLine(Session["Name"]);

                var filess = Uploads("files");

                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                // Parse Json to Dictionary
                Dictionary<string, string> body = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(datas);
                body.Add("timestamps", now);
                body["file"] =  filess["val"];
                //body.Add("file", filess["val"]);
                if (Session["NRP"] != null) {
                    body.Add("created_by", Session["NRP"].ToString());
                } 

                // Parsing Format Date
                DateTime temp;
                if (DateTime.TryParse(body["start"], out temp)) body["start"] = DateTime.Parse(body["start"]).ToString("yyyy-MM-dd");
                if (DateTime.TryParse(body["end"], out temp)) body["end"] = DateTime.Parse(body["end"]).ToString("yyyy-MM-dd");

                // Parse Dictionary to Json
                string bodyDecode = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                TBL_LICENSE_REGISTER bodys = Newtonsoft.Json.JsonConvert.DeserializeObject<TBL_LICENSE_REGISTER>(bodyDecode);


                db2.TBL_LICENSE_REGISTERs.InsertOnSubmit(bodys);

                for (int i = 0; i < Int32.Parse(bodys.qty); i++)
                {
                    TblLicenseDetail bodysLcsDetail = new TblLicenseDetail();
                    bodysLcsDetail.LicenseNumber = bodys.license_number;
                    bodysLcsDetail.LicenseNumberDetail = bodys.license_number + "-" + (i + 1).ToString("0000");
                    bodysLcsDetail.RegistrationNumber = "1";
                    db2.TblLicenseDetails.InsertOnSubmit(bodysLcsDetail);
                }

                db2.SubmitChanges();

                return Json(new { result = "SUCCESS" });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Method2 error.");
                return this.Json(new { error = e.ToString() });
            }

        }

        [HttpPost]
        public JsonResult DeleteLicense(string datas)
        {
            try
            {
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                //db2.TBL_LICENSE_REGISTERs.InsertOnSubmit(bodys);
                var del = db2.TBL_LICENSE_REGISTERs.Where(s => s.id == Int32.Parse(datas)).FirstOrDefault();
                db2.TBL_LICENSE_REGISTERs.DeleteOnSubmit(del);
                db2.SubmitChanges();

                //System.Diagnostics.Debug.WriteLine("Method2 successful.");
                return Json(new { result = "SUCCESS" });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Method2 error.");
                return this.Json(new { result = e.ToString() });
            }

        }

        [HttpPost]
        public JsonResult EditLicense(string datas)
        {
            try
            {
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                Dictionary<string, string> body = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(datas);

                // Parsing Format Date
                //DateTime temp;
                //if (DateTime.TryParse(body["start"], out temp)) body["start"] = DateTime.Parse(body["start"]).ToString("yyyy-MM-dd");
                //if (DateTime.TryParse(body["end"], out temp)) body["end"] = DateTime.Parse(body["end"]).ToString("yyyy-MM-dd");

                var e = db2.TBL_LICENSE_REGISTERs.Where(s => s.id == Int32.Parse(body["id"])).FirstOrDefault();

                DateTime temp;
                if (DateTime.TryParse(body["start"], out temp)) body["start"] = DateTime.Parse(body["start"]).ToString("yyyy-MM-dd");
                if (DateTime.TryParse(body["end"], out temp)) body["end"] = DateTime.Parse(body["end"]).ToString("yyyy-MM-dd");

                e.software_id = body["software_id"];
                e.license_number = body["license_number"];
                e.license_type = body["license_type"];
                e.po_number = body["po_number"];
                e.item_number = body["item_number"];
                e.qty = body["qty"];
                e.vendor_name = body["vendor_name"];
                e.currency = body["currency"];
                e.price = body["price"];
                e.license_category = body["license_category"];
                e.start = body["start"];
                e.end = body["end"];
                e.desc = body["desc"];
                e.last_update = now;
                if (Session["NRP"] != null)
                {
                    e.updated_by = Session["NRP"].ToString();
                }
                db2.SubmitChanges();

                //System.Diagnostics.Debug.WriteLine("Method2 successful.");
                return Json(new { result = "SUCCESS" });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return this.Json(new { res = e.ToString() });
            }

        }

        [HttpGet]
        public JsonResult getLicenseNumber(string datas)
        {
            try
            {
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
             
                var tbl = db2.TBL_LICENSE_REGISTERs.OrderByDescending(c => c.id).FirstOrDefault();

                string licenseNumber;

                var year = DateTime.Now.ToString("yy");

                if (tbl == null)
                {
                    licenseNumber = "PMLI" + year + 1.ToString("0000");
                } else
                {
                    var oldLicenseNumber = tbl.license_number;
                    var skip = 6;
                    var getNumber = oldLicenseNumber.Substring(skip, oldLicenseNumber.Length - skip);
                    var newNumber = (Int32.Parse(getNumber) + 1).ToString("0000");
                    //System.Diagnostics.Debug.WriteLine();
                    licenseNumber = "PMLI" + year + newNumber;
                }
                

                //System.Diagnostics.Debug.WriteLine("Method2 successful.");
                return Json(new { result = "SUCCESS", data = licenseNumber }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return this.Json(new { res = e.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult RenewalLicenseList(string datas)
        {
            if (datas == null || datas == "undefined")
            {
                datas = "";
            }
            try
            {

                var tbl = db2.TBL_LICENSE_REGISTERs.Where(c => c.license_number.Contains(datas)).Skip(0).Take(50).OrderByDescending( c =>c.id).ToList();
                return Json(new { result = "SUCCESS", data = tbl }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult RenewalLicenseData(string datas)
        {
            if (datas == null || datas == "undefined")
            {
                datas = "";
            }
            try
            {

                var tbl = db2.TBL_LICENSE_REGISTERs.Where(c => c.license_number == datas).FirstOrDefault();
                return Json(new { result = "SUCCESS", data = tbl }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult RenewalLicenseUpdate(string datas)
        {
            if (datas == null || datas == "undefined")
            {
                datas = "";
            }
            try
            {
                    string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    Dictionary<string, string> body = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(datas);

                    // Parsing Format Date
                    DateTime temp;
                    if (DateTime.TryParse(body["start"], out temp)) body["start"] = DateTime.Parse(body["start"]).ToString("yyyy-MM-dd");
                    if (DateTime.TryParse(body["end"], out temp)) body["end"] = DateTime.Parse(body["end"]).ToString("yyyy-MM-dd");

                    var e = db2.TBL_LICENSE_REGISTERs.Where(c => c.license_number == body["license_number"]).FirstOrDefault();


                    var tblToJson = Newtonsoft.Json.JsonConvert.SerializeObject(e);
                    TBL_LICENSE_RENEWAL bodys = Newtonsoft.Json.JsonConvert.DeserializeObject<TBL_LICENSE_RENEWAL>(tblToJson);
                    bodys.timestamps = now;
                    

                db2.TBL_LICENSE_RENEWALs.InsertOnSubmit(bodys);

                    e.software_id = body["software_id"];
                    e.license_number = body["license_number"];
                    e.license_type = body["license_type"];
                    e.po_number = body["po_number"];
                    e.item_number = body["item_number"];
                    e.qty = body["qty"];
                    e.vendor_name = body["vendor_name"];
                    e.currency = body["currency"];
                    e.price = body["price"];
                    e.license_category = body["license_category"];
                    e.start = body["start"];
                    e.end = body["end"];
                    e.desc = body["desc"];
                    e.last_update = now;

                    db2.SubmitChanges();

                return Json(new { result = "SUCCESS", data = e });

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult GridlLicenseDetail(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            try
            {
                var tbl = db2.VW_TblLicenseDetails;
                var data = tbl.OrderBy(c => c.id).ToDataSourceResult(take, skip, sort, filter);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }

        }

        [HttpPost]
        public JsonResult GetDistrict(string datas)
        {
            if (datas == null || datas == "undefined")
            {
                datas = "";
            }
            try
            {
                var tbl = db2.TBL_LIST_DISTRICTs.Where(c => c.district.Contains(datas)).ToList();
                return Json(new { result = "SUCCESS", data = tbl }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult MappingLicenseDistribution(string datas)
        {
            try
            {
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                Dictionary<dynamic, dynamic> body = JsonToDict(datas);
                Dictionary<dynamic, dynamic> data = JsonToDict(body["datas"]);

                List<dynamic> keys = JsonToList(body["keys"]);

                var tbl = new List<TblLicenseDetail>(db2.TblLicenseDetails.Where(c => keys.Contains(c.id)).ToList());

                tbl.ForEach(c => {
                    c.DistrictID = data["district"];
                    if (data["desc"] != "")
                    {
                        c.Description = data["desc"];
                    }
                    
                });
                
                db2.SubmitChanges();

                return Json(new { result = "SUCCESS" });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return this.Json(new { res = e.ToString() });
            }

        }












        [HttpPost]
        public JsonResult Tess(string datas)
        {
            try
            {

                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                System.Diagnostics.Debug.WriteLine("Method Tes successful.");
                return Json(now);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Method Tes error.");
                return this.Json(new { error = e.ToString() });
            }

        }












    }
}