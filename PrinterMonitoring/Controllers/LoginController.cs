using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrinterMonitoring.Models;
using Kendo.DynamicLinq;
using FormsAuth;
using System.Web.Security;
using System.Web.Configuration;

namespace PrinterMonitoring.Controllers
{
    public class LoginController : Controller
    {
        private DtClassAppsDataContextDataContext i_obj_ctx;
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult getUser(string pnrp = "", string password = "", string domain = "")
        {
            i_obj_ctx = new DtClassAppsDataContextDataContext();
            //DtClass_CloneDataContext db_cloneContext = new DtClass_CloneDataContext();
            //bool bl_status = true;
            bool bl_status = checkValidUser(pnrp, password, domain);
            if (bl_status)
            {
                pnrp = pnrp.Length == 0 ? "0" : pnrp;
                pnrp = pnrp.Substring(1, pnrp.Length - 1);
                var list_gpId = i_obj_ctx.vw_gpIds.Where(f => f.NRP == pnrp).ToList();
                FormsAuthentication.SetAuthCookie(pnrp, true);
                if (list_gpId.Count > 0)
                {
                    foreach (var v in list_gpId)
                    {
                        Session["PNRP"] = pnrp;
                        Session["empId"] = v.NRP;
                        Session["NRP"] = v.NRP;
                        Session["Name"] = v.NAMA;
                        Session["Nama"] = v.NAMA;
                        Session["Nama_Nrpp"] = string.Format("{0} - {1}", v.NRP, v.NAMA);
                        Session["distrik"] = v.DISTRIK;
                        //Session["pathImg"] = db_cloneContext.TBL_R_PATH_DSTRCTs.Where(f => f.DSTRCT_CODE == v.DISTRIK).FirstOrDefault().PATH;
                    }
                    return RedirectToAction("Profiles", "Login");
                }
                TempData["notice"] = "User NRP anda tidak di temukan di database, Pastikan anda sudah terdaftar.. !!";
            }
            else
            {
                TempData["notice"] = "Failed login to domain pamapersada";
            }
            return RedirectToAction("Index", "Login");
        }


        public bool checkValidUser(string pnrp = "", string password = "", string domain = "")
        {
            bool iReturn = false;
            if (domain == "1") //PAMAPERSADA
            {
                try
                {
                    var ldap = new LdapAuthentication("LDAP://PAMAPERSADA:389");
                    //iReturn = ldap.IsAuthenticated("PAMAPERSADA", pnrp, password);

                    //by pass
                     iReturn = true;
                }
                catch (Exception)
                {
                    iReturn = false;
                }
            }
            else if (domain == "2") //DATABASE
            {  // DATABASE
                i_obj_ctx = new DtClassAppsDataContextDataContext();

                try
                {
                    pnrp = pnrp.Length == 0 ? "0" : pnrp;
                    pnrp = pnrp.Substring(1, pnrp.Length - 1);

                    var profile = i_obj_ctx.vw_employees.Where(f => f.NRP.Equals(pnrp) && f.BIRTH_DATE.Equals(password)).FirstOrDefault();

                    if (profile != null)
                    {
                        iReturn = true;
                    }

                    //by pass
                    //iReturn = true;
                }
                catch (Exception)
                {
                    iReturn = false;
                }
            }
            return iReturn;
        }



        [HttpPost]
        public ActionResult profileSelect(string idDistrik = "", int idProfile = 10000, string idPICasset = "")
        {
            Session["leftMenu"] = null;
            Session["gpId"] = idProfile;
            Session["distrik"] = idDistrik;
            string i_str_empId = Convert.ToString(Session["empId"]);
            Session["PIC_ASSET"] = idPICasset;
            i_obj_ctx = new DtClassAppsDataContextDataContext();
            var list_viewGp = i_obj_ctx.vw_gpIds
                                        .Where(
                                            f => f.NRP == i_str_empId
                                              && f.GP == idProfile
                                              && f.DISTRIK == idDistrik
                                         ).ToList();

            foreach (var v in list_viewGp)
            {
                Session["Name"] = Convert.ToString(v.NAMA);
                Session["description"] = Convert.ToString(v.Deskripsi_ID);

            }

            if (list_viewGp.Count == 0)
            {
                TempData["notice"] = "User NRP anda tidak di temukan di database, Pastikan anda sudah terdaftar.. !!";
                return RedirectToAction("Index", "Login");
            }

            i_obj_ctx.Dispose();
            return RedirectToAction("Index", "Home");
        }


