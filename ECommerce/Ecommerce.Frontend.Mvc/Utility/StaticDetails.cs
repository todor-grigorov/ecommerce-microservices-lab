namespace Ecommerce.Frontend.Mvc.Utility
{
    public class StaticDetails
    {
        public static string CouponApiBase { get; set; }
        public static string IdentityApiBase { get; set; }
        public static string ProductApiBase { get; set; }
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "JWTToken";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
