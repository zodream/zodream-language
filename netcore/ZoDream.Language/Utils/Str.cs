using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Language.Utils
{
    public class Str
    {
        public static string Studly(string val, bool firstUpper = true)
        {
            var data = val.Split('-', '_', '.', ' ');
            var res = new StringBuilder();
            var AIndex = 'A';
            var aIndex = 'a';
            var diff = AIndex - aIndex;
            foreach (var item in data)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                var code = item[0];
                if (!firstUpper && res.Length == 0)
                {
                    if (code >= AIndex && code <= 'Z')
                    {
                        res.Append((char)(code - diff));
                    } else
                    {
                        res.Append(item);
                        continue;
                    }
                } else if (code >= aIndex && code <= 'z')
                {
                    res.Append((char)(code + diff));
                } else
                {
                    res.Append(item);
                    continue;
                }
                if (item.Length > 1)
                {
                    res.Append(item[1..]);
                }
            }
            return res.ToString();
        }
    }
}
