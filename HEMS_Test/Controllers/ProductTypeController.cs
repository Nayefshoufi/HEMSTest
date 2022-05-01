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
    public class ProductTypeController : Controller
    {
        // GET: Product
        public ActionResult ProductTypes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetProductTypes(DataTableAjaxPostModel model)
        {
            List<ProductType> data = new List<ProductType>();
            using (HEMS_Test.Models.HEMS_testEntities hEMS_TestEntities = new Models.HEMS_testEntities())
            {
                data = hEMS_TestEntities.ProductTypes.Include(x => x.ProductCategory)
                    .Where(x => x.Status == "ACTIVE").ToList();
            }
            var result = data.Select(x => new
            {
                x.Product_Type_Code,
                x.Type_Name,
                x.Type_Description,
                Create_Date = x.Create_Date.ToString("dd/MM/yyyy HH:mm"),
                Update_Date = x.Update_Date?.ToString("dd/MM/yyyy HH:mm"),
                x.ProductCategory.Category_Name,                
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
        public ActionResult CreateProductType()
        {
            return PartialView("Partials/CreateProductType", new ProductType());
        }

        [HttpPost]
        public ActionResult CreateProductType(ProductType model)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var ErrN = new System.Data.Entity.Core.Objects.ObjectParameter("ErrN", 0);
                var ErrM = new System.Data.Entity.Core.Objects.ObjectParameter("ErrM", "");

                entity.SaveProductType(new System.Data.Entity.Core.Objects.ObjectParameter("seq", 0),model.Product_Type_Code
                    , model.Product_Category_Code
                    , model.Type_Name
                    , model.Type_Description
                    , ErrN
                    , ErrM);

                return Json(new { result = (int)ErrN.Value, message = (string)ErrM.Value }, JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpGet]
        public ActionResult EditProductType(string code)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var model = entity.ProductTypes.FirstOrDefault((x => x.Status == "ACTIVE" && x.Product_Type_Code == code));
                return PartialView("Partials/EditProductType", model);
            }
        }

        [HttpGet]
        public JsonResult DeleteProductType(string code)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var errN = new System.Data.Entity.Core.Objects.ObjectParameter("ErrN", 0);
                var errM = new System.Data.Entity.Core.Objects.ObjectParameter("ErrM", "");
                var model = entity.DeleteProductType(code, errN, errM);
                return Json(new { result = (int)errN.Value, message = (string)errM.Value }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult EditProductType(ProductType model)
        {
            using (HEMS_testEntities entity = new HEMS_testEntities())
            {
                var ErrN = new System.Data.Entity.Core.Objects.ObjectParameter("ErrN", 0);
                var ErrM = new System.Data.Entity.Core.Objects.ObjectParameter("ErrM", "");

                entity.SaveProductType(new System.Data.Entity.Core.Objects.ObjectParameter("seq", 0), model.Product_Type_Code
                    , model.Product_Category_Code
                    , model.Type_Name
                    , model.Type_Description
                    , ErrN
                    , ErrM);

                return Json(new { result = (int)ErrN.Value, message = (string)ErrM.Value }, JsonRequestBehavior.AllowGet);
            }
        }
        

    }
}