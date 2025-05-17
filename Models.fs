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