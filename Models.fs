namespace FinTrack

type Transaction = {
    Id: string
    Description: string
    Amount: decimal
    Date: System.DateTime
    Category: string
}

type SummaryItem = {
    Category: string
    Total: decimal
}

type MonthlySummary = {
    Month: string
    Total: decimal
}

type WeeklySummary() =
    member val Week = "" with get, set
    member val Total = 0.0 with get, set