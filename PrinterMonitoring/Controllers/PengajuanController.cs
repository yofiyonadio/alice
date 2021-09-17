using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PrinterMonitoring.Models;
using Kendo.DynamicLinq;

namespace PrinterMonitoring.Controllers
{
    public class PengajuanController : Controller
    {
        public DtClassAppsDataContextDataContext iObjContext_apps;
        DtClassAppsDataContextDataContext db_Context;
        DtClassAliceContextDataContext db_Context_alice;
        //DtClass_SM_ARAPDataContext db_ContextSmArap;
        private MenuLeftClass menuLeftClass = new MenuLeftClass();
        private _GeneralSettingClass GeneralSettingClass = new _GeneralSettingClass();
        private string iStrSessNRP = string.Empty;
        private string iStrSessDistrik = string.Empty;
        private string iStrSessGPID = string.Empty;
        private string iStrLoginDept = string.Empty;
        private string iStrNama = string.Empty;

        public string pIdQty = "";

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
        [HttpGet]
        public JsonResult getIDTransaction(string sKategori, string sitem, string req_date, string sqty,string sdiv,string sdept)
        {
            pv_CustLoadSession();
            string iNewID = Guid.NewGuid().ToString().Replace("-", "");
            pIdQty = iNewID;
            idUnit.unitid = iNewID;
            idUnit.itempengajuan = sitem;
            idUnit.qty = Convert.ToInt32(sqty);

            db_Context_alice = new DtClassAliceContextDataContext();
            Tbl_Pengajuan iPengajuan = new Tbl_Pengajuan();
            iPengajuan.PIC = iStrSessNRP;
            iPengajuan.Nama = iStrNama;
            iPengajuan.DivisiID = sdiv;
            iPengajuan.DepartmentID = sdept;
            iPengajuan.Kategori = iStrLoginDept;
            iPengajuan.ID_PENGAJUAN = iNewID;
            iPengajuan.QTY = Convert.ToInt32(sqty);
            iPengajuan.Item = sitem;
            string iStartReqDate = req_date.ToString();
            DateTime iDatetime = DateTime.Parse(iStartReqDate);
            iPengajuan.RequestDate = iDatetime;
            db_Context_alice.Tbl_Pengajuans.InsertOnSubmit(iPengajuan);
            db_Context_alice.SubmitChanges();
            db_Context_alice.Dispose();

            return this.Json(new { dataid = iNewID }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult Read(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var tbl = db.Tbl_Pengajuans;
                var data = tbl.ToDataSourceResult(take, skip, sort, filter);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
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

        [HttpPost]
        public JsonResult ReadKatalog()
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.TBL_R_MAPPING_BARANGs.OrderBy(d => d.Deskripsi);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult doSaveQtyDetail(string snrp_order, string sname_order, string sdept_order, string semail_order)
        {
            try
            {
                pv_CustLoadSession();
                string iUnitIdGet = null;
                IEnumerable<SelectListItem> items;
                iUnitIdGet = idUnit.unitid;
                string iStrItem = idUnit.itempengajuan;
                int itemNum = Convert.ToInt32(idUnit.qty);
                bool dismisPopup=false;

                db_Context_alice = new DtClassAliceContextDataContext();

                int iTblCountDetail = db_Context_alice.Tbl_QTYDetails.Where(c => c.IDPENGAJUAN.Equals(iUnitIdGet)).Count();
                if (itemNum == iTblCountDetail)
                {
                    return Json(new { status_return = true,return_code = 101, total_data_terisi = iTblCountDetail, return_dismis = dismisPopup, remarks = "Qty Sudah ditentukan" });
                }
                else {
                        var iTBlQtyDetail = db_Context_alice.Tbl_QTYDetails.Where(c => c.IDPENGAJUAN.Equals(iUnitIdGet) && c.NRP.Equals(snrp_order)).FirstOrDefault();
                        if (iTBlQtyDetail == null)
                        {
                                Tbl_QTYDetail iQtyDetail = new Tbl_QTYDetail();
                                iQtyDetail.PID_PENGAJUAN_QTY = Guid.NewGuid().ToString();
                                iQtyDetail.IDPENGAJUAN = iUnitIdGet;
                                iQtyDetail.NRP = snrp_order;
                                iQtyDetail.NAMA = sname_order;
                                iQtyDetail.POS_TITLE = sdept_order;
                                iQtyDetail.EMAIL_INTERNET = semail_order;
                                iQtyDetail.ITEM_PENGAJUAN = iStrItem;
                                iQtyDetail.APPROVAL_UNIT_FINAL = false;
                                iQtyDetail.STATUS_APROVAL = 0;
                                iQtyDetail.APROVED_BY = "Not Execution";
                                iQtyDetail.UPDATE_DATE = DateTime.Now;
                                iQtyDetail.UPDATE_BY = iStrSessNRP;
                                db_Context_alice.Tbl_QTYDetails.InsertOnSubmit(iQtyDetail);
                                db_Context_alice.SubmitChanges();
                                iTblCountDetail = db_Context_alice.Tbl_QTYDetails.Where(c => c.IDPENGAJUAN.Equals(iUnitIdGet)).Count();
                                db_Context_alice.Dispose();
                                    if (itemNum == iTblCountDetail)
                                    {
                                        dismisPopup = true;
                                    }
                            return Json(new { status_return = true, return_code = 202,total_data_terisi = iTblCountDetail,return_dismis = dismisPopup, remarks = "Berhasil Menambah data" });
                            }
                        else
                        {
                            return Json(new { status_return = false, return_code = 404, total_data_terisi = iTblCountDetail, return_dismis = dismisPopup, remarks = "Karyawan yang anda pilih sudah didaftarkan" });
                        }

                }
            }
            catch (Exception e)
            {
                return this.Json(new { status_return = false, error = e.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ReadKategori()
        {
            try
            {
                pv_CustLoadSession();
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var i_tbl = db.VW_KATEGORIs.Where(d => d.Department_Access.Equals(iStrLoginDept) && d.Status == true);
                return Json(new { Total = i_tbl.Count(), Data = i_tbl });


            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() }, JsonRequestBehavior.AllowGet);
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
        public JsonResult Insert(Tbl_Pengajuan log)
        {
            if (log.Nama == "" || log.Nama == null || log.DivisiID == "" || log.DivisiID == null || log.DepartmentID == "" || log.DepartmentID == null ||
                log.Kategori == "" || log.Kategori == null || log.Item == "" || log.Item == null || log.QTY == null)
            {
                return this.Json(new { remarks = "masih ada yang kosong!" });
            }
            else
            {
                try
                {
                    DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                    db.Tbl_Pengajuans.InsertOnSubmit(log);
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
        public JsonResult Update(Tbl_Pengajuan log)
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var data = db.Tbl_Pengajuans.Where(a => a.PIC == log.PIC).FirstOrDefault();
                data.Nama = log.Nama;

                data.DivisiID = log.DivisiID;
                data.DepartmentID = log.DepartmentID;
                data.Kategori = log.Kategori;
                data.Item = log.Item;
                data.QTY = log.QTY;
                db.SubmitChanges();
                return this.Json(new { remarks = "data telah diupdate" });
            }
            catch (Exception e)
            {
                return this.Json(new { remarks = "Gagal Update", error = e.ToString() });
            }
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

        public class idUnit
        {
            public static string unitid { get; set; }
            public static string itempengajuan { get; set; }
            public static int? qty { get; set; }
        }


        [HttpPost]
        public JsonResult DeletePengajuan(string sidPengajuan)
        {
            try
            {

                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                db.cusp_delete_pengajuan(sidPengajuan);
                return this.Json(new { remarks = "data telah dihapus", status_return = true });

            }
            catch (Exception e)
            {
                return this.Json(new { remarks = "Gagal hapus data", status_return = false, error = e.ToString() });
            }
        }



        [HttpPost]
        public JsonResult ReadQtyDetail(string sidpengaju, int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            try
            {
                DtClassAliceContextDataContext db = new DtClassAliceContextDataContext();
                var tbl = db.Tbl_QTYDetails.Where(a => a.IDPENGAJUAN.Equals(sidpengaju));
                var data = tbl.ToDataSourceResult(take, skip, sort, filter);
                return Json(data);

            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }


    }
}