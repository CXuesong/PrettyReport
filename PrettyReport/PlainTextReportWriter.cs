using System;
using System.Collections.Generic;
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
        public override void WriteLine(string s)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(params string[] cells)
        {
            throw new NotImplementedException();
        }

        public override void BeginTable(TableColumnDefinition[] cols)
        {
            throw new NotImplementedException();
        }

        public override void EndTable()
        {
            throw new NotImplementedException();
        }

        public override void WriteHorizontalLine()
        {
            throw new NotImplementedException();
        }
    }
}
