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
    public class ProductCategoryController : Controller
    {
        // GET: Product
        public ActionResult ProductCategories()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetProductCategories(DataTableAjaxPostModel model)
        {
            List<ProductCategory> data = new List<ProductCategory>();
            using (HEMS_Test.Models.HEMS_testEntities hEMS_TestEntities = new Models.HEMS_testEntities())
            {
                data = hEMS_TestEntities.ProductCategories
                    .Where(x => x.Status == "ACTIVE").ToList();
            }
            var result = data.Select(x => new
            {
                x.Product_Category_Code,
                x.Category_Name,
                x.Category_Description,
                Create_Date = x.Create_Date.ToString("dd/MM/yyyy HH:mm"),
                Update_Date = x.Update_Date?.ToString("dd/MM/yyyy HH:mm"),                
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
        public ActionResult CreateProductCategory()
        {
            return PartialView("Partials/CreateProductCategory", new ProductCategory());
        }

        [HttpPost]
        public ActionResult CreateProductCategory(ProductCategory model)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var ErrN = new System.Data.Entity.Core.Objects.ObjectParameter("ErrN", 0);
                var ErrM = new System.Data.Entity.Core.Objects.ObjectParameter("ErrM", "");

                entity.SaveProductCategory(new System.Data.Entity.Core.Objects.ObjectParameter("seq", 0),model.Product_Category_Code
                    , model.Category_Name
                    , model.Category_Description
                    , ErrN
                    , ErrM);

                return Json(new { result = (int)ErrN.Value, message = (string)ErrM.Value }, JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpGet]
        public ActionResult EditProductCategory(string code)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var model = entity.ProductCategories.FirstOrDefault((x => x.Status == "ACTIVE" && x.Product_Category_Code == code));
                return PartialView("Partials/EditProductCategory", model);
            }
        }

        [HttpGet]
        public JsonResult DeleteProductCategory(string code)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var errN = new System.Data.Entity.Core.Objects.ObjectParameter("ErrN", 0);
                var errM = new System.Data.Entity.Core.Objects.ObjectParameter("ErrM", "");
                var model = entity.DeleteProductCategory(code, errN, errM);
                return Json(new { result = (int)errN.Value, message = (string)errM.Value }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditProductCategory(ProductCategory model)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var ErrN = new System.Data.Entity.Core.Objects.ObjectParameter("ErrN", 0);
                var ErrM = new System.Data.Entity.Core.Objects.ObjectParameter("ErrM", "");

                entity.SaveProductCategory(new System.Data.Entity.Core.Objects.ObjectParameter("seq", 0)
                    , model.Product_Category_Code
                    , model.Category_Name
                    , model.Category_Description
                    , ErrN
                    , ErrM);

                return Json(new { result = (int)ErrN.Value, message = (string)ErrM.Value }, JsonRequestBehavior.AllowGet);
            }
        }
        

    }
}