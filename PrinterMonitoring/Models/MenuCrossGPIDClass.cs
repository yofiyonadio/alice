using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrinterMonitoring.Models
{
    public class MenuCrossGPIDClass
    {
        public DtClassAppsDataContextDataContext iObjContext_apps;
        List<vw_menuCrossGPID> iList = new List<vw_menuCrossGPID>();

        public List<vw_menuCrossGPID> GetMenuCrossGPID(int id = 0, int gpId = 0)
        {
            iObjContext_apps = new DtClassAppsDataContextDataContext();
            var iListMenu = iObjContext_apps.vw_menuCrossGPIDs
                            .Where(f => f.GP_ID == gpId && f.Id == id)
                            .OrderBy(f => f.Id).OrderBy(f => f.Urutan);

            foreach (var itemMenu in iListMenu)
            {
                if (id == 0)
                {
                    iList.Add(itemMenu);
                }

                if ((int)itemMenu.Menu_link > 0)
                {
                    recursiveSubMenu((int)itemMenu.Menu_link, gpId);
                }
            }
            iObjContext_apps.Dispose();
            return iList;
        }

        private void recursiveSubMenu(int id = 0, int gpId = 0)
        {
            iObjContext_apps = new DtClassAppsDataContextDataContext();
            var iListMenu = iObjContext_apps.vw_menuCrossGPIDs
                            .Where(f => f.GP_ID == gpId && f.Id == id)
                            .OrderBy(f => f.Id).OrderBy(f => f.Urutan);

            foreach (var itemMenu in iListMenu)
            {
                itemMenu.Menu = "    --    " + itemMenu.Menu;
                iList.Add(itemMenu);
                if ((int)itemMenu.Menu_link > 0)
                {
                    recursiveSubMenu((int)itemMenu.Menu_link, gpId);
                }
            }
            iObjContext_apps.Dispose();
        }
    }
}