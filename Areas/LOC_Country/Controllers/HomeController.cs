using MetronicAddressBook.BAL;
using MetronicAddressBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using MetronicAddressBook.Areas.LOC_Country.Models;

namespace MetronicAddressBook.Areas.LOC_Country.Controllers
{
    [CheckAccess]
    [Area("LOC_Country")]
    [Route("LOC_Country/[controller]/[action]")]
    public class HomeController : Controller
    {
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
            string connectionstr = this.Configuration.GetConnectionString("myConnectionString");
            //PrePare a connection
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(connectionstr);
            conn.Open();

            //Prepare a Command
            SqlCommand objCmd = conn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_LOC_Country_SelectAll";

            SqlDataReader objSDR = objCmd.ExecuteReader();
            dt.Load(objSDR);
            conn.Close();
            return View("Index", dt);
        }
        #endregion

        #region Add
        public IActionResult Add(int? CountryID)
        {
            if (CountryID != null)
            {
                string connectionstr = this.Configuration.GetConnectionString("myConnectionString");
                //PrePare a connection
                SqlConnection conn = new SqlConnection(connectionstr);
                conn.Open();

                //Prepare a Command
                SqlCommand objCmd = conn.CreateCommand();
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.CommandText = "PR_LOC_Country_SelectByPK";
                objCmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = CountryID;
                DataTable dt = new DataTable();
                SqlDataReader objSDR = objCmd.ExecuteReader();
                dt.Load(objSDR);

                LOC_CountryModel modelLOC_Country = new LOC_CountryModel();

                foreach (DataRow dr in dt.Rows)
                {
                    modelLOC_Country.CountryID = Convert.ToInt32(dr["CountryID"]);
                    modelLOC_Country.CountryName = dr["CountryName"].ToString();
                    modelLOC_Country.CountryCode = dr["CountryCode"].ToString();
                }

                return View("LOC_CountryAddEdit", modelLOC_Country);
            }
            return View("LOC_CountryAddEdit");
        }
        #endregion

        #region Insert
        [HttpPost]
        public IActionResult Save(LOC_CountryModel modelLOC_Country)
        {
            if (ModelState.IsValid)
            {
                string connectionstr = this.Configuration.GetConnectionString("myConnectionString");
                //PrePare a connection
                SqlConnection conn = new SqlConnection(connectionstr);
                conn.Open();

                //Prepare a Command
                SqlCommand objCmd = conn.CreateCommand();
                objCmd.CommandType = CommandType.StoredProcedure;

                if (modelLOC_Country.CountryID == null)
                {
                    objCmd.CommandText = "PR_LOC_Country_Insert";
                    objCmd.Parameters.Add("@Created", SqlDbType.DateTime).Value = DBNull.Value;
                }
                else
                {
                    objCmd.CommandText = "PR_LOC_Country_Update";
                    objCmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = modelLOC_Country.CountryID;
                }
                objCmd.Parameters.Add("@CountryName", SqlDbType.VarChar).Value = modelLOC_Country.CountryName;
                objCmd.Parameters.Add("@CountryCode", SqlDbType.VarChar).Value = modelLOC_Country.CountryCode;
                objCmd.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DBNull.Value;

                if (Convert.ToBoolean(objCmd.ExecuteNonQuery()))
                {
                    if (modelLOC_Country.CountryID == null)
                        TempData["CountryInsertMsg"] = "Record Inserted Successfully";
                    else
                        return RedirectToAction("Index");
                }
                conn.Close();
            }
            return View("LOC_CountryAddEdit");
        }
        #endregion

        #region Delete 
        public IActionResult Delete(int CountryID)
        {
            string str = this.Configuration.GetConnectionString("myConnectionString");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand sqlCommand = conn.CreateCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_LOC_Country_Delete";
            sqlCommand.Parameters.AddWithValue("@CountryID", CountryID);
            sqlCommand.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("Index");
        }
        #endregion
    }
}
