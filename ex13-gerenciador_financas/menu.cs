using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace FinanceTracker
{
  class Menu
  {
    public static void MostrarMenu()
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
  }
}