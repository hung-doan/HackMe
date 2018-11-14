using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HackMe01.Models;
using HackMe01.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HackMe01.Controllers
{
    public class ProductController : Controller
    {
        private IConfiguration _configuration;
        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult List([FromQuery]ProductListRequest request)
        {
            IEnumerable<ProductListResponse> result;
            using (var conn = GetDbConnection())
            {
                conn.Open();

                var query = @"SELECT Id, Name, Price FROM Products ";
                if (request != null && !string.IsNullOrEmpty(request.Name))
                {
                    query += $"WHERE Name LIKE N'%{request.Name}%'";
                }

                result = conn.Query<ProductListResponse>(query);
            }

            ViewBag.Request = request??new ProductListRequest();
            return View(result);
        }

        private IDbConnection GetDbConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(connectionString);
        }
    }
}