using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrinterMonitoring.Models
{
    public class MenuLeftClass
    {
        private DtClassAppsDataContextDataContext i_obj_ctx;
        private string str_menuResult = "";
        private string urlPath = System.Configuration.ConfigurationManager.AppSettings["urlAppPath"].ToString();
        public string recursiveMenu(int id = 0, int gpId = 1)
        {
            i_obj_ctx = new DtClassAppsDataContextDataContext();
            var iListMenu = i_obj_ctx.Menus.Where(f => f.GP_ID == gpId && f.Id == id && f.Deskripsi == "MENU").OrderBy(f => f.Id).OrderBy(f => f.Urutan);

            foreach (var itemMenu in iListMenu)
            {
                if (id == 0)
                {
                    str_menuResult += "<li class='nav-item'>";
                    str_menuResult += "<a href='" + (string)itemMenu.Link +"' class='nav-link'>";
                    str_menuResult += "         <i class='nav-icon fas'></i>";
                    str_menuResult += "         <p>" + (string)itemMenu.Menu1 + "</p> ";
                    str_menuResult += "    </a>";
                    str_menuResult += "</li>";

                    //str_menuResult += "<span class='" + (string)itemMenu.style_class
                    //                   + "'></span><a href='" + urlPath + (string)itemMenu.Link
                    //                   + "'><span class='me-menu-span'>" + (string)itemMenu.Menu1 + "</span></a>";
                }

                //if ((int)itemMenu.Menu_link > 0)
                //{
                //    str_menuResult += "<ul>";
                //    recursiveSubMenu((int)itemMenu.Menu_link, gpId);
                //    str_menuResult += "</ul>";
                //}

                //if (id == 0)
                //{
                //    str_menuResult += "</li>";
                //}
            }
           
            i_obj_ctx.Dispose();
            return str_menuResult;
        }

        private void recursiveSubMenu(int id = 0, int gpId = 0)
        {
            i_obj_ctx = new DtClassAppsDataContextDataContext();
            var iListMenu = i_obj_ctx.Menus.Where(f => f.GP_ID == gpId && f.Id == id).OrderBy(f => f.Id).OrderBy(f => f.Urutan);

            Boolean i_str_isFA = false;
            string i_str_fa_nrp = "6109813";
            string i_str_fa_key = "74-893-1Z3321";

            foreach (var itemMenu in iListMenu)
            {
                if (itemMenu.Link.Contains("FAT_OTHERS") == true)
                {
                    i_str_isFA = true;
                }

                str_menuResult += "<li data-expanded='true'>";
                str_menuResult += "<span class='" + (string)itemMenu.style_class + "'></span><a href='"
                    + (i_str_isFA == true ? (string)itemMenu.Link + "s_str_NRP=" + i_str_fa_nrp + "&s_str_key=" + i_str_fa_key + "" : (urlPath + (string)itemMenu.Link))
                    + "'><span class='me-menu-span'>" + (string)itemMenu.Menu1 + "</span></a>";

                if ((int)itemMenu.Menu_link > 0)
                {
                    str_menuResult += "<ul>";
                    recursiveSubMenu((int)itemMenu.Menu_link, gpId);
                    str_menuResult += "</ul>";
                }
                str_menuResult += "</li>";
                i_str_isFA = false;
            }
            i_obj_ctx.Dispose();
        }

        public bool isUserValidForm(string sStrSessGPID, int sPrimer)
        {
            var iBlReturn = false;
            DtClassAppsDataContextDataContext iDClas_Apps = new DtClassAppsDataContextDataContext();
            var iUserMenu_GP = iDClas_Apps.Menu_GPs
                               .Where(f => f.GP_ID == Convert.ToInt32(sStrSessGPID) && f.Primer == sPrimer)
                               .FirstOrDefault();
            iBlReturn = iUserMenu_GP != null ? true : false;
            iDClas_Apps.Dispose();
            return iBlReturn;
        }
    }
}