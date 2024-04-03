using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

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
                await DialogService.Alert(ex.Message, "Transform Error", new AlertOptions()
                {
                    OkButtonText = "OK",
                    ShowClose = false
                });
            }
        }

        /// <summary>
        /// Clears the selected public field.
        /// </summary>
        /// <param name="field"></param>
        private async Task ClearField(string field)
        {
            try
            {
                GetType().GetField(field)?.SetValue(this, default);
            }
            catch (Exception ex)
            {
                await DialogService.Alert(ex.StackTrace, ex.Message);
            }
        }
        #endregion
    }
}
