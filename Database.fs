namespace FinTrack

open System
open System.Data
open Microsoft.Data.Sqlite
open Dapper

module Database =

    let connectionString = "Data Source=fintrack.db"

    let ensureDatabase () =
        use conn = new SqliteConnection(connectionString)
        conn.Open()
        conn.Execute("""
            CREATE TABLE IF NOT EXISTS Transactions (
                Id TEXT PRIMARY KEY,
                Description TEXT NOT NULL,
                Amount REAL NOT NULL,
                Date TEXT NOT NULL,
                Category TEXT NOT NULL
            );
        """) |> ignore

    let getAllTransactions () =
        use conn = new SqliteConnection(connectionString)
        conn.Open()
        use cmd = conn.CreateCommand()
        cmd.CommandText <- "SELECT Id, Description, Amount, Date, Category FROM Transactions"

        use reader = cmd.ExecuteReader()
        let results = ResizeArray<Transaction>()
        while reader.Read() do
            let tx = {
                Id = reader.GetString(0)
                Description = reader.GetString(1)
                Amount = reader.GetDecimal(2)
                Date = DateTime.Parse(reader.GetString(3))
                Category = reader.GetString(4)
            }
            results.Add(tx)
        results |> Seq.toList

    let addTransaction (tx: Transaction) =
        use conn = new SqliteConnection(connectionString)
        conn.Open()
        conn.Execute("""
            INSERT INTO Transactions (Id, Description, Amount, Date, Category)
            VALUES (@Id, @Description, @Amount, @Date, @Category)
        """, tx) |> ignore
        tx

    let getSummaryByCategory () =
        use conn = new SqliteConnection(connectionString)
        conn.Open()
        use cmd = conn.CreateCommand()
        cmd.CommandText <- """
            SELECT Category, SUM(Amount)
            FROM Transactions
            GROUP BY Category
        """
        use reader = cmd.ExecuteReader()

        let results = ResizeArray<SummaryItem>()
        while reader.Read() do
            let item = {
                Category = reader.GetString(0)
                Total = reader.GetDecimal(1)
            }
            results.Add(item)
        results |> Seq.toList

    let getMonthlySummary () =
        use conn = new SqliteConnection(connectionString)
        conn.Open()
        use cmd = conn.CreateCommand()
        cmd.CommandText <- """
            SELECT substr(Date, 1, 7) AS Month, SUM(Amount)
            FROM Transactions
            GROUP BY substr(Date, 1, 7)
            ORDER BY Month
        """
        use reader = cmd.ExecuteReader()

        let results = ResizeArray<MonthlySummary>()
        while reader.Read() do
            let item = {
                Month = reader.GetString(0)
                Total = reader.GetDecimal(1)
            }
            results.Add(item)
        results |> Seq.toList

    let deleteTransaction (id: string) =
        use conn = new SqliteConnection(connectionString)
        conn.Open()
        let parameters = dict ["Id", box id]
        let affected = conn.Execute("DELETE FROM Transactions WHERE Id = @Id", parameters)
        affected > 0