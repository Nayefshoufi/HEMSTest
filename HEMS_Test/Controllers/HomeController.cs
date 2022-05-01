using HEMS_Test.DataAccess.Repository;
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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<ProductCategory> model = new List<ProductCategory>();
            using (HEMS_testEntities hEMS_TestEntities = new HEMS_testEntities())
            {
                model = hEMS_TestEntities.ProductCategories
                    .Where(y => y.Status == "ACTIVE")
                    .Include(x => x.ProductTypes)
                    .Include(x => x.ProductTypes.Select(y => y.Products))
                    .ToList();
                if (model.Count > 0)
                {
                    model.ForEach(x => 
                    { 
                        x.ProductTypes = x.ProductTypes.Where(y => y.Status == "ACTIVE").ToList();
                        if (x.ProductTypes.Count > 0)
                        {
                            foreach (var pt in x.ProductTypes)
                            {
                                pt.Products = pt.Products.Where(y => y.Status == "ACTIVE").ToList();
                            }
                        }
                    });
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new CreateModel());
        }

        [HttpPost]
        public JsonResult Create(CreateModel model) 
        {
            using (HEMS_testEntities hEMS_TestEntities = new HEMS_testEntities())
            {
                var errN = new System.Data.Entity.Core.Objects.ObjectParameter("ErrN", 0);
                var errM = new System.Data.Entity.Core.Objects.ObjectParameter("ErrM", "");
                int result = hEMS_TestEntities.SaveProductFull(
                    model.ProductCode,
                    model.ProductName,
                    model.ProductDescription,
                    model.TypeCode,
                    model.TypeName,
                    model.TypeDescription,
                    model.CategoryCode,
                    model.CategoryName,
                    model.CategoryDescription,
                    errN,
                    errM);
                return Json(new
                {
                    result = (int)errN.Value,
                    message = (string)errM.Value
                }, JsonRequestBehavior.AllowGet);
            }            
        }
    }
}