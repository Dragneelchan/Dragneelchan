using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Project.Pages.Input
{
    public class AccountModel : PageModel
    {
        private readonly string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";

        public List<Account> listAccount { get; set; } = new List<Account>();
        public List<AccountData> listUserData { get; set; } = new List<AccountData>();

        [BindProperty(SupportsGet = true)]
        public int SelectedId { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public string SelectedInfo { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOrder { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = DateTime.MaxValue;

        public decimal TotalValue { get; set; }
        public List<int> AvailableIds { get; set; } = new List<int>();

        [BindProperty(SupportsGet = true)]
        public bool IsFilterSubmitted { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedStartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedEndDate { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        public async Task OnGet()
        {
            try
            {
                await GetData();
                SelectedStartDate = StartDate.ToString("yyyy-MM-dd");
                SelectedEndDate = EndDate.ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                // Handle the exception if required
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public async Task<IActionResult> OnGetExportToCSV()
        {
            try
            {
                await GetData();

                // Generate the CSV data
                StringBuilder csvData = new StringBuilder();
                csvData.AppendLine("Payment ID,Username,Info,Etc,Value,Date,Time,Type");

                foreach (var account in listAccount)
                {
                    csvData.AppendLine($"{account.Payment_ID},{account.Id} - {account.User},{account.Info},{account.Etc},{account.Value},{account.Date},{account.Time},{account.Type}");
                }

                // Append the total value to the CSV data
                csvData.AppendLine($",,,,,,,Total:,{TotalValue}");

                // Set the response headers
                Response.Headers.Add("Content-Disposition", "attachment; filename=PaymentList.csv");
                Response.ContentType = "text/csv";

                // Write the CSV data to the response body
                byte[] csvBytes = Encoding.UTF8.GetBytes(csvData.ToString());
                return new FileContentResult(csvBytes, "text/csv");
            }
            catch (Exception ex)
            {
                // Handle the exception if required
                Console.WriteLine("An error occurred: " + ex.Message);
                return BadRequest("An error occurred while exporting to CSV.");
            }
        }

        private async Task GetData()
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

                // Modify the date filtering conditions
                if (StartDate != DateTime.MinValue && EndDate != DateTime.MaxValue)
                {
                    conditions.Add("Payment.Date BETWEEN @startDate AND @endDate");
                }

                if (conditions.Count > 0)
                {
                    sql += " WHERE " + string.Join(" AND ", conditions);
                }

                if (!string.IsNullOrEmpty(SortOrder))
                {
                    if (SortOrder == "lowToHigh")
                    {
                        sql += " ORDER BY CAST(Payment.Value AS INT) ASC";
                    }
                    else if (SortOrder == "highToLow")
                    {
                        sql += " ORDER BY CAST(Payment.Value AS INT) DESC";
                    }
                }

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
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
                    if (StartDate != DateTime.MinValue && EndDate != DateTime.MaxValue)
                    {
                        command.Parameters.AddWithValue("@startDate", StartDate);
                        command.Parameters.AddWithValue("@endDate", EndDate);
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
                    }
                }
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