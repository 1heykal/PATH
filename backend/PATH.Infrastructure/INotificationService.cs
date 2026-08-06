using System;
using System.Collections.Generic;
using System.Text;

namespace PATH.Infrastructure
{
    public interface INotificationService
    {
        Task SendTaskAssignedNotification(Guid userId, string taskTitle, string assignedBy);
    }
}
