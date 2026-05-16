using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace FinanceTracker
{
  class Categorias
  {
    public static void AdicionarCategoria(string connectionString)
    {
      Console.Clear();
      Console.WriteLine("\nadicionar categoria");

      Console.WriteLine("insira o tipo da categoria que quer adicionar (1 para entrada e 2 para saída): ");
      bool validCategoryType = int.TryParse(Console.ReadLine(), out int categoryType);

      while (!validCategoryType || categoryType > 2 || categoryType < 0)
      {
        Console.WriteLine("incorreto. insira novamente: ");
        validCategoryType = int.TryParse(Console.ReadLine(), out categoryType);
      }
      string sqlCategoryType = "";
      if (categoryType == 1)
      {
        sqlCategoryType = "Entrada";
      }

      if (categoryType == 2)
      {
        sqlCategoryType = "Saída";
      }

      Console.WriteLine("insira o nome da categoria que quer adicionar: ");
      string categoryName = Console.ReadLine() ?? "";

      using var connAdd = new SqliteConnection(connectionString);
      connAdd.Open();
      using var command = connAdd.CreateCommand();
      command.CommandText = "INSERT INTO categories (name, type) VALUES ($cName, $cType)";
      command.Parameters.AddWithValue("$cName", categoryName);
      command.Parameters.AddWithValue("$cType", sqlCategoryType);

      command.ExecuteNonQuery();
      Console.ReadKey();
    }

    public static void DeletarCategoria(string connectionString)
    {
      Console.Clear();
      Console.WriteLine("\nlistar categorias\n");
      using var connCategory = new SqliteConnection(connectionString);
      connCategory.Open();
      using var command = connCategory.CreateCommand();
      command.CommandText = "SELECT * FROM categories";
      using var reader = command.ExecuteReader();

      int quantidadeCategorias = 0;
      var idsValidosCategorias = new List<int>();

      while (reader.Read())
      {
        int catId = reader.GetInt32(0);
        string catName = reader.GetString(1);
        string catType = reader.GetString(2);

        Console.WriteLine($"id da categoria: {catId}\nnome da categoria: {catName}\ntipo da categoria: {catType}\n\n");

        idsValidosCategorias.Add(catId);
        quantidadeCategorias++;
      }

      reader.Close();

      if (quantidadeCategorias < 1)
      {
        System.Console.WriteLine("não existem categorias para excluir.");
        return;
      }

      System.Console.WriteLine("insira o id da categoria que quer excluir (0 para sair): ");
      bool validTransactionDeleteId = int.TryParse(Console.ReadLine(), out int tIdCategoryDelete);

      if (tIdCategoryDelete == 0) return;

      while (!validTransactionDeleteId || !idsValidosCategorias.Contains(tIdCategoryDelete))
      {
        System.Console.WriteLine("id da categoria incorreto. insira novamente: ");
        validTransactionDeleteId = int.TryParse(Console.ReadLine(), out tIdCategoryDelete);

        if (tIdCategoryDelete == 0) return;
      }

      command.CommandText = "DELETE FROM transactions WHERE category_id = $catId";
      command.Parameters.AddWithValue("$catId", tIdCategoryDelete);
      command.ExecuteNonQuery();

      command.CommandText = "DELETE FROM categories WHERE id = $catId";
      command.ExecuteNonQuery();
      Console.ReadKey();
    }

    public static void ListarCategorias(string connectionString)
    {
      Console.Clear();
      Console.WriteLine("\nlistar categorias\n");
      using var connListar = new SqliteConnection(connectionString);
      connListar.Open();
      using var command = connListar.CreateCommand();
      command.CommandText = "SELECT * FROM categories";
      using var reader = command.ExecuteReader();

      while (reader.Read())
      {
        int catId = reader.GetInt32(0);
        string catName = reader.GetString(1);
        string catType = reader.GetString(2);

        Console.WriteLine($"id da categoria: {catId}\nnome da categoria: {catName}\ntipo da categoria: {catType}\n\n");
      }
      Console.ReadKey();
    }
  }
}