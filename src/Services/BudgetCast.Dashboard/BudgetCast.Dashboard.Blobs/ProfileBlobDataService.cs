using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.Blobs;

namespace BudgetCast.Dashboard.Blobs
{
    public class ProfileBlobDataService : BlobDataService, IProfileBlobDataService
    {
        public ProfileBlobDataService(string connectionString, string containerName) : 
            base(connectionString, containerName)
        {
        }

        public Task<string> UploadImage(string name, byte[] content)
        {
            var fileName = $"images/{name}";
            return Upload(fileName, content);
        }
    }
}