using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PrinterMonitoring.Models;
using Kendo.DynamicLinq;

namespace PrinterMonitoring.Controllers
{
    public class KatalogController : Controller
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

        // GET: Katalog
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
                var tbl = db.TBL_R_MAPPING_BARANGs;
                var data = tbl.ToDataSourceResult(take, skip, sort, filter);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }


        [HttpPost]
        public JsonResult Insert(TBL_R_MAPPING_BARANG log)

        {
            if(log.KodeBarang == "" || log.KodeBarang == null ||log.C_O ==""|| log.C_O==null || log.Deskripsi=="" || log.Deskripsi==null || log.EstimasiHargaIDR == null) 
            {
                return this.Json(new { remarks = "masih ada yang kosong" });
            }
            else
            {
                try
                {
                    DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                    db.TBL_R_MAPPING_BARANGs.InsertOnSubmit(log);
                    db.SubmitChanges();

                    return this.Json(new { remarks = "Success" });
                }
                catch (Exception e)
                {
                    return this.Json(new { remarks = "Gagal input! kode barang sama", error = e.ToString() });
                }
            }
          
        }
        [HttpPost]
        public JsonResult Update(TBL_R_MAPPING_BARANG log)
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var data = db.TBL_R_MAPPING_BARANGs.Where(a => a.KodeBarang == log.KodeBarang).FirstOrDefault();
                data.Deskripsi = log.Deskripsi;
                data.Keterangan = log.Keterangan;
                data.EstimasiHargaUSD = log.EstimasiHargaUSD;
                data.EstimasiHargaIDR = log.EstimasiHargaIDR;
                data.EstimasiHargaSetIDR = log.EstimasiHargaSetIDR;
                data.C_O = log.C_O;
                db.SubmitChanges();
                return this.Json(new { remarks = "Success Update" });
            }
            catch (Exception e)
            {
                return this.Json(new { remarks = "Gagal Update", error = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult Delete(TBL_R_MAPPING_BARANG log)
        {
            try
            {
                if (log != null)
                {
                    DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                    var del = db.TBL_R_MAPPING_BARANGs.Where(s => s.KodeBarang == log.KodeBarang).FirstOrDefault();
                    db.TBL_R_MAPPING_BARANGs.DeleteOnSubmit(del);
                    db.SubmitChanges();

                    return this.Json(new { remarks = "Data telah dihapus" ,status=true});
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