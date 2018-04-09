using System.Collections.Generic;
using TaskLog2ndGen.Models;

namespace TaskLog2ndGen.ViewModels
{
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