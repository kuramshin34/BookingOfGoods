using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Transactions;

namespace BookingOfGoods
{
    public class ShopStorage : IDisposable
    {

        private SqlConnection _sqlConnection;

        private object _lock = new object();
        public ShopStorage()
        {
           _sqlConnection = new SqlConnection("Server=WIN-2J6811OHPJK\\SQLEXPRESS;Database=BookingOfGoodsDB;Trusted_Connection=True;Encrypt=False");
            _sqlConnection.Open();
        }

        public bool Reserve(string productName, string name, int count)
        {
            lock (_lock)
            {
                using (var transaction = _sqlConnection.BeginTransaction())
                {
                    try
                    {
                        var product = GetProduct(productName, transaction);
                        if (product == null)
                        {
                            return false;
                        }
                        if (product.Count >= count)
                        {
                            InsertBooking(name, count, product.Id, transaction);
                            product.Count -= count;
                            Update(product, transaction);
                            transaction.Commit();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        private Product GetProduct(string productName, SqlTransaction transaction)
        {
            Product product = null; 
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = _sqlConnection;
                cmd.Transaction = transaction;
                cmd.CommandText = "SELECT TOP 1 Id, Name, Count FROM Products WHERE Name = @ProductName";
                cmd.Parameters.AddWithValue("@ProductName", productName);

               
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Count = (int)reader["Count"]
                        };
                    }
                } 
            }
            return product;
           
        }
        private void Update(Product product, SqlTransaction transaction)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = _sqlConnection;
                cmd.Transaction = transaction;
                cmd.CommandText = $"UPDATE Products SET Count = @Count  WHERE Name = @Name AND Id = @Id";
                cmd.Parameters.AddWithValue("@Count", product.Count);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@Id", product.Id);

                cmd.ExecuteNonQuery();
            }
        }
        private void InsertBooking(string name, int count, int productId, SqlTransaction transaction)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = _sqlConnection;
                cmd.Transaction = transaction;
                cmd.CommandText = "INSERT INTO Bookings (Name, Count, DateBooking, DateDelivery, ProductId) " +
                    "VALUES (@Name, @Count, @DateBooking, @DateDelivery, @ProductId)";
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Count", count);
                cmd.Parameters.AddWithValue("@DateBooking", DateTime.Now);
                cmd.Parameters.AddWithValue("@DateDelivery", DateTime.Now.AddHours(1));
                cmd.Parameters.AddWithValue("@ProductId", productId);

                cmd.ExecuteNonQuery();
            }
        }

        ~ShopStorage()
        {
            _sqlConnection?.Close();
        }
        public void Dispose()
        {
            _sqlConnection.Close();
        }
        private class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Count { get; set; }
        }
    }

    
}
