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
        [BindProperty]
        public int SelectedId { get; set; }

        [BindProperty]
        public string Info { get; set; }

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

        public void OnGet()
        {
            PopulateExistingUsers();
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                PopulateExistingUsers();
                return;
            }

            try
            {
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Retrieve the selected user's information
                    string getUserSql = "SELECT * FROM [user] WHERE Id = @id";
                    using (SqlCommand getUserCommand = new SqlCommand(getUserSql, connection))
                    {
                        getUserCommand.Parameters.AddWithValue("@id", SelectedId);
                        using (SqlDataReader userReader = getUserCommand.ExecuteReader())
                        {
                            if (userReader.Read())
                            {
                                string user = userReader.GetString(1);

                                // Insert the new data into the selected user's record
                                string insertDataSql = "INSERT INTO data ([user], [info], [type], [value]) VALUES (@user, @info, @type, @value)";
                                using (SqlCommand insertDataCommand = new SqlCommand(insertDataSql, connection))
                                {
                                    insertDataCommand.Parameters.AddWithValue("@user", user);
                                    insertDataCommand.Parameters.AddWithValue("@info", Info);
                                    insertDataCommand.Parameters.AddWithValue("@type", Type);
                                    insertDataCommand.Parameters.AddWithValue("@value", Value);

                                    insertDataCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }

                // Clear the input fields
                Info = string.Empty;
                Type = string.Empty;
                Value = string.Empty;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateExistingUsers();
                return;
            }

            // Redirect to the index page or any other desired page
            // return RedirectToPage("/Index");
        }

        private void PopulateExistingUsers()
        {
            // Retrieve the existing users from the database
            ExistingUsers = new List<UserModel>();

            try
            {
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM [user]";
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
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
    }
}