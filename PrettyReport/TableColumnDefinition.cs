using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undefined.PrettyReport
{
    public enum TextAlignment
    {
        Left,
        Right,
        Center
    }

    public enum OverflowBehavior
    {
        Truncate,
        TruncateWithEllipsis,
        //Wrap
    }

    /// <summary>
    /// 表示表格中一列的定义。
    /// Represents the definition of a column in the table.
    /// </summary>
    public struct TableColumnDefinition
    {
        public TableColumnDefinition(string title, int width) : this(title, null, width, TextAlignment.Left, OverflowBehavior.Truncate)
        {
        }

        public TableColumnDefinition(string title, string format, int width) : this(title, format, width, TextAlignment.Left, OverflowBehavior.Truncate)
        {
        }

        public TableColumnDefinition(string title, string format, int width, TextAlignment textAlignment) : this(title, format, width, textAlignment, OverflowBehavior.Truncate)
        {
        }

        public TableColumnDefinition(string title, string format, int width, TextAlignment textAlignment, OverflowBehavior overflowBehavior)
        {
            Title = title;
            Format = format;
            Width = width;
            TextAlignment = textAlignment;
            OverflowBehavior = overflowBehavior;
        }

        /// <summary>
        /// 表格的标题。
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// 传递给 <see cref="IFormattable.ToString(string, IFormatProvider)"/> 的格式字符串。
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// 列宽度。
        /// </summary>
        public int Width { get; }

        public TextAlignment TextAlignment { get; }

        /// <summary>
        /// 发生字符溢出时的处理方式。
        /// </summary>
        public OverflowBehavior OverflowBehavior { get; }
    }
}
