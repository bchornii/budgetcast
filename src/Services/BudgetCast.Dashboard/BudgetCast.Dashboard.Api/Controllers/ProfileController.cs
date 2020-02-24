using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Api.Infrastructure.Files;
using BudgetCast.Dashboard.Api.Infrastructure.Filters;
using BudgetCast.Dashboard.Api.Models;
using BudgetCast.Dashboard.Api.Services;
using BudgetCast.Dashboard.Api.ViewModels.Account;
using BudgetCast.Dashboard.Compensations;
using BudgetCast.Dashboard.Domain.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static BudgetCast.Dashboard.Compensations.ExecSteps.ProfileImage.Delete;
using static BudgetCast.Dashboard.Compensations.ExecSteps.ProfileImage.Upload;

namespace BudgetCast.Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly SignInManager<AppIdentityUser> _signInManager;
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly IFileStreamReader _fileStreamReader;
        private readonly IProfileBlobDataService _profileBlobDataService;
        private readonly IExecutionHistoryStore _historyStore;

        public ProfileController(
            SignInManager<AppIdentityUser> signInManager, 
            UserManager<AppIdentityUser> userManager, 
            IFileStreamReader fileStreamReader,
            IProfileBlobDataService profileBlobDataService,
            IExecutionHistoryStore historyStore)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileStreamReader = fileStreamReader;
            _profileBlobDataService = profileBlobDataService;
            _historyStore = historyStore;
        }

        //[Authorize]
        [HttpPost("uploadImg", Name = "UploadImage")]
        [ServiceFilter(typeof(CompensationFilter))]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> Upload()
        {
            var readResult = await _fileStreamReader
                .Read(Request.Body, Request.ContentType);

            if (readResult == null)
            {
                return BadRequest("Please select file to upload.");
            }

            if (!readResult.Success)
            {
                return BadRequest(readResult.Error);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var location = await _profileBlobDataService
                    .UploadImage(readResult.FileName, readResult.Content);

                if (!string.IsNullOrWhiteSpace(location))
                {
                    _historyStore.Add(BlobUploaded, location);

                    var previousImageLink = user.ProfileImageLink;
                    user.ProfileImageLink = location;
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        _historyStore.Add(DbRecordAdded);
                        if (!string.IsNullOrWhiteSpace(previousImageLink))
                        {
                            await _profileBlobDataService.Delete(previousImageLink);
                        }
                    }
                    else
                    {
                        await _profileBlobDataService.Delete(location);
                    }

                    return Ok();
                }
            }

            return BadRequest("Bad request data.");
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            await RemoveClaimsAsync(user,
                ClaimTypes.GivenName, ClaimTypes.Surname);

            await _userManager.AddClaimsAsync(user, new[]
            {
                new Claim(ClaimTypes.GivenName, model.GivenName),
                new Claim(ClaimTypes.Surname, model.SurName)
            });

            await _signInManager.RefreshSignInAsync(user);

            return Ok();
        }

        private async Task RemoveClaimsAsync(AppIdentityUser user, params string[] claimType)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var claimsToRemove = claims.Where(c => claimType.Contains(c.Type));
            await _userManager.RemoveClaimsAsync(user, claimsToRemove);
        }
    }
}