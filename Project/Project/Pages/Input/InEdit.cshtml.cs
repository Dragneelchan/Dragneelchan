using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Project.Pages.Input
{
    public class InEditModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int No { get; set; }

        [BindProperty]
        public int SelectedId { get; set; }

        [BindProperty]
        public string Info { get; set; }

        [BindProperty]
        public string Etc { get; set; }

        [BindProperty]
        public string Type { get; set; }

        [BindProperty]
        public string Value { get; set; }

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
        [BindProperty]
        public string SuccessMessage { get; set; }

        public void OnGet()
        {
            try
            {
                // Retrieve existing data from the database
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM [dbo].[Payment] WHERE [No] = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", No);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                SelectedId = reader.GetInt32(reader.GetOrdinal("Id"));
                                Info = reader.GetString(reader.GetOrdinal("info"));
                                Etc = reader["etc"] == DBNull.Value ? null : reader.GetString(reader.GetOrdinal("etc"));
                                Type = reader.GetString(reader.GetOrdinal("type"));
                                Value = reader.GetString(reader.GetOrdinal("Value"));
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
                // Update the data in the database
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Update mode: Update the existing record
                    string updateSql = "UPDATE [dbo].[Payment] SET [Id] = @user, [info] = @info, [etc] = @etc, [type] = @type, [Value] = @value, [Date] = @date, [Time] = @time WHERE [No] = @No";
                    using (SqlCommand updateCommand = new SqlCommand(updateSql, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@user", SelectedId);
                        updateCommand.Parameters.AddWithValue("@info", Info);
                        updateCommand.Parameters.AddWithValue("@etc", string.IsNullOrEmpty(Etc) ? (object)DBNull.Value : Etc);
                        updateCommand.Parameters.AddWithValue("@type", Type);
                        updateCommand.Parameters.AddWithValue("@value", Value);
                        updateCommand.Parameters.AddWithValue("@date", Date);
                        updateCommand.Parameters.AddWithValue("@time", string.IsNullOrEmpty(Time) ? (object)DBNull.Value : TimeSpan.Parse(Time));
                        updateCommand.Parameters.AddWithValue("@No", No);

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            SuccessMessage = "List updated successfully!";
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
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
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
