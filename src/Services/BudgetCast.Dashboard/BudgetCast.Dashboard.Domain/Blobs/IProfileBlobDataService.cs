using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Domain.Blobs
{
    public interface IProfileBlobDataService : IBlobDataService
    {
        Task<string> UploadImage(string name, byte[] content);
    }
}