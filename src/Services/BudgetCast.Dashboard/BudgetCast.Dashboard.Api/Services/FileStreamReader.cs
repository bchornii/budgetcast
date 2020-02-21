using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Api.Infrastructure.Files;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace BudgetCast.Dashboard.Api.Services
{
    public class FileStreamReader : IFileStreamReader
    {
        private readonly string[] _permittedExtensions;
        private readonly long _sizeLimit;

        private static readonly FormOptions DefaultFormOptions = new FormOptions();

        public FileStreamReader(string[] permittedExtensions, long sizeLimit)
        {
            _permittedExtensions = permittedExtensions;
            _sizeLimit = sizeLimit;
        }

        public async Task<FileStreamReadResult> Read(Stream stream, string contentType)
        {
            if (!MultipartRequestHelper.IsMultipartContentType(contentType))
            {
                throw new ArgumentException($"Expected a multipart request, but got {contentType}");
            }
            
            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(contentType),
                DefaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, stream);
            var section = await reader.ReadNextSectionAsync();
            return section != null ? await UploadFile(section) : null;
        }

        private async Task<FileStreamReadResult> UploadFile(MultipartSection section)
        {
            var hasContentDispositionHeader = ContentDispositionHeaderValue
                .TryParse(section.ContentDisposition, out var contentDisposition);

            if (hasContentDispositionHeader)
            {
                if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                {
                    var fileName = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                    var (errMsg, content) = await FileHelpers.ProcessStreamedFile(
                        section, contentDisposition, _permittedExtensions, _sizeLimit);

                    return new FileStreamReadResult(
                        string.IsNullOrEmpty(errMsg), errMsg, fileName, content);
                }
            }
            return null;
        }
    }
}
