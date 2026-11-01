using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dboptions;

        public EmailService(DbContextOptions<AppDbContext> dboptions)
        {
            _dboptions = dboptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
           //Create Email body to send here
           StringBuilder message =new StringBuilder();
            message.AppendLine("<br/>Cart Email Requested");
            message.AppendLine("<br/> Total :" + cartDto.CartHeader.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");
            foreach (var item in cartDto.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.product.Name + "x" + item.Count);
                message.Append("</li>");
                
            }
            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }
        private async Task<bool> LogAndEmail(string messge, string Email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email= Email,
                    EmailSent=DateTime.Now,
                    Message=messge
                };

                await using var _db = new AppDbContext(_dboptions);
                await _db.EmailLoggers.AddAsync(emailLog);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        
        }
    }
}
