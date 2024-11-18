using MetronicAddressBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using MetronicAddressBook.Areas.LOC_City.Models;
using MetronicAddressBook.BAL;
using MetronicAddressBook.Areas.LOC_State.Models;
using MetronicAddressBook.Areas.LOC_Country.Models;

namespace MetronicAddressBook.Areas.LOC_City.Controllers
{
    [CheckAccess]
    [Area("LOC_City")]
    [Route("LOC_City/[controller]/[action]")]
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
            string str = this.Configuration.GetConnectionString("myConnectionString");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand sqlCommand = conn.CreateCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_LOC_City_SelectAll";
            DataTable dt = new DataTable();
            SqlDataReader rdr = sqlCommand.ExecuteReader();
            dt.Load(rdr);
            conn.Close();
            return View("Index", dt);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int CityID)
        {
            string str = this.Configuration.GetConnectionString("myConnectionString");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand sqlCommand = conn.CreateCommand();
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_LOC_City_Delete";
            sqlCommand.Parameters.AddWithValue("@CityID", CityID);
            sqlCommand.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("Index");
        }
        #endregion

        #region Add
        public IActionResult Add(int? CityID)
        {
            #region ComboBox
            string connectionstr = this.Configuration.GetConnectionString("myConnectionString");
            //PrePare a connection
            DataTable dt1 = new DataTable();
            SqlConnection conn1 = new SqlConnection(connectionstr);
            conn1.Open();
            SqlCommand objCmd1 = conn1.CreateCommand();
            objCmd1.CommandType = CommandType.StoredProcedure;
            objCmd1.CommandText = "PR_LOC_Country_SelectComboBox";
            SqlDataReader objSDR1 = objCmd1.ExecuteReader();
            dt1.Load(objSDR1);
            //conn1.Close();

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

            List<LOC_StateDropDownModel> list1 = new List<LOC_StateDropDownModel>();
            ViewBag.StateList = list1;


            #region Record Select by PK
            if (CityID != null)
            {
                //PrePare a connection
                DataTable dt = new DataTable();
                SqlConnection conn = new SqlConnection(connectionstr);
                conn.Open();
                SqlCommand objCmd = conn.CreateCommand();
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.CommandText = "PR_LOC_City_SelectByPK";
                objCmd.Parameters.AddWithValue("@CityID", CityID);
                SqlDataReader objSDR = objCmd.ExecuteReader();
                dt.Load(objSDR);
                if (dt.Rows.Count > 0)
                {
                    LOC_CityModel model = new LOC_CityModel();
                    foreach (DataRow dr in dt.Rows)
                    {
                        model.CityID = Convert.ToInt32(dr["CityID"]);
                        model.StateID = Convert.ToInt32(dr["StateID"]);
                        model.CityName = dr["CityName"].ToString();
                        model.CityCode = dr["CityCode"].ToString();
                        model.CountryID = Convert.ToInt32(dr["CountryID"]);
                    }

                    DataTable dt2 = new DataTable();
                    SqlConnection conn2 = new SqlConnection(connectionstr);
                    conn2.Open();
                    SqlCommand objCmd2 = conn1.CreateCommand();
                    objCmd2.CommandType = CommandType.StoredProcedure;
                    objCmd2.CommandText = "PR_LOC_State_SelectComboBoxByCountryID";
                    objCmd2.Parameters.AddWithValue("@CountryID", model.CountryID);
                    SqlDataReader objSDR2 = objCmd2.ExecuteReader();
                    dt2.Load(objSDR2);

                    List<LOC_StateDropDownModel> list2 = new List<LOC_StateDropDownModel>();

                    foreach (DataRow dr in dt2.Rows)
                    {
                        LOC_StateDropDownModel vlst = new LOC_StateDropDownModel();
                        vlst.StateID = Convert.ToInt32(dr["StateID"]);
                        vlst.StateName = dr["StateName"].ToString();
                        list2.Add(vlst);
                    }

                    ViewBag.StateList = list2;

                    return View("LOC_CityAddEdit", model);
                }
            }
            #endregion

            return View("LOC_CityAddEdit");
        }
        #endregion

        #region Save 
        [HttpPost]
        public IActionResult Save(LOC_CityModel modelLOC_City)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection objConn = new SqlConnection(this.Configuration.GetConnectionString("myConnectionString")))
                {
                    objConn.Open();
                    using (SqlCommand objCmd = objConn.CreateCommand())
                    {
                        try
                        {
                            #region Prepare Command
                            objCmd.CommandType = CommandType.StoredProcedure;

                            if (modelLOC_City.CityID == null)
                            {
                                objCmd.CommandText = "PR_LOC_City_Insert";
                                objCmd.Parameters.Add("@Created", SqlDbType.Date).Value = DBNull.Value;

                            }
                            else
                            {
                                objCmd.CommandText = "PR_LOC_City_Update";
                                objCmd.Parameters.Add("@CityID", SqlDbType.Int).Value = modelLOC_City.CityID;
                            }
                            objCmd.Parameters.Add("@CityName", SqlDbType.VarChar).Value = modelLOC_City.CityName;
                            objCmd.Parameters.Add("@CityCode", SqlDbType.VarChar).Value = modelLOC_City.CityCode;
                            objCmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = modelLOC_City.CountryID;
                            objCmd.Parameters.Add("@StateID", SqlDbType.Int).Value = modelLOC_City.StateID;
                            objCmd.Parameters.Add("@Modified", SqlDbType.Date).Value = DBNull.Value;
                            #endregion Prepare Command

                            if (Convert.ToBoolean(objCmd.ExecuteNonQuery()))
                            {
                                if (modelLOC_City.CityID == null)
                                    TempData["CityInsertMessage"] = "Record Inserted Successfully";
                                else
                                    return RedirectToAction("Index");
                            }
                        }
                        catch (SqlException sqlex)
                        {
                            //ViewBag.Message = sqlex.InnerException.Message.ToString();
                        }
                        catch (Exception ex)
                        {
                            //ViewBag.Message = ex.InnerException.Message.ToString();
                        }
                        finally
                        {
                            if (objConn.State == ConnectionState.Open)
                                objConn.Close();
                        }
                    }
                }
            }
            return RedirectToAction("Add");
        }
        #endregion

        #region Dropdownfill
        [HttpPost]
        public IActionResult DropdownByCountry(int CountryID)
        {
            string connectionstr = this.Configuration.GetConnectionString("myConnectionString");
            //PrePare a connection
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(connectionstr);
            conn.Open();
            SqlCommand objCmd = conn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            objCmd.CommandText = "PR_LOC_State_SelectComboBoxByCountryID";
            objCmd.Parameters.AddWithValue("@CountryID", CountryID);
            SqlDataReader objSDR = objCmd.ExecuteReader();
            dt.Load(objSDR);
            conn.Close();

            List<LOC_StateDropDownModel> list = new List<LOC_StateDropDownModel>();

            foreach (DataRow dr in dt.Rows)
            {
                LOC_StateDropDownModel vlst = new LOC_StateDropDownModel();
                vlst.StateID = Convert.ToInt32(dr["StateID"]);
                vlst.StateName = dr["StateName"].ToString();
                list.Add(vlst);
            }

            var vModel = list;
            return Json(vModel);
        }
        #endregion
    }
}
