using Kendo.DynamicLinq;
using PrinterMonitoring.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrinterMonitoring.Controllers
{
    public class AddUserController : Controller
    {

        public DtClassAppsDataContextDataContext iObjContext_apps;
        DtClassAppsDataContextDataContext db_Context;
        //DtClass_SM_ARAPDataContext db_ContextSmArap;
        private MenuLeftClass menuLeftClass = new MenuLeftClass();
        private _GeneralSettingClass GeneralSettingClass = new _GeneralSettingClass();
        private string iStrSessNRP = string.Empty;
        private string iStrSessDistrik = string.Empty;
        private string iStrSessGPID = string.Empty;

        public string iStrREmarks = "";
        public char iChrTransc;

        // GET: AddUser
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

        private void pv_CustLoadSession()
        {
            iStrSessNRP = (string)Session["NRP"];
            iStrSessDistrik = (string)Session["distrik"];
            iStrSessGPID = Convert.ToString(Session["gpId"] == null ? "1000" : Session["gpId"]);
            ViewBag.gp = iStrSessGPID;
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
        public JsonResult AjaxReadEmployee(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            bool? iBlStatus = false;
            string iStrREmarks = string.Empty;
            this.pv_CustLoadSession();

            try
            {
                iObjContext_apps = new DtClassAppsDataContextDataContext();
                var vw_employee_ = iObjContext_apps.vw_employees.Where(f => f.EMP_STATUS == "A");
                return Json(vw_employee_.ToDataSourceResult(take, skip, sort, filter));
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult AjaxReadEmployeeQty(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            bool? iBlStatus = false;
            string iStrREmarks = string.Empty;
            this.pv_CustLoadSession();

            try
            {
                iObjContext_apps = new DtClassAppsDataContextDataContext();
                var vw_employee_ = iObjContext_apps.vw_employees.Where(f => f.EMP_STATUS == "A");
                return Json(vw_employee_.ToDataSourceResult(take, skip, sort, filter));
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ajaxInsert(string nrp = "", string gp = "100", string distrik = "")
        {
            //if (nrp.Trim() == "")
            //    return this.Json(new { error = "NRP Belum diisi.. !!", status = false }, JsonRequestBehavior.AllowGet);

            bool? iBlStatus = false;
            string iStrREmarks = string.Empty;
            this.pv_CustLoadSession();

            try
            {
                if (nrp.Trim() == "")
                    return this.Json(new { error = "NRP Belum diisi.. !!", status = false }, JsonRequestBehavior.AllowGet);

                if (GeneralSettingClass.IsValidPermission(1001, Convert.ToInt32(iStrSessGPID), _GeneralSettingClass.Action.Insert, ref iStrREmarks))
                {
                    iObjContext_apps = new DtClassAppsDataContextDataContext();
                    var iUser_ = iObjContext_apps.TBL_USERs.Where(f => f.NRP.Equals(nrp) && f.GP.Equals(gp) && f.DISTRIK.Equals(distrik));

                    if (iUser_.Count() <= 0)
                    {
                        TBL_USER iTbl_user = new TBL_USER();
                        iTbl_user.NRP = nrp;
                        iTbl_user.GP = Convert.ToInt32(gp);
                        iTbl_user.DISTRIK = distrik;
                        iObjContext_apps.TBL_USERs.InsertOnSubmit(iTbl_user);
                        iObjContext_apps.SubmitChanges();
                        iObjContext_apps.Dispose();

                        return this.Json(new { message = "NRP " + nrp + " saved in database", status = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return this.Json(new { message = "NRP " + nrp + " sudah digunakan", status = true }, JsonRequestBehavior.AllowGet);
                    }
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