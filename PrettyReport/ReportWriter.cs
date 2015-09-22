using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Undefined.PrettyReport
{
    /// <summary>
    /// 提供了基本的报表输出方式。
    /// Provides basic interfaces for writing a report.
    /// </summary>
    public abstract class ReportWriter
    {
        public abstract void WriteLine(string s);

        public abstract void WriteLine(params string[] cells);

        public abstract void BeginTable(TableColumnDefinition[] cols);

        public abstract void EndTable();

        public abstract void WriteHorizontalLine();
    }
}
