using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undefined.PrettyReport
{
    /// <summary>
    /// 实现了纯文本报告的输出。
    /// Implements interfaces for plain text report output.
    /// </summary>
    public class PlainTextReportWriter : ReportWriter
    {
        private bool needDisposeWriter = false;
        private string _IndensionText = string.Empty;
        //private int _TabSite;
        //private string tabReplacementText;

        public TextWriter Writer { get; }

        /// <summary>
        /// Determines the width per indension.
        /// </summary>
        public int IndensionSize { get; set; } = 4;

        private string IndensionText
        {
            get
            {
                if (LeftIndention*IndensionSize != _IndensionText.Length)
                    _IndensionText = new string(' ', LeftIndention*IndensionSize);
                return _IndensionText;
            }
        }

        ///// <summary>
        ///// 制表符的字符宽度。
        ///// Width of Tab character.
        ///// </summary>
        //public int TabSite
        //{
        //    get { return _TabSite; }
        //    set
        //    {
        //        _TabSite = value;
        //        tabReplacementText = new string(' ', value);
        //    }
        //}

        public override void WriteLine(string s)
        {
            Writer.Write(IndensionText);
            Writer.WriteLine(s);
        }

        #region Table
        private TableColumnDefinition[] currentTable;
        private string columnSpacingString;

        public override void WriteRow(IEnumerable cells)
        {
            if (currentTable == null) throw new InvalidOperationException("Currently not in a table.");
            Writer.Write(IndensionText);
            var i = 0;
            foreach (var cell in cells)
            {
                var cellStr = cell == null ? string.Empty : cell as string;
                if (cellStr != null) goto WRITE;
                var ifmt = cell as IFormattable;
                if (ifmt != null)
                {
                    cellStr = ifmt.ToString(currentTable[i].Format, null);
                    goto WRITE;
                }
                cellStr = cell.ToString();
                WRITE:
                Writer.Write(PadClipText(cellStr, currentTable[i].Width, currentTable[i].TextAlignment,
                    currentTable[i].OverflowBehavior == OverflowBehavior.ClipWithEllipsis));
                Writer.Write(columnSpacingString);
                i++;
            }
            Writer.WriteLine();
        }

        public override void BeginTable(int columnSpacing, params TableColumnDefinition[] cols)
        {
            if (cols == null) throw new ArgumentNullException(nameof(cols));
            if (currentTable != null) throw new InvalidOperationException("Embeded table is not supported.");
            currentTable = cols;
            columnSpacingString = new string(' ', columnSpacing);
            //Headers
            Writer.Write(IndensionText);
            foreach (var col in cols)
            {
                Writer.Write(PadClipText(col.Title, col.Width, col.TextAlignment,
                    col.OverflowBehavior == OverflowBehavior.ClipWithEllipsis));
                Writer.Write(columnSpacingString);
            }
            Writer.WriteLine();
            Writer.Write(IndensionText);
            foreach (var col in cols)
            {
                Writer.Write(new string('-', col.Width));
                Writer.Write(columnSpacingString);
            }
            Writer.WriteLine();
        }

        public override void EndTable()
        {
            if (currentTable == null) throw new InvalidOperationException("Currently not in a table.");
            currentTable = null;
        }
        #endregion

        public override void WriteHorizontalLine()
        {
            Writer.WriteLine(new string('=', 40));
        }

        public PlainTextReportWriter(TextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            Writer = writer;
        }

        public PlainTextReportWriter(string path) : this(new StreamWriter(path, false))
        {
            needDisposeWriter = true;
        }

        #region Utility

        private int TextWidth(char c)
        {
            if (c > 0xff) return 2;
            if (c == '\t') return 4;
            return 1;
        }

        /// <summary>
        /// 获取使用英文字符数量表示的文本宽度。
        /// Get text width, in measure of English letters.
        /// </summary>
        private int TextWidth(string s)
        {
            if (string.IsNullOrEmpty(s)) return 0;
            return s.Sum(c => TextWidth(c));
        }

        /// <summary>
        /// 通过在字符串的右侧填充空格，使得整个字符串的MBCS长度达到指定数值。
        /// </summary>
        private string PadClipText(string s, int length, TextAlignment alignment, bool showEllipsis)
        {

            if (string.IsNullOrWhiteSpace(s)) return new string(' ', length);
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (length == 0) return "";
            var lastspaceLength = 0;
            var spaceLength = length;
            for (var i = 0; i < s.Length; i++)
            {
                spaceLength -= TextWidth(s[i]);
                if (spaceLength < 0)
                {
                    //length 没地方了
                    //当前字符 s(i) 就不显示了
                    //显示到前一个字符为止。
                    return s.Substring(0, i) + new string(' ', lastspaceLength);
                }
                lastspaceLength = spaceLength;
            }
            switch (alignment)
            {
                case TextAlignment.Left: return s.PadRight(s.Length + spaceLength);
                case TextAlignment.Right: return s.PadLeft(s.Length + spaceLength);
                case TextAlignment.Center:
                    return s.PadLeft(s.Length + spaceLength/2).PadRight(s.Length + spaceLength);
                default:
                    throw new ArgumentOutOfRangeException(nameof(alignment));
            }
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();
            if (needDisposeWriter) Writer?.Dispose();
        }
    }
}
