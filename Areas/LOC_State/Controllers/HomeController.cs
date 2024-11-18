using MetronicAddressBook.BAL;
using MetronicAddressBook.DAL;
using MetronicAddressBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using MetronicAddressBook.Areas.LOC_State.Models;
using MetronicAddressBook.Areas.LOC_Country.Models;

namespace MetronicAddressBook.Areas.LOC_State.Controllers
{
    [CheckAccess]
    [Area("LOC_State")]
    [Route("LOC_State/[controller]/[action]")]
    public class HomeController : Controller
    {
        LOC_DAL dalLOC = new LOC_DAL();

        #region Connection String
        private IConfiguration Configuration;
        public HomeController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        #endregion

        #region SelectAll
        public IActionResult Index()
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            string str = this.Configuration.GetConnectionString("myConnectionString");
            DataTable dt = dalLOC.dbo_PR_LOC_State_SelectAll(str);
            return View("Index", dt);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int StateID)
        {
            string str = this.Configuration.GetConnectionString("myConnectionString");
            if (Convert.ToBoolean(dalLOC.dbo_PR_LOC_State_Delete(str, StateID)))
                return RedirectToAction("Index");
            return View("Index");
        }
        #endregion 

        #region Add/Edit
        public IActionResult Add(int? StateID)
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            #region ComboBox
            string str = this.Configuration.GetConnectionString("myConnectionString");
            DataTable dt1 = dalLOC.dbo_PR_LOC_Country_SelectByDropdownList(str);

            List<LOC_CountryDropDownModel> list = new List<LOC_CountryDropDownModel>();
            foreach (DataRow dr in dt1.Rows)
            {
                LOC_CountryDropDownModel vlst = new LOC_CountryDropDownModel();
                vlst.CountryID = Convert.ToInt32(dr["CountryID"]);
                vlst.CountryName = dr["CountryName"].ToString();
                list.Add(vlst);
            }
            ViewBag.CountryList = list;
            #endregion

            #region Record Select by PK
            if (StateID != null)
            {
                DataTable dt = dalLOC.dbo_PR_LOC_State_SelectByPK(str, StateID);
                if (dt.Rows.Count > 0)
                {
                    LOC_StateModel model = new LOC_StateModel();
                    foreach (DataRow dr in dt.Rows)
                    {
                        model.StateID = Convert.ToInt32(dr["StateID"]);
                        model.StateName = dr["StateName"].ToString();
                        model.StateCode = dr["StateCode"].ToString();
                        model.CountryID = Convert.ToInt32(dr["CountryID"]);
                    }
                    return View("LOC_StateAddEdit", model);
                }
            }
            #endregion

            return View("LOC_StateAddEdit");
        }
        #endregion

        #region Save 
        [HttpPost]
        public IActionResult Save(LOC_StateModel modelLOC_State)
        {
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            string str = this.Configuration.GetConnectionString("myConnectionString");
            if (ModelState.IsValid)
            {
                if (modelLOC_State.StateID == null)
                {
                    if (Convert.ToBoolean(dalLOC.dbo_PR_LOC_State_Insert(str, modelLOC_State)))
                        TempData["StateInsertMessage"] = "Record Inserted Successfully";
                }
                else
                {
                    if (Convert.ToBoolean(dalLOC.dbo_PR_LOC_State_Update(str, modelLOC_State)))
                        return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Add");
        }
        #endregion
    }
}
