using System.Security.Claims;
using GoldenGems.Application.Interfaces.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GoldenGems.API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    private Guid GetUserId() =>
        Guid.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        await base.OnConnectedAsync();
    }

    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");
    }

    public async Task SendMessage(string conversationId, string content)
    {
        var userId = GetUserId();
        var convId = Guid.Parse(conversationId);
        var result = await _chatService.SendMessageAsync(convId, userId, content, default);

        if (result.Success && result.Data != null)
        {
            await Clients.Group($"conversation-{conversationId}")
                .SendAsync("ReceiveMessage", result.Data);
        }
    }

    public async Task SendPriceOffer(string conversationId, decimal price)
    {
        var userId = GetUserId();
        var convId = Guid.Parse(conversationId);
        var result = await _chatService.OfferPriceAsync(convId, userId, price, default);

        if (result.Success && result.Data != null)
        {
            await Clients.Group($"conversation-{conversationId}")
                .SendAsync("ReceiveMessage", result.Data);
        }
    }

    public async Task AcceptPrice(string conversationId)
    {
        var userId = GetUserId();
        var convId = Guid.Parse(conversationId);
        var result = await _chatService.AcceptPriceAsync(convId, userId, default);

        if (result.Success && result.Data != null)
        {
            await Clients.Group($"conversation-{conversationId}")
                .SendAsync("PriceAccepted", result.Data.AgreedPrice);
        }
    }

    public async Task RejectPrice(string conversationId)
    {
        var userId = GetUserId();
        var convId = Guid.Parse(conversationId);
        var result = await _chatService.RejectPriceAsync(convId, userId, default);

        if (result.Success)
        {
            await Clients.Group($"conversation-{conversationId}")
                .SendAsync("PriceRejected");
        }
    }

    public async Task Typing(string conversationId)
    {
        var userId = GetUserId();
        await Clients.OthersInGroup($"conversation-{conversationId}")
            .SendAsync("UserTyping", userId.ToString());
    }
}
