using System;
using System.Collections.Generic;

namespace SpreadsheetLight
{
    internal enum SLSheetType
    {
        Unknown = 0,
        Worksheet,
        Chartsheet,
        /// <summary>
        /// I hope I don't have to support this... This complements macro-enabled Excel files?
        /// </summary>
        DialogSheet,
        /// <summary>
        /// In future? Is this for macro-enabled Excel files?
        /// </summary>
        Macrosheet
    }

    internal class SLWorkbook
    {
        internal SLWorkbookProperties WorkbookProperties { get; set; }
        internal List<SLWorkbookView> WorkbookViews { get; set; }
        internal List<SLSheet> Sheets { get; set; }
        internal List<SLDefinedName> DefinedNames { get; set; }
        internal List<SLCalculationCell> CalculationCells { get; set; }

        internal uint PossibleTableId { get; set; }
        internal HashSet<uint> TableIds { get; set; }
        internal HashSet<string> TableNames { get; set; }

        internal SLWorkbook()
        {
            this.WorkbookProperties = new SLWorkbookProperties();
            this.WorkbookViews = new List<SLWorkbookView>();
            this.Sheets = new List<SLSheet>();
            this.DefinedNames = new List<SLDefinedName>();
            this.CalculationCells = new List<SLCalculationCell>();
            PossibleTableId = 1;
            TableIds = new HashSet<uint>();
            TableNames = new HashSet<string>();
        }

        internal void RefreshPossibleTableId()
        {
            PossibleTableId = 1;
            // possible infinite loop, but how many tables do you have anyway?
            while (this.TableIds.Contains(PossibleTableId))
            {
                ++PossibleTableId;
            }
        }

        /// <summary>
        /// Call RefreshPossibleTableId() first!
        /// </summary>
        /// <returns></returns>
        internal string GetNextPossibleTableName()
        {
            uint i = PossibleTableId;
            string sName = string.Format("Table{0}", i);
            // possible infinite loop, but how many tables do you have anyway?
            while (this.TableNames.Contains(sName))
            {
                ++i;
                sName = string.Format("Table{0}", i);
            }
            return sName;
        }

        internal bool HasTableName(string TableName)
        {
            return this.TableNames.Contains(TableName);
        }

        /// <summary>
        /// Adds a calculation cell if it doesn't already exist
        /// </summary>
        /// <param name="cc"></param>
        internal void AddCalculationCell(SLCalculationCell cc)
        {
            bool bFound = false;
            foreach (SLCalculationCell calc in this.CalculationCells)
            {
                if (calc.SheetId == cc.SheetId && calc.RowIndex == cc.RowIndex && calc.ColumnIndex == cc.ColumnIndex)
                {
                    bFound = true;
                    break;
                }
            }

            if (!bFound) this.CalculationCells.Add(cc.Clone());
        }
    }
}
