using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace FinanceTracker
{
  class Program
  {
    static void Main(string[] args)
    {
      string connectionString = "Data Source=financialtrack.db";
      InicializarDataBase(connectionString);
      Console.WriteLine("finance tracker application");
      Console.WriteLine("===========================\n");
      Run(connectionString);
    }

    static void Run(string connectionString)
    {
      bool running = true;

      while (running)
      {
        MostrarMenu();
        bool validMenuChoice = int.TryParse(Console.ReadLine(), out int choice);

        while (!validMenuChoice || choice < 1 || choice > 8)
        {
          System.Console.WriteLine("incorreto. insira novamente: ");
          validMenuChoice = int.TryParse(Console.ReadLine(), out choice);
        }
        switch (choice)
        {
          case 1:
            AdicionarCategoria(connectionString);
            break;

          case 2:
            ListarCategorias(connectionString);
            break;
          case 3:
            DeletarCategoria(connectionString);
            break;
          case 4:
            AdicionarTransacao(connectionString);
            break;
          case 5:
            ListarTransacoes(connectionString);
            break;
          case 6:
            DeletarTransacao(connectionString);
            break;
          case 7:
            VerSaldoAtual(connectionString);
            break;
          case 8:
            running = false;
            break;
          default:
            Console.WriteLine("opção inválida.");
            break;
        }
      }
    }

    static void InicializarDataBase(string connectionString)
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
    static void MostrarMenu()
    {
      Console.Clear();
      Console.WriteLine("\nmenu do app: FINANCE TRACKER. uma solução prática para suas finanças pessoais.:");
      Console.WriteLine("1. adicionar categoria de renda");
      Console.WriteLine("2. listar categorias");
      Console.WriteLine("3. deletar categoria\n\n");
      Console.WriteLine("4. adicionar transação em uma categoria de renda");
      Console.WriteLine("5. listar transações");
      Console.WriteLine("6. deletar transação \n");
      Console.WriteLine("7. ver saldo atual");
      Console.WriteLine("8. sair");
    }

    static void AdicionarCategoria(string connectionString)
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

    static void DeletarCategoria(string connectionString)
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

    static void ListarCategorias(string connectionString)
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

    static void AdicionarTransacao(string connectionString)
    {
      Console.Clear();
      Console.WriteLine("\nadicionar transação");
      using var connTransacao = new SqliteConnection(connectionString);
      connTransacao.Open();
      using var command = connTransacao.CreateCommand();
      command.CommandText = "SELECT * FROM categories";
      using var reader = command.ExecuteReader();

      var idsValidosCategorias = new List<int>();
      int quantidadeCategorias = 0;

      Console.WriteLine("categorias disponíveis: \n");
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

      if (quantidadeCategorias == 0)
      {
        System.Console.WriteLine("não há categorias disponíveis para adicionar uma nova transação...");
        Console.ReadKey();
        return;
      }
      System.Console.WriteLine("insira o id da categoria que quer inserir uma transação (0 para sair): ");
      bool validId = int.TryParse(Console.ReadLine(), out int idCategoria);

      if (idCategoria == 0) return;

      while (!validId || !idsValidosCategorias.Contains(idCategoria))
      {
        System.Console.WriteLine("inválido ou id de categoria não existente. insira novamente: ");
        validId = int.TryParse(Console.ReadLine(), out idCategoria);

        if (idCategoria == 0) return;
      }

      System.Console.WriteLine("insira a descrição da sua transação: ");
      string descricaoTransacao = Console.ReadLine() ?? "";

      System.Console.WriteLine("insira o valor da sua transação: ");
      bool validTransaction = double.TryParse(Console.ReadLine(), out double transactionValue);

      while (!validTransaction || transactionValue < 0)
      {
        System.Console.WriteLine("incorreto. insira novamente: ");
        validTransaction = double.TryParse(Console.ReadLine(), out transactionValue);
      }

      System.Console.WriteLine("insira a data da transação: ");
      string dataTransacao = Console.ReadLine() ?? "";

      command.CommandText = @"INSERT INTO transactions (description, amount, date, category_id) VALUES
      ($description, $amount, $date, $catId)";
      command.Parameters.AddWithValue("$description", descricaoTransacao);
      command.Parameters.AddWithValue("$amount", transactionValue);
      command.Parameters.AddWithValue("$date", dataTransacao);
      command.Parameters.AddWithValue("$catId", idCategoria);

      command.ExecuteNonQuery();
      Console.ReadKey();
    }

    static void ListarTransacoes(string connectionString)
    {
      Console.Clear();
      Console.WriteLine("\nlistar transações\n");
      using var connListTransacao = new SqliteConnection(connectionString);
      connListTransacao.Open();
      using var command = connListTransacao.CreateCommand();
      command.CommandText = @"SELECT t.id, t.description, t.amount, t.date, c.name, c.type
      FROM transactions t JOIN categories c ON t.category_id = c.id";
      using var reader = command.ExecuteReader();

      while (reader.Read())
      {
        int tId = reader.GetInt32(0);
        string tDescription = reader.GetString(1);
        double tAmount = reader.GetDouble(2);
        string tDate = reader.GetString(3);
        string cName = reader.GetString(4);
        string cType = reader.GetString(5);

        Console.WriteLine($"CATEGORIA: {cName}\nTIPO DA CATEGORIA: {cType}\nid da transação: {tId}\ndescrição da transação: {tDescription}\nvalor da transação: R${tAmount:F2}\ndata da transação: {tDate}\n\n");
      }
      Console.ReadKey();
    }
    static void DeletarTransacao(string connectionString)
    {
      Console.Clear();
      Console.WriteLine("\ndeletar transação\n");
      using var connListTransacaoDeleteTransacao = new SqliteConnection(connectionString);
      connListTransacaoDeleteTransacao.Open();
      using var command = connListTransacaoDeleteTransacao.CreateCommand();
      command.CommandText = @"SELECT t.id, t.description, t.amount, t.date, c.name, c.type
      FROM transactions t JOIN categories c ON t.category_id = c.id";
      using var reader = command.ExecuteReader();

      int quantidadeTransacoes = 0;
      var idsValidos = new List<int>();

      while (reader.Read())
      {
        int tId = reader.GetInt32(0);
        string tDescription = reader.GetString(1);
        double tAmount = reader.GetDouble(2);
        string tDate = reader.GetString(3);
        string cName = reader.GetString(4);
        string cType = reader.GetString(5);

        Console.WriteLine($"categoria: {cName}\ntipo da categoria: {cType}\n\nid da transação: {tId}\ndescrição da transação: {tDescription}\nvalor da transação: {tAmount}\ndata da transação: {tDate}\n\n\n");
        quantidadeTransacoes++;
        idsValidos.Add(tId);
      }

      reader.Close();

      if (quantidadeTransacoes < 1)
      {
        System.Console.WriteLine("não existem transações para excluir.");
        return;
      }

      System.Console.WriteLine("insira o id da transação que quer excluir: (0 para sair) ");
      bool validTransactionDeleteId = int.TryParse(Console.ReadLine(), out int tIdDelete);

      if (tIdDelete == 0) return;

      while (!validTransactionDeleteId || !idsValidos.Contains(tIdDelete))
      {
        System.Console.WriteLine("incorreto. insira novamente: ");
        validTransactionDeleteId = int.TryParse(Console.ReadLine(), out tIdDelete);

        if (tIdDelete == 0) return;
      }

      command.CommandText = "DELETE FROM transactions WHERE id = $idInput";
      command.Parameters.AddWithValue("$idInput", tIdDelete);
      command.ExecuteNonQuery();
      Console.ReadKey();
    }

    static void VerSaldoAtual(string connectionString)
    {
      Console.Clear();
      Console.WriteLine("\nver saldo atual");

      double totalEntrada = 0;
      double totalSaida = 0;

      using var connVerSaldo = new SqliteConnection(connectionString);
      connVerSaldo.Open();
      using var command = connVerSaldo.CreateCommand();
      command.CommandText = @"SELECT c.type, SUM(t.amount)
      FROM transactions t
      JOIN categories c ON t.category_id = c.id
      GROUP BY c.type";

      using var reader = command.ExecuteReader();

      while (reader.Read())
      {
        string tipo = reader.GetString(0);
        double total = reader.GetDouble(1);

        if (tipo == "Entrada")
          totalEntrada = total;

        else
          totalSaida = total;
      }

      double saldo = totalEntrada - totalSaida;
      System.Console.WriteLine($"saldo atual: R${saldo:F2}.");
      Console.ReadKey();
    }
  }
}