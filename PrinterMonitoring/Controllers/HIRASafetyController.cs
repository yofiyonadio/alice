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
    public class HIRASafetyController : Controller
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

        // GET: HIRASafety
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
                var tbl = db.TBL_M_HIRAs;
                var data = tbl.ToDataSourceResult(take, skip, sort, filter);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }
        [HttpPost]
        public JsonResult ReadProses()
        {
            DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
            var tbl_profile = db.TBL_DAFTAR_PROSES_HIRAs;
            return Json(new { Total = tbl_profile.Count(), Data = tbl_profile }, JsonRequestBehavior.AllowGet);
        }
        

        [HttpPost]
        public JsonResult ReadHIRA(string sid)
        {
            try
            {
               DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_AKTIFITAS_HIRAs.Where(a => a.ID.Equals(sid));
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
           }
        }

        public class idUnit
        {
            public static string unitid { get; set; }
        }
        [HttpPost]
        public JsonResult Insert(TBL_M_HIRA_TEMP hira)
        {
                try
                {
                    pv_CustLoadSession();
                    string iNewID = Guid.NewGuid().ToString().Replace("-", "");
                    idUnit.unitid = iNewID;
                    db = new DtClassAliceContextDataContext();
                    hira.UPLOAD_FILE = UrlUpload.iUrlUpload;
                    hira.ID = iNewID;
                    hira.UPDATE_DATE = DateTime.Now;
                    hira.UPDATE_BY = iStrSessNRP;
                    db.TBL_M_HIRA_TEMPs.InsertOnSubmit(hira);
                    db.SubmitChanges();

                    return this.Json(new { remarks = "Success" });
                }
                catch (Exception e)
                {
                    return this.Json(new { remarks = "Gagal", error = e.ToString() });
                }
            
        }

        [HttpPost]
        public JsonResult Update(TBL_M_HIRA hira)
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var data = db.TBL_M_HIRAs.Where(a => a.ID == hira.ID).FirstOrDefault();
                data.DAFTAR_PROSES = hira.DAFTAR_PROSES;
                data.AKTIFITAS = hira.AKTIFITAS;
                data.UPLOAD_FILE = hira.UPLOAD_FILE;
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

        [HttpPost]
        public JsonResult Delete(TBL_M_HIRA hira)
        {
            try
            {
                if (hira != null)
                {
                    DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                    var del = db.TBL_M_HIRAs.Where(s => s.ID == hira.ID).FirstOrDefault();
                    db.TBL_M_HIRAs.DeleteOnSubmit(del);
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

        //public ActionResult AjaxUpload(IEnumerable<HttpPostedFileBase> file)
        //{
        //    // The Name of the Upload component is "files"
        //    if (file != null)
        //    {
        //        foreach (var file2 in file)
        //        {

        //            // Some browsers send file names with full path. This needs to be stripped.
        //            var fileName = Path.GetFileName(file2.FileName);
        //            var physicalPath = Path.Combine(Server.MapPath("~/FileUpload/FileHIRA"), fileName);
        //            //The files are not actually saved in this demo
        //            //file.SaveAs(physicalPath);
        //        }
        //    }
        //    // Return an empty string to signify success
        //    return Content("");
        //}



        [HttpPost]
        public JsonResult AjaxUpload2()
        {
            string iStrREmarks = string.Empty;
            DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
            var fileContent = Request.Files["filehira"]; //Mengambil file dari txtupload (name dari index)
            var iPath = string.Empty;
           string fileExt = System.IO.Path.GetExtension(fileContent.FileName);
            if (fileContent != null && fileContent.ContentLength <= 2000000 && fileExt == ".pdf")// Kondisi jika fileContent ada isinya
            {
               
                var fileName = Path.GetFileName(fileContent.FileName);// Membaca nama file dan ekstensi dari fileContent
                var path = Path.Combine(Server.MapPath("~/FileUpload/FileHIRA"), fileName);// Membuat path/alamat penyimpanan file + nama file -> dalam bentuk variabel
                iPath = "/FileUpload/FileHIRA/" + fileName;// Membaca path penyimpanan cuma folder dan nama file aja

                fileContent.SaveAs(path);// Untuk upload file

                UrlUpload.iUrlUpload = iPath;
                //var newUpload = db.TBL_M_HIRAs.Where(a => a.ID == idp).FirstOrDefault();
                //newUpload.UPLOAD_FILE = iPath;
                db.SubmitChanges();  //saveUploadCareerPath(iPath);

                return this.Json(new { status = true, remark = "Upload Berhasil", iStrPath = iPath }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(new { status = false, remark = "Tipe Harus PDF & Ukuran < 2000 Kb" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult AjaxUpload1()
        {
            var fileContent = Request.Files["uploadhira"]; //Mengambil file dari txtupload (name dari index)
            var iPath = string.Empty;
            //string fileExt = System.IO.Path.GetExtension(fileContent.FileName).ToLower();

            if (fileContent != null && fileContent.ContentLength <= 200000 /*&& fileExt == ".pdf"*/)// Kondisi jika fileContent ada isinya
            {
                var fileName = Path.GetFileName(fileContent.FileName);// Membaca nama file dan ekstensi dari fileContent
                var path = Path.Combine(Server.MapPath("~/FileUpload/FileHIRA"), fileName);// Membuat path/alamat penyimpanan file + nama file -> dalam bentuk variabel
                iPath = "/FileUpload/FileHIRA/" + fileName;// Membaca path penyimpanan cuma folder dan nama file aja
                fileContent.SaveAs(path);// Untuk upload file
                return this.Json(new { status = true, remark = "Upload Berhasil", iStrPath = iPath }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return this.Json(new { status = false, remark = "Tipe Harus PDF & Ukuran < 200 Kb" }, JsonRequestBehavior.AllowGet);
            }
        }


        public static class UrlUpload
        {
            public static string iUrlUpload { get; set; }
        }
    }
}