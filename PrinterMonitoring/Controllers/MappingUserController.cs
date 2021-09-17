using Kendo.DynamicLinq;
using PrinterMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrinterMonitoring.Controllers
{
    public class MappingUserController : Controller
    {
        public DtClassAppsDataContextDataContext iObjContext_apps;
        private MenuLeftClass menuLeftClass = new MenuLeftClass();
        private _GeneralSettingClass GeneralSettingClass = new _GeneralSettingClass();
        private string iStrSessNRP = string.Empty;
        private string iStrSessDistrik = string.Empty;
        private string iStrSessGPID = string.Empty;

        public string iStrREmarks = "";
        public char iChrTransc;

        // GET: MappingUser
        public ActionResult Index()
        {
            if (Session["NRP"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            ViewBag.leftMenu = loadMenu();
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

        [HttpPost]
        public JsonResult AjaxRead(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            bool? iBlStatus = false;
            string iStrREmarks = string.Empty;
            this.pv_CustLoadSession();

            try
            {
                if (GeneralSettingClass.IsValidPermission(1003, Convert.ToInt32(iStrSessGPID), _GeneralSettingClass.Action.Read, ref iStrREmarks))
                {
                    iObjContext_apps = new DtClassAppsDataContextDataContext();
                    var list_view = iObjContext_apps.vw_gpIds;
                    return this.Json(list_view.ToDataSourceResult(take, skip, sort, filter));
                }
                return Json(new { status = iBlStatus, remarks = iStrREmarks, error = iStrREmarks });
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AjaxDelete(vw_gpId vw)
        {
            bool? iBlStatus = false;
            string iStrREmarks = string.Empty;
            this.pv_CustLoadSession();

            try
            {
                if (GeneralSettingClass.IsValidPermission(1003, Convert.ToInt32(iStrSessGPID), _GeneralSettingClass.Action.Delete, ref iStrREmarks))
                {
                    iObjContext_apps = new DtClassAppsDataContextDataContext();
                    var tblUser_ = iObjContext_apps.TBL_USERs.Where(f => f.ID == vw.ID).FirstOrDefault();
                    if (tblUser_ != null)
                        iObjContext_apps.TBL_USERs.DeleteOnSubmit(tblUser_);
                    iObjContext_apps.SubmitChanges();
                    iObjContext_apps.Dispose();
                }
                return Json(new { status = iBlStatus, remarks = iStrREmarks, error = iStrREmarks });
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString(), status = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}