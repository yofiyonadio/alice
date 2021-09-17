using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrinterMonitoring.Models;
using PrinterMonitoring;
using Kendo.DynamicLinq;
namespace PrinterMonitoring.Controllers
{
    public class InventoryController : Controller
    {

        private InventoryDataContext db1 = new InventoryDataContext();
        private MenuLeftClass menuLeftClass = new MenuLeftClass();
        private DateTime now = DateTime.Now;
        //var users = Session["NRP"]; // Get User

        
        private string iStrSessNRP = string.Empty;
        private string iStrSessGPID = string.Empty;
        private string iStrNama = string.Empty;
        


        // GET: Inventory
        public ActionResult Index()
        {
            //ViewBag.leftMenu = loadMenu();
            return View();
        }

        //-------------------------------- Initialzize


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
            iStrNama = (string)Session["Name"];
            iStrSessGPID = Convert.ToString(Session["gpId"] == null ? "1000" : Session["gpId"]);
            ViewBag.gp = iStrSessGPID;
        }

        //--------------------------------


        //-------------------------------- Costum Function

        // Generate Product Id
        private string generateProductId()
        {
            string newProductId;
            string prefix = "INV";
            string suffix = "00000";
            var getProductId = db1.inv_master_products.OrderByDescending(c => c.id_product).FirstOrDefault();
            if (getProductId == null)
            {
                newProductId = prefix + 1.ToString(suffix);
            }
            else
            {
                var oldProductId = getProductId.id_product;
                var skip = 3;
                var getNumber = oldProductId.Substring(skip, oldProductId.Length - skip);
                var newNumber = (Int32.Parse(getNumber) + 1).ToString(suffix);
                newProductId = prefix + newNumber;
            }
            return newProductId;
        }

        // --------------------------------------------------------


        // -------------------------------------------------------- Model Parser


        // Convert String Json to Models
        private inv_master_product inv_master_product(string datas)
        {
            Dictionary<string, string> body = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(datas);
            string bodyDecode = Newtonsoft.Json.JsonConvert.SerializeObject(body);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<inv_master_product>(bodyDecode);
        }

        private inv_master_in_out inv_master_in_out(string datas)
        {
            Dictionary<string, string> body = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(datas);
            string bodyDecode = Newtonsoft.Json.JsonConvert.SerializeObject(body);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<inv_master_in_out>(bodyDecode);
        }

        private inv_master_stock inv_master_stock(string datas)
        {
            Dictionary<string, string> body = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(datas);
            string bodyDecode = Newtonsoft.Json.JsonConvert.SerializeObject(body);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<inv_master_stock>(bodyDecode);
        }

        // --------------------------------------------------------





        // -------------------------------------------------------- Models to Database

        //var getProducts = db1.inv_master_products.OrderBy(c => c.id_product).ToDataSourceResult(take, skip, sort, filter);

        private Kendo.DynamicLinq.DataSourceResult inv_master_product_Grid(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            var getProducts = db1.inv_master_products.OrderByDescending(c => c.id_product).ToDataSourceResult(take, skip, sort, filter);
            return getProducts;
        }

        private inv_master_product inv_master_product_Update(string datas)
        {
            inv_master_product bodys = inv_master_product(datas);
            var getProducts = db1.inv_master_products.Where(c => c.id_product == bodys.id_product).FirstOrDefault();
            getProducts.desc = bodys.desc != null ? bodys.desc : getProducts.desc;
            getProducts.no_asset = bodys.no_asset != null ? bodys.no_asset : getProducts.no_asset;
            getProducts.unit_each = bodys.unit_each != null ? bodys.unit_each : getProducts.unit_each;
            getProducts.unit_koli = bodys.unit_koli != null ? bodys.unit_koli : getProducts.unit_koli;
            getProducts.expired = bodys.expired != null ? bodys.expired : getProducts.expired;
            getProducts.jenis = bodys.jenis != null ? bodys.jenis : getProducts.jenis;
            getProducts.section = bodys.section != null ? bodys.section : getProducts.section;
            getProducts.type = bodys.type != null ? bodys.type : getProducts.type;
            getProducts.unit_default = bodys.unit_default != null ? bodys.unit_default : getProducts.unit_default;
            getProducts.last_update = now;
            //getProducts.insert_by = 
            db1.SubmitChanges();
            return getProducts;
        }

        private inv_master_product inv_master_product_Delete(string datas)
        {
            inv_master_product bodys = inv_master_product(datas);
            var getProducts = db1.inv_master_products.Where(c => c.id_product == bodys.id_product).FirstOrDefault();
            db1.inv_master_products.DeleteOnSubmit(getProducts);
            db1.SubmitChanges();
            return getProducts;
        }

