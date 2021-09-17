using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrinterMonitoring.Models;
using Kendo.DynamicLinq;

namespace PrinterMonitoring.Controllers
{
    public class HMSafetyController : Controller
    {
        // GET: HMSafety

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
        public JsonResult Read(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var tbl = db.VW_HMs;
                var data = tbl.ToDataSourceResult(take, skip, sort, filter);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }
        [HttpPost]
        public JsonResult ReadDept()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.VW_DD_DEPT_CODEs.OrderBy(d => d.DEPT_CODE);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ReadSite()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.VW_DISTRICT_CODEs.OrderBy(d => d.DSTRCT_CODE);
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
        public JsonResult Update(TBL_M_HOURS_MACHINE_SAFETY log)
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var data = db.TBL_M_HOURS_MACHINE_SAFETies.Where(a => a.PID_HOURS_MACHINE == log.PID_HOURS_MACHINE).FirstOrDefault();
                data.PERIOD = log.PERIOD;
                data.DEPARTMENT = log.DEPARTMENT;
                data.SITE = log.SITE;
                data.PIC = log.PIC;
                data.JENIS_MESIN = log.JENIS_MESIN;
                data.JUMLAH = log.JUMLAH;
                data.JAM = log.JAM;
                data.HARI = log.HARI;
                data.MODIFIED_BY = iStrSessNRP;
                data.MODIFIED_DATE = DateTime.Now;
                db.SubmitChanges();
                return this.Json(new { remarks = "Success Update" });
            }
            catch (Exception e)
            {
                return this.Json(new { remarks = "Gagal Update", error = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult Delete(TBL_M_HOURS_MACHINE_SAFETY log)
        {
            try
            {
                if (log != null)
                {
                    DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                    var del = db.TBL_M_HOURS_MACHINE_SAFETies.Where(s => s.PID_HOURS_MACHINE == log.PID_HOURS_MACHINE).FirstOrDefault();
                    db.TBL_M_HOURS_MACHINE_SAFETies.DeleteOnSubmit(del);
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
                return this.Json(new { remarks = "Gagal hapus data", error = e.ToString() });
            }
        }
    }
}