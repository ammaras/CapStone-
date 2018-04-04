using System.Collections.Generic;
using TaskLog2ndGen.Models;

namespace TaskLog2ndGen.ViewModels
{
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