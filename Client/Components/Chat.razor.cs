using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Netrunner.Shared.Chat;
using Netrunner.Shared.Services;

namespace Netrunner.Client.Components
{
    public partial class Chat //: IAsyncDisposable
    {
        private List<ChatMessage> _messages = new();
        private string? _messageInput;
        private ChatRoom? _room;
        private Guid? _subscription;

        [Parameter]
        public string? RoomId { get; set; }

        public bool IsConnected => true;

        protected override async Task OnInitializedAsync()
        {
            if (_subscription != null)
                _serviceHelper.Unsubscribe(_subscription.Value);

            _subscription = await _serviceHelper.Subscribe<ChatMessage>($"netrunner.chat.{RoomId}.messages", message =>
            {
                if (message.RoomId != RoomId)
                    return;
                _messages.Add(message);
                StateHasChanged();
            });
        }

        protected override async Task OnParametersSetAsync()
        {
            _messages.Clear();

            if (string.IsNullOrWhiteSpace(RoomId))
                return;
            Console.WriteLine($"Retrieving messages for room {RoomId}");
            var chatService = await _serviceHelper.GetService<IChatService>();
            var messages = await chatService.GetMessages(RoomId).ConfigureAwait(false);
            if (messages != null)
            {
                messages.Reverse();
                _messages = messages;
            }

            Console.WriteLine($"Got {messages.Count} messages for room {RoomId}");

            _room = await chatService.GetRoomDetails(RoomId);
        }

        private async Task Send()
        {
            var chatService = await _serviceHelper.GetService<IChatService>();
            await chatService.SendMessage(new ChatMessage
            {
                Message = _messageInput,
                RoomId = RoomId
            });
            _messageInput = string.Empty;
        }

        private async Task LeaveRoom()
        {
            var chatService = await _serviceHelper.GetService<IChatService>();
            await chatService.LeaveRoom(RoomId);
            NavigationManager.NavigateTo("chat");
        }
    }
}