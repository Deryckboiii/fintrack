FinTrack – Personal Finance Tracker

**FinTrack** is a simple web-based application built with F# that helps you track your personal financial transactions. It supports categorizing expenses, generating summaries, visualizing data with charts, and exporting to CSV.

---

Features

- Add transactions (description, amount, date, category)
- Summary by category and month (text + charts)
- Filter by date range
- Export summaries to CSV
- Delete transactions
- Search by description or category

---

Technologies

- **Backend:** F# with Giraffe, SQLite, Dapper
- **Frontend:** HTML, JavaScript, Chart.js
- **Database:** SQLite (auto-initialized)

---

Getting Started

Prerequisites
- [.NET SDK 6.0+](https://dotnet.microsoft.com/download)

Clone the repository

```bash
git clone https://github.com/yourusername/FinTrack.git
cd FinTrack

Run the application

dotnet run