using PATH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PATH.Domain.Models
{
    public class GetTaskItemResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }

        public DateTime DueDate { get; set; }

        public Guid AssignedToId { get; set; }

        public string AssignedToName { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid ProjectId { get; set; }


        public static GetTaskItemResponse FromEntity(TaskItem taskItem)
        {
            return new GetTaskItemResponse
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                Status = taskItem.Status,
                Priority = taskItem.Priority,
                DueDate = taskItem.DueDate,
                AssignedToId = taskItem.AssignedToId,
                AssignedToName = $"{taskItem.AssignedTo.FirstName} {taskItem.AssignedTo.LastName}",
                CreatedAt = taskItem.CreatedAt,
                ProjectId = taskItem.ProjectId
            };
        }

        public GetTaskItemResponse()
        {

        }

        public GetTaskItemResponse(TaskItem task, string userFullName)
        {
            Id = task.Id;
            Title = task.Title;
            Description = task.Description;
            Status = task.Status;
            Priority = task.Priority;
            DueDate = task.DueDate;
            AssignedToId = task.AssignedToId;
            AssignedToName = userFullName;
            CreatedAt = task.CreatedAt;
            ProjectId = task.ProjectId;
        }
    }
}
