using SAPbobsCOM;
using System.Runtime.CompilerServices;

namespace DescuentoDamasco.Controllers
{
    public class SapConnection
    {
        public static Company company = null;

        public static bool connect()
        {
            try {

                company = new Company();
                company.Server = "ERPSAP\\SAPB1";
                company.CompanyDB = "DAMASCO_PRODUCTIVA";
                company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2019;
                company.UserName = "emahmud";
                company.Password = "Zizou..10";
                int result = company.Connect();
                string errMsg = company.GetLastErrorDescription();
                int Errno = company.GetLastErrorCode();
                if (Errno != 0)
                {
                    //errro de conexion
                    Console.WriteLine($"Error de conexion de sap {errMsg}  ");
                    return false;
                }
                else
                {
                    Console.WriteLine("Coexion a SAP exitosa");
                    return true;
                }

            }
            catch(Exception ex) {

                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
            
          
        }
        public static void Disconnect()
        {
            try
            {
                if (company != null && company.Connected)
                {
                    company.Disconnect();
                }
            }
            catch (Exception ex)
            {
                // Manejar posibles errores al desconectar
                Console.WriteLine($"Error al desconectar: {ex.Message}");
            }
        }
    }
}
