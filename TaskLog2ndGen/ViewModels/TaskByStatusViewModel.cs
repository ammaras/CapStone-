using System.Collections.Generic;
using TaskLog2ndGen.Models;

namespace TaskLog2ndGen.ViewModels
{
    /// <summary>
    /// Viewmodel class for grouping tasks by status
    /// </summary>
    public class TaskByStatusViewModel
    {
        public TaskStatu taskStatus;
        public List<Task> tasks;

        public TaskByStatusViewModel()
        {
            tasks = new List<Task>();
        }
    }
}