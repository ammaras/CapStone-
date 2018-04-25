using System.Collections.Generic;
using TaskLog2ndGen.Models;

namespace TaskLog2ndGen.ViewModels
{
    /// <summary>
    /// Viewmodel class for generating employees time report
    /// </summary>
    public class EmployeesTimeViewModel
    {
        public Employee employee;
        public List<EmployeeTimeViewModel> employeeTimes;

        public EmployeesTimeViewModel()
        {
            employeeTimes = new List<EmployeeTimeViewModel>();
        }
    }
}