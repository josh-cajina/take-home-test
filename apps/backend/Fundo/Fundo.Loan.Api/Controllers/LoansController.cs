using Asp.Versioning;
using Fundo.Loan.Application.Loans.AddPayment;
using Fundo.Loan.Application.Loans.CreateLoan;
using Fundo.Loan.Application.Loans.Dtos;
using Fundo.Loan.Application.Loans.GetAllLoans;
using Fundo.Loan.Application.Loans.GetLoanById;
using Fundo.Loan.Application.Loans.UpdateLoanStatus;
using Fundo.Loan.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Loan.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/loans")]
[Authorize] // All loan endpoints require authentication
public class LoansController : ControllerBase
{
    private readonly ISender _mediator;

    public LoansController(ISender mediator)
    {
        _mediator = mediator;
    }

    // POST /api/v1/loans
    [HttpPost]
    [Authorize(Roles = Roles.Requester)]
    [ProducesResponseType(typeof(Guid), 201)]
    public async Task<IActionResult> CreateLoan(CreateLoanCommand command)
    {
        Guid loanId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetLoanById), new { id = loanId, version = "1.0" }, loanId);
    }

    // GET /api/v1/loans
    [HttpGet]
    [ProducesResponseType(typeof(List<LoanBriefDto>), 200)]
    public async Task<IActionResult> GetAllLoans()
    {
        List<LoanBriefDto> loans = await _mediator.Send(new GetAllLoansQuery());
        return Ok(loans);
    }

    // GET /api/v1/loans/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LoanDetailsDto), 200)]
    public async Task<IActionResult> GetLoanById(Guid id)
    {
        LoanDetailsDto loan = await _mediator.Send(new GetLoanByIdQuery { Id = id });
        return Ok(loan);
    }

    // PUT /api/v1/loans/{id}/status
    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = Roles.Analyst)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> UpdateLoanStatus(Guid id, UpdateLoanStatusCommand command)
    {
        if (id != command.LoanId)
        {
            return BadRequest("Loan ID mismatch.");
        }
        await _mediator.Send(command);
        return NoContent();
    }

    // POST /api/v1/loans/{id}/payments
    [HttpPost("{id:guid}/payments")]
    [Authorize(Roles = $"{Roles.Analyst},{Roles.Admin}")] // Only Analysts or Admins can log payments
    [ProducesResponseType(typeof(Guid), 201)]
    public async Task<IActionResult> AddPayment(Guid id, AddPaymentCommand command)
    {
        if (id != command.LoanId)
        {
            return BadRequest("Loan ID mismatch.");
        }
        Guid paymentId = await _mediator.Send(command);
        return Ok(paymentId);
    }
}
