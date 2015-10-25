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
        [TestMethod]
        public void TestMethod1()
        {
            using (var sw = new StringWriter())
            using (var rw = new PlainTextReportWriter(sw))
            {
                rw.WriteLine("华文卓越 AaBbCc");
                rw.WriteLine("华文卓越 AaBbCc");
                rw.WriteLine("华文卓越 AaBbCc");
                rw.WriteHorizontalLine();
                rw.LeftIndention = 4;
                rw.BeginTable(new[]
                {
                    new TableColumnDefinition("File Name", 20),
                    new TableColumnDefinition("File Size", "N", 20, TextAlignment.Right),
                    new TableColumnDefinition("Creation Time", 30, TextAlignment.Center),
                });
                foreach (var f in Directory.EnumerateFiles("."))
                {
                    var fi = new FileInfo(f);
                    rw.WriteRow(fi.Name, fi.Length, fi.CreationTime);
                }
                rw.EndTable();
                Trace.WriteLine(sw.ToString());
            }

        }
    }
}
