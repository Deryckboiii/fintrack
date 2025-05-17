namespace FinTrack

open System
open System.Collections.Generic

module Storage =
    let transactions = new List<Transaction>()

    let getAll () = transactions |> Seq.toList

    let add (tx: Transaction) =
        transactions.Add(tx)
        tx