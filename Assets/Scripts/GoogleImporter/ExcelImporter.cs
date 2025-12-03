using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

namespace GoogleImporter
{
    public class ExcelImporter : IImporter
    {
        private readonly string _filePath;

        public ExcelImporter(string filePath) =>
            _filePath = Path.Combine(Application.dataPath, filePath).Replace('/', Path.DirectorySeparatorChar);

        public async Task DownloadAndParseSheet(string sheetName, IGoogleSheetParser googleSheetParser)
        {
            List<string> headers = new();

            if (!File.Exists(_filePath))
            {
                Debug.LogError($"Excel file not found at: {_filePath}");
                return;
            }

            await Task.Run(async () =>
            {
                using FileStream fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
                XSSFWorkbook workbook = new XSSFWorkbook(fileStream);

                ISheet sheet = workbook.GetSheet(sheetName);
                if (sheet == null)
                {
                    Debug.LogError($"Sheet {sheetName} not found in Excel file.");
                    return;
                }

                IRow headerRow = sheet.GetRow(0);
                if (headerRow != null)
                {
                    for (int col = 0; col < headerRow.LastCellNum; col++)
                    {
                        ICell cell = headerRow.GetCell(col);
                        if (cell != null && !string.IsNullOrEmpty(cell.ToString()))
                        {
                            headers.Add(cell.ToString());
                        }
                    }
                }

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    IRow dataRow = sheet.GetRow(row);
                    if (dataRow != null)
                    {
                        for (int col = 0; col < headers.Count; col++)
                        {
                            ICell cell = dataRow.GetCell(col);
                            string cellValue = cell != null ? cell.ToString() : "";
                            await googleSheetParser.Parse(headers[col], cellValue);
                        }
                    }
                }

                Debug.Log($"Sheet {sheetName} parsed successfully.");
            });
        }

        public Task AppendRow(string sheetName, IList<object> row)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateRange(string sheetName, string range, IList<IList<object>> values)
        {
            throw new System.NotImplementedException();
        }
    }
}