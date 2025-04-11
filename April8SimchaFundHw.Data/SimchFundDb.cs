using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace April8SimchaFundHw.Data
{
  
    public class Contributions
    {
        public int ContributorId { get; set; }
        public bool Contribute { get; set; }
       public Contributor contributor { get; set; }
        public decimal Amount { get; set; }
       
    }

    public class SimchFundDb
    {
        private readonly string _connectionString;
        public SimchFundDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Select c.Id, c.FirstName,c.LastName,c.CellNumber,c.AlwaysInclude,c.Date, Coalesce(Sum(d.Amount),0)- Coalesce(Sum(co.Amount),0) as 'balance' From Contributors c
Left Join Deposits d on d.ContributorId = c.Id
Left Join Contributions co on co.ContributorId = c.Id
Group By c.Id, c.FirstName,c.LastName,c.CellNumber,c.AlwaysInclude,c.Date";
            connection.Open();
            var contributors = new List<Contributor>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributors.Add(new()
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    Date = (DateTime)reader["Date"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Balance = (decimal)reader["Balance"]
                   
                });
            }
            return contributors;
        }

        public List<Simcha> GetSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Select s.* ,Sum(COALESCE(c.Amount,0)) As 'Total', Count(c.ContributorId) As 'ContributionsCount' From Simchas s
Left Join Contributions c
on s.Id = c.SimchaId
Where s.Id = s.Id
Group By s.Name,s.Id,s.Date";
            connection.Open();
            var simchas = new List<Simcha>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                simchas.Add(new()
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    Total = (decimal)reader["Total"],
                    ContributionsCount = (int)reader["ContributionsCount"],
                });
            }
            return simchas;
        }
        public int GetCountContributores()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Select Count(id) from Contributors";
            connection.Open();
           return (int)cmd.ExecuteScalar();


        }
        public void NewContributor(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Insert into Contributors Values(@FirstName,@LastName,@Cell,@Date,@alwaysInclude); Select Scope_identity();";
            cmd.Parameters.AddWithValue("@FirstName", c.FirstName);
            cmd.Parameters.AddWithValue("@LastName", c.LastName);
            cmd.Parameters.AddWithValue("@Cell", c.CellNumber);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@AlwaysInclude", c.AlwaysInclude);

            connection.Open();
            c.Id = (int)(decimal)cmd.ExecuteScalar();

            
        }
        public void UpdateContributor(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Contributors set AlwaysInclude = @AlwaysInclude, FirstName = @firstName, LastName = @LastName ,CellNumber = @Cell
Where id = @id";
            cmd.Parameters.AddWithValue("@id", c.Id);
            cmd.Parameters.AddWithValue("@FirstName", c.FirstName);
            cmd.Parameters.AddWithValue("@LastName", c.LastName);
            cmd.Parameters.AddWithValue("@Cell", c.CellNumber);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);
            cmd.Parameters.AddWithValue("@AlwaysInclude", c.AlwaysInclude);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public void AddDeposite(Deposit d)
        {

            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Insert into Deposits Values(@contributorId,@Amount,@Date)";
            cmd.Parameters.AddWithValue("@contributorId", d.ContributorId);
            cmd.Parameters.AddWithValue("@Amount", d.Amount);
            cmd.Parameters.AddWithValue("@Date", d.Date);
            connection.Open();

            cmd.ExecuteNonQuery();

        }
        public List<Contributions> GetContributionsForID()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT c.Id, c.FirstName,c.LastName,c.AlwaysInclude,Sum(d.Amount)- Coalesce(Sum(co.Amount),0) as 'balance' From Contributors c
Left Join Deposits d on d.ContributorId = c.Id
Left Join Contributions co on co.ContributorId = c.Id
Group By c.Id, c.FirstName,c.LastName,c.AlwaysInclude";

            connection.Open();

            var contributions = new List<Contributions>();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributions.Add(new()
                {

                    contributor = new Contributor()
                    {
                        Id = (int)reader["Id"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],

                        AlwaysInclude = (bool)reader["AlwaysInclude"],
                        Balance = (decimal)reader["Balance"]
                    }




                });
            }
            return contributions;
        }



        public void UpdateContributions(List<Contributions> Contributions, int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();


            cmd.CommandText = @"Delete From Contributions Where SimchaId = @simchaId";
           connection.Open();
            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "Insert into Contributions(SimchaId, ContributorId, Amount) Values(@simchaId, @contributorId, @amount)";

         
            foreach (Contributions c in Contributions)
            {
                if(c.Contribute)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@simchaId", simchaId);
                    cmd.Parameters.AddWithValue("@contributorId", c.ContributorId);
                    cmd.Parameters.AddWithValue("@amount", c.Amount);
                    cmd.ExecuteNonQuery();
                }

            }
           


        }

        public void NewSimcha(Simcha simcha)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Insert into Simchas Values(@Name,@Date)";
            cmd.Parameters.AddWithValue("@Name", simcha.Name);
            cmd.Parameters.AddWithValue("@date",simcha.Date);

            connection.Open();
           cmd.ExecuteNonQuery();
        }
        public int GetTotal()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Select
(Select COALESCE(Sum(amount),0)From deposits) - 
(Select COALESCE(Sum(amount),0)From Contributions)";

            connection.Open();
            return (int)(decimal)cmd.ExecuteScalar();

        }
    }
}
