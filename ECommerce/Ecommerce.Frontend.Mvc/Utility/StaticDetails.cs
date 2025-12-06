namespace Ecommerce.Frontend.Mvc.Utility
{
    public class StaticDetails
    {
        public static string CouponApiBase { get; set; }
        public static string IdentityApiBase { get; set; }
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