        private inv_master_product inv_master_product_Insert(string datas)
        {
            inv_master_product bodys = inv_master_product(datas);
            bodys.last_update = now;
            bodys.id_product = generateProductId();
            db1.inv_master_products.InsertOnSubmit(bodys);
            db1.SubmitChanges();
            return bodys;
        }

        private List<inv_master_product> inv_master_product_Get(string datas)
        {
            inv_master_product bodys = inv_master_product(datas);
            var getProducts = db1.inv_master_products.Where(c => c.id_product == bodys.id_product).Take(1).ToList();
            return getProducts;
        }

        private List<inv_master_product> inv_master_product_GetAll(string datas)
        {
            inv_master_product bodys = inv_master_product(datas);
            var getProducts = db1.inv_master_products.ToList();
            return getProducts;
        }




        // ------------------------------------------------------------------------- INVENTORY IN OUT

        private Kendo.DynamicLinq.DataSourceResult inv_master_in_out_Grid(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            var getProducts = db1.inv_master_in_outs.OrderByDescending(c => c.id).ToDataSourceResult(take, skip, sort, filter);
            return getProducts;
        }

        private inv_master_in_out inv_master_in_out_Insert(string datas, string type)
        {
            inv_master_in_out bodys = inv_master_in_out(datas);
            bodys.last_update = now;
            if (type == "IN") {
                bodys.in_out = "IN";
                bodys.status_receipt = false;
            } else if (type == "OUT") {
                bodys.in_out = "OUT";
            };
            System.Diagnostics.Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(bodys));
            db1.inv_master_in_outs.InsertOnSubmit(bodys);
            db1.SubmitChanges();
            return bodys;
        }

        private inv_master_in_out inv_master_in_out_Update(string datas)
        {
            inv_master_in_out bodys = inv_master_in_out(datas);
            var getProducts = db1.inv_master_in_outs.Where(c => c.id == bodys.id).FirstOrDefault();
            getProducts.desc = bodys.desc != null ? bodys.desc : getProducts.desc;
            getProducts.no_po = bodys.no_po != null ? bodys.no_po : getProducts.no_po;
            getProducts.qty_each = bodys.qty_each != null ? bodys.qty_each : getProducts.qty_each;
            getProducts.qty_koli = bodys.qty_koli != null ? bodys.qty_koli : getProducts.qty_koli;
            getProducts.id_product = bodys.id_product != null ? bodys.id_product : getProducts.id_product;
            getProducts.unit = bodys.unit != null ? bodys.unit : getProducts.unit;
            getProducts.in_out = bodys.in_out != null ? bodys.in_out : getProducts.in_out;
            getProducts.site = bodys.site != null ? bodys.site : getProducts.site;
            getProducts.vendor = bodys.vendor != null ? bodys.vendor : getProducts.vendor;
            getProducts.officer = bodys.officer != null ? bodys.officer : getProducts.officer;
            getProducts.insert_by = bodys.insert_by != null ? bodys.insert_by : getProducts.insert_by;
            getProducts.last_update = now;
            db1.SubmitChanges();
            return getProducts;
        }

        private inv_master_in_out inv_master_in_out_Delete(string datas)
        {
            inv_master_in_out bodys = inv_master_in_out(datas);
            var getProducts = db1.inv_master_in_outs.Where(c => c.id == bodys.id).FirstOrDefault();
            db1.inv_master_in_outs.DeleteOnSubmit(getProducts);
            db1.SubmitChanges();
            return getProducts;
        }

        private List<inv_master_in_out> inv_master_in_out_Get(string datas)
        {
            inv_master_in_out bodys = inv_master_in_out(datas);
            var getProducts = db1.inv_master_in_outs.Where(c => c.id == bodys.id).Take(1).ToList();
            return getProducts;
        }

        private List<inv_master_in_out> inv_master_in_out_GetAll(string datas)
        {
            inv_master_in_out bodys = inv_master_in_out(datas);
            var getProducts = db1.inv_master_in_outs.ToList();
            return getProducts;
        }

        //---------------------------------------------------------------------------------------

        // ------------------------------------------------------------------------- INVENTORY STOCK

        private Kendo.DynamicLinq.DataSourceResult inv_master_stock_Grid(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            var getProducts = db1.inv_master_stocks.OrderByDescending(c => c.stock_each).ToDataSourceResult(take, skip, sort, filter);
            return getProducts;
        }

        private List<inv_master_stock> inv_master_stock_Get(string datas)
        {
            inv_master_stock bodys = inv_master_stock(datas);
            var getProducts = db1.inv_master_stocks.Where(c => c.id_product == bodys.id_product).Take(1).ToList();
            return getProducts;
        }

        private List<inv_master_stock> inv_master_stock_GetAll(string datas)
        {
            var getProducts = db1.inv_master_stocks.ToList();
            return getProducts;
        }

        //---------------------------------------------------------------------------------------

        // ------------------------------------------------------------------------- INVENTORY STOCK BY PO

        //inv_master_stock_by_po
        private Kendo.DynamicLinq.DataSourceResult inv_master_stock_by_po_Grid(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter, string datas)
        {
            var getProducts = db1.inv_master_stock_by_pos.Where(c => c.id_product == datas).OrderByDescending(c => c.stock_each).ToDataSourceResult(take, skip, sort, filter);
            return getProducts;
        }





        //-------------------------------- Controllers

        [HttpPost]
        public JsonResult MasterGrid(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            try
            {
                var getProducts = inv_master_product_Grid(take, skip, sort, filter);
                return Json(getProducts);
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult MasterInsert(string datas)
        {
            try
            {
                var getProducts = inv_master_product_Insert(datas);
                //System.Diagnostics.Debug.WriteLine(decode);
                return Json(new { result = "SUCCESS", data = getProducts });

            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult MasterUpdate(string datas)
        {
            try
            {
                var getProducts = inv_master_product_Update(datas);

                return Json(new { result = "SUCCESS", data = getProducts });
            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult MasterGet(string datas)
        {
            try
            {
                var getProducts = inv_master_product_Get(datas);

                return Json(new { result = "SUCCESS", data = getProducts });
            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult MasterGetAll(string datas)
        {
            try
            {
                var getProducts = inv_master_product_GetAll(datas);

                return Json(new { result = "SUCCESS", data = getProducts });
            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult MasterDelete(string datas)
        {
            try
            {
                var getProducts = inv_master_product_Delete(datas);

                return Json(new { result = "SUCCESS", data = getProducts });
            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }

        // --------------------------------------------------- INVENTORY IN OUT

        [HttpPost]
        public JsonResult InventoryGrid(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            try
            {
                var getProducts = inv_master_in_out_Grid(take, skip, sort, filter);
                return Json(getProducts);
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }

        }

        [HttpPost]
        public JsonResult InventoryIn(string datas)
        {
            try
            {
                var res = inv_master_in_out_Insert(datas, "IN");
                return Json(new { result = "SUCCESS", data = res });

            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult InventoryOut(string datas)
        {
            try
            {
                var res = inv_master_in_out_Insert(datas, "OUT");
                return Json(new { result = "SUCCESS", data = res });

            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult InventoryGet(string datas)
        {
            try
            {
                var getProducts = inv_master_in_out_Get(datas);

                return Json(new { result = "SUCCESS", data = getProducts });
            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult InventoryGetAll(string datas)
        {
            try
            {
                var getProducts = inv_master_in_out_GetAll(datas);

                return Json(new { result = "SUCCESS", data = getProducts });
            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult InventoryUpdate(string datas)
        {
            try
            {
                var getProducts = inv_master_in_out_Update(datas);

                return Json(new { result = "SUCCESS", data = getProducts });
            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }

        }

        [HttpPost]
        public JsonResult InventoryDelete(string datas)
        {
            try
            {
                var getProducts = inv_master_in_out_Delete(datas);

                return Json(new { result = "SUCCESS", data = getProducts });
            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }

        // ---------------------------------------------------

        // --------------------------------------------------- INVENTORY STOCK

        [HttpPost]
        public JsonResult StockGrid(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter)
        {
            try
            {
                var getProducts = inv_master_stock_Grid(take, skip, sort, filter);
                return Json(getProducts);
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult StockGet(string datas)
        {
            try
            {
                var getProducts = inv_master_stock_Get(datas);

                return Json(new { result = "SUCCESS", data = getProducts });
            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }

        [HttpPost]
        public JsonResult StockGetAll(string datas)
        {
            try
            {
                var getProducts = inv_master_stock_GetAll(datas);

                return Json(new { result = "SUCCESS", data = getProducts });
            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }
        }


        // ---------------------------------------------------


        // --------------------------------------------------- INVENTORY STOCK BY PO

        [HttpPost]
        public JsonResult StockGridbyPo(int take, int skip, IEnumerable<Kendo.DynamicLinq.Sort> sort, Kendo.DynamicLinq.Filter filter, string datas)
        {
            try
            {
                var getProducts = inv_master_stock_by_po_Grid(take, skip, sort, filter, datas);
                return Json(getProducts);
            }
            catch (Exception e)
            {
                return this.Json(new { error = e.ToString() });
            }
        }


        // ---------------------------------------------------


        // --------------------------------------------------- OTHER

        [HttpPost]
        public JsonResult FormInGetItem(string datas)
        {
            try
            {
                var getProducts = inv_master_product_Get(datas);

                return Json(new { result = "SUCCESS", data = getProducts });
            }
            catch (Exception e)
            {
                return this.Json(new { result = "ERROR", data = e.ToString() });
            }

        }

        // --------------------------------------------------------------------






    }
}