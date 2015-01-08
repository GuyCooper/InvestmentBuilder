using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace ExcelAccountsManager
{
    public static class WorksheetHelpers
    {
        //try and get a double value from the reference row and column
        public static bool GetValueDouble(this Microsoft.Office.Interop.Excel._Worksheet sheet, string col, int row, ref double dVal)
        {
            var obj = sheet.get_Range(col + row).Value;
            if (obj != null && obj is double)
            {
                dVal = (double)obj;
                return true;
            }
            return false;
        }

        //try and get a int value from the reference row and column
        public static bool GetValueInt(this Microsoft.Office.Interop.Excel._Worksheet sheet, string col, int row, ref int iVal)
        {
            var obj = sheet.get_Range(col + row).Value;
            if (obj != null && obj is int)
            {
                iVal = (int)obj;
                return true;
            }
            return false;
        }

        //try and get a datetime value from the reference row and column
        public static DateTime? GetValueDateTime(this Microsoft.Office.Interop.Excel._Worksheet sheet, string col, int row)
        {
            var obj = sheet.get_Range(col + row).Value;
            if (obj != null && obj is DateTime)
            {
                return (DateTime)obj;
            }
            return null;
        }

        //determine if referenced cell is populated
        public static bool IsCellPopulated(this Microsoft.Office.Interop.Excel._Worksheet sheet, string col, int row)
        {
            return sheet.get_Range(col + row).Value != null;
        }

        //return the row number of the cell containing the specified value in the specified column
        public static bool TryGetRowReference(this _Worksheet sheet, string column, string val, ref int row)
        {
            int maxRows = sheet.UsedRange.Rows.Count;
            for (row = 1; row <= maxRows; row++)
            {
                var cell = sheet.get_Range(column + row).Value as string;
                if (cell != null && cell.ToUpper() == val.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }

        //try and return the last populated row in the specified column from this sheet starting at specified row
        public static int GetLastPopulatedRow(this _Worksheet sheet, string column, int start)
        {
            int count = start;
            int row = start;
            while (sheet.IsCellPopulated(column, count))
            {
                row = count;
                count++;
            }
            return row;
        }

        public static _Worksheet FindWorksheet(this _Workbook book, string name)
        {
            foreach (_Worksheet sheet in book.Worksheets)
            {
                if (sheet.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return sheet;
                }
            }
            return null;
        }

        public static bool GetUnitValueFromAssetSheet(this _Worksheet sheet, ref double unitValue)
        {
            int row = 0; ;
            if (sheet.TryGetRowReference("I", "VALUE PER UNIT", ref row))
            {
                unitValue = (double)sheet.get_Range("K" + row).Value;
                return true;
            }
            return false;
        }
    }
}
