using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Project.Pages.User;
using static Project.Pages.Input.AddModel;

namespace Project.Pages.Input
{
    public class newModel : PageModel
    {
        public userInfo data = new userInfo();
        public String errorMessage = "";
        public String successMessage = "";
        private int incomeCount = 0;
        private int outcomeCount = 0;

        public void OnGet()
        {
        }

        public void OnPost()
        {
            data.user = Request.Form["user"];
            data.role = Request.Form["role"];

            if (data.user.Length == 0 || string.IsNullOrEmpty(data.role))
            {
                errorMessage = "All the fields are required";
                return;
            }

            try
            {
                String connectionString = "Data Source=192.168.1.65;Initial Catalog=intern;Persist Security Info=True;User ID=sa;Password=01Password";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the generated Income_ID exists in the Income table
                    string generatedIncomeID = "In_" + (++incomeCount).ToString();
                    string validateQuery = "SELECT COUNT(*) FROM Income WHERE Income_ID = @incomeID";
                    using (SqlCommand validateCommand = new SqlCommand(validateQuery, connection))
                    {
                        validateCommand.Parameters.AddWithValue("@incomeID", generatedIncomeID);
                        int matchingCount = (int)validateCommand.ExecuteScalar();

                        if (matchingCount == 0)
                        {
                            errorMessage = "Invalid Income_ID. Please provide a valid Income_ID.";
                            return;
                        }
                    }

                    // Proceed with inserting into the data table
                    String sql = "INSERT INTO data " +
                                 "([user], [role], [Income_ID], [Outcome_ID]) VALUES " +
                                 "(@user, @role, @Income_ID, @Outcome_ID);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", data.user);
                        command.Parameters.AddWithValue("@role", data.role);
                        command.Parameters.AddWithValue("@Income_ID", generatedIncomeID);
                        command.Parameters.AddWithValue("@Outcome_ID", "Out_" + (++outcomeCount).ToString());

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            data.user = "";
            data.role = "";
            successMessage = "New list added!";
            Response.Redirect("/Index");
        }

    }
}
