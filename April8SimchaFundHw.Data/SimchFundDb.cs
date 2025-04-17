using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
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
        public DateTime SimchaDate { get; set; }
        public string SimchaName { get; set; }

    }
    public class HistoryTransacions
    {
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
       
    }
    public class History
    {
        public List<Deposit> Deposits { get; set; }
        public List<HistoryTransacions> ContributrionHistory { get; set; }
    }
    public class SimchFundDb
    {
        string queryString = @"SELECT c.*, ISNULL(d.TotalDeposits, 0) - ISNULL(con.TotalContributions, 0) AS 'Balance'
FROM Contributors c
LEFT JOIN (
    SELECT ContributorId, SUM(Amount) AS TotalDeposits
    FROM Deposits
    GROUP BY ContributorId
) d ON c.Id = d.ContributorId
LEFT JOIN (
    SELECT ContributorId, SUM(Amount) AS TotalContributions
    FROM Contributions
    GROUP BY ContributorId
) con ON c.Id = con.ContributorId";
        private readonly string _connectionString;
        public SimchFundDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = queryString;
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
            cmd.CommandText = @"Select s.* ,Sum(c.Amount) As 'Total', Count(c.ContributorId) As 'ContributionsCount' From Simchas s
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
                    Total = reader.GetOrNull<decimal>("Total"),
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
            cmd.CommandText = queryString;

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
        public string GetSimchaNamebyId(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Select Name From Simchas Where id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            string name = (string)cmd.ExecuteScalar();

            return name;


        }
        public string GetContributorNamebyId(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Select firstName,lastname From contributors Where id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            var reader = cmd.ExecuteReader();
            reader.Read();
            string firstName = (string)reader["firstName"];
            string LastName = (string)reader["LastName"];

            return firstName + " " + LastName;


        }
        public decimal GetBalanceById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Select (select sum(Amount) from Deposits where ContributorId = @id) as 'deposit Total', (select sum(Amount) from Contributions where ContributorId = @id) as 'contribution total'";

            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            reader.Read();

            decimal contribTotal = reader.GetOrNull<decimal>("contribution total");
            decimal depositTotal = reader.GetOrNull<decimal>("deposit Total");

            return depositTotal - contribTotal;


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
                if (c.Contribute)
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
            cmd.Parameters.AddWithValue("@date", simcha.Date);

            connection.Open();
            cmd.ExecuteNonQuery();
        }


        public int GetTotal()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Select (Select COALESCE(Sum(Amount),0) From deposits)-(Select COALESCE(Sum(Amount),0) From Contributions)";

            connection.Open();
            return (int)(decimal)cmd.ExecuteScalar();

        }

        public List<HistoryTransacions> DepositsHistory(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Select d.Amount ,d.Date from Deposits d
join Contributors c
on c.Id = d.ContributorId
Where c.Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            var depositHistory = new List<HistoryTransacions>();

            connection.Open();

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                depositHistory.Add(new()
                {
                    Name = "Deposit",
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["date"],

                });

            }
            return depositHistory;
        }
    
        public List<HistoryTransacions> HistoryContributions(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"Select co.Amount,s.Date,s.Name from Contributions co
Join Simchas s
on s.Id = co.SimchaId
Join Contributors c
on c.Id = co.ContributorId
Where c.Id = @id
";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            var contributionsHistory = new List<HistoryTransacions>();
            while (reader.Read())
            {
                contributionsHistory.Add(new()
                {
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["date"],
                    Name = "contribution for" + " " +(string)reader["Name"]

                });

            }
            return contributionsHistory;
        }

        }
    

    public static class Extensions
    {
        public static T GetOrNull<T>(this SqlDataReader reader, string columnName)
        {
            var value = reader[columnName];
            if (value == DBNull.Value)
            {
                return default(T);
            }

            return (T)value;
        }
    }
}

