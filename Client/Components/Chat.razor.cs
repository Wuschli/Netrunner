using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Netrunner.Shared.Chat;
using System.Net.Http.Json;

namespace Netrunner.Client.Components
{
    public partial class Chat : IAsyncDisposable
    {
        private HubConnection _hubConnection;
        private readonly List<ChatMessage> _messages = new();
        private string _messageInput;

        [Parameter]
        public string RoomId { get; set; }

        public bool IsConnected =>
            _hubConnection.State == HubConnectionState.Connected;

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"), options => { options.AccessTokenProvider = () => AuthService.AccessToken; })
                .AddMessagePackProtocol()
                .Build();

            _hubConnection.On<ChatMessage>("ReceiveMessage", (message) =>
            {
                if (message.RoomId != RoomId)
                    return;
                _messages.Add(message);
                StateHasChanged();
            });

            await _hubConnection.StartAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            _messages.Clear();

            if (string.IsNullOrWhiteSpace(RoomId))
                return;
            var messages = await Http.GetFromJsonAsync<IEnumerable<ChatMessage>>($"api/v1/message/{RoomId}");
            if (messages != null)
                _messages.AddRange(messages.Reverse());
        }

        private async Task Send()
        {
            await Http.PostAsJsonAsync("api/v1/message", new ChatMessage
            {
                Message = _messageInput,
                RoomId = RoomId
            });
            _messageInput = string.Empty;
        }


        public async ValueTask DisposeAsync()
        {
            await _hubConnection.DisposeAsync();
        }
    }
}