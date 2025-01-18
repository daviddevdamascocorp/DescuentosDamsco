using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace DescuentoDamasco.Models
{

    public class DescuentoModel
    {
        public List<SelectListItem>? Sucursales { get; set; }
        public string SucursalId { get; set; }


    }
    public class CouponModel
    {
        
        public ClientInfo ClienteInfo { get; set; }

      

       

}


    public class Article
    {
        public string codarticulo { get; set; }
        public string nomArticulo { get; set; }
    }



    public class ClientInfo {

        [JsonProperty("numeroFactura")]
        public string InvoiceNumber { get; set; }
        [JsonProperty("idSucursal")]
        public string idSucursal { get; set; }
        [JsonProperty("Cedula")]

        public string Cedula { get; set; }
       
        [JsonProperty("fechaFactura")]
        public DateTime InvoiceDate { get; set; }

        [JsonProperty("codTienda")]
        public string StoreCode { get; set; }
        [JsonProperty("nombreCliente")]
        public string NameClient { get; set; }
        [JsonProperty("apellidoCliente")]

        public string CorreoCliente { get; set; }
        public string SurnameClient { get; set; }
        

        [JsonProperty("telefonoCliente")]

        public string PhoneNumberClient { get; set; }
        [JsonProperty("telefonoCliente2")]
        public string? PhoneNumberClient2 { get; set; }

        [JsonProperty("productosCliente")]
        public List<Article> Articles { get; set; }

    
        [JsonProperty("direcionCliente")]
        public String AddressClient { get; set; }

        public String Observation {  get; set; }
    

    }

    public class Address
    {
        [JsonProperty("paisCliente")]
        public string codeCountry { get; set; }
        [JsonProperty("estadoCliente")]
        public string codeProvince { get; set; }
        [JsonProperty("ciudadCliente")]
        public string codeCity { get; set; }
        [JsonProperty("municipioCliente")]
        public string codeMunicipality { get; set; }
        [JsonProperty("zonaCliente")]
        public string codeZone { get; set; }
    }

    public class MessageContent
    {
        [JsonProperty("mensaje")]
        public string Message { get; set; }
        [JsonProperty("num_dest")]
        public string ClientNumber { get; set; }
        [JsonProperty("prior")]
        public int PriorityNumber { get; set; }
        [JsonProperty("dep")]
        public string Department { get; set; }
        [JsonProperty("id")]
        public string IdCampaing { get; set; }
    }

    public class CheckCouponModel
    {
        public string CouponStore { get; set; }
        public string CouponCode { get; set; }

    }

    public class CouponResultCheck {
        public string CouponCode { get; set; }
        public bool Status { get; set; }
        public DateTime DateUntilCoupon { get; set; }
    }
  
   /* public class Sucursal
    {
        public List<SelectListItem>? Sucursales { get; set; }
    }*/
}
