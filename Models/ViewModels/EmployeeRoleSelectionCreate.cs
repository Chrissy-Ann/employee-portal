using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LabAssignment6.Models.ViewModels
{
    public class EmployeeRoleSelectionCreate
    {
        [Required(ErrorMessage = "Employee name is required.")]
        [RegularExpression(@"^\s*\w+\s+\w+(\s+\w+)*\s*$", ErrorMessage = "Please enter a first name followed by a last name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Network ID is required.")]
        [MinLength(3, ErrorMessage = "Network ID must be at least 3 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters.")]
        public string Password { get; set; }
        public List<string> SelectedRoles { get; set; } = new List<string>();
        public List<string> Roles { get; set; } = new List<string>();
    }
}
