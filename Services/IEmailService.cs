using System.Threading.Tasks;
using TKGroopBG.Models;

namespace TKGroopBG.Services
{
    public interface IEmailService
    {
        // Методът трябва да съвпада точно с този, който викаш в контролера
        Task SendOrderEmailAsync(OrderRequest order);
    }
}
