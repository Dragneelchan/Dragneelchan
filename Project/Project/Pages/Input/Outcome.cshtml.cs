using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Project.Pages.Input
{
    public class OutcomeModel : PageModel
    {
        public List<Outcome> listOutcomeData { get; set; }
        public UserData UserData { get; set; }

        public List<Outcome> listOutcome { get; set; }

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT data.Id, data.[user], Outcome.Outcome_ID, Outcome.info, Outcome.etc, Outcome.Value_Outcome, Outcome.Date, Outcome.Time, Outcome.type FROM Outcome JOIN data ON Outcome.Id = data.Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            listOutcome = new List<Outcome>();
                            while (reader.Read())
                            {
                                Outcome outcome = new Outcome
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    User = reader.GetString(reader.GetOrdinal("user")),
                                    Outcome_ID = reader.GetInt32(reader.GetOrdinal("Outcome_ID")),
                                    Info = reader.GetString(reader.GetOrdinal("info")),
                                    Etc = reader.GetString(reader.GetOrdinal("etc")),
                                    Value_Outcome = reader.GetString(reader.GetOrdinal("Value_Outcome")),
                                    Date = reader.GetDateTime(reader.GetOrdinal("Date")).ToString("yyyy-MM-dd"),
                                    Time = reader.GetTimeSpan(reader.GetOrdinal("Time")).ToString(@"hh\:mm\:ss"),
                                    Type = reader.GetString(reader.GetOrdinal("type"))
                                };
                                listOutcome.Add(outcome);
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

    public class Outcome
    {
        public int Id { get; set; }
        public string User { get; set; }
        public int Outcome_ID { get; set; }
        public string Info { get; set; }
        public string Etc { get; set; }
        public string Value_Outcome { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
    }
}
