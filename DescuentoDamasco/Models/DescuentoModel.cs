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
        public string CouponId { get; set; }

        public string PercentageDiscount { get; set; }
        public DateTime dateUntilCoupon { get; set; }
        public ClientInfo ClienteInfo { get; set; }
    }

    public class ClientInfo {
        [JsonProperty("numeroFactura")]
        public string InvoiceNumber { get; set; }
        [JsonProperty("fechaFactura")]

        public string Cedula { get; set; }
        [JsonProperty("Cedula")]
        public DateTime InvoiceDate { get; set; }
        [JsonProperty("codTienda")]
        public string StoreCode { get; set; }
        [JsonProperty("nombreCliente")]
        public string NameClient { get; set; }
        [JsonProperty("apellidoCliente")]
        public string SurnameClient { get; set; }

        [JsonProperty("correoCliente")]
        public string EmailClient { get; set; }
        [JsonProperty("telefonoCliente")]
        public string PhoneNumberClient { get; set; }

        [JsonProperty("montoFacturaCliente")]
        public Decimal AmountInvoice { get; set; }
        [JsonProperty("direcionCliente")]
        public Address AddressClient { get; set; }

    }

    public class Address
    {
        [JsonProperty("paisCliente")]
        public int codeContry { get; set; }
        [JsonProperty("estadoCliente")]
        public int codeProvince { get; set; }
        [JsonProperty("ciudadCliente")]
        public int codeCity { get; set; }
        [JsonProperty("municipioCliente")]
        public int codeMunicipality { get; set; }
        [JsonProperty("zonaCliente")]
        public int codeZone { get; set; }
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
        public string Department {  get; set; }
        [JsonProperty("id")]
        public string IdCampaing { get; set; }
    }
  
   /* public class Sucursal
    {
        public List<SelectListItem>? Sucursales { get; set; }
    }*/
}
