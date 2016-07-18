using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetLight;

namespace Undefined.PrettyReport
{
    public class ExcelReportWriter : ReportWriter
    {
        private bool needDisposeWriter;

        public Stream Stream { get; }

        private SLDocument document;

        private string currentWorksheetName;

        private int currentRow;

        private TableColumnDefinition[] currentTable;

        private int tableFirstDataRow;

        private void CreateWorksheet(string sheetName)
        {
            bool result;
            if (currentRow == 1 && currentWorksheetName == "Sheet1")
                result = document.RenameWorksheet(currentWorksheetName, sheetName);
            else
                result = document.AddWorksheet(sheetName);
            if (!result)
                throw new ArgumentException($"Cannot create worksheet {sheetName} .", nameof(sheetName));
            currentWorksheetName = sheetName;
            currentRow = 1;
        }

        private void WriteCell(int colIndex, object value)
        {
            // Note row & col starts from 1
            bool result;
            colIndex += LeftIndention + 1;
            Debug.Assert(currentRow > 0);
            Debug.Assert(colIndex > 0);
            if (value is string)
                result = document.SetCellValue(currentRow, colIndex, (string) value);
            else if (value is bool)
                result = document.SetCellValue(currentRow, colIndex, (bool) value);
            else if (value is byte)
                result = document.SetCellValue(currentRow, colIndex, (byte) value);
            else if (value is short)
                result = document.SetCellValue(currentRow, colIndex, (short) value);
            else if (value is int)
                result = document.SetCellValue(currentRow, colIndex, (int) value);
            else if (value is long)
                result = document.SetCellValue(currentRow, colIndex, (long) value);
            else if (value is decimal)
                result = document.SetCellValue(currentRow, colIndex, (decimal) value);
            else if (value is float)
                result = document.SetCellValue(currentRow, colIndex, (float) value);
            else if (value is double)
                result = document.SetCellValue(currentRow, colIndex, (double) value);
            else if (value is DateTime)
                result = document.SetCellValue(currentRow, colIndex, (DateTime) value);
            else
                result = document.SetCellValue(currentRow, colIndex, Convert.ToString(value));
            if (!result) throw new InvalidOperationException($"Cannot set value for cell ({currentRow},{colIndex}).");
        }

        public ExcelReportWriter(Stream stream)
        {
            Stream = stream;
            // We're not trying to open an existing Excel file.
            // So please, invoke Dispose after writing all the data.
            // Create a new spreadsheet with a worksheet with the default sheet name.
            document = new SLDocument();
            needDisposeWriter = false;
            currentWorksheetName = document.GetCurrentWorksheetName();
            currentRow = 1;
        }

        public ExcelReportWriter(string path) : this(File.OpenWrite(path))
        {
            needDisposeWriter = true;
        }

        /// <summary>
        /// Determines whether to remove leading blank lines for each worksheets. (Defaults to true)
        /// </summary>
        public bool RemoveLeadingBlankLines { get; set; } = true;

        public override void WriteLine(string s)
        {
            if (currentRow == 1 && RemoveLeadingBlankLines && string.IsNullOrEmpty(s)) return;
            WriteCell(0, s);
            currentRow++;
        }

        public override void BeginTable(int columnSpacing, params TableColumnDefinition[] cols)
        {
            currentTable = cols;
            WriteRow(cols.Select(c => c.Title));
            document.ApplyNamedCellStyleToRow(currentRow - 1, SLNamedCellStyleValues.Total);
            tableFirstDataRow = currentRow;
        }

        public override void WriteRow(IEnumerable cells)
        {
            var i = 0;
            foreach (var cell in cells)
            {
                WriteCell(i, cell);
                i++;
            }
            currentRow++;
        }

        public override void EndTable()
        {
            if (currentTable == null) throw new InvalidOperationException();
            if (currentRow > tableFirstDataRow)
            {
                for (int i = 0; i < currentTable.Length; i++)
                {
                    if (!string.IsNullOrEmpty(currentTable[i].Format))
                    {
                        document.SetCellStyle(tableFirstDataRow, i + 1 + LeftIndention,
                            currentRow - 1, i + 1 + LeftIndention,
                            new SLStyle {FormatCode = currentTable[i].Format});
                    }
                    if (document.GetColumnWidth(i + 1 + LeftIndention) < currentTable[i].Width)
                        document.SetColumnWidth(i + 1 + LeftIndention, currentTable[i].Width);
                }
            }
            currentTable = null;
        }

        public override void WriteHorizontalLine()
        {
            //WriteLine(new string('-', 32));
            document.ApplyNamedCellStyleToRow(currentRow, SLNamedCellStyleValues.Accent1);
            document.SetRowHeight(currentRow, 2);
            currentRow++;
        }

        public override void WriteHeader(string headerText, int level)
        {
            if (level == 0)
                CreateWorksheet(headerText);
            else
            {
                if (level > 4) level = 4;
                WriteCell(0, headerText);
                document.ApplyNamedCellStyleToRow(currentRow, SLNamedCellStyleValues.Heading1 + level - 1);
                currentRow++;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            document.SaveAs(Stream);
            document.Dispose();
            if (needDisposeWriter) Stream.Dispose();
        }
    }
}
