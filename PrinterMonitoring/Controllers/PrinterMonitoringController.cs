using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrinterMonitoring.Models;
using Kendo.DynamicLinq;

namespace PrinterMonitoring.Controllers
{
    public class PrinterMonitoringController : Controller
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

        // GET: PrinterMonitoring
        public ActionResult Index()
        {
            if (Session["NRP"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.leftMenu = loadMenu();
            ViewBag.profile = getList("gp");
            ViewBag.distrik = getList("distrik");
            //ViewBag.login = getList("distrik");
            ViewBag.nrp = Session["NRP"];
            ViewBag.nama_in = Session["Name"];
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
            iStrLoginDept = (string)Session["description"];
            iStrNama = (string)Session["Name"];
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
        public JsonResult InsertAsset(TBL_R_PRINTER log)
        {
            db = new DtClassAliceContextDataContext();

            int iTblasset = db.TBL_R_PRINTERs.Where(c => c.ASSET_NO.Equals(log.ASSET_NO)).Count();

            if (iTblasset == 0)
            {
                if (log.ASSET_NO == null || log.ASSET_NO == "" || log.DSTRCT_CODE == null || log.DSTRCT_CODE == "" || log.ASSET_CLASSIF == null
                   || log.ASSET_CLASSIF == "" || log.ASSET_DESC == null || log.ASSET_DESC == null || log.ASSET_LOCAT == "" || log.ASSET_LOCAT == null
                   || log.SERIAL_NO == "" || log.SERIAL_NO == null)
                {
                    return this.Json(new { remarks = "masih ada yang kosong" });
                }
                else
                {
                    try
                    {
                        pv_CustLoadSession();
                        //string iNewID = Guid.NewGuid().ToString().Replace("-", "");
                        //idUnit.unitid = iNewID;
                        db = new DtClassAliceContextDataContext();
                        //log.ID_PRINTER = iNewID;
                        log.UPDATE_DATE = DateTime.Now;
                        log.UPDATE_BY = iStrSessNRP;
                        db.TBL_R_PRINTERs.InsertOnSubmit(log);
                        db.SubmitChanges();

                        return this.Json(new { remarks = "Success" });
                    }
                    catch (Exception e)
                    {
                        return this.Json(new { remarks = "Gagal", error = e.ToString() });
                    }
                }

            }
            else
            {
                return this.Json(new { remarks = "Kode Printer sudah ada" });
            }
        }
        [HttpPost]
        public JsonResult Read(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var tbl = db.TBL_M_DATA_PRINTERs;
                var data = tbl.ToDataSourceResult(take, skip, sort, filter);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }

        }
        public class idUnit
        {
            public static string unitid { get; set; }
        }
        [HttpPost]
        public JsonResult Insert(TBL_M_DATA_PRINTER log)
        {
            db = new DtClassAliceContextDataContext();

            int iTblprinter = db.TBL_M_DATA_PRINTERs.Where(c => c.KODE_PRINTER.Equals(log.KODE_PRINTER)).Count();
    
            if (iTblprinter == 0)
            {
                if (log.NAMA_PRINTER == null || log.NAMA_PRINTER == "" || log.KODE_PRINTER == null || log.KODE_PRINTER == "" || log.JENIS_PRINTER == null
                   || log.JENIS_PRINTER == "" || log.STATUS_PRINTER == null || log.IP_PRINTER == null || log.LOKASI == "" || log.LOKASI == null)
                {
                    return this.Json(new { remarks = "masih ada yang kosong" });
                }
                else
                {
                    try
                    {
                        pv_CustLoadSession();
                        string iNewID = Guid.NewGuid().ToString().Replace("-", "");
                        idUnit.unitid = iNewID;
                        db = new DtClassAliceContextDataContext();
                        log.ID_PRINTER = iNewID;
                        log.UPDATE_DATE = DateTime.Now;
                        log.UPDATE_BY = iStrSessNRP;
                        db.TBL_M_DATA_PRINTERs.InsertOnSubmit(log);
                        db.SubmitChanges();

                        return this.Json(new { remarks = "Success" });
                    }
                    catch (Exception e)
                    {
                        return this.Json(new { remarks = "Gagal", error = e.ToString() });
                    }
                }

            }else
            {
                return this.Json(new { remarks = "Kode Printer sudah ada" });
            } 
            }
        //}

        [HttpPost]
        public JsonResult Update(TBL_M_DATA_PRINTER printer)
        {
            try
            {
                pv_CustLoadSession();
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var data = db.TBL_M_DATA_PRINTERs.Where(a => a.ID_PRINTER == printer.ID_PRINTER).FirstOrDefault();
                data.KODE_PRINTER = printer.KODE_PRINTER;
                data.NAMA_PRINTER = printer.NAMA_PRINTER;
                data.DETAIL_PRINTER = printer.DETAIL_PRINTER;
                data.UMUR_PRINTER = printer.UMUR_PRINTER;
                data.JENIS_PRINTER = printer.JENIS_PRINTER;
                data.STATUS_PRINTER = printer.STATUS_PRINTER;
                data.IP_PRINTER = printer.IP_PRINTER;
                data.LOKASI = printer.LOKASI;
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
        public JsonResult ReadPeminjam(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            try
            {
                pv_CustLoadSession();
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var tbl = db.TBL_T_PINJAM_PRINTERs.OrderBy(d => d.KODE_PRINTER_PINJAM);
                var data = tbl.ToDataSourceResult(take, skip, sort, filter);
                return Json(data);
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }
        [HttpPost]
        public JsonResult Delete(TBL_M_DATA_PRINTER printer)
        {
            try
            {
                if (printer != null)
                {
                    DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                    var del = db.TBL_M_DATA_PRINTERs.Where(s => s.ID_PRINTER == printer.ID_PRINTER).FirstOrDefault();
                    db.TBL_M_DATA_PRINTERs.DeleteOnSubmit(del);
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
        [HttpPost]
        public JsonResult AjaxReadPrinter(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            bool? iBlStatus = false;
            string iStrREmarks = string.Empty;
            this.pv_CustLoadSession();

            try
            {
                db = new DtClassAliceContextDataContext();
                var printer = db.TBL_R_PRINTERs;
                return Json(printer.ToDataSourceResult(take, skip, sort, filter));
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult Insert_peminjaman(TBL_T_PINJAM_PRINTER log)
        {
            if (log.KODE_PRINTER_PINJAM == null || log.KODE_PRINTER_PINJAM == "" ||  log.NAMA_USER_PINJAM == null || log.NAMA_USER_PINJAM == ""
                || log.TGL_PINJAM == null || log.NRP_USER_PINJAM == null || log.NRP_USER_PINJAM == "" || log.DIV_USER_PINJAM == null || log.DIV_USER_PINJAM == "" || log.SELESAI_PINJAM == null || log.SELESAI_PINJAM == "")
            {
                return this.Json(new { remarks = "masih ada yang kosong" });
            }
            else
            {
                try
                {
                    pv_CustLoadSession();
                    string iNewID = Guid.NewGuid().ToString().Replace("-", "");
                    idUnit.unitid = iNewID;
                    db = new DtClassAliceContextDataContext();
                    log.ID_PINJAM = iNewID;
                    log.UPDATE_DATE = DateTime.Now;
                    log.UPDATE_BY = iStrSessNRP;
                    db.TBL_T_PINJAM_PRINTERs.InsertOnSubmit(log);
                    db.SubmitChanges();

                    return this.Json(new { remarks = "Success" });
                }
                catch (Exception e)
                {
                    return this.Json(new { remarks = "Gagal", error = e.ToString() });
                }
            }
        }

        [HttpPost]
        public JsonResult Update_peminjaman(TBL_T_PINJAM_PRINTER peminjaman)
        {
            if (peminjaman.KODE_PRINTER_PINJAM == null || peminjaman.KODE_PRINTER_PINJAM == "" || peminjaman.NAMA_USER_PINJAM == null || peminjaman.NAMA_USER_PINJAM == ""
                  || peminjaman.NRP_USER_PINJAM == null || peminjaman.NRP_USER_PINJAM == "" || peminjaman.DIV_USER_PINJAM == null || peminjaman.DIV_USER_PINJAM == "")
            {
                return this.Json(new { remarks = "masih ada yang kosong" });
            }
            else
            {
                try
                {
                    pv_CustLoadSession();
                    db = new DtClassAliceContextDataContext();
                    //db2 = new DtClassAliceContextDataContext();
                    var data = db.TBL_T_PINJAM_PRINTERs.Where(a => a.ID_PINJAM == peminjaman.ID_PINJAM).FirstOrDefault();
                    data.KODE_PRINTER_PINJAM = peminjaman.KODE_PRINTER_PINJAM;
                    data.NAMA_PRINTER_PINJAM = peminjaman.NAMA_PRINTER_PINJAM;
                    data.JENIS_PRINTER_PINJAM = peminjaman.JENIS_PRINTER_PINJAM;
                    data.TGL_PINJAM = peminjaman.TGL_PINJAM;
                    data.NAMA_USER_PINJAM = peminjaman.NAMA_USER_PINJAM;
                    data.NRP_USER_PINJAM = peminjaman.NRP_USER_PINJAM;
                    data.DIV_USER_PINJAM = peminjaman.DIV_USER_PINJAM;
                    data.SELESAI_PINJAM = peminjaman.SELESAI_PINJAM;
                    data.MODIFIED_BY = iStrSessNRP;
                    data.MODIFIED_DATE = DateTime.Now;
                    //var dt = db2.TBL_M_DATA_PRINTERs.Where(a => a.KODE_PRINTER == peminjaman.KODE_PRINTER_PINJAM).FirstOrDefault();
                    //dt.STATUS_PRINTER = "Dipinjam";
                    //dt.LOKASI = peminjaman.DIV_USER_PINJAM;
                    //db2.SubmitChanges();
                    db.SubmitChanges();
                    return this.Json(new { remarks = "Success" });
                }
                catch (Exception e)
                {
                    return this.Json(new { remarks = "Gagal", error = e.ToString() });
                }
            }
        }

        [HttpPost]
        public JsonResult Delete_peminjaman(TBL_T_PINJAM_PRINTER peminjaman)
        {
            try
            {
                if (peminjaman != null)
                {
                    DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                    var del = db.TBL_T_PINJAM_PRINTERs.Where(s => s.ID_PINJAM == peminjaman.ID_PINJAM).FirstOrDefault();
                    db.TBL_T_PINJAM_PRINTERs.DeleteOnSubmit(del);
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
        [HttpGet]
        public JsonResult getNotifCount()
        {
            DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
            var tersedia = db.TBL_M_DATA_PRINTERs.Where(a => a.STATUS_PRINTER.ToLower() == "tersedia").OrderBy(a => a.ID_PRINTER);
            var dipinjam = db.TBL_M_DATA_PRINTERs.Where(a => a.STATUS_PRINTER.ToLower() == "dipinjam").OrderBy(a => a.ID_PRINTER);
            var service = db.TBL_M_DATA_PRINTERs.Where(a => a.STATUS_PRINTER.ToLower() == "service").OrderBy(a => a.ID_PRINTER);
            var rusak = db.TBL_M_DATA_PRINTERs.Where(a => a.STATUS_PRINTER.ToLower() == "rusak").OrderBy(a => a.ID_PRINTER);
            var scarpt = db.TBL_M_DATA_PRINTERs.Where(a => a.STATUS_PRINTER.ToLower() == "scarpt").OrderBy(a => a.ID_PRINTER);
            if (tersedia.Count() > 0 || dipinjam.Count() > 0 || service.Count() > 0 || rusak.Count() > 0 || scarpt.Count() > 0)
            {
                return Json(new { data = tersedia, tersedia = tersedia.Count(), dipinjam = dipinjam.Count(), service = service.Count(), rusak = rusak.Count(), scarpt = scarpt.Count(), ID_PRINTER = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ID_PRINTER = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}