using System;
using System.Collections.Generic;
using System.Text;

namespace ProcedureScanner.Models
{
    public class ScanResult
    {
        public string ProcedureName { get; set; } = string.Empty;
        public List<string> WriteTables { get; set; } = new();
        public List<string> ReadTables { get; set; } = new();

        public List<string> Procedures { get; set; } = new();
        public List<string> Errors { get; set; } = new();

        public string ToMarkdown()
        {
            var sb = new StringBuilder();
            sb.AppendLine("---");
            sb.AppendLine($"name: {ProcedureName}");
            sb.AppendLine("---");
            sb.AppendLine();
            sb.AppendLine("# Direct Read");
            foreach (var item in ReadTables)
                sb.AppendLine($"- {item}");

            sb.AppendLine();
            sb.AppendLine("# Direct Write");
            foreach (var item in WriteTables)
                sb.AppendLine($"- {item}");

            sb.AppendLine();
            sb.AppendLine("# Calls");
            foreach (var item in Procedures)
                sb.AppendLine($"- {item}");

            return sb.ToString();

        }
    }
}
