using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace FinanceTracker
{
  class Transacoes
  {
    public static void AdicionarTransacao(string connectionString)
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

    public static void ListarTransacoes(string connectionString)
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
    public static void DeletarTransacao(string connectionString)
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

    public static void VerSaldoAtual(string connectionString)
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