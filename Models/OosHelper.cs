using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace puppeteer_sharp.Models
{
    public class OosHelper
    {
        private string _connectionString;
        IConfiguration configuration;
        public OosHelper(IConfiguration configuration)
        {
            this.configuration = configuration;

            this._connectionString = configuration.GetConnectionString(nameof(OosHelper));

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentException(nameof(OosHelper));
            }
        }


        public IDbConnection GetOpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public IDbConnection GetCloseConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            //conn.Open();
            return conn;
        }

    }
}
