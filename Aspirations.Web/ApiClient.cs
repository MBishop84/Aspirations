using Radzen;

namespace Aspirations.Web;

public class ApiClient(HttpClient httpClient, DialogService dialog)
{
    private DialogService _dialog = dialog;

    public async Task<T> GetAsync<T>(string endpoint)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<T>(endpoint);
        }
        catch (Exception ex)
        {
            await _dialog.Alert(ex.StackTrace, ex.Message);
            return default;
        }
    }
}
