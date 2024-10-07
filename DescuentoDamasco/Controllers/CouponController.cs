using DescuentoDamasco.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SAPbobsCOM;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace DescuentoDamasco.Controllers
{
    public class CouponController : Controller
    {
    
        private SqlConnection _connection;
        private SqlConnection _connectionDamscoProd;
        private SqlConnection _connectionKlkPos;
        private SqlConnection _connectionDescuento;
        private bool isSuccess;
        private string message;

        public IConfiguration _configuration {  get; set; }

        public CouponController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Connection()
        {
            string connectionCupon = _configuration["ConnectionStrings:SQLConnection"];
            _connection = new SqlConnection(connectionCupon);
        }

        public void ConnectionDamscoProd() {

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


        [HttpGet]
        public JsonResult GetStates(string cntry)
        {
            List<ProvinceModel> prov = States();
            Console.WriteLine(prov);
            return Json(new { prov });

        }
        [HttpGet]
        public JsonResult GetMunicipio(string state)
        {
            List<MunicipalityModel> mun = Municipality(state);
            return Json(new { mun });
        }

        [HttpGet]
        public JsonResult GetCities(string state)
        {
            List<CityModel> city = Cities(state);
            return Json(new { city });

        }

        [HttpGet]
        public JsonResult GetZone(string state, string mun)
        {

            List<ZoneModel> zoneList = Zones(state, mun);
            return Json(new { zoneList });


        }

        public List<ProvinceModel> States()
        {
            Connection();
            List<ProvinceModel> listaEstados = new List<ProvinceModel>();
            SqlCommand command = new SqlCommand("select CD_PROVINCIA ,NOMBRE_DE_PROVINCIA  FROM PROVINCIA", _connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            System.Data.DataTable dataTable = new System.Data.DataTable();

            _connection.Open();
            adapter.Fill(dataTable);
            _connection.Close();
            foreach (DataRow item in dataTable.Rows)
            {
                listaEstados.Add(new ProvinceModel
                {
                    ProvinceId = Convert.ToString(item["CD_PROVINCIA"]),
                    ProvinceName = Convert.ToString(item["NOMBRE_DE_PROVINCIA"])
                });

            }
            return listaEstados;
        }

        public List<MunicipalityModel> Municipality(string stateId)
        {
            Connection();
            List<MunicipalityModel> listaMunicipios = new List<MunicipalityModel>();
            SqlCommand commandMunicipality = new SqlCommand("select CD_MUNICIPIO ,NOMBRE_DE_MUNICIPIO  " +
                "FROM MUNICIPIO where CD_PROVINCIA =  @ParameterName", _connection);
            SqlDataAdapter adapterMun = new SqlDataAdapter(commandMunicipality);
            commandMunicipality.Parameters.AddWithValue("@ParameterName", stateId);
            System.Data.DataTable dataTableMun = new System.Data.DataTable();
            _connection.Open();
            adapterMun.Fill(dataTableMun);
            _connection.Close();
            foreach (DataRow item in dataTableMun.Rows)
            {
                listaMunicipios.Add(new MunicipalityModel
                {
                    MunucipId = Convert.ToString(item["CD_MUNICIPIO"]),
                    MunucipName = Convert.ToString(item["NOMBRE_DE_MUNICIPIO"]),
                });
            }
            return listaMunicipios;
        }

        public List<CityModel> Cities(string stateId)
        {
            Connection();
            List<CityModel> citiesList = new List<CityModel>();
            SqlCommand commandCity = new SqlCommand("select CD_CIUDAD ,DE_CIUDAD  FROM CIUDAD where CD_PROVINCIA = @provinceId", _connection);
            SqlDataAdapter adapterCit = new SqlDataAdapter(commandCity);
            commandCity.Parameters.AddWithValue("provinceId", stateId);
            System.Data.DataTable dataTableCity = new System.Data.DataTable();

            _connection.Open();
            adapterCit.Fill(dataTableCity);
            _connection.Close();
            foreach (DataRow item in dataTableCity.Rows)
            {
                citiesList.Add(new CityModel
                {
                    CityId = Convert.ToString(item["CD_CIUDAD"]),
                    CityName = Convert.ToString(item["DE_CIUDAD"]),
                });
            }
            return citiesList;
        }
        public List<ZoneModel> Zones(string stateId, string MunucipId)
        {
            Connection();
            List<ZoneModel> zoneList = new List<ZoneModel>();
            SqlCommand commandZone = new SqlCommand("select CD_ZONA ,NOMBRE_DE_ZONA  FROM ZONA where CD_PROVINCIA = @provinceId and CD_MUNICIPIO = @muniId", _connection);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(commandZone);
            commandZone.Parameters.AddWithValue("@provinceId", stateId);
            commandZone.Parameters.AddWithValue("@muniId", MunucipId);
            System.Data.DataTable zoneData = new System.Data.DataTable();
            _connection.Open();
            sqlDataAdapter.Fill(zoneData);
            _connection.Close();

            foreach (DataRow item in zoneData.Rows)
            {
                zoneList.Add(new ZoneModel { ZoneId = Convert.ToString(item["CD_ZONA"]), ZoneName = Convert.ToString(item["NOMBRE_DE_ZONA"]) });
            }
            return zoneList;
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

        public IActionResult ObtenerDatos(long sucursal, string numFactura)
        {
            ConnectionKlkPos();
            string destinoStr = sucursal.ToString();
            int totalDigits = destinoStr.Length;

            int primerosDosDigitos = (int)(sucursal / (int)Math.Pow(10, totalDigits - 2));

            string Almacen2 = ObtenerAlmacen(primerosDosDigitos);

            string Almacen = Regex.Replace(Almacen2, @"\s+", "");
            string query = @"
                        SELECT li.NumFactura, fa.FechaFactura, fa.CodCliente, fa.NomCliente, li.CodArticulo,li.Serial,li.Descripcion,ROUND(li.PrecioUSD, 2) AS PrecioUSD, li.CodigoAlmacen
                    FROM KLK_FACTURALINE li
                    INNER JOIN KLK_FACTURAHDR fa ON li.NumFactura = fa.NumFactura
                    WHERE fa.NumFactura = @numFactura AND fa.IDSucursal = @Almacen";

            var result = new List<FacturaDataModel>();
            SqlCommand sqlCommand = new SqlCommand(query,_connectionKlkPos);
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            sqlCommand.Parameters.AddWithValue("@numFactura", numFactura);
            sqlCommand.Parameters.AddWithValue("@Almacen", primerosDosDigitos);
            
            _connectionKlkPos.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var precioDefinDolar = Math.Round(Convert.ToDecimal(reader["PrecioUSD"]), 0);
                var precioSinInva = precioDefinDolar / 1.16m;
                var precioPoliza = Math.Round(precioSinInva * 0.06m, 2);
                result.Add(new FacturaDataModel
                {
                    NumFactura = reader["NumFactura"].ToString(),
                    FechaFactura = Convert.ToDateTime(reader["FechaFactura"]),
                    CodCliente = reader["CodCliente"].ToString(),
                    NomCliente = reader["NomCliente"].ToString(),
                    Serial = reader["Serial"].ToString(),
                    CodArticulo = reader["CodArticulo"].ToString(),
                    Descripcion = reader["Descripcion"].ToString(),
                    PrecioUSD = Math.Round(Convert.ToDecimal(reader["PrecioUSD"]), 0),
                    PrecioPoliza = precioPoliza,
                    CodigoAlmacen = reader["CodigoAlmacen"].ToString()
                });
            }
            _connectionKlkPos.Close();
            return Json(result);
        }
        private string ObtenerAlmacen(long sucursal)
        {
            ConnectionDamscoProd();
            string almacen = null;

            string query = @"
        SELECT u_almacen 
            FROM aplicativofront 
        WHERE id_sucursal = @sucursal";
            SqlCommand cmd = new SqlCommand(query,_connectionDamscoProd);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@sucursal",sucursal);
            
            _connectionDamscoProd.Open();
            var result = cmd.ExecuteScalar();
            if(result != null)
            {
                almacen = result.ToString();
            }
            _connectionDamscoProd.Close();
            return almacen;
        }

        [HttpPost]
        public IActionResult InfoClient([FromBody] ClientInfo clientInfo)
        {
            var porcentageDiscu = 0;
            CouponModel couponModel = new CouponModel();
            if (clientInfo.AmountInvoice >= 10.00m && clientInfo.AmountInvoice <= 500.99m)
            {
                couponModel.PercentageDiscount = "5";
                couponModel.AmountDiscount = clientInfo.AmountInvoice * 0.05m;
            }
            else if (clientInfo.AmountInvoice >= 501.00m)
            {
                couponModel.PercentageDiscount = "10";
                couponModel.AmountDiscount = clientInfo.AmountInvoice * 0.10m;
            }
            couponModel.ClienteInfo = clientInfo;
            couponModel.dateUntilCoupon = clientInfo.InvoiceDate.AddDays(15);
            var dateGen = DateTime.Now;

            couponModel.CouponId = "DCTO" + dateGen.Minute + dateGen.Second + dateGen.Millisecond;



            var respMessage = SendMessage(couponModel);
            SaveDataPolicy(couponModel);
            Console.WriteLine(respMessage);

            return Json(new { success = true}); ;
        }
       public async Task  SendMessage( CouponModel couponModel)
        {
           MessageContent messageContent = new MessageContent();
            DateTime dates = couponModel.dateUntilCoupon;
            var CouponDateFormatted = dates.ToString("dd/MM/yyyy");
            var result = "";
            var messageBody = $"Tienes un  saldo a tu favor de {couponModel.AmountDiscount:n}, con este cupón {couponModel.CouponId}, dcto intransferible valido" +
                $" hasta el {CouponDateFormatted}";
            var url = "http://200.74.198.50:14010/notifismsdamas";
            messageContent.Message = messageBody;
            messageContent.ClientNumber = couponModel.ClienteInfo.PhoneNumberClient;
            messageContent.PriorityNumber = 0;
            messageContent.Department = "01";
            messageContent.IdCampaing = "PromoDamasco";
            string messageReq = JsonConvert.SerializeObject(messageContent);
            HttpResponseMessage resp = null;
            using (var httpClient = new HttpClient()) 
            {
            
                var bodyRequest = new StringContent(messageReq, Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Add("X-Auth-Apikey", "57xg$mG8%H4*");
                resp = await httpClient.PostAsync(url, bodyRequest);
                resp.EnsureSuccessStatusCode();
              
            }

           
        }


        private void SaveDataPolicy(CouponModel couponModel)
        {
            Descuento();

            try
            {
                // Abrir la conexión
                _connectionDescuento.Open();

                using (SqlCommand sqlCommand = new SqlCommand(@"INSERT INTO InfoDescuento 
                ([Cedula_Cliente], [Nombre_Cliente], [nro.Cupon], [Fecha_Emision], [Fecha_Vencimiento], [Correo], [Telefono]) 
                VALUES (@cedula, @Nombre, @Cupon, @fecha_Emision, @fecha_vencimiento, @Correo, @Telefono)",
                        _connectionDescuento))
                {
                    // Asignar parámetros
                    sqlCommand.Parameters.AddWithValue("@cedula", couponModel.ClienteInfo.Cedula );
                    sqlCommand.Parameters.AddWithValue("@Nombre", couponModel.ClienteInfo.NameClient );
                    sqlCommand.Parameters.AddWithValue("@Cupon", couponModel.CouponId );
                    sqlCommand.Parameters.AddWithValue("@fecha_Emision", couponModel.ClienteInfo.InvoiceDate);
                    sqlCommand.Parameters.AddWithValue("@fecha_vencimiento", couponModel.dateUntilCoupon);
                    sqlCommand.Parameters.AddWithValue("@Correo", couponModel.ClienteInfo.EmailClient );
                    sqlCommand.Parameters.AddWithValue("@Telefono", couponModel.ClienteInfo.PhoneNumberClient );

                    // Ejecutar la consulta
                    sqlCommand.ExecuteNonQuery();
                    isSuccess = true;
                    message = "Ingreso guardado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = "Error al guardar en la base de datos: " + ex.Message;
            }
            finally
            {
              
                if (_connectionDescuento.State == System.Data.ConnectionState.Open)
                {
                    _connectionDescuento.Close();
                }
            }
        }

        

    }
}
