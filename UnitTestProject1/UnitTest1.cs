using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Undefined.PrettyReport;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private void TestReportWriter(ReportWriter rw)
        {
            rw.WriteLine("华文卓越 AaBbCc");
            rw.WriteLine("华文卓越 AaBbCc");
            rw.WriteLine("华文卓越 AaBbCc");
            rw.WriteHeader("华文卓越 AaBbCc");
            rw.WriteHeader("华文卓越 AaBbCc", 1);
            rw.WriteHeader("华文卓越 AaBbCc", 2);
            rw.WriteHorizontalLine();
            rw.LeftIndention = 1;
            if (rw is ExcelReportWriter)
            {
                rw.BeginTable(new TableColumnDefinition("File Name", 20),
                    new TableColumnDefinition("File Size", 30),
                    new TableColumnDefinition("Creation Time", "[$-x-sysdate]dddd, mmmm dd, yyyy", 30));
            }
            else
            {
                rw.BeginTable(new TableColumnDefinition("File Name", 20),
                    new TableColumnDefinition("File Size", "N", 20, TextAlignment.Right),
                    new TableColumnDefinition("Creation Time", 30, TextAlignment.Center));
            }
            foreach (var f in Directory.EnumerateFiles("."))
            {
                var fi = new FileInfo(f);
                rw.WriteRow(fi.Name, fi.Length, fi.CreationTime);
            }
            rw.EndTable();
        }

        [TestMethod]
        public void TestMethod1()
        {
            using (var sw = new StringWriter())
            using (var rw = new PlainTextReportWriter(sw))
            {
                TestReportWriter(rw);
                Trace.WriteLine(sw.ToString());
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            const string fileName = "TestMethod2.xlsx";
            using (var rw = new ExcelReportWriter(fileName))
            {
                TestReportWriter(rw);
                rw.Stream.Flush();
            }
            Trace.WriteLine(fileName + " written.");
        }
    }
}
