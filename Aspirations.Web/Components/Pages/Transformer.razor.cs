using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Radzen;
using System.Globalization;
using System.Text;

namespace Aspirations.Web.Components.Pages
{
    public partial class Transformer
    {
        #region Injected Services

        [Inject]
        private IJSRuntime JS { get; init; }

        [Inject]
        private DialogService DialogService { get; init; }

        #endregion

        #region Feilds

        private string _input, _output;
        private int _height = 1000;
        private bool _dynamic = false;
        public string? Split, Join, BoundAll, BoundEach;

        #endregion

        #region Methods

        /// <summary>
        /// Overrides the default behavior of the OnAfterRenderAsync method.
        /// </summary>
        /// <param name="firstRender"></param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            _height = await JS.InvokeAsync<int>("GetHeight");
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Transforms the input string to the desired output based on variables.
        /// </summary>
        /// <returns>Sets the output</returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task Transform()
        {
            try
            {
                if (!string.IsNullOrEmpty(BoundAll) && BoundAll.Split('.').Length != 2)
                {
                    throw new ArgumentException("Bound-All must be separated by a period(.)");
                }
                if (!string.IsNullOrEmpty(BoundEach) && BoundEach.Split('.').Length != 2)
                {
                    throw new ArgumentException("Bound-Each must be separated by a period(.)");
                }
                var split = Split?.Replace("\\n", "\n").Replace("\\t", "\t") ?? string.Empty;
                var join = Join?.Replace("\\n", "\n").Replace("\\t", "\t") ?? string.Empty;

                var frontBracket = string.IsNullOrEmpty(BoundAll)
                    ? string.Empty
                    : BoundAll.Split('.')[0];
                var endBracket = string.IsNullOrEmpty(BoundAll)
                    ? string.Empty
                    : BoundAll.Split('.')[1];
                var frontParentheses = string.IsNullOrEmpty(BoundEach)
                    ? string.Empty
                    : BoundEach.Split('.')[0];
                var endParentheses = string.IsNullOrEmpty(BoundEach)
                    ? string.Empty
                    : BoundEach.Split('.')[1];

                _output = $"{frontBracket}{string.Join(join, _input?.Split(split).Select(x =>
                {
                    return _dynamic switch
                    {
                        true => int.TryParse(x, out var i) || x.Equals("null", StringComparison.OrdinalIgnoreCase)
                            ? $"{x}" : $"{frontParentheses}{x}{endParentheses}",
                        false => $"{frontParentheses}{x}{endParentheses}"
                    };
                }) ?? [])}{endBracket}";
            }
            catch (Exception ex)
            {
                await DialogService.OpenAsync<CustomDialog>(
                    "Transform Error",
                    new Dictionary<string, object>
                    {
                        { "Type", Enums.DialogTypes.Error },
                        { "Message", $"{ex.Message}\n\n{ex.StackTrace}" }
                    },
                    new DialogOptions()
                    {
                        Width = "50vw",
                        Height = "50vh"
                    });
            }
        }

        /// <summary>
        /// Clears the selected public field.
        /// </summary>
        /// <param name="field"></param>
        private void ClearField(string field) =>
            GetType().GetField(field)?.SetValue(this, default);

        /// <summary>
        /// Converts the results of a SQL query to a C# class.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Shows the query to run to get the appropriate results for the input on error.
        /// </exception>
        private async Task ClassFromQuery()
        {
            try
            {
                if (string.IsNullOrEmpty(_input))
                {
                    throw new ArgumentException("Input is Empty");
                }
                var lines = _input.Split("\n");
                var result = new StringBuilder();

                foreach (var line in lines)
                {
                    var properties = line.Split("\t");
                    result.Append($"///<summary>\n/// Gets/Sets the {properties[0]}.\n///</summary>\n");
                    switch (properties.Length)
                    {
                        case 1:
                            throw new ArgumentException("Insufficient arguments.\n\n");
                        case 2:
                            result.Append(properties[1] switch
                            {
                                var a when a.Contains("int", StringComparison.OrdinalIgnoreCase) =>
                                    $"public int {properties[0]} {{ get; set; }}\n\n",
                                var b when b.Contains("date", StringComparison.OrdinalIgnoreCase) =>
                                    $"public DateTime {properties[0]} {{ get; set; }}\n\n",
                                var b when b.Contains("bit", StringComparison.OrdinalIgnoreCase) =>
                                    $"public bool {properties[0]} {{ get; set; }}\n\n",
                                var b when b.Contains("unique", StringComparison.OrdinalIgnoreCase) =>
                                    $"public Guid {properties[0]} {{ get; set; }}\n\n",
                                _ => $"public string {properties[0]} {{ get; set; }}\n\n"
                            });
                            break;
                        case 3:
                            var isNullable = properties[2].Equals("YES", StringComparison.OrdinalIgnoreCase) ? "?" : "";
                            result.Append(properties[1] switch
                            {
                                var a when a.Contains("int", StringComparison.OrdinalIgnoreCase) =>
                                    $"public int{isNullable} {properties[0]} {{ get; set; }}\n\n",
                                var b when b.Contains("date", StringComparison.OrdinalIgnoreCase) =>
                                    $"public DateTime{isNullable} {properties[0]} {{ get; set; }}\n\n",
                                var b when b.Contains("bit", StringComparison.OrdinalIgnoreCase) =>
                                    $"public bool{isNullable} {properties[0]} {{ get; set; }}\n\n",
                                var b when b.Contains("unique", StringComparison.OrdinalIgnoreCase) =>
                                    $"public Guid{isNullable} {properties[0]} {{ get; set; }}\n\n",
                                _ => $"public string {properties[0]} {{ get; set; }}\n\n"
                            });
                            break;
                    }
                }
                _output = result.ToString();
            }
            catch (Exception ex)
            {
                await DialogService.OpenAsync<CustomDialog>(
                    "ClassFromQuery Error",
                    new Dictionary<string, object>
                    {
                        { "Type", Enums.DialogTypes.ClassFromQuery },
                        { "Message", $"{ex.Message}\n\n{ex.StackTrace}" }
                    },
                    new DialogOptions()
                    {
                        Width = "50vw",
                        Height = "50vh"
                    });
            }
        }

        /// <summary>
        /// Converts a JSON object to a C# class.
        /// </summary>
        /// <remarks>Useful for making objects from api responses.</remarks>
        /// <exception cref="ArgumentException"></exception>
        private async Task JsonToClass()
        {
            try
            {
                if (string.IsNullOrEmpty(_input))
                    throw new ArgumentException("Input is Empty");
                if (!_input.StartsWith("{"))
                    _input = $"{{{_input}}}";
                dynamic? jsonObject = JsonConvert.DeserializeObject(_input);
                if (jsonObject == null) return;
                var result = new StringBuilder();
                List<string> nestedItems = [];
                foreach (var i in jsonObject)
                {
                    var nested = false;
                    foreach (var x in i)
                    {
                        if (!x.HasValues) continue;
                        var objectName = $"{i.Name}";
                        result.AppendFormat(
                            "///<summary>\n/// Gets/Sets the {0}.\n///</summary>\npublic {1} {0} {{ get; set; }}",
                            i.Name,
                            $"{char.ToUpper(objectName[0])}{objectName[1..]}");
                        nestedItems.Add($"{char.ToUpper(objectName[0])}{objectName[1..]}");
                        nested = true;
                    }

                    if (nested)
                    {
                        result.Append("}\n");
                        continue;
                    }
                    result.AppendFormat("///<summary>\n/// Gets/Sets the {0}.\n///</summary>\n", i.Name);
                    result.Append($"{i.Value}" switch
                    {
                        var a when int.TryParse(a, out var itemInt) =>
                            $"public int {i.Name} {{ get; set; }}\n\n",
                        var b when DateTime.TryParse(b, CultureInfo.InvariantCulture, DateTimeStyles.None, out var itemDate) =>
                            $"public DateTime? {i.Name} {{ get; set; }}\n\n",
                        var c when bool.TryParse(c, out var iBool) =>
                            $"public bool {i.Name} {{ get; set; }}\n\n",
                        _ => $"public string {i.Name} {{ get; set; }} = string.Empty;\n\n"
                    });
                }
                _output = result.ToString();
                if(nestedItems.Any())
                    throw new ArgumentException(
                        $"Nested objects are not fully supported.\n\n{string.Join("\n\t",nestedItems)}\n\nHave been added as objects.");
            }
            catch (Exception ex)
            {
                await DialogService.OpenAsync<CustomDialog>(
                    "JsonToClass Error",
                    new Dictionary<string, object>
                    {
                        { "Type", Enums.DialogTypes.Error },
                        { "Message", $"{ex.Message}\n{ex.StackTrace}" }
                    },
                    new DialogOptions()
                    {
                        Width = "50vw",
                        Height = "50vh"
                    });
            }
        }

        /// <summary>
        /// Converts XML to a C# class.
        /// </summary>
        /// <remarks>Not yet Implemented</remarks>
        private async Task XmlToClass()
        {
            await DialogService.OpenAsync<CustomDialog>(
                "XmlToClass not implemented",
                new Dictionary<string, object>
                {
                    { "Type", Enums.DialogTypes.Info },
                    { "Message", "Sorry!\nXml->Class is not yet implemented." }
                },
                new DialogOptions()
                {
                    Width = "50vw",
                    Height = "50vh"
                });
        }
        #endregion
    }
}
