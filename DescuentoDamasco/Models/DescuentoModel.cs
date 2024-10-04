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
        public int CouponId { get; set; }

        public string AmountInvoice { get; set; }
        public string AmountTotal { get; set; }
        public string AmountCoupon { get; set; }
        public string PercentageDiscount { get; set; }

        public ClientInfo ClienteInfo { get; set; }
    }

    public class ClientInfo {
        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }
        public string StoreCode { get; set; }
        public string NameClient { get; set; }
        public string SurnameClient { get; set; }
        public string EmailClient { get; set; }
        public string PhoneNumberClient { get; set; }

        public Address AddressClient { get; set; }

    }

    public class Address
    {

        public int codeContry { get; set; }

        public int codeProvince { get; set; }

        public int codeCity { get; set; }

        public int codeMunicipality { get; set; }
        public int codeZone { get; set; }
    }

    public class MessageContent
    {
        [JsonProperty("mensaje")]
        public string Message { get; set; }
        [JsonProperty("num_dest")]
        public string ClientNumber { get; set; }
        [JsonProperty("prior")]
        public string PriorityNumber { get; set; }
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
