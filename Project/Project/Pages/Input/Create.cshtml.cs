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
            data.Username = Request.Form["Username"];
            data.Password = Request.Form["Password"];

            if (data.user.Length == 0 || string.IsNullOrEmpty(data.role) || string.IsNullOrEmpty(data.Username) || string.IsNullOrEmpty(data.Password))
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
                                 "([user], [role], [Username], [Password]) VALUES " +
                                 "(@user, @role, @Username, @Password);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@user", data.user);
                        command.Parameters.AddWithValue("@role", data.role);
                        command.Parameters.AddWithValue("@Username", data.Username);
                        command.Parameters.AddWithValue("@Password", data.Password);


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
            data.Username = "";
            data.Password = "";
            successMessage = "New list added!";
            Response.Redirect("/Input/user");
        }

    }
}
