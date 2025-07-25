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
public class OnboardingController : ControllerBase
{
    private readonly IKafkaService _onboardingService;

    public OnboardingController(OnboardingService onboardingService)
    {
        _onboardingService = onboardingService;
    }

    [HttpPost("produce_onboarding")]
    public async Task<IActionResult> ProduceValidation(string message)
    {
        if (message == null) {
            return BadRequest("No message sent.");
        }
        
        var result = await _onboardingService.ProduceMessage(message);

        return Ok(new { Message = "Message was sent to topic" });
    }

    [HttpGet("consume_onboarding")]
    public async Task<IActionResult> ConsumeValidation()
    {
        var consumedMessage = await _onboardingService.ConsumeMessage();
        return Ok(new { message = consumedMessage });
    }
}
