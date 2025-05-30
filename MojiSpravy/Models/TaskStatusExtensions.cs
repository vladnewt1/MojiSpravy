using System;

namespace MojiSpravy.Models
{
    public static class TaskStatusExtensions
    {
        public static string ToUkrainian(this TaskStatus status)
        {
            return status switch
            {
                TaskStatus.NotStarted => "Не розпочато",
                TaskStatus.InProgress => "В процесі",
                TaskStatus.Completed => "Виконано",
                TaskStatus.Cancelled => "Скасовано",
                _ => status.ToString()
            };
        }
    }
} 