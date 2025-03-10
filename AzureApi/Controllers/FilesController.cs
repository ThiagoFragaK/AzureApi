using AzureApi.Entities;
using AzureApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FilesService _fileService;
        public FilesController(FilesService filesService)
        {
            _fileService = filesService;
        }

        [HttpGet("List")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ListBlobs()
        {
            var result = await _fileService.ListAsync();
            return Ok(result);
        }

        [HttpPost("UploadFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var result = await _fileService.UploadAsync(file);
            return Ok(result);
        }

        [HttpGet("DownloadFiles")]
        public async Task<IActionResult> DownloadByName(string filename)
        {
            var result = await _fileService.DownloadAsync(filename);
            return File(
                    result.Content, 
                    result.ContentType,
                    result.Name
                );
        }

        [HttpDelete("RemoveFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteByName(string filename)
        {
            var result = await _fileService.DeleteAsync(filename);
            return Ok(result);
        }
    }
}
