using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrinterMonitoring.Models;
using Kendo.DynamicLinq;

namespace PrinterMonitoring.Controllers
{
    public class PermintaanPRController : Controller
    {
        // GET: PermintaanPR
        public DtClassAppsDataContextDataContext iObjContext_apps;
        DtClassAppsDataContextDataContext db_Context;
        private MenuLeftClass menuLeftClass = new MenuLeftClass();
        private _GeneralSettingClass GeneralSettingClass = new _GeneralSettingClass();
        private string iStrSessNRP = string.Empty;
        private string iStrSessDistrik = string.Empty;
        private string iStrSessGPID = string.Empty;

        public string iStrREmarks = "";
        public char iChrTransc;


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
                var tbl = db.TBL_T_MACHINE_SAFETies;
                var data = tbl.ToDataSourceResult(take, skip, sort, filter);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }
        [HttpPost]
        public JsonResult ReadPosisi()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_POSITIONs.OrderBy(d => d.Deskripsi);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ReadCOA()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_COAs.OrderBy(d => d.Kategori);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ReadDelivery()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_DELIVERY_POINTs.OrderBy(d => d.Deskripsi);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ReadMaterial()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_MATERIAL_GCs.OrderBy(d => d.Deskripsi);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ReadPriority()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_PRIORITies.OrderBy(d => d.Deskripsi);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ReadElement()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_EXPENSE_ELEMENTs.OrderBy(d => d.Deskripsi);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ReadPurchasing()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_PURCHASING_OFFICERs.OrderBy(d => d.Deskripsi);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ReadUOM()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_UOMs.OrderBy(d => d.Deskripsi);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ReadWarehouse()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_WAREHOUSEs.OrderBy(d => d.TABLE_DESC);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ReadOfficer()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_M_PURCHASING_OFFICERs.OrderBy(d => d.Deskripsi);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}