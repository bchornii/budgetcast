using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Domain.Blobs
{
    public interface IBlobDataService
    {
        Task<bool> Exists(string name);
        Task<string> Upload(string name, byte[] content);
        Task Delete(string name);
        Task Undelete(string name);
    }
}
