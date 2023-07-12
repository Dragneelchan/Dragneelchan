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
        public string Info { get; set; }

        [BindProperty]
        public string Type { get; set; }

        [BindProperty]
        public string Value { get; set; }

        public List<string> AvailableTypes { get; set; }

        public IActionResult OnGet()
        {
            try
            {
                AvailableTypes = new List<string> { "Income", "Outcome" };
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM data WHERE Id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                User = reader.GetString(1);
                                Info = reader.GetString(2);
                                Type = reader.GetString(3);
                                Value = reader.GetString(4);
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

            AvailableTypes = new List<string> { "Income", "Outcome" };

            return Page();
        }

        public IActionResult OnPost()
        {
            try
            {
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE data SET [user] = @user, [info] = @info, [type] = @type, [value] = @value WHERE Id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", User);
                        command.Parameters.AddWithValue("@info", Info);
                        command.Parameters.AddWithValue("@type", Type);
                        command.Parameters.AddWithValue("@value", Value);
                        command.Parameters.AddWithValue("@id", Id);

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
