using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Project.Pages.Input
{
    public class IncomeModel : PageModel
    {
        public List<Income> listUser { get; set; }

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Income";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            listUser = new List<Income>();
                            while (reader.Read())
                            {
                                Income income = new Income
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Income_ID = reader.GetInt32(reader.GetOrdinal("Income_ID")),
                                    Info = reader.GetString(reader.GetOrdinal("Info")),
                                    Etc = reader.GetString(reader.GetOrdinal("etc")),
                                    Value_Income = reader.GetString(reader.GetOrdinal("Value_Income")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")).ToString("yyyy-MM-dd"),
                                    Time = reader.GetTimeSpan(reader.GetOrdinal("Time")).ToString(@"hh\:mm\:ss"),
                                    Type = reader.GetString(reader.GetOrdinal("type"))
                                };
                                listUser.Add(income);
                            }
                        }
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

    public class Income
    {
        public int Id { get; set; }
        public int Income_ID { get; set; }
        public string Info { get; set; }
        public string Etc { get; set; }
        public string Value_Income { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
    }
}