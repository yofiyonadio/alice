using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrinterMonitoring.Models;
using Kendo.DynamicLinq;

namespace PrinterMonitoring.Controllers
{
    public class PeminjamanPrinterController : Controller
    {
        public DtClassAppsDataContextDataContext iObjContext_apps;
        DtClassAppsDataContextDataContext db_Context;
        private DtClassAliceContextDataContext db;
        private MenuLeftClass menuLeftClass = new MenuLeftClass();
        private _GeneralSettingClass GeneralSettingClass = new _GeneralSettingClass();
        private string iStrSessNRP = string.Empty;
        private string iStrSessDistrik = string.Empty;
        private string iStrSessGPID = string.Empty;

        public string iStrREmarks = "";
        public char iChrTransc;

        // GET: HM
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
                var tbl_pinjam = db.TBL_T_PINJAM_PRINTERs;
                var data = tbl_pinjam.ToDataSourceResult(take, skip, sort, filter);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }

        }
        [HttpPost]
        public JsonResult ReadDiv()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.VW_DIV_CODEs.OrderBy(d => d.DIV_CODE);
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
        public JsonResult Insert_peminjaman(TBL_T_PINJAM_PRINTER log)
        {
            if (log.KODE_PRINTER_PINJAM == null || log.KODE_PRINTER_PINJAM == "" || log.NAMA_PRINTER_PINJAM == null || log.NAMA_PRINTER_PINJAM == "" || log.JENIS_PRINTER_PINJAM == null || log.JENIS_PRINTER_PINJAM == "" || log.NAMA_USER_PINJAM == null || log.NAMA_USER_PINJAM == ""
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
            try
            {
                pv_CustLoadSession();
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
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
                db.SubmitChanges();
                return this.Json(new { remarks = "Success" });
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
                var printer = db.TBL_M_DATA_PRINTERs;
                return Json(printer.ToDataSourceResult(take, skip, sort, filter));
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
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

                    return this.Json(new { remarks = "Data telah dihapus", status = true });
                }
                else
                {
                    return this.Json(new { remarks = "id not found", status = false });
                }

            }
            catch (Exception e)
            {
                return this.Json(new { remarks = "Gagal", error = e.ToString() });
            }
        }
    }
}