using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.Blobs;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace BudgetCast.Dashboard.Blobs
{
    public abstract class BlobDataService : IBlobDataService
    {
        private readonly CloudBlobContainer _cloudBlobContainer;

        protected BlobDataService(string connectionString, string containerName)
        {
            _cloudBlobContainer = CloudStorageAccount.Parse(connectionString)
                .CreateCloudBlobClient().GetContainerReference(containerName);
        }

        public async Task<bool> Exists(string name)
        {
            return await _cloudBlobContainer
                .GetBlockBlobReference(name).ExistsAsync();
        }

        public async Task<string> Upload(string name, byte[] content)
        {
            var blob = _cloudBlobContainer.GetBlockBlobReference(name);

            if (!await blob.ExistsAsync())
            {
                blob.Properties.ContentType = MimeTypes.GetMimeType(name);
                await blob.UploadFromByteArrayAsync(content, 0, content.Length);
            }

            return blob.Uri?.PathAndQuery;
        }

        public async Task Delete(string name)
        {
            await _cloudBlobContainer
                .GetBlockBlobReference(name).DeleteIfExistsAsync();
        }

        public async Task Undelete(string name)
        {
            await _cloudBlobContainer
                .GetBlockBlobReference(name).UndeleteAsync();
        }
    }
}