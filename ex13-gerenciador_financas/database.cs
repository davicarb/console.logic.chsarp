using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace FinanceTracker
{
class DataBase
  {
    public static void InicializarDataBase(string connectionString)
    {
      using var conn = new SqliteConnection(connectionString);
      conn.Open();
      using var command = conn.CreateCommand();
      using var command2 = conn.CreateCommand();

      command.CommandText = @"CREATE TABLE IF NOT EXISTS categories (
    id   INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE,
    type TEXT NOT NULL);";

      command.ExecuteNonQuery();

      command2.CommandText = @"CREATE TABLE IF NOT EXISTS transactions (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    description TEXT NOT NULL,
    amount      REAL NOT NULL,
    date        TEXT NOT NULL,
    category_id INTEGER NOT NULL,
    FOREIGN KEY (category_id) REFERENCES categories(id))";

      command2.ExecuteNonQuery();
    }
  }
}