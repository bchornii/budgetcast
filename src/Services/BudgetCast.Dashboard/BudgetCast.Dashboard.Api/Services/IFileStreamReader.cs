using System.IO;
using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Api.Services
{
    public interface IFileStreamReader
    {
        Task<FileStreamReadResult> Read(Stream stream, string contentType);
    }
}