using Microsoft.AspNetCore.Mvc.Rendering;

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
    }

    public class ClientInfo {
        public string InvoiceNumber { get; set; }
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

   /* public class Sucursal
    {
        public List<SelectListItem>? Sucursales { get; set; }
    }*/
}
