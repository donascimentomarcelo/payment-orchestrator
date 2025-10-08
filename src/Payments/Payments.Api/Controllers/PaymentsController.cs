using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.DTOs;
using Payments.Application.Abstractions;
using Payments.Application.UseCases.CapturePayment;
using Payments.Application.UseCases.CreatePayment;
using Payments.Application.UseCases.RefundPayment;

namespace Payments.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PaymentsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromServices] ICreatePaymentUseCase useCase,
            [FromBody] CreatePaymentDto dto,
            CancellationToken cancellationToken
        )
        {
            var payment = await useCase.ExecuteAsync(
                new CreatePaymentInput(dto.Amount, dto.Currency),
                cancellationToken
            );
            return CreatedAtAction(
                nameof(GetById),
                new { id = payment.Id },
                new
                {
                    payment.Id,
                    payment.Amount,
                    payment.Currency,
                    payment.CreatedAt,
                }
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            [FromServices] IPaymentRepository repo,
            Guid id,
            CancellationToken ct
        )
        {
            var payment = await repo.GetByIdAsync(id, ct);
            return payment is null
                ? NotFound()
                : Ok(
                    new
                    {
                        payment.Id,
                        payment.Amount,
                        payment.Currency,
                        payment.CreatedAt,
                        payment.Status,
                    }
                );
        }

        [HttpPost("{id}/capture")]
        public async Task<IActionResult> Capture(
            Guid id,
            [FromServices] ICapturePaymentUseCase useCase,
            CancellationToken ct
        )
        {
            await useCase.ExecuteAsync(id, ct);
            return Accepted(new { paymentId = id, action = "capture-requested" });
        }

        [HttpPost("{id}/refund")]
        public async Task<IActionResult> Refund(
            Guid id,
            [FromServices] IRefundPaymentUseCase useCase,
            CancellationToken ct
        )
        {
            await useCase.ExecuteAsync(id, ct);
            return Accepted(new { paymentId = id, action = "refund-requested" });
        }
    }
}
