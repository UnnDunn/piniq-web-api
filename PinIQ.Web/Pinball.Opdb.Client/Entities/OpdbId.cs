using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Pinball.OpdbClient.Entities
{
    public class OpdbId
    {
        public static Regex parseRegex = new Regex(@"^G([a-zA-Z0-9]+)(?:-M([a-zA-Z0-9]+)(?:-A([a-zA-Z0-9]+))?)?$");
        public string GroupId { get; set; }
        public string? MachineId { get; set; }
        public string? AliasId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder($"G{GroupId}");
            if(!string.IsNullOrEmpty(MachineId))
            {
                sb.Append($"-M{MachineId}");

                if(!string.IsNullOrEmpty(AliasId))
                {
                    sb.Append($"-A{AliasId}");
                }
            }
            return sb.ToString();
        }

        public string GroupString => $"G{GroupId}";

        public static implicit operator OpdbId?(string input)
        {
            var regexResult = parseRegex.Match(input);
            if(regexResult.Success && regexResult.Groups.Count > 1)
            {
                var result = new OpdbId
                {
                    GroupId = regexResult.Groups[1].Value,
                    MachineId = regexResult.Groups[2]?.Value ?? null,
                    AliasId = regexResult.Groups[3]?.Value ?? null
                };

                return result;
            }
            return null;
        }
    }
}
