using ECommerce.Services.EmailAPI.Data;
using ECommerce.Services.EmailAPI.Dto;
using ECommerce.Services.EmailAPI.Models;
using ECommerce.Services.EmailAPI.Service.IService;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ECommerce.Services.EmailAPI.Service
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder emailBody = new StringBuilder();

            emailBody.AppendLine("<h3>Your Shopping Cart Details</h3>");
            emailBody.AppendLine("<br/>Total " + cartDto.CartHeader.CartTotal);
            emailBody.Append("<br/>");
            emailBody.Append("<ul>");
            foreach (var detail in cartDto.CartDetails)
            {
                emailBody.AppendFormat("<li>{0} - {1} x {2} = {3}</li>", detail.Product.Name, detail.Count, detail.Product.Price, detail.Count * detail.Product.Price);
            }
            emailBody.Append("</ul>");

            await LogAndEmail(emailBody.ToString(), cartDto.CartHeader.Email);
        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            StringBuilder emailBody = new StringBuilder();
            emailBody.AppendFormat("<h3>Welcome to Our E-Commerce Platform {0}!</h3>", email);
            emailBody.AppendLine("<br/>Thank you for registering with us. We're excited to have you on board!");
            await LogAndEmail(emailBody.ToString(), email);
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new EmailLogger()
                {
                    Email = email,
                    EmailSent = DateTime.Now.ToUniversalTime(),
                    Message = message
                };



                await using var db = new AppDbContext(_dbOptions);
                await db.EmailLoggers.AddAsync(emailLog);
                await db.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging email: {ex.Message}");
                return false;
            }
        }
    }
}
