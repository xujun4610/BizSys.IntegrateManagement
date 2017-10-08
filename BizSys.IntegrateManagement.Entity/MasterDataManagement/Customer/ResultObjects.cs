using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.MasterDataManagement.Customer
{
    public class ResultObjects
    {
        public string type { get; set; }
        public string isDeleted { get; set; }
        public string isDirty { get; set; }
        public string isNew { get; set; }
        public string Account { get; set; }
        public string AccountBalance { get; set; }
        public string AccountPeriod { get; set; }
        public string Activation { get; set; }
        public string ActiveFrom { get; set; }
        public string ActiveTo { get; set; }
        public string AlertThreshold { get; set; }
        public string ApprovalStatus { get; set; }
        public string BillingAddress { get; set; }
        public string BillingTelephone { get; set; }
        public string BillToStreet { get; set; }
        public string BillToZipCode { get; set; }
        public string BPCurrency { get; set; }
        public string BusinessPartnerNature { get; set; }
        public string Channel { get; set; } //曼恩-渠道类型 ，存放销售员
        public string City { get; set; }
        public string Consignee { get; set; }
        public string ContactPerson { get; set; }
        public string County { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string CreditScore { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCreditManagement { get; set; }
        public List<CustomerItems> CustomerItems { get; set; }
        public string CustomerLevel { get; set; }
        public string CustomerName { get; set; }
        public string DataOwner { get; set; }
        public string Deleted { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string FaxNumber { get; set; }
        public string FederalTaxID { get; set; }
        public string GroupCode { get; set; }
        public string HouseBank { get; set; }
        public string Inactive { get; set; }
        public string InactiveFrom { get; set; }
        public string InactiveTo { get; set; }
        public string InvoiceRecipient { get; set; }
        public string LogInst { get; set; }
        public string MobilePhone { get; set; }
        public string ObjectCode { get; set; }
        public string ObjectKey { get; set; }
        public string Organization { get; set; }
        public string OrganizationId { get; set; }
        public double PaidToCredit { get; set; }
        public string PaymentMethod { get; set; }
        public string Postcodes { get; set; }
        public string PreCharge { get; set; }
        public int PriceListNumber { get; set; }
        public string Province { get; set; }
        public string Referenced { get; set; }
        public string Remarks { get; set; }
        public int Series { get; set; }
        public string TaxNumber { get; set; }
        public string TaxRate { get; set; }
        public string TeamMembers { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Town { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string Website { get; set; }
    }
}
