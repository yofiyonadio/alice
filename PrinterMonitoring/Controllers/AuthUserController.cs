using PrinterMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrinterMonitoring.Controllers
{
    public class AuthUserController : Controller
    {
        // GET: AuthUser
        private DtClassAppsDataContextDataContext db_;
        private MenuCrossGPIDClass menuAuthLeftClass = new MenuCrossGPIDClass();
        private _GeneralSettingClass GeneralSettingClass = new _GeneralSettingClass();
        private MenuLeftClass menuLeftClass = new MenuLeftClass();
        private string iStrSessNRP = string.Empty;
        private string iStrSessDistrik = string.Empty;
        private string iStrSessGPID = string.Empty;

        // GET: AuthUser
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
        public JsonResult AjaxRead(int sGPID)
        {
            bool? iBlStatus = false;
            string iStrREmarks = string.Empty;
            this.pv_CustLoadSession();

            try
            {
                if (GeneralSettingClass.IsValidPermission(1002, Convert.ToInt32(iStrSessGPID), _GeneralSettingClass.Action.Read, ref iStrREmarks))
                {
                    var iList = menuAuthLeftClass.GetMenuCrossGPID(0, sGPID);
                    return this.Json(new { Data = iList, Total = iList.Count() }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = iBlStatus, remarks = iStrREmarks, error = iStrREmarks });
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult AjaxGroup()
        {
            db_ = new DtClassAppsDataContextDataContext();
            var tbl_profile = db_.TBL_Profiles;
            return Json(new { Total = tbl_profile.Count(), Data = tbl_profile }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxUpdateCheckBox(IEnumerable<SetMenuGP_class> s_cls_data)
        {
            db_ = new DtClassAppsDataContextDataContext();

            try
            {

                foreach (SetMenuGP_class data in s_cls_data)
                {
                    db_.cusp_SetMenuGP(data.Primer, data.GP_ID, data.isChek, data.A, data.D, data.E, data.R);
                }

                return this.Json(new { remark = "Data Berhasil diubah" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString(), status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public class SetMenuGP_class
        {
            public int Primer { get; set; }
            public int GP_ID { get; set; }
            public int isChek { get; set; }
            public bool A { get; set; }
            public bool D { get; set; }
            public bool E { get; set; }
            public bool R { get; set; }
        }
    }
}