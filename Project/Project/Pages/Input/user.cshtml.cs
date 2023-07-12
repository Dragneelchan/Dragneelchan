using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace Project.Pages.User
{
    public class userModel : PageModel
    {
        public List<userInfo> listUser = new List<userInfo>();

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM data";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userInfo data = new userInfo();
                                data.Id = reader.GetInt32(0);
                                data.role = reader.GetString(5);
                                data.Income_ID = reader.GetString(6);
                                data.Outcome_ID = reader.GetString(7);
                                listUser.Add(data);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception if required
            }
        }
    }

    public class userInfo
    {
        public int Id;
        public string user;
        public string info;
        public string type;
        public string value;
        public string role;
        public string Income_ID;
        public string Outcome_ID;
    }
}
