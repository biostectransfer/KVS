using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace KVSCommon.PDF
{
    /// <summary>
    /// Hilfsklasse fuer die Micra Doc Tabelle
    /// </summary>
    public static class PDFCreatorExtension
    {
        /// <summary>
        /// Erstellt dynamisch neue Reihen in der MicraDoc Tabelle
        /// </summary>
        /// <param name="table">Table</param>
        /// <param name="contents">contents</param>
        /// <returns>Row</returns>
        public static Row AddRow(this Table table, params object[] contents)
        {
            var row = table.AddRow();
            for (int i = 0; i < contents.Length; i++)
            {
                if (contents[i] != null)
                {
                    row.Cells[i].AddParagraph(contents[i].ToString());
                }
            }

            return row;
        }
    }
}
