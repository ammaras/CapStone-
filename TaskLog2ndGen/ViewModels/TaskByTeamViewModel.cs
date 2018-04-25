using System.Collections.Generic;
using TaskLog2ndGen.Models;

namespace TaskLog2ndGen.ViewModels
{
    /// <summary>
    /// Viewmodel class for grouping tasks by team
    /// </summary>
    public class TaskByTeamViewModel
    {
        public Team team;
        public List<Task> tasks;

        public TaskByTeamViewModel()
        {
            tasks = new List<Task>();
        }
    }
}