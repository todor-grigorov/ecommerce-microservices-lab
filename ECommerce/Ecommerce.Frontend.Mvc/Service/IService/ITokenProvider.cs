namespace ECommerce.Frontend.Mvc.Service.IService
{
    public interface ITokenProvider
    {
        void SetToken(string token);
        string? GetToken();
        void ClearToken();
    }
}
