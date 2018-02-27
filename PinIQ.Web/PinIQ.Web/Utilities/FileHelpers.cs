using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Pinball.IpdbParser;
using Pinball.Web.Models;

namespace Pinball.Web.Utilities
{
    public class FileHelpers
    {
        public static async Task<(string, string)> ProcessFormFile<T>(IFormFile formFile, ModelStateDictionary modelState, Func<string, Task<IEnumerable<T>>> parser = null)
        {
            var fieldDisplayName = string.Empty;

            var property =
                typeof(CatalogUpload).GetProperty(formFile.Name.Substring(formFile.Name.IndexOf(".") + 1));

            if (property != null)
            {
                var displayAttribute = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

                if (displayAttribute != null)
                {
                    fieldDisplayName = $"{displayAttribute.Name} ";
                }
            }

            var filename = WebUtility.HtmlEncode(Path.GetFileName(formFile.FileName));

            if (formFile.ContentType.ToLower() != "text/html")
            {
                modelState.AddModelError(formFile.Name,
                    $"The {fieldDisplayName}file ({filename}) must be an html file.");
            }

            if (formFile.Length == 0)
            {
                modelState.AddModelError(formFile.Name, $"The {fieldDisplayName}file ({filename}) is empty.");
            }
            else if (formFile.Length > 5242880)
            {
                modelState.AddModelError(formFile.Name, $"The {fieldDisplayName}file ({filename}) exceeds 5MB.");
            }
            else
            {
                try
                {
                    var ansi = Encoding.GetEncoding(1252);

                    using (var reader = new StreamReader(formFile.OpenReadStream(), ansi))
                    {
                        var ansiFileContents = await reader.ReadToEndAsync();

                        if (ansiFileContents.Length > 0)
                        {
                            var ansiBytes = ansi.GetBytes(ansiFileContents);
                            var utf8Bytes = Encoding.Convert(ansi, Encoding.UTF8, ansiBytes);

                            var utf8String = Encoding.UTF8.GetString(utf8Bytes);

                            if (parser == null) return (utf8String, null);

                            var parsedResult = await parser.Invoke(utf8String);

                            if (!parsedResult.Any())
                            {
                                modelState.AddModelError(formFile.Name, $"The {fieldDisplayName}file ({filename}) was not parsed successfully.");
                                return (utf8String, null);
                            }

                            var jsonResult = JsonConvert.SerializeObject(parsedResult);

                            return (utf8String, jsonResult);
                        }

                        modelState.AddModelError(formFile.Name,
                            $"The {fieldDisplayName}file ({filename}) is empty.");
                    }
                }
                catch (Exception ex)
                {
                    modelState.AddModelError(formFile.Name, $"The {fieldDisplayName}file ({filename}) upload failed. " +
                                                            $"Error: {ex.Message}");
                }
            }

            return (string.Empty, null);
        }

        private async Task<IEnumerable<PinballTable>> ParsePinballTables(string tableHtml)
        {
            var tableParser = new PinballTableParser();
            var pinballTables = await tableParser.ParseData(tableHtml);
            var pinballTableEntities = pinballTables.Select(p => new PinballTable
            {
                Name = p.Name,
                Manufacturer = p.Manufacturer,
                PlayerCount = p.PlayerCount,
                IpdbUrl = p.IpdbUrl,
                Id = p.Id,
                SortName = p.Name.SortableTitle(),
                ReleaseDate = p.ReleaseDate
            }).ToList();
            return pinballTableEntities;
        }

        private async Task<IEnumerable<Abbreviation>> ParseAbbreviations(string abbreviationHtml)
        {
            var abbreviationParser = new AbbreviationParser();
            var pinballAbbreviations = await abbreviationParser.ParseData(abbreviationHtml);
            var abbreviationEntities = pinballAbbreviations
                .Select(a => new Abbreviation { Name = a.Name, TableID = a.TableID }).ToList();
            return abbreviationEntities;
        }
    }
}