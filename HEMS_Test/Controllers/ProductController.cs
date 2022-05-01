using HEMS_Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using HEMS_Test.ViewModel;

namespace HEMS_Test.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Products()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetProducts(DataTableAjaxPostModel model)
        {
            List<Product> data = new List<Product>();
            using (HEMS_Test.Models.HEMS_testEntities hEMS_TestEntities = new Models.HEMS_testEntities())
            {
                data = hEMS_TestEntities.Products.Include(x => x.ProductType).Include(x => x.ProductCategory)
                    .Where(x => x.Status == "ACTIVE").ToList();
            }
            var result = data.Select(x => new
            {
                x.Product_Code,
                x.Product_Name,
                x.Product_Description,
                Create_Date = x.Create_Date.ToString("dd/MM/yyyy HH:mm"),
                Update_Date = x.Update_Date?.ToString("dd/MM/yyyy HH:mm"),
                x.ProductCategory.Category_Name,
                x.ProductType.Type_Name
            });
            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = data.Count,
                recordsFiltered = data.Count,
                data = result
            });
        }

        [HttpGet]
        public ActionResult CreateProduct()
        {
            return PartialView("Partials/CreateProduct", new Product());
        }

        [HttpPost]
        public ActionResult CreateProduct(Product model)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var ErrN = new System.Data.Entity.Core.Objects.ObjectParameter("ErrN", 0);
                var ErrM = new System.Data.Entity.Core.Objects.ObjectParameter("ErrM", "");

                entity.SaveProduct(new System.Data.Entity.Core.Objects.ObjectParameter("seq", 0), model.Product_Code
                    , model.Product_Category_Code
                    , model.Product_Type_Code
                    , model.Product_Name
                    , model.Product_Description
                    , ErrN
                    , ErrM);

                return Json(new { result = (int)ErrN.Value, message = (string)ErrM.Value }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetFilteredTypes(string CatId)
        {
            using (HEMS_Test.Models.HEMS_testEntities hEMS_TestEntities = new Models.HEMS_testEntities())
            {
                var data = hEMS_TestEntities.ProductTypes
                    .Where(x => x.Status == "ACTIVE" && x.Product_Category_Code == CatId).Select(x => new { code = x.Product_Type_Code, name = x.Type_Name}).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult EditProduct(string code)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var model = entity.Products.FirstOrDefault((x => x.Status == "ACTIVE" && x.Product_Code == code));
                return PartialView("Partials/EditProduct", model);
            }
        }

        [HttpGet]
        public JsonResult DeleteProduct(string code)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var errN = new System.Data.Entity.Core.Objects.ObjectParameter("ErrN", 0);
                var errM = new System.Data.Entity.Core.Objects.ObjectParameter("ErrM", "");
                var model = entity.DeleteProduct(code, errN, errM);
                return Json(new { result = (int)errN.Value, message = (string)errM.Value }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditProduct(Product model)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var ErrN = new System.Data.Entity.Core.Objects.ObjectParameter("ErrN", 0);
                var ErrM = new System.Data.Entity.Core.Objects.ObjectParameter("ErrM", "");

                entity.SaveProduct(new System.Data.Entity.Core.Objects.ObjectParameter("seq", 0), model.Product_Code
                    , model.Product_Category_Code
                    , model.Product_Type_Code
                    , model.Product_Name
                    , model.Product_Description
                    , ErrN
                    , ErrM);

                return Json(new { result = (int)ErrN.Value, message = (string)ErrM.Value }, JsonRequestBehavior.AllowGet);
            }
        }
        

    }
}