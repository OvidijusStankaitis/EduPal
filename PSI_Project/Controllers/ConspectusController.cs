using Microsoft.AspNetCore.Mvc;
using PSI_Project.DTO;
using PSI_Project.Repositories;
using System.Threading.Tasks;
using PSI_Project.Models;

namespace PSI_Project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConspectusController : ControllerBase
    {
        private readonly ConspectusRepository _conspectusRepository;

        public ConspectusController(ConspectusRepository conspectusRepository)
        {
            _conspectusRepository = conspectusRepository;
        }

        [HttpGet("get/{conspectusId}")]
        public IActionResult GetConspectus(string conspectusId)
        {
            Stream? pdfStream = _conspectusRepository.GetPdfStream(conspectusId);
            return pdfStream != null
                ? File(pdfStream, "application/pdf")
                : NotFound(new { error = "File not found." });
        }

        [HttpGet("list/{topicId}")]
        public async Task<IActionResult> ListConspectuses(string topicId)
        {
            return Ok(await _conspectusRepository.GetConspectusListByTopicIdAsync(topicId));
        }

        [HttpPost("upload/{topicId}")]
        public async Task<IActionResult> UploadFiles(string topicId, List<IFormFile> files)
        {
            return Ok(await _conspectusRepository.UploadAsync(topicId, files));
        }

        [HttpGet("download/{conspectusId}")]
        public IActionResult DownloadFile(string conspectusId)
        {
            ConspectusFileContentDTO? response = _conspectusRepository.Download(conspectusId);
            if (response == null)
                return NotFound();

            Response.Headers.Add("Content-Disposition", "attachment; filename=" + response.Name);
            return response.FileContent;
        }

        [HttpDelete("{conspectusId}/delete")]
        public async Task<IActionResult> DeleteFile(string conspectusId)
        {
            return (await _conspectusRepository.RemoveAsync(conspectusId))
                ? Ok("File has been successfully deleted")
                : BadRequest("An error occured while deleting file");
        }

        [HttpPost("rateUp/{conspectusId}")]
        public async Task<IActionResult> RateConspectusUp(string conspectusId)
        {
            bool success = await _conspectusRepository.ChangeRatingAsync(conspectusId, true);
            if (!success)
                return NotFound(new { error = "File not found in database." });

            Conspectus? conspectus = await _conspectusRepository.GetAsync(conspectusId);
            return Ok(conspectus);
        }

        [HttpPost("rateDown/{conspectusId}")]
        public async Task<IActionResult> RateConspectusDown(string conspectusId)
        {
            bool success = await _conspectusRepository.ChangeRatingAsync(conspectusId, false);
            if (!success)
                return NotFound(new { error = "File not found in database." });

            Conspectus? conspectus = await _conspectusRepository.GetAsync(conspectusId);
            return Ok(conspectus);
        }
    }
}
