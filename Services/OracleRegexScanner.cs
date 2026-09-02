using ProcedureScanner.Models;
using System.Text.RegularExpressions;

namespace ProcedureScanner.Services
{
    public static class OracleRegexScanner
    {
        private static readonly HashSet<string> SqlKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "SELECT", "WHERE", "DUAL", "SYS", "SYSTEM"
        };

        private static readonly HashSet<string> PlSqlKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "BEGIN", "END", "IF", "ELSE", "ELSIF", "THEN", "LOOP", "WHILE", "FOR",
            "EXIT", "CONTINUE", "RETURN", "RAISE", "COMMIT", "ROLLBACK", "NULL",
            "EXCEPTION", "WHEN", "PRAGMA", "GOTO"
        };

        public static ScanResult ParseScript(string rawSql)
        {
            var result = new ScanResult();
            if (string.IsNullOrWhiteSpace(rawSql)) return result;

            string normalizedSql = rawSql.Replace("\r\n", "\n").Replace('\r', '\n');
            string cleanSql = RemoveComments(normalizedSql);

            var matchName = Regex.Match(cleanSql, @"\b(?:CREATE\s+(?:OR\s+REPLACE\s+)?(?:PROCEDURE|FUNCTION))\s+([a-zA-Z0-9_\.""]+)", RegexOptions.IgnoreCase);
            result.ProcedureName = matchName.Groups[1].ToString().Replace("\"", "");

            // 1. READ Tables (FROM, JOIN)
            var readMatches = Regex.Matches(cleanSql, @"\b(?:FROM|JOIN)\s+(?!\()(.*?)(?=\b(?:WHERE|ORDER|GROUP|HAVING)\b|[;\r\n\)]|$)", RegexOptions.IgnoreCase);
            foreach (Match match in readMatches)
            {
                string table = match.Groups[1].Value.Trim().ToUpper();
                var tableGroup = table.Split(",");
                foreach (var tab in tableGroup)
                {
                    var name = tab.ToString().Trim().Split(" ")[0];
                    if (!Regex.IsMatch(name, @"^[a-zA-Z0-9_\.@]+$"))
                    {
                        continue;
                    }
                    if (name == "DUAL")
                    {
                        break;
                    }
                    if (!name.Contains("."))
                    {
                        name = "MCGDATA." + name;
                    }
                    if (IsValid(name) && !result.ReadTables.Contains(name, StringComparer.OrdinalIgnoreCase))
                        result.ReadTables.Add(name);
                }


            }

            // 2. WRITE Tables (INSERT INTO, UPDATE, DELETE FROM, MERGE INTO)
            var writeMatches = Regex.Matches(cleanSql, @"\b(?:INSERT\s+INTO|UPDATE|DELETE\s+FROM|MERGE\s+INTO|TRUNCATE\sTABLE)\s+([a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase);
            foreach (Match match in writeMatches)
            {
                string name = match.Groups[1].Value.Trim().ToUpper();
                if (!name.Contains("."))
                {
                    name = "MCGDATA." + name;
                }
                if (IsValid(name) && !result.WriteTables.Contains(name, StringComparer.OrdinalIgnoreCase))
                    result.WriteTables.Add(name);
            }

            // 3. CALLS (Matches EXEC/CALL OR direct procedure invocations like pkg.proc_name();)


            // A) Explicit calls: CALL my_proc() or EXEC my_proc()
            var explicitCalls = Regex.Matches(
                cleanSql,
                @"\b(?:CALL|EXEC|EXECUTE)\s+([a-zA-Z0-9_\.]+)",
                RegexOptions.IgnoreCase
            );

            foreach (Match match in explicitCalls)
            {
                string name = match.Groups[1].Value.Trim();
                if (!PlSqlKeywords.Contains(name) && !result.Procedures.Contains(name, StringComparer.OrdinalIgnoreCase))
                    result.Procedures.Add(name);
            }

            // B) Direct calls: my_package.my_procedure(...) or my_procedure;
            // Matches statements starting a line/after semicolon that don't contain :=
            var directCalls = Regex.Matches(
                cleanSql,
                @"(?<=;|\bBEGIN\b|\bTHEN\b|\bELSE\b|\bLOOP\b)\s*([a-zA-Z0-9_]+\.[a-zA-Z0-9_]+|[a-zA-Z0-9_]+)\s*(?:\([^;]*\))?\s*;",
                RegexOptions.IgnoreCase
            );

            foreach (Match match in directCalls)
            {
                string candidate = match.Groups[1].Value.Trim();

                // Ensure it's not a PL/SQL control word or a variable assignment
                if (!PlSqlKeywords.Contains(candidate) &&
                    !SqlKeywords.Contains(candidate) &&
                    !result.Procedures.Contains(candidate, StringComparer.OrdinalIgnoreCase))
                {
                    result.Procedures.Add(candidate);
                }
            }

            return result;
        }

        private static string RemoveComments(string sql)
        {
            string noBlock = Regex.Replace(sql, @"/\*.*?\*/", "", RegexOptions.Singleline);
            return Regex.Replace(noBlock, @"--.*$", "", RegexOptions.Multiline);
        }

        private static bool IsValid(string name) =>
            !string.IsNullOrWhiteSpace(name) && !name.StartsWith("(") && !SqlKeywords.Contains(name);
    }
}
