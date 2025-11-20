using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using OfficeOpenXml; // Потрібен пакет EPPlus (для роботи з Excel)

namespace ExcelToSQLAdapter
{
// Цільовий інтерфейс (Target) — те, що чекає система
interface ISQLDatabase
{
void InsertData(DataTable table);
}

```
// Реалізація цільового інтерфейсу
class SQLDatabase : ISQLDatabase
{
    private string _connectionString;

    public SQLDatabase(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void InsertData(DataTable table)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            foreach (DataRow row in table.Rows)
            {
                string columns = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                string values = string.Join(", ", row.ItemArray.Select(v => $"'{v}'"));
                string query = $"INSERT INTO MyTable ({columns}) VALUES ({values})";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }
        Console.WriteLine("Дані з Excel додані до SQL бази.");
    }
}

// Старий клас (Adaptee) — читає Excel
class ExcelReader
{
    public DataTable ReadExcel(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            ExcelWorksheet sheet = package.Workbook.Worksheets[0];
            DataTable table = new DataTable();
            
            // Додаємо колонки
            foreach (var firstRowCell in sheet.Cells[1, 1, 1, sheet.Dimension.End.Column])
                table.Columns.Add(firstRowCell.Text);

            // Додаємо рядки
            for (int rowNum = 2; rowNum <= sheet.Dimension.End.Row; rowNum++)
            {
                var wsRow = sheet.Cells[rowNum, 1, rowNum, sheet.Dimension.End.Column];
                DataRow row = table.NewRow();
                int i = 0;
                foreach (var cell in wsRow)
                {
                    row[i++] = cell.Text;
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}

// Адаптер
class ExcelToSQLAdapter : ISQLDatabase
{
    private ExcelReader _excelReader;

    public ExcelToSQLAdapter(ExcelReader excelReader)
    {
        _excelReader = excelReader;
    }

    public void InsertData(DataTable table)
    {
        // Тут ми можемо трансформувати дані перед передачею у SQL, якщо потрібно
        // В даному прикладі просто передаємо
    }

    public void InsertExcelToSQL(string excelPath, ISQLDatabase sqlDb)
    {
        DataTable table = _excelReader.ReadExcel(excelPath);
        sqlDb.InsertData(table);
    }
}

class Program
{
    static void Main()
    {
        string excelPath = @"C:\path\to\file.xlsx";
        string connectionString = "Server=.;Database=MyDB;Trusted_Connection=True;";

        ExcelReader reader = new ExcelReader();
        ISQLDatabase sqlDb = new SQLDatabase(connectionString);
        ExcelToSQLAdapter adapter = new ExcelToSQLAdapter(reader);

        adapter.InsertExcelToSQL(excelPath, sqlDb);
    }
}
```

}
