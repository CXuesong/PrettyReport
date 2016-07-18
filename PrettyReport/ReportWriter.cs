﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undefined.PrettyReport
{
    /// <summary>
    /// 预定义的段落格式。
    /// Pre-defined paragraph styles.
    /// </summary>
    public enum ParagraphStyle
    {
        
    }
    /// <summary>
    /// 提供了基本的报表输出方式。
    /// Provides basic interfaces for writing a report.
    /// </summary>
    public abstract class ReportWriter : IDisposable
    {
        public abstract void WriteLine(string s);

        public virtual void WriteLine()
            => WriteLine(null);

        public virtual void WriteLine(string format, object arg0)
            => WriteLine(string.Format(format, arg0));

        public virtual void WriteLine(string format, params object[] args)
            => WriteLine(string.Format(format, args));

        public void BeginTable(params TableColumnDefinition[] cols) => BeginTable(2, cols);
        
        public abstract void BeginTable(int columnSpacing, params TableColumnDefinition[] cols);

        public virtual void WriteRow(params object[] cells)
        {
            WriteRow((IEnumerable) cells);
        }

        public abstract void WriteRow(IEnumerable cells);

        public abstract void EndTable();

        public abstract void WriteHorizontalLine();

        public virtual void WriteHeader(string headerText, int headerLevel)
        {
            WriteLine();
            switch (headerLevel)
            {
                case 0:
                    WriteHorizontalLine();
                    WriteLine(headerText);
                    WriteHorizontalLine();
                    break;
                case 1:
                    WriteLine(headerText);
                    WriteHorizontalLine();
                    break;
                default:
                    WriteLine(headerText);
                    break;
            }
        }

        public void WriteHeader(string headerText) => WriteHeader(headerText, 0);

        public virtual void Dispose()
        {
            
        }

        /// <summary>
        /// 左缩进。
        /// </summary>
        public int LeftIndention { get; set; }

    }
}
