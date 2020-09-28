using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace handshake.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class HelloWorldController : ControllerBase
  {
    string connectionString = "Server=tcp:server2.database.windows.net,1433;Initial Catalog=handshake;Persist Security Info=False;User ID=Name;Password=5LQazj2DTtz6e32;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

    [HttpGet]
    public string Get()
    {
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        string sql = "SELECT NAME, NAME2, AGE FROM PEOPLE";
        using (SqlCommand command = new SqlCommand(sql, connection))
        {
          using (SqlDataReader reader = command.ExecuteReader())
          {
            StringBuilder sb = new StringBuilder();

            while (reader.Read())
            {
              sb.Append(reader.GetString(0));
              sb.Append(' ');
              sb.AppendLine(reader.GetString(1));
            }

            return sb.ToString();
          }
        }
      }
    }
  }
}
