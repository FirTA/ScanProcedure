using ProcedureScanner.Models;
using System.Text.RegularExpressions;

namespace ProcedureScanner.Services
{
    public static class PostgresRegexScanner
    {
        private static readonly HashSet<string> sqlKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "SELECT", "WHERE", "LATERAL", "ONLY", "UNNEST", "PG_CATALOG", "INFORMATION_SCHEMA"
        };

        public static ScanResult ParseScript(string rawSql)
        {
            var result = new ScanResult();

            if (string.IsNullOrWhiteSpace(rawSql))
                return result;

            string normalizedSql = rawSql.Replace("\r\n", "\n").Replace('\r', '\n');
            // 1. Clean SQL (strip single-line -- and multi-line /* */ comments)
            string cleanSql = RemoveComments(normalizedSql);

            // 2. Extract Tables (FROM, JOIN, INTO, UPDATE)
            /*
            var tableMatches = Regex.Matches(
                cleanSql,
                @"\b(?:FROM|JOIN|INTO|UPDATE)\s+([a-zA-Z0-9_\.]+)",
                RegexOptions.IgnoreCase
            );*/

            string procedureName = Regex.Match(cleanSql, @"\b(?:CREATE\s+(?:OR\s+REPLACE\s+)?(?:PROCEDURE|FUNCTION))\s+([a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase).ToString();
            result.ProcedureName = procedureName;
            var readMatches = Regex.Matches(cleanSql, @"\b(?:FROM|JOIN)\s+([a-zA-Z0-9_\.]+)", RegexOptions.IgnoreCase);

            foreach (Match match in readMatches)
            {
                string tableName = match.Groups[1].Value.Trim().ToUpper();
                if (!tableName.Contains("."))
                {
                    tableName = "MCGDATA." + tableName;
                }
                if (IsValidObject(tableName) && !ContainsIgnoreCase(result.ReadTables, tableName))
                {
                    result.ReadTables.Add(tableName);
                }
            }

            var writeMatches = Regex.Matches(
                cleanSql,
                @"\b(?:INSERT\s+INTO|UPDATE|DELETE\s+FROM)\s+([a-zA-Z0-9_\.]+)",
                RegexOptions.IgnoreCase
            );

            foreach (Match match in writeMatches)
            {
                string tableName = match.Groups[1].Value.Trim().ToUpper();
                if (!tableName.Contains("."))
                {
                    tableName = "MCGDATA." + tableName;
                }
                if (IsValidObject(tableName) && !ContainsIgnoreCase(result.WriteTables, tableName))
                {
                    result.WriteTables.Add(tableName);
                }
            }
            /*foreach (Match match in tableMatches)
            {
                string tableName = match.Groups[1].Value.Trim();
                if (IsValidObject(tableName) && !ContainsIgnoreCase(result.Tables, tableName))
                {
                    result.Tables.Add(tableName);
                }
            }*/

            // 3. Extract Routines/Procedures (CALL, PERFORM)
            var routineMatches = Regex.Matches(
                cleanSql,
                @"\b(?:CALL|PERFORM)\s+([a-zA-Z0-9_\.]+)",
                RegexOptions.IgnoreCase
            );

            foreach (Match match in routineMatches)
            {
                string routineName = match.Groups[1].Value.Trim().ToUpper();
                if (!routineName.Contains("."))
                {
                    routineName = "MCGDATA." + routineName;
                }
                if (IsValidObject(routineName) && !ContainsIgnoreCase(result.Procedures, routineName))
                {
                    result.Procedures.Add(routineName);
                }
            }

            return result;
        }

        private static string RemoveComments(string sql)
        {
            string noBlockComments = Regex.Replace(sql, @"/\*.*?\*/", "", RegexOptions.Singleline);
            return Regex.Replace(sql, @"--.*$", "", RegexOptions.Multiline);
        }

        private static bool IsValidObject(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && !name.StartsWith("(") && !sqlKeywords.Contains(name);
        }
        private static bool ContainsIgnoreCase(List<string> list, string value)
        {
            return list.Any(item => item.Equals(value, StringComparison.OrdinalIgnoreCase));
        }
    }
}
