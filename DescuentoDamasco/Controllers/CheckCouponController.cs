using DescuentoDamasco.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SAPbobsCOM;
using System.Data.SqlClient;

namespace DescuentoDamasco.Controllers
{
    public class CheckCouponController : Controller
    {
        private SqlConnection _connection;
        private SqlConnection _connectionDamscoProd;
        private SqlConnection _connectionKlkPos;
        private SqlConnection _connectionDescuento;
        private bool isSuccess;
        private string message;

        public IConfiguration _configuration { get; set; }

        public CheckCouponController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void Connection()
        {
            string connectionCupon = _configuration["ConnectionStrings:SQLConnection"];
            _connection = new SqlConnection(connectionCupon);
        }

        public void ConnectionDamscoProd()
        {

            string connectionDamascoProd = _configuration["ConnectionStrings:SQLConnection2"];

            _connectionDamscoProd = new SqlConnection(connectionDamascoProd);
        }

        public void ConnectionKlkPos()
        {
            string connectionKlkPos = _configuration["ConnectionStrings:SQLConnection3"];
            _connectionKlkPos = new SqlConnection(connectionKlkPos);

        }


        public void Descuento()
        {
            string connectionDescuento = _configuration["ConnectionStrings:SQLConnection4"];
            _connectionDescuento = new SqlConnection(connectionDescuento);

        }
        public IActionResult Index()
        {
            var sucursales = ObtenerSucursales();
            var model = new DescuentoModel
            {

                Sucursales = sucursales
            };
            return View(model);
        }

        private List<SelectListItem> ObtenerSucursales()
        {
            List<SelectListItem> sucursales = new List<SelectListItem>();

            try
            {
                if (SapConnection.connect())
                {
                    Console.WriteLine("Conexión establecida correctamente para obtener sucursales");
                    SAPbobsCOM.Company company = SapConnection.company;
                    Recordset recordset = company.GetBusinessObject(BoObjectTypes.BoRecordset);
                    string query = "select * from aplicativofront";
                    recordset.DoQuery(query);

                    while (!recordset.EoF)
                    {
                        string storeId = recordset.Fields.Item("id_sucursal").Value.ToString();
                        string Almacen = recordset.Fields.Item("u_almacen").Value.ToString();
                        string storeName = recordset.Fields.Item("nombre").Value.ToUpper().ToString();

                        sucursales.Add(new SelectListItem
                        {
                            Value = storeId + Almacen,
                            Text = storeName,
                        });

                        recordset.MoveNext();
                    }
                }
                else
                {
                    Console.WriteLine("Error de conexión a SAP Business One al intentar obtener sucursales");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener sucursales: " + ex.Message);
            }
            finally
            {
                SapConnection.Disconnect();
            }

            return sucursales;
        }

        [HttpPost]
        public IActionResult ValidateCoupon([FromBody] CheckCouponModel checkCouponForm)
        {
            Descuento();

            _connectionDescuento.Open();
            string destinoStr = checkCouponForm.CouponStore.ToString();
            int totalDigits = destinoStr.Length;
            CouponResultCheck couponResultCheck = new CouponResultCheck();
            var numberTienda = Convert.ToInt64(checkCouponForm.CouponStore);
            int primerosDosDigitos = (int)(numberTienda / (int)Math.Pow(10, totalDigits - 2));

          
            string query = @"SELECT  [nro.Cupon], [Estatus], [Fecha_Vencimiento]  FROM [Descuento].[dbo].[InfoDescuento] where [nro.Cupon] = @Coupon";
            SqlCommand sqlCommand = new SqlCommand(query, _connectionDescuento);
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            sqlCommand.Parameters.AddWithValue("@Coupon", checkCouponForm.CouponCode);
            JsonResult rest = null;
            using (var reader = sqlCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    couponResultCheck.CouponCode = reader["nro.Cupon"].ToString().Trim();
                    var stat = Convert.ToBoolean(reader["Estatus"]);
                  
                    var dates = Convert.ToDateTime(reader["Fecha_Vencimiento"]);
                    Console.WriteLine(dates);
             
                    couponResultCheck.Status = stat;
                    couponResultCheck.DateUntilCoupon = dates;
                    rest = UpdateCoupon(checkCouponForm, couponResultCheck,primerosDosDigitos);
                }
            }
           
            _connectionDescuento.Close();
            return Json(rest);
        }

        public JsonResult UpdateCoupon(CheckCouponModel checkCouponForm, CouponResultCheck couponResultCheck, int tienda)
        {
          var respStatus = false;
         var messageResp = string.Empty;
            DateTime dateClaim = DateTime.Now;
            if (couponResultCheck.CouponCode == null)
            {
                return Json(new { success = false, couponResultCheck.Status });
            }
            else if (couponResultCheck.CouponCode == checkCouponForm.CouponCode && couponResultCheck.Status == false &&  dateClaim < couponResultCheck.DateUntilCoupon)
            {

                try
                {

                    string queryUpadate = @"UPDATE [Descuento].[dbo].[InfoDescuento]
                                 SET [Estatus] = @stat , [TiendaConsumo] = @store
                                    WHERE [nro.Cupon] = @Coupon";
                    SqlCommand sqlCommand = new SqlCommand(queryUpadate, _connectionDescuento);
                    sqlCommand.Parameters.AddWithValue("@stat", true);
                    sqlCommand.Parameters.AddWithValue("@store", tienda);
                    sqlCommand.Parameters.AddWithValue("@Coupon", checkCouponForm.CouponCode);
                  
                    sqlCommand.ExecuteNonQuery();
                    isSuccess = true;
                    respStatus = true;
                    messageResp = "Cupón reclamado";

                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    message = "Error al actualizar en la base de datos: " + ex.Message;
                    respStatus = false;
                    messageResp = message;
                }


              



            }
            if (couponResultCheck.CouponCode != checkCouponForm.CouponCode)
            {
                respStatus = false;
                messageResp = "Cupón invalido";
            }
            else if (couponResultCheck.CouponCode == checkCouponForm.CouponCode && couponResultCheck.Status == true && dateClaim < couponResultCheck.DateUntilCoupon)
            {
                respStatus = false;
                messageResp = "El Cupón ya fue usado";
            }else if(couponResultCheck.CouponCode == checkCouponForm.CouponCode && couponResultCheck.Status == false && dateClaim > couponResultCheck.DateUntilCoupon)
            {
                respStatus = false;
                messageResp = "El Cupón esta vencido";

            }
            return Json(new { success = respStatus, message = messageResp }); ;
        }

       


    }
}
