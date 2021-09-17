using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrinterMonitoring.Models;
using Kendo.DynamicLinq;

namespace PrinterMonitoring.Controllers
{
    public class KPISafetyController : Controller
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
                var tbl = db.VW_LTI_SAFETies;
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
        public JsonResult Insert(TBL_T_LTI_SAFETY log)
        {
            if ( log.MONTHS == null || log.NUMBER_MONTH_ACT == null ||  log.NUMBER_MINOR_MONTH_ACT == null || log.NUMBER_TIME_MONTH_ACT == null || log.NUMBER_FATAL_MONTH_ACT == null || log.SHIFTS_LOST_MONTH_ACT == null)
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
                    log.PID_LTI = iNewID;
                    db.TBL_T_LTI_SAFETies.InsertOnSubmit(log);
                    db.SubmitChanges();

                    return this.Json(new { remarks = "Success" });
                }
                catch (Exception e)
                {
                    return this.Json(new { remarks = "Gagal input! ", error = e.ToString() });
                }
            }
        }
        
        public JsonResult Update(TBL_T_LTI_SAFETY log)
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var data = db.TBL_T_LTI_SAFETies.Where(a => a.PID_LTI == log.PID_LTI).FirstOrDefault();
                data.MONTHS = log.MONTHS;
                data.NUMBER_MONTH_ACT = log.NUMBER_MONTH_ACT;
                data.NUMBER_MINOR_MONTH_ACT = log.NUMBER_MINOR_MONTH_ACT;
                data.NUMBER_TIME_MONTH_ACT = log.NUMBER_TIME_MONTH_ACT;
                data.NUMBER_FATAL_MONTH_ACT = log.NUMBER_FATAL_MONTH_ACT;
                data.SHIFTS_LOST_MONTH_ACT = log.SHIFTS_LOST_MONTH_ACT;
                db.SubmitChanges();
                return this.Json(new { remarks = "Success Update" });
            }
            catch (Exception e)
            {
                return this.Json(new { remarks = "Gagal Update", error = e.ToString() });
            }
        }
        [HttpPost]
        public JsonResult Delete(TBL_T_LTI_SAFETY log)
        {
            try
            {
                if (log != null)
                {
                    DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                    var del = db.TBL_T_LTI_SAFETies.Where(s => s.PID_LTI == log.PID_LTI).FirstOrDefault();
                    db.TBL_T_LTI_SAFETies.DeleteOnSubmit(del);
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