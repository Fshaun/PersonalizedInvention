using Microsoft.AspNetCore.Mvc;

namespace PersonalizedInvention.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService) => _paymentService = paymentService;

        // POST /api/payments/create-intent
        [HttpPost("create-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentDto dto)
        {
            var result = await _paymentService.CreatePaymentIntentAsync(dto);
            return Ok(result);
        }

        // POST /api/payments/confirm
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentDto dto)
        {
            var success = await _paymentService.ConfirmPaymentAsync(dto.PaymentIntentId, dto.OrderId);
            return success ? Ok(new { message = "Payment confirmed." }) : BadRequest();
        }
    }

    public class ConfirmPaymentDto
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public int OrderId { get; set; }
    }
}
