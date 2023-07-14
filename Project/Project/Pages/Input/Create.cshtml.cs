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

                   
                    
               

                    // Proceed with inserting into the data table
                    String sql = "INSERT INTO data " +
                                 "([user], [role]) VALUES " +
                                 "(@user, @role);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", data.user);
                        command.Parameters.AddWithValue("@role", data.role);
        

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
