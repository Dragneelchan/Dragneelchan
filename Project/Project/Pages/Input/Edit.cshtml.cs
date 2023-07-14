using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Pages.User;

namespace Project.Pages.Input
{
    public class EditModel : PageModel
    {
        public userInfo data { get; set; }
        [BindProperty]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string SuccessMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public string User { get; set; }
        [BindProperty]
        public string role { get; set; }

        public IActionResult OnGet()
        {
            try
            {
                // Retrieve existing data from the database
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM data WHERE Id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                User = reader.GetString(1);
                                role = reader.GetString(2);
                            }
                            else
                            {
                                ErrorMessage = "List not found.";
                                return Page();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                // Update the role and user columns in the data table
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE data SET [user] = @user, [role] = @role WHERE Id = @Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", User);
                        command.Parameters.AddWithValue("@role", role);
                        command.Parameters.AddWithValue("@Id", Id);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            SuccessMessage = "List updated successfully!";
                        }
                        else
                        {
                            ErrorMessage = "Failed to update list.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return Page();
        }
    }
}
