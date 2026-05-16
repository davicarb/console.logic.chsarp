using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace FinanceTracker
{
  class Program
  {
    public static void Main(string[] args)
    {
      string connectionString = "Data Source=financialtrack.db";
      DataBase.InicializarDataBase(connectionString);
      Console.WriteLine("finance tracker application");
      Console.WriteLine("===========================\n");
      Run(connectionString);
    }

    public static void Run(string connectionString)
    {
      bool running = true;

      while (running)
      {
        Menu.MostrarMenu();
        bool validMenuChoice = int.TryParse(Console.ReadLine(), out int choice);

        while (!validMenuChoice || choice < 1 || choice > 8)
        {
          System.Console.WriteLine("incorreto. insira novamente: ");
          validMenuChoice = int.TryParse(Console.ReadLine(), out choice);
        }
        switch (choice)
        {
          case 1:
            Categorias.AdicionarCategoria(connectionString);
            break;

          case 2:
            Categorias.ListarCategorias(connectionString);
            break;
          case 3:
            Categorias.DeletarCategoria(connectionString);
            break;
          case 4:
            Transacoes.AdicionarTransacao(connectionString);
            break;
          case 5:
            Transacoes.ListarTransacoes(connectionString);
            break;
          case 6:
            Transacoes.DeletarTransacao(connectionString);
            break;
          case 7:
            Transacoes.VerSaldoAtual(connectionString);
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
  }
}