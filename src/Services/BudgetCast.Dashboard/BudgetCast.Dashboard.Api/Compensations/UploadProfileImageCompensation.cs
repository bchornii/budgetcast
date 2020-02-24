using System.Threading.Tasks;
using BudgetCast.Dashboard.Compensations;
using BudgetCast.Dashboard.Domain.Blobs;

namespace BudgetCast.Dashboard.Api.Compensations
{
    /// <summary>
    /// This compensation removes newly uploaded profile image in
    /// case during updating user profile database record error was thrown.
    /// </summary>
    [CompensationAction(Name = "UploadImage")]
    public class UploadProfileImageCompensation : ICompensationAction
    {
        private readonly IExecutionHistoryStore _historyStore;
        private readonly IProfileBlobDataService _profileBlobDataService;

        public UploadProfileImageCompensation(
            IExecutionHistoryStore historyStore,
            IProfileBlobDataService profileBlobDataService)
        {
            _historyStore = historyStore;
            _profileBlobDataService = profileBlobDataService;
        }

        public async Task Compensate()
        {
            var isDbUpdated = _historyStore.Exists(ExecSteps.ProfileImage.Upload.DbRecordAdded);
            var location = _historyStore.Get<string>(ExecSteps.ProfileImage.Upload.BlobUploaded);

            if (!isDbUpdated && !string.IsNullOrWhiteSpace(location))
            {
                await _profileBlobDataService.Delete(location);
            }
        }
    }
}
