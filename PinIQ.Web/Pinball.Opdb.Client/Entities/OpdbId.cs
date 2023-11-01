using System.Text;
using System.Text.RegularExpressions;

namespace Pinball.OpdbClient.Entities
{
    public class OpdbId
    {
        public static Regex parseRegex = new Regex(@"^G([a-zA-Z0-9]+)(?:-M([a-zA-Z0-9]+)(?:-A([a-zA-Z0-9]+))?)?$");
        public string GroupId { get; set; } = null!;
        public string? MachineId { get; set; }
        public string? AliasId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder($"G{GroupId}");
            if (string.IsNullOrEmpty(MachineId)) return sb.ToString();
            sb.Append($"-M{MachineId}");

            if(!string.IsNullOrEmpty(AliasId))
            {
                sb.Append($"-A{AliasId}");
            }
            return sb.ToString();
        }

        public string GroupString => $"G{GroupId}";

        public static implicit operator OpdbId?(string? input)
        {
            if (input is null) return null;
            var regexResult = parseRegex.Match(input);
            if (regexResult is not { Success: true, Groups.Count: > 1 }) return null;
            var result = new OpdbId
            {
                GroupId = regexResult.Groups[1].Value,
                MachineId = regexResult.Groups[2]?.Value ?? null,
                AliasId = regexResult.Groups[3]?.Value ?? null
            };

            return result;
        }
    }
}
