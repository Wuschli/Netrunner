using System.Security.Claims;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Netrunner.Server.Helpers;

namespace Netrunner.Server.Services;

public interface IMqttAuthService
{
    void Configure(MqttServer server);
}

public class MqttAuthService : IMqttAuthService
{
    private readonly ITokenValidationService _tokenValidation;
    private readonly IUsersService _users;

    public MqttAuthService(ITokenValidationService tokenValidation, IUsersService users)
    {
        _tokenValidation = tokenValidation;
        _users = users;
    }

    public void Configure(MqttServer server)
    {
        server.ClientConnectedAsync += ClientConnectedAsync;
        server.ValidatingConnectionAsync += ValidatingConnectionAsync;
        server.InterceptingSubscriptionAsync += InterceptingSubscriptionAsync;
    }

    private async Task ClientConnectedAsync(ClientConnectedEventArgs e)
    {
    }

    private async Task ValidatingConnectionAsync(ValidatingConnectionEventArgs e)
    {
        var user = await _tokenValidation.ValidateAsync(e.Password);
        if (user == null)
            e.ReasonCode = MqttConnectReasonCode.NotAuthorized;
        e.SessionItems["user"] = user;
    }

    private async Task InterceptingSubscriptionAsync(InterceptingSubscriptionEventArgs e)
    {
        if (!e.SessionItems.Contains("user"))
        {
            e.Response.ReasonCode = MqttSubscribeReasonCode.NotAuthorized;
            return;
        }

        if (e.SessionItems["user"] is ClaimsPrincipal claims)
        {
            if (claims.IsAdmin())
                return;

            var user = await _users.GetOrCreateUser(claims.GetId());
        }
    }
}