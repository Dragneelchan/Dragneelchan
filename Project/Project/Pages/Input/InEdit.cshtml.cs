using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Project.Pages.Input
{
    public class EditCheckinModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int No { get; set; }

        [BindProperty]
        public int SelectedId { get; set; }

        [BindProperty]
        public string Type { get; set; }

        [BindProperty]
        public DateTime Date { get; set; }

        [BindProperty]
        public string Time { get; set; }

        public class UserModel
        {
            public int Id { get; set; }
            public string User { get; set; }
        }

        public List<UserModel> ExistingUsers { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            try
            {
                // Retrieve existing data from the database (checkin table)
                string connectionString = "YourConnectionStringHere";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM [dbo].[checkin] WHERE [No] = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", No);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                SelectedId = reader.GetInt32(reader.GetOrdinal("Id"));
                                Type = reader.GetString(reader.GetOrdinal("type"));
                                Time = reader["Time"] == DBNull.Value ? null : reader.GetTimeSpan(reader.GetOrdinal("Time")).ToString();
                                Date = reader.GetDateTime(reader.GetOrdinal("Date"));
                            }
                            else
                            {
                                ErrorMessage = "List not found.";
                                PopulateExistingUsers();
                                return;
                            }
                        }
                    }
                }

                PopulateExistingUsers(); // Populate the dropdown list for users
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                // Update the data in the database (checkin table)
                string connectionString = "YourConnectionStringHere";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Update mode: Update the existing record in the checkin table
                    string updateSql = "UPDATE [dbo].[checkin] SET [Id] = @user, [Date] = @date, [Time] = @time, [type] = @type WHERE [No] = @No";
                    using (SqlCommand updateCommand = new SqlCommand(updateSql, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@user", SelectedId);
                        updateCommand.Parameters.AddWithValue("@date", Date);
                        updateCommand.Parameters.AddWithValue("@time", string.IsNullOrEmpty(Time) ? (object)DBNull.Value : TimeSpan.Parse(Time));
                        updateCommand.Parameters.AddWithValue("@type", Type);
                        updateCommand.Parameters.AddWithValue("@No", No);

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            // Redirect to a success page or show a success message
                            return RedirectToPage("/Input/Checkin");
                        }
                        else
                        {
                            ErrorMessage = "Failed to update list. No matching record found.";
                        }
                    }
                }

                PopulateExistingUsers(); // Populate the dropdown list for users
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while updating the list: " + ex.Message;
            }

            return Page();
        }

        private void PopulateExistingUsers()
        {
            ExistingUsers = new List<UserModel>();

            try
            {
                string connectionString = "YourConnectionStringHere";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM [dbo].[data]";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ExistingUsers.Add(new UserModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    User = reader.GetString(reader.GetOrdinal("User"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or log it
                // Set ExistingUsers to an empty list or null if appropriate
            }
        }
    }
}
