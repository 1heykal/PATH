using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Infrastructure
{
    public class NotificationService : INotificationService
    {

        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendTaskAssignedNotification(Guid userId, string taskTitle, string assignedBy)
        {
            await _hubContext.Clients.Group(userId.ToString())
                .SendAsync("TaskAssigned", new { taskTitle, assignedBy });
        }
    }
}
