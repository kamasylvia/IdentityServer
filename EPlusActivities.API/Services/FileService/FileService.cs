using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.FileDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FileService> _logger;
        private readonly IConfiguration _configuration;

        public FileService(
            IHttpClientFactory httpClientFactory,
            ILogger<FileService> logger,
            IConfiguration configuration
        ) {
            _httpClientFactory =
                httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<FileStream> DownloadFileByKeyAsync(
            DownloadFileByKeyRequestDto downloadPhotoDto
        ) {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file/key"
            );

            var requestUrl = QueryHelpers.AddQueryString(
                uriBuilder.Uri.ToString(),
                new Dictionary<string, string>
                {
                    ["OwnerId"] = downloadPhotoDto.OwnerId.ToString(),
                    ["Key"] = downloadPhotoDto.Key
                }
            );

            return await _httpClientFactory.CreateClient().GetStreamAsync(requestUrl) as FileStream;
        }

        public async Task<string> GetContentTypeByKeyAsync(
            DownloadFileByKeyRequestDto downloadFileDto
        ) {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file/content-type/key"
            );

            var requestUrl = QueryHelpers.AddQueryString(
                uriBuilder.Uri.ToString(),
                new Dictionary<string, string>
                {
                    ["FileId"] = downloadFileDto.OwnerId.ToString(),
                    ["Key"] = downloadFileDto.Key
                }
            );

            return await _httpClientFactory.CreateClient().GetStringAsync(requestUrl);
        }

        public async Task<FileStream> DownloadFileByIdAsync(
            DownloadFileByIdRequestDto downloadPhotoDto
        ) {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file/id"
            );

            var requestUrl = QueryHelpers.AddQueryString(
                uriBuilder.Uri.ToString(),
                new Dictionary<string, string> { ["FileId"] = downloadPhotoDto.FileId.ToString() }
            );

            return await _httpClientFactory.CreateClient().GetStreamAsync(requestUrl) as FileStream;
        }

        public async Task<string> GetContentTypeByIdAsync(
            DownloadFileByIdRequestDto downloadPhotoDto
        ) {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file/content-type/id"
            );

            var requestUrl = QueryHelpers.AddQueryString(
                uriBuilder.Uri.ToString(),
                new Dictionary<string, string> { ["FileId"] = downloadPhotoDto.FileId.ToString() }
            );

            return await _httpClientFactory.CreateClient().GetStringAsync(requestUrl);
        }

        public async Task<HttpResponseMessage> UploadFileAsync(UploadFileRequestDto uploadFileDto)
        {
            var uriBuilder = new UriBuilder(
                scheme: _configuration["FileServiceUriBuilder:Scheme"],
                host: _configuration["FileServiceUriBuilder:Host"],
                port: Convert.ToInt32(_configuration["FileServiceUriBuilder:Port"]),
                pathValue: "api/file"
            );

            var httpClient = _httpClientFactory.CreateClient();
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(uploadFileDto.OwnerId.ToString()), "ownerId");
            formData.Add(new StringContent(uploadFileDto.Key), "key");
            var streamContent = new StreamContent(uploadFileDto.FormFile.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                uploadFileDto.FormFile.ContentType
            );
            formData.Add(streamContent, "formFile", uploadFileDto.FormFile.FileName);
            var response = await httpClient.PostAsync(uriBuilder.Uri, formData);
            // System.Console.WriteLine(response);
            return response;
        }
    }
}
