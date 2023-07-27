using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Project.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string _connectionString;

        public IndexModel(IConfiguration configuration)
        {
            _connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
        }

        public IActionResult OnGet()
        {
            // This method handles the HTTP GET request for the page.
            // You don't need to return anything here, as the Razor Page will be rendered automatically.
            return Page();
        }

        public IActionResult OnPost(string username, string password)
        {
            // Implement your logic to validate the username and password against the database.
            // You can use the _connectionString to connect to the database and check the credentials.
            // If the credentials are correct, you can redirect the user to another page (e.g., dashboard).

            // Sample logic to check credentials (replace with your actual logic):
            if (IsValidCredentials(username, password))
            {
               return RedirectToPage("/Input/user");
            }

            // If the credentials are not correct, return the same login page with an error message.
            ViewData["ErrorMessage"] = "Invalid username or password.";
            return Page();
        }

        // Sample method to check the credentials (replace with your actual logic):
        private bool IsValidCredentials(string Username, string Password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open(); // Open the connection before executing the command.

                // Implement your logic to query the database and check if the credentials are valid.
                // Here, we are assuming you have a Users table with columns Username and Password.
                using (var command = new SqlCommand("SELECT COUNT(*) FROM data WHERE Username = @Username AND Password = @Password", connection))
                {
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Password", Password);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            } // The connection will be automatically closed when exiting the 'using' block.
        }
    }
}
