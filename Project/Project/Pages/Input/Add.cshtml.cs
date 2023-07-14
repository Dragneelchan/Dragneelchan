using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Project.Pages.User;

namespace Project.Pages.Input
{
    public class AddModel : PageModel
    {
        public AddModel()
        {
            ExistingUsers = new List<UserModel>(); // Initialize the property here
        }
        public int Id { get; set; }
        public string user { get; set; }
        [BindProperty]
        public int SelectedId { get; set; }

        [BindProperty]
        public string Info { get; set; }
        [BindProperty]
        public string time { get; set; }
        [BindProperty]
        public string etc { get; set; }
        [BindProperty]
        public string Type { get; set; }

        [BindProperty]
        public string Value { get; set; }

        public class UserModel
        {
            public int Id { get; set; }
            public string User { get; set; }
        }

        public List<UserModel> ExistingUsers { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet()
        {
            PopulateExistingUsers();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                PopulateExistingUsers();
                return Page();
            }

            try
            {
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve the selected user's information
                    string getUserSql = "SELECT * FROM [dbo].[data] WHERE Id = @id";
                    using (SqlCommand getUserCommand = new SqlCommand(getUserSql, connection))
                    {
                        getUserCommand.Parameters.AddWithValue("@id", SelectedId);
                        using (SqlDataReader userReader = getUserCommand.ExecuteReader())
                        {
                            if (userReader.Read())
                            {
                                string user = userReader.GetString(1);

                                // Close the userReader before executing the insertDataCommand
                                userReader.Close();

                                // Insert the new data into the selected user's record
                                string insertDataSql = "";
                                if (Type == "income")
                                {
                                    insertDataSql = "INSERT INTO [dbo].[Income] ([Id], [Info], [etc], [Value_Income], [Date], [Time], [Type]) VALUES (@id, @info, @etc, @value, @date, @time, @type)";
                                }
                                else if (Type == "outcome")
                                {
                                    insertDataSql = "INSERT INTO [dbo].[Outcome] ([Id], [Info], [etc], [Value_Outcome], [Date], [Time], [Type]) VALUES (@id, @info, @etc, @value, @date, @time, @type)";
                                }
                                else
                                {
                                    // Invalid type
                                    ErrorMessage = "Invalid Type value.";
                                    PopulateExistingUsers();
                                    return Page();
                                }

                                using (SqlCommand insertDataCommand = new SqlCommand(insertDataSql, connection))
                                {
                                    insertDataCommand.Parameters.AddWithValue("@id", SelectedId);
                                    insertDataCommand.Parameters.AddWithValue("@info", Info);
                                    insertDataCommand.Parameters.AddWithValue("@etc", etc);
                                    insertDataCommand.Parameters.AddWithValue("@value", Value);
                                    insertDataCommand.Parameters.AddWithValue("@date", DateTime.Now.Date);
                                    insertDataCommand.Parameters.AddWithValue("@time", time);
                                    insertDataCommand.Parameters.AddWithValue("@type", Type);

                                    insertDataCommand.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // User not found
                                ErrorMessage = "Selected user not found.";
                                PopulateExistingUsers();
                                return Page();
                            }
                        }
                    }
                }

                // Clear the input fields
                Info = string.Empty;
                Type = string.Empty;
                Value = string.Empty;

                // Set success message
                SuccessMessage = "New list added!";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                PopulateExistingUsers();
                return Page();
            }
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
                                    Id = reader.GetInt32(0),
                                    User = reader.GetString(1)
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
