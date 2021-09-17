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
    public class ServiceUpdatePrinterController : Controller
    {
        public DtClassAppsDataContextDataContext iObjContext_apps;
        DtClassAppsDataContextDataContext db_Context;
        private DtClassAliceContextDataContext db;
        private MenuLeftClass menuLeftClass = new MenuLeftClass();
        private _GeneralSettingClass GeneralSettingClass = new _GeneralSettingClass();
        private string iStrSessNRP = string.Empty;
        private string iStrSessDistrik = string.Empty;
        private string iStrSessGPID = string.Empty;
        private string iStrLoginDept = string.Empty;
        private string iStrNama = string.Empty;
        public string iStrREmarks = "";
        public char iChrTransc;


        // GET: ServiceUpdatePrinter
        public ActionResult Index()
        {
            if (Session["NRP"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.leftMenu = loadMenu();
            ViewBag.profile = getList("gp");
            ViewBag.distrik = getList("distrik");
            return View();
        }

        private string loadMenu()
        {
            this.pv_CustLoadSession();
            if (Session["leftMenu"] == null)
            {
                Session["leftMenu"] = menuLeftClass.recursiveMenu(0, Convert.ToInt32(iStrSessGPID));
            }
            return (string)Session["leftMenu"];
        }

        private void pv_CustLoadSession()
        {
            iStrSessNRP = (string)Session["NRP"];
            iStrSessDistrik = (string)Session["distrik"];
            iStrSessGPID = Convert.ToString(Session["gpId"] == null ? "1000" : Session["gpId"]);
            ViewBag.gp = iStrSessGPID;
        }

        public IEnumerable<SelectListItem> getList(string s_type)
        {
            List<itemSelect> ls = new List<itemSelect>();
            IEnumerable<SelectListItem> items;
            if (s_type == "gp")
            {
                iObjContext_apps = new DtClassAppsDataContextDataContext();

                if ((string)Session["distrik"] != "JIEP")
                {
                    var tbl_profile_filter_ = iObjContext_apps.TBL_Profiles.Where(f => f.GP_ID != 1);

                    foreach (var item in tbl_profile_filter_)
                    {
                        ls.Add(new itemSelect { text = item.Deskripsi, value = item.GP_ID.ToString() });
                    }
                }
                else
                {
                    var tbl_profile_ = iObjContext_apps.TBL_Profiles;

                    foreach (var item in tbl_profile_)
                    {
                        ls.Add(new itemSelect { text = item.Deskripsi, value = item.GP_ID.ToString() });
                    }
                }
            }

            if (s_type == "distrik")
            {
                db_Context = new DtClassAppsDataContextDataContext();
                //var view_distrik_ = db_ContextEngSOW.VW_DWH_DISTRIKs;
                var view_distrik_ = db_Context.VW_DWH_DISTRIKs;

                foreach (var item in view_distrik_)
                {
                    ls.Add(new itemSelect { text = item.DSTRCT_CODE, value = item.DSTRCT_CODE });
                }
            }

            items = ls.Select(c => new SelectListItem
            {
                Value = c.value,
                Text = c.text
            });

            return items;
        }

        public class itemSelect
        {
            public string text { get; set; }
            public string value { get; set; }
        }

        [HttpPost]
        public JsonResult Read(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var tbl = db.VW_UPDATE_SERVICE_PRINTERs;
                var data = tbl.ToDataSourceResult(take, skip, sort, filter);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult ReadStatus()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_STATUS_SERVICE_PRINTERs.OrderBy(d => d.Deskripsi);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Update(string lblpickup, string lblterima, TBL_T_SERVICE_PRINTER printer)
        {
            if (printer.TANDA_TERIMA_PICKUP == null && printer.TANDA_TERIMA_SELESAI == null)
            {
                try
                {
                    string iStrREmarks = string.Empty;
                    DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();

                    var data = db.TBL_T_SERVICE_PRINTERs.Where(a => a.ID_SERVICE == printer.ID_SERVICE).FirstOrDefault();
                    data.NO_PRINTER = printer.NO_PRINTER;
                    data.NAMA_PRINTER = printer.NAMA_PRINTER;
                    data.JENIS_PRINTER = printer.JENIS_PRINTER;
                    data.TGL_RUSAK = printer.TGL_RUSAK;
                    data.KETERANGAN = printer.KETERANGAN;
                    data.EST_PERBAIKAN = printer.EST_PERBAIKAN;
                    data.BIAYA = printer.BIAYA;
                    data.STATUS = printer.STATUS;
                    data.PR = printer.PR;
                    data.NO_ITEM_PR = printer.NO_ITEM_PR;
                    data.TANDA_TERIMA_PICKUP = lblpickup;
                    data.TANDA_TERIMA_SELESAI = lblterima;
                    data.MODIFIED_BY = iStrSessNRP;
                    data.MODIFIED_DATE = DateTime.Now;
                    db.SubmitChanges();
                    return this.Json(new { remarks = "Success" });
                }
                catch (Exception e)
                {
                    return this.Json(new { remarks = "Gagal", error = e.ToString() });
                }
            }
            else
            {
                try
                {
                    string iStrREmarks = string.Empty;
                    DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                    var data = db.TBL_T_SERVICE_PRINTERs.Where(a => a.ID_SERVICE == printer.ID_SERVICE).FirstOrDefault();
                    data.NO_PRINTER = printer.NO_PRINTER;
                    data.NAMA_PRINTER = printer.NAMA_PRINTER;
                    data.JENIS_PRINTER = printer.JENIS_PRINTER;
                    data.TGL_RUSAK = printer.TGL_RUSAK;
                    data.NAMA_PRINTER = printer.NAMA_PRINTER;
                    data.JENIS_PRINTER = printer.JENIS_PRINTER;
                    data.KETERANGAN = printer.KETERANGAN;
                    data.EST_PERBAIKAN = printer.EST_PERBAIKAN;
                    data.BIAYA = printer.BIAYA;
                    data.STATUS = printer.STATUS;
                    data.PR = printer.PR;
                    data.NO_ITEM_PR = printer.NO_ITEM_PR;
                    data.TANDA_TERIMA_PICKUP = printer.TANDA_TERIMA_PICKUP;
                    data.TANDA_TERIMA_SELESAI = printer.TANDA_TERIMA_SELESAI;
                    data.MODIFIED_BY = iStrSessNRP;
                    data.MODIFIED_DATE = DateTime.Now;
                    db.SubmitChanges();
                    return this.Json(new { remarks = "Success" });
                }
                catch (Exception e)
                {
                    return this.Json(new { remarks = "Gagal", error = e.ToString() });
                }
            }

        }

        //---------------------------------------------------------------------------------------

        //[HttpPost]
        public JsonResult Gets(string datas)
        {
            if (datas == null)
            {
                datas = "";
            }
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var body = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(datas); // Get data dari form
                var uploadpickup = Uploads("uploadpickup");
                var uploadselesai = Uploads("uploadselesai");
                
                var tables = db.TBL_T_SERVICE_PRINTERs.Where(a => a.ID_SERVICE == body["ID_SERVICE"]).FirstOrDefault();
               
                tables.NO_PRINTER = body["NO_PRINTER"];
                tables.NAMA_PRINTER = body["NAMA_PRINTER"];
                tables.JENIS_PRINTER = body["JENIS_PRINTER"];
                tables.TGL_RUSAK = body["TGL_RUSAK"];
                tables.NAMA_PRINTER = body["NAMA_PRINTER"];
                tables.JENIS_PRINTER = body["JENIS_PRINTER"];
                tables.KETERANGAN = body["KETERANGAN"];
                tables.EST_PERBAIKAN = body["EST_PERBAIKAN"];
                tables.BIAYA = decimal.Parse(body["BIAYA"]);
                tables.STATUS = body["STATUS"];
                tables.PR = body["PR"];
                tables.NO_ITEM_PR = body["NO_ITEM_PR"];
                tables.TANDA_TERIMA_PICKUP = uploadpickup["res"] == "SUCCESS" ? uploadpickup["val"] : tables.TANDA_TERIMA_PICKUP;
                tables.TANDA_TERIMA_SELESAI = uploadselesai["res"] == "SUCCESS" ? uploadselesai["val"] : tables.TANDA_TERIMA_SELESAI;
                tables.MODIFIED_BY = iStrSessNRP;
                tables.MODIFIED_DATE = DateTime.Now;
                db.SubmitChanges();
                
                string jsons = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                return Json(new { remarks = "Success" });
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
                    var fileName = fileContent.FileName; // get file name
                    string iGenerateID = Guid.NewGuid().ToString().Substring(1, 3); // generate id unik
                    var path = Path.Combine(Server.MapPath("~/FileUpload/CareerPath"), iGenerateID + "_" + fileName); // path untuk upload file
                    var iPath = "/FileUpload/CareerPath/" + iGenerateID + "_" + fileName; // path untuk database
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
                    return result;
                }
            }
            catch (Exception e)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                result.Add("res", "ERROR");
                return result;
            }



        }

        //---------------------------------------------------------------------------------------

        [HttpPost]
        public JsonResult Delete(TBL_T_SERVICE_PRINTER printer)
        {
            try
            {
                if (printer != null)
                {
                    DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                    var del = db.TBL_T_SERVICE_PRINTERs.Where(s => s.ID_SERVICE == printer.ID_SERVICE).FirstOrDefault();
                    db.TBL_T_SERVICE_PRINTERs.DeleteOnSubmit(del);
                    db.SubmitChanges();

                    return this.Json(new { remarks = "Success" });
                }
                else
                {
                    return this.Json(new { remarks = "id not found" });
                }

            }
            catch (Exception e)
            {
                return this.Json(new { remarks = "Gagal", error = e.ToString() });
            }
        }
        public JsonResult AjaxUpload(string idp)
        {
            string iStrREmarks = string.Empty;
            DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
            var fileContent = Request.Files["uploadpickup"]; //M    engambil file dari txtupload (name dari index)
            var iPath = string.Empty;
            string fileExt = System.IO.Path.GetExtension(fileContent.FileName).ToLower();
            if (fileContent != null && fileContent.ContentLength <= 2000000 && fileExt == ".pdf")// Kondisi jika fileContent ada isinya
            {
                string iGenerateID = Guid.NewGuid().ToString().Substring(1, 3);
                var fileName = Path.GetFileName(fileContent.FileName);// Membaca nama file dan ekstensi dari fileContent
                var path = Path.Combine(Server.MapPath("~/FileUpload/CareerPath"), iGenerateID + fileName);// Membuat path/alamat penyimpanan file + nama file -> dalam bentuk variabel
                iPath = "/FileUpload/CareerPath/" + iGenerateID + fileName;// Membaca path penyimpanan cuma folder dan nama file aja
                fileContent.SaveAs(path);// Untuk upload file

                var newUpload = db.TBL_T_SERVICE_PRINTERs.Where(a => a.ID_SERVICE == idp).FirstOrDefault();
                newUpload.TANDA_TERIMA_PICKUP = iPath;
                //db.TBL_SERVICE_PRINTERs.InsertOnSubmit(newUpload);
                db.SubmitChanges();  //saveUploadCareerPath(iPath);
                //buat view chrome 
                //string location = @"file:///D:/alice2/PrinterMonitoring/FileUpload/CareerPath/3ac8e07F95253.pdf";
                //byte[] pdfByteArray = System.IO.File.ReadAllBytes(location);
                //string base64EncodedPDF = System.Convert.ToBase64String(pdfByteArray);
                //tutup view chrome 

                return this.Json(new { status = true, remark = "Upload Berhasil", iStrPath = iPath/*, resultLink = base64EncodedPDF*/ }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(new { status = false, remark = "Tipe Harus PDF & Ukuran < 2000 Kb" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult AjaxUploadSelesai(string idu)
        {
            string iStrREmarksupload = string.Empty;
            DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
            var fileContent = Request.Files["uploadselesai"]; //M    engambil file dari txtupload (name dari index)
            var iPathupload = string.Empty;
            string fileExt = System.IO.Path.GetExtension(fileContent.FileName).ToLower();
            if (fileContent != null && fileContent.ContentLength <= 2000000 && fileExt == ".pdf")// Kondisi jika fileContent ada isinya
            {
                string iGenerateID = Guid.NewGuid().ToString().Substring(1, 3);
                var fileNameupload = Path.GetFileName(fileContent.FileName);// Membaca nama file dan ekstensi dari fileContent
                var path = Path.Combine(Server.MapPath("~/FileUpload/UploadSelesai"), iGenerateID + fileNameupload);// Membuat path/alamat penyimpanan file + nama file -> dalam bentuk variabel
                iPathupload = "/FileUpload/UploadSelesai/" + iGenerateID + fileNameupload;// Membaca path penyimpanan cuma folder dan nama file aja
                fileContent.SaveAs(path);// Untuk upload file

                var newUpload = db.TBL_T_SERVICE_PRINTERs.Where(a => a.ID_SERVICE == idu).FirstOrDefault();
                newUpload.TANDA_TERIMA_PICKUP = iPathupload;
              
                db.SubmitChanges();  //saveUploadCareerPath(iPath);
             
                return this.Json(new { status = true, remark = "Upload Berhasil", iStrPathupload = iPathupload}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(new { status = false, remark = "Tipe Harus PDF & Ukuran < 2000 Kb" }, JsonRequestBehavior.AllowGet);
            }
        }
       
    }
}
