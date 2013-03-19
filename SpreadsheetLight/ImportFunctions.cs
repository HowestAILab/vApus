using System;
using System.Data;

namespace SpreadsheetLight
{
    public partial class SLDocument
    {
        /// <summary>
        /// Import a System.Data.DataTable as a data source, with the first data row and first data column at a specific cell.
        /// </summary>
        /// <param name="CellReference">The cell reference, such as "A1".</param>
        /// <param name="Data">The data table.</param>
        /// <param name="IncludeHeader">True if the data table's column names are to be used in the first row as a header row. False otherwise.</param>
        public void ImportDataTable(string CellReference, DataTable Data, bool IncludeHeader)
        {
            int iRowIndex = -1;
            int iColumnIndex = -1;
            if (!SLTool.FormatCellReferenceToRowColumnIndex(CellReference, out iRowIndex, out iColumnIndex))
            {
                return;
            }

            ImportDataTable(iRowIndex, iColumnIndex, Data, IncludeHeader);
        }

        /// <summary>
        /// Import a System.Data.DataTable as a data source, with the first data row and first data column at a specific cell.
        /// </summary>
        /// <param name="RowIndex">The row index.</param>
        /// <param name="ColumnIndex">The column index.</param>
        /// <param name="Data">The data table.</param>
        /// <param name="IncludeHeader">True if the data table's column names are to be used in the first row as a header row. False otherwise.</param>
        public void ImportDataTable(int RowIndex, int ColumnIndex, DataTable Data, bool IncludeHeader)
        {
            int i, j;
            Type[] taColumns;
            string[] saColumnNames;
            int iDefaultColumnLength = 10;
            int iColumnLength = 0;

            if (Data.Columns.Count == 0)
            {
                iColumnLength = iDefaultColumnLength;
                taColumns = new Type[iColumnLength];
                saColumnNames = new string[iColumnLength];
                for (i = 0; i < iColumnLength; ++i)
                {
                    taColumns[i] = typeof(string);
                    saColumnNames[i] = string.Format("Column{0}", i + 1);
                }
            }
            else
            {
                iColumnLength = Data.Columns.Count;
                taColumns = new Type[iColumnLength];
                saColumnNames = new string[iColumnLength];
                for (i = 0; i < iColumnLength; ++i)
                {
                    taColumns[i] = Data.Columns[i].DataType;
                    saColumnNames[i] = Data.Columns[i].ColumnName;
                }
            }

            // "Optimisation" order:
            // double, float, decimal, int, long, string, DateTime,
            // short, ushort, uint, ulong, char, byte, sbyte, bool,
            // TimeSpan, byte[]

            if (IncludeHeader)
            {
                for (i = 0; i < iColumnLength; ++i)
                {
                    this.SetCellValue(RowIndex, ColumnIndex + i, saColumnNames[i]);
                }

                // get to the next row for the data part
                ++RowIndex;
            }

            int iRowCount = Data.Rows.Count;
            int iItemCount;
            int iRowIndex, iColumnIndex;
            DataRow dr;
            for (i = 0; i < iRowCount; ++i)
            {
                iRowIndex = RowIndex + i;
                dr = Data.Rows[i];
                iItemCount = dr.ItemArray.Length;
                for (j = 0; j < iItemCount; ++j)
                {
                    iColumnIndex = ColumnIndex + j;
                    if (j <= iColumnLength)
                    {
                        // in case the the data table cell is DBNull
                        // This code part sent in by Troye Stonich.
                        if (dr.ItemArray[j].GetType() == typeof(System.DBNull))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, string.Empty);
                            continue;
                        }

                        if (taColumns[j] == typeof(double))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (double)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(float))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (float)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(decimal))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (decimal)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(int))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (int)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(long))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (long)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(string))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (string)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(DateTime))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (DateTime)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(short))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (short)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(ushort))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (ushort)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(uint))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (uint)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(ulong))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (ulong)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(char))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (char)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(byte))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (byte)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(sbyte))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (sbyte)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(bool))
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, (bool)dr.ItemArray[j]);
                        }
                        else if (taColumns[j] == typeof(TimeSpan))
                        {
                            // what do you do with TimeSpans?
                            this.SetCellValue(iRowIndex, iColumnIndex, ((TimeSpan)dr.ItemArray[j]).ToString());
                        }
                        // what do you do with byte[]?
                        //else if (taColumns[j] == typeof(byte[]))
                        //{
                        //}
                        else
                        {
                            this.SetCellValue(iRowIndex, iColumnIndex, dr.ItemArray[j].ToString());
                        }
                    }
                    else
                    {
                        // this value is in the data row, but isn't defined in the columns
                        this.SetCellValue(iRowIndex, iColumnIndex, (string)dr.ItemArray[j]);
                    }
                }
            }
        }
    }
}
