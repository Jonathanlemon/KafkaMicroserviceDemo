using KafkaTester.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ValidationController : ControllerBase
{
    private readonly IKafkaService _validationService;
    private readonly IKafkaService _onboardingService;

    public ValidationController(ValidationService validationService, OnboardingService onboardingService)
    {
        _validationService = validationService;
        _onboardingService = onboardingService;
    }

    // POST api/validation/produce_validation
    [HttpPost("produce_validation")]
    public async Task<IActionResult> ProduceValidation(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        List<string> messages = new();

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line)) { 
                    messages.Add(line);
                }
            }
        }

        if (messages.Count == 0)
            return BadRequest("Spreadsheet is empty or no readable data found.");

        var produceResults = new List<string>();
        foreach (var msg in messages)
        {
            var result = await _validationService.ProduceMessage(msg);
            produceResults.Add(result);
        }

        return Ok(new { producedCount = messages.Count, results = produceResults });
    }

    // GET api/validation/consume_validation
    [HttpGet("consume_validation")]
    public async Task<IActionResult> ConsumeValidation()
    {
        var consumedMessage = await _validationService.ConsumeMessage();
        if (consumedMessage != null && !consumedMessage.Substring(0,3).Equals("ERR")) {
            await _onboardingService.ProduceMessage(consumedMessage);
        }
        return Ok(new { message = consumedMessage });
    }
}
