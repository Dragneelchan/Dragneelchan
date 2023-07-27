using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Project.Pages.Input
{
    public class AccountModel : PageModel
    {
        private readonly string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";

        public List<Account> listAccount { get; set; } = new List<Account>();
        public List<AccountData> listUserData { get; set; } = new List<AccountData>();

        // Properties for filtering
        [BindProperty(SupportsGet = true)]
        public int SelectedId { get; set; } = 0; // Default: 0 to show all
        [BindProperty(SupportsGet = true)]
        public string SelectedInfo { get; set; } // Default: 0 to show all
        [BindProperty(SupportsGet = true)]
        public string SelectedType { get; set; } // No default to show all types
        [BindProperty(SupportsGet = true)]
        public string SelectedValue { get; set; } // Added property for filtering by Value
        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; } // Default: empty to show unsorted
        public decimal TotalValue { get; set; } 
        public List<int> AvailableIds { get; set; } = new List<int>();

        public async Task OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Get the distinct Ids from the Payment table
                    string idQuery = "SELECT DISTINCT Id, [user] FROM data";
                    using (SqlCommand idCommand = new SqlCommand(idQuery, connection))
                    {
                        using (SqlDataReader idReader = await idCommand.ExecuteReaderAsync())
                        {
                            AvailableIds.Clear();
                            listUserData.Clear(); // Clear the list to prevent duplicates
                            while (idReader.Read())
                            {
                                var id = idReader.GetInt32(0);
                                var user = idReader.GetString(1);
                                // Add Id and User to the listUserData for displaying in the dropdown
                                listUserData.Add(new AccountData { Id = id, User = user });
                            }
                        }
                    }


                    string sql = "SELECT data.Id, data.[user], Payment.No, Payment.info, Payment.etc, Payment.Value, Payment.Date, Payment.Time, Payment.Type FROM Payment JOIN data ON Payment.Id = data.Id";
                    // Apply filtering if any of the filters is selected
                    var conditions = new List<string>();
                    if (SelectedId != 0)
                    {
                        conditions.Add("data.Id = @selectedId");
                    }
                    if (!string.IsNullOrEmpty(SelectedType))
                    {
                        conditions.Add("Payment.Type = @selectedType");
                    }
                    if (!string.IsNullOrEmpty(SelectedInfo))
                    {
                        conditions.Add("Payment.info = @selectedInfo");
                    }

                    if (conditions.Count > 0)
                    {
                        sql += " WHERE " + string.Join(" AND ", conditions);
                    }

                    // Apply sorting based on SortOrder
                    if (!string.IsNullOrEmpty(SortOrder))
                    {
                        if (SortOrder == "lowToHigh")
                        {
                            // Modify sorting for numeric values (assuming "Value" is of numeric data type in the database)
                            sql += " ORDER BY CAST(Payment.Value AS INT) ASC";
                        }
                        else if (SortOrder == "highToLow")
                        {
                            // Modify sorting for numeric values (assuming "Value" is of numeric data type in the database)
                            sql += " ORDER BY CAST(Payment.Value AS INT) DESC";
                        }
                    }
                    

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters for filtering if needed
                        if (SelectedId != 0)
                        {
                            command.Parameters.AddWithValue("@selectedId", SelectedId);
                        }
                        if (!string.IsNullOrEmpty(SelectedType))
                        {
                            command.Parameters.AddWithValue("@selectedType", SelectedType);
                        }
                        if (!string.IsNullOrEmpty(SelectedInfo))
                        {
                            command.Parameters.AddWithValue("@selectedInfo", SelectedInfo);
                        }


                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            listAccount.Clear();
                            while (reader.Read())
                            {
                                Account account = new Account
                                {
                                    Payment_ID = reader.GetInt32(reader.GetOrdinal("No")),
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    User = reader.GetString(reader.GetOrdinal("user")),
                                    Info = reader.GetString(reader.GetOrdinal("info")),
                                    Etc = reader.GetString(reader.GetOrdinal("etc")),
                                    Value = reader.GetString(reader.GetOrdinal("Value")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")).ToString("yyyy-MM-dd"),
                                    Time = reader.GetTimeSpan(reader.GetOrdinal("Time")).ToString(@"hh\:mm\:ss"),
                                    Type = reader.GetString(reader.GetOrdinal("Type"))
                                };
                                listAccount.Add(account);

                                // Retrieve user data based on Payment.Id
                                AccountData userData = new AccountData
                                {
                                    Id = account.Id,
                                    User = account.User
                                };
                                listUserData.Add(userData);
                            }
                        }
                        decimal totalValue = 0;
                        foreach (var account in listAccount)
                        {
                            if (account.Type == "income")
                            {
                                totalValue += account.NumericValue;
                            }
                            else if (account.Type == "outcome")
                            {
                                totalValue -= account.NumericValue;
                            }
                        }
                        TotalValue = totalValue;
                        /*decimal totalValue = 0;
                        foreach (var item in listAccount)
                        {
                            totalValue += item.NumericValue;
                        }
                        TotalValue = totalValue;*/
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception if required
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }

    public class Account
    {
        public int Id { get; set; }
        public string User { get; set; }
        public int Payment_ID { get; set; }
        public string Info { get; set; }
        public string Etc { get; set; }
        public string Value { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
        public decimal NumericValue => decimal.TryParse(Value, out decimal numericValue) ? numericValue : 0;
    
}

    public class AccountData
    {
        public int Id { get; set; }
        public string User { get; set; }
    }
}
