using FinanceTracker.Api.Contracts.Requests;
using FinanceTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using FinanceTracker.Api.Services.Mapping;
using Microsoft.AspNetCore.Authorization;
using FinanceTracker.Api.Extensions;
using FinanceTracker.Api.Contracts.Response;

namespace FinanceTracker.Api.Controllers;

[ApiController]
[EnableRateLimiting("sliding")]
[Route("api/transactions")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _service;

    public TransactionController(ITransactionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<TransactionResponse>>> Get([FromQuery] GetItemsRequest request)
    {
        var userId = User.GetUserId();

        var (transactions, totalCount) = await _service.GetUserTransactionsAsync(userId, request);

        var responseItems = transactions.Select(TransactionMapper.ToResponse);
        
        var result = new PagedResponse<TransactionResponse>(
            responseItems,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        return Ok(result);
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var userId = User.GetUserId();
        
        var balance = await _service.GetBalanceAsync(userId);
        
        return Ok(new { Balance = balance });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
    {
        var userId = User.GetUserId();

        await _service.AddTransactionAsync(
            userId,
            request.Amount,
            request.Category,
            request.Type!.Value
        );

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();

        await _service.DeleteTransactionAsync(id, userId);

        return NoContent();
    }
}