        public ActionResult Profiles()
        {
            if (Session["PNRP"] == null)
                return RedirectToAction("Index", "Login");

            IEnumerable<SelectListItem> items;
            List<itemSelect> ls = new List<itemSelect>();
            ls = getList("ditrik");
            items = ls.Select(c => new SelectListItem
            {
                Value = c.value,
                Text = c.text
            });

            ViewBag.Distrik = items;
            IEnumerable<SelectListItem> itemsProfile;
            if (ls.Count > 0)
            {
                foreach (itemSelect p in ls)
                {
                    Session["distrik"] = p.value;
                    break;
                }
            }

            itemsProfile = getList("profile").Select(c => new SelectListItem
            {
                Value = c.value,
                Text = c.text
            });

            ViewBag.Profile = itemsProfile;
            return View();
        }


        [HttpGet]
        public JsonResult getProfileDesc(string distrik = "")
        {
            List<itemSelect> ls = new List<itemSelect>();
            i_obj_ctx = new DtClassAppsDataContextDataContext();
            string i_str_empId = Session["empId"] == null ? "" : Session["empId"].ToString();
            var list_viewGp = i_obj_ctx.vw_gpIds
                                        .Where(f => f.NRP == i_str_empId
                                            && f.DISTRIK == distrik
                                        ).ToList();

            foreach (var v in list_viewGp)
            {
                ls.Add(new itemSelect { text = Convert.ToString(v.Deskripsi), value = Convert.ToString(v.GP) });
            }

            return this.Json(new { data = ls }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult getDistrikDesc(int sProfile = 1000)
        {
            List<itemSelect> ls = new List<itemSelect>();
            i_obj_ctx = new DtClassAppsDataContextDataContext();
            string i_str_empId = Session["empId"] == null ? "" : Session["empId"].ToString();
            var list_viewGp = i_obj_ctx.vw_gpIds
                                    .Where(f => f.NRP == i_str_empId
                                             && f.GP == sProfile
                                    ).ToList();

            foreach (var v in list_viewGp)
            {
                ls.Add(new itemSelect { text = Convert.ToString(v.DISTRIK), value = Convert.ToString(v.DISTRIK) });
            }

            return this.Json(new { data = ls }, JsonRequestBehavior.AllowGet);
        }


        public List<itemSelect> getList(string s_type)
        {
            List<itemSelect> ls = new List<itemSelect>();
            i_obj_ctx = new DtClassAppsDataContextDataContext();

            if (s_type == "ditrik")
            {
                string i_str_empId = Session["empId"] == null ? "" : Session["empId"].ToString();
                var lsDistrik = (from b in i_obj_ctx.vw_gpIds
                                 where b.NRP == i_str_empId
                                 orderby b.DISTRIK
                                 select new { DISTRIK = b.DISTRIK }).
                                 GroupBy(e => e.DISTRIK).
                                 Select(grp => grp.First()).ToList();

                foreach (var vw in lsDistrik)
                {
                    ls.Add(new itemSelect { text = vw.DISTRIK, value = vw.DISTRIK });
                }
                i_obj_ctx.Dispose();
            }

            if (s_type == "profile")
            {
                string i_str_empId = Session["empId"] == null ? "" : Session["empId"].ToString();

                var lsProfile = (from b in i_obj_ctx.vw_gpIds
                                 where b.NRP == i_str_empId
                                 orderby b.GP
                                 select new { GP = b.GP, Deskripsi = b.Deskripsi }).
                                 GroupBy(e => new
                                 {
                                     e.GP,
                                     e.Deskripsi
                                 }).
                                 Select(grp => grp.First()).ToList();
                foreach (var vw in lsProfile)
                {
                    ls.Add(new itemSelect { text = vw.Deskripsi, value = vw.GP.ToString() });
                }
                i_obj_ctx.Dispose();
            }
            return ls;
        }

        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("index", "login");
        }

        public class itemSelect
        {
            public string text { get; set; }
            public string value { get; set; }
        }





    }
}