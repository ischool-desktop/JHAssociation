using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using System.Data;

namespace JHSchool.Association
{
    internal class DataGridViewExport
    {
        Workbook _workbook;
        Worksheet _worksheet;
        List<int> _colIndexes;

        public DataGridViewExport(DataGridView dgv)
        {
            _workbook = new Workbook();
            _workbook.Worksheets.Clear();
            _worksheet = _workbook.Worksheets[_workbook.Worksheets.Add()];
            _worksheet.Name = "Sheet1";
            _colIndexes = new List<int>();

            int sheetRowIndex = 0;
            int sheetColIndex = 0;
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Visible == false) continue;
                _colIndexes.Add(col.Index);
                _worksheet.Cells[sheetRowIndex, sheetColIndex++].PutValue(col.HeaderText);
            }

            foreach (DataGridViewRow row in dgv.Rows)
            {
                sheetRowIndex++;
                sheetColIndex = 0;
                foreach (int colIndex in _colIndexes)
                    _worksheet.Cells[sheetRowIndex, sheetColIndex++].PutValue("" + row.Cells[colIndex].Value);
            }

            _worksheet.AutoFitColumns();
        }

        public DataGridViewExport(DataTable dgv)
        {
            _workbook = new Workbook();
            _workbook.Worksheets.Clear();
            _worksheet = _workbook.Worksheets[_workbook.Worksheets.Add()];
            _worksheet.Name = "Sheet1";
            List<string> columnName = new List<string>();
            int sheetRowIndex = 0;
            int sheetColIndex = 0;
            foreach (DataColumn col in dgv.Columns)
            {
                columnName.Add(col.ColumnName);
                _worksheet.Cells[sheetRowIndex, sheetColIndex++].PutValue(col.ColumnName);
            }

            foreach (DataRow row in dgv.Rows)
            {
                sheetRowIndex++;
                sheetColIndex = 0;
                foreach (string colInName in columnName)
                    _worksheet.Cells[sheetRowIndex, sheetColIndex++].PutValue("" + row[colInName]);
            }
            //AutoFitterOptions Options = new AutoFitterOptions();
            //Options.AutoFitMergedCells = true;
            //Options.OnlyAuto = true;
            //_worksheet.AutoFitColumns(0, sheetColIndex, Options);

            _worksheet.AutoFitColumns();
        }

        public void Save(string path)
        {
            try
            {
                // 2017/10/26 羿均修改，支援.xlsx檔案的匯入匯出。
                _workbook.Save(path);
            }
            catch (Exception ex)
            {
                MsgBox.Show("匯出失敗：" + ex.Message);
            }
        }
    }
}
