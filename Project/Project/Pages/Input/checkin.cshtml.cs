using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Principal;

namespace Project.Pages.Input
{
    public class CheckinModel : PageModel
    {
        public CheckinModel()
        {
            ExistingUsers = new List<UserModel>();
            listUser = new List<CheckinData>();
        }

        public int Id { get; set; }
        public string user { get; set; }

        [BindProperty]
        public int SelectedId { get; set; }

        [BindProperty]
        public string Type { get; set; }

        // New properties for filtering
        [BindProperty(SupportsGet = true)]
        public int filterID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string filterType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string filterDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        [BindProperty]
        public int TotalRecords { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        public class UserModel
        {
            public int Id { get; set; }
            public string User { get; set; }
        }

        public List<UserModel> ExistingUsers { get; set; }

        public List<CheckinData> listUser { get; set; }
        public string ErrorMessage { get; private set; }

        public IActionResult OnGet()
        {
            PopulateExistingUsers();

            // Call the method to get the filtered data based on the query parameters
            GetCheckinData();

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

                                // Insert the new data into the checkin table
                                string insertDataSql = "INSERT INTO [dbo].[checkin] ([Id], [Date], [Time], [type]) VALUES (@id, @date, @time, @type)";

                                using (SqlCommand insertDataCommand = new SqlCommand(insertDataSql, connection))
                                {
                                    insertDataCommand.Parameters.AddWithValue("@id", SelectedId);
                                    insertDataCommand.Parameters.AddWithValue("@date", DateTime.Now.Date);
                                    insertDataCommand.Parameters.AddWithValue("@time", DateTime.Now.ToString("HH:mm:ss"));
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

                Type = string.Empty;

                // Reload the check-in data after successful check-in
                GetCheckinData();

                return RedirectToPage("/Input/Checkin");
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
                Response.Redirect("/Input/account");
            }
        }

        public void GetCheckinData()
        {
            listUser = new List<CheckinData>();

            try
            {
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT c.[No], c.[Id], c.[Date], c.[Time], c.[Type], d.[user] " +
                     "FROM [dbo].[checkin] c " +
                    "JOIN [dbo].[data] d ON c.[Id] = d.[Id]";

                    // Apply filtering if any of the filters is selected
                    var conditions = new List<string>();
                    if (filterID != 0)
                    {
                        conditions.Add("c.Id = @filterID");
                    }
                    if (!string.IsNullOrEmpty(filterType))
                    {
                        conditions.Add("c.Type = @filterType");
                    }
                    if (!string.IsNullOrEmpty(filterDate))
                    {
                        conditions.Add("c.Date = @filterDate");
                    }

                    if (conditions.Count > 0)
                    {
                        sql += " WHERE " + string.Join(" AND ", conditions);
                    }

                    // Apply sorting based on SortBy and SortOrder
                    if (!string.IsNullOrEmpty(SortBy))
                    {
                        if (SortOrder == "desc")
                        {
                            sql += $" ORDER BY c.{SortBy} DESC";
                        }
                        else
                        {
                            sql += $" ORDER BY c.{SortBy} ASC";
                        }
                    }
                    else
                    {
                        sql += " ORDER BY c.Date DESC, c.Time DESC";
                    }

                    /* Apply pagination
                    int skip = (PageNumber - 1) * PageSize;
                    sql += $" OFFSET {skip} ROWS FETCH NEXT {PageSize} ROWS ONLY";*/

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters for filtering if needed
                        if (filterID != 0)
                        {
                            command.Parameters.AddWithValue("@filterID", filterID);
                        }
                        if (!string.IsNullOrEmpty(filterType))
                        {
                            command.Parameters.AddWithValue("@filterType", filterType);
                        }
                        if (!string.IsNullOrEmpty(filterDate))
                        {
                            command.Parameters.AddWithValue("@filterDate", DateTime.Parse(filterDate).Date);
                        }

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int userId = reader.GetInt32(reader.GetOrdinal("Id"));

                                listUser.Add(new CheckinData
                                {
                                    No = reader.GetInt32(reader.GetOrdinal("No")),
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                    Time = reader.GetTimeSpan(reader.GetOrdinal("Time")),
                                    Type = reader.GetString(reader.GetOrdinal("Type")),
                                    User = reader.GetString(reader.GetOrdinal("user")),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or log it
                // Set listUser to an empty list or null if appropriate
            }
        }

        public class CheckinData
        {
            public int No { get; set; }
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan Time { get; set; }
            public string Type { get; set; }
            public string User { get; set; }
        }
    }
}
