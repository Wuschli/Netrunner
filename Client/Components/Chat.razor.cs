using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Netrunner.Shared.Chat;
using System.Net.Http.Json;

namespace Netrunner.Client.Components
{
    public partial class Chat //: IAsyncDisposable
    {
        private readonly List<ChatMessage> _messages = new();
        private string? _messageInput;
        private ChatRoom? _room;

        [Parameter]
        public string? RoomId { get; set; }

        public bool IsConnected => true;
        //_hubConnection?.State == HubConnectionState.Connected;

        //protected override async Task OnInitializedAsync()
        //{
        //TODO connect to wamp
        //    _hubConnection = new HubConnectionBuilder()
        //        .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"), options => { options.AccessTokenProvider = () => _authHelper.AccessToken; })
        //        .AddMessagePackProtocol()
        //        .Build();

        //    _hubConnection.On<ChatMessage>("ReceiveMessage", (message) =>
        //    {
        //        if (message.RoomId != RoomId)
        //            return;
        //        _messages.Add(message);
        //        StateHasChanged();
        //    });

        //    await _hubConnection.StartAsync();
        //}

        protected override async Task OnParametersSetAsync()
        {
            _messages.Clear();

            if (string.IsNullOrWhiteSpace(RoomId))
                return;
            var messages = await Http.GetFromJsonAsync<IEnumerable<ChatMessage>>($"api/v1/message/{RoomId}");
            if (messages != null)
                _messages.AddRange(messages.Reverse());

            _room = await Http.GetFromJsonAsync<ChatRoom>($"api/v1/rooms/{RoomId}");
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


        private async Task LeaveRoom()
        {
            await Http.PostAsync($"api/v1/rooms/leave/{RoomId}", new StringContent(string.Empty));
            NavigationManager.NavigateTo("chat");
        }

        //public async ValueTask DisposeAsync()
        //{
        //    if (_hubConnection != null)
        //        await _hubConnection.DisposeAsync();
        //}
    }
}