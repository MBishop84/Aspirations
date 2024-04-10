using Radzen;

namespace Aspirations.Web;

public class ApiClient(HttpClient httpClient, DialogService dialog)
{
    private DialogService _dialog = dialog;

    public async Task<T?> GetAsync<T>(string endpoint)
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

    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
    {
        try
        {
            return await httpClient.PostAsJsonAsync<T>(endpoint, data);
        }
        catch (Exception ex)
        {
            await _dialog.Alert(ex.StackTrace, ex.Message);
            return new HttpResponseMessage();
        }
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        try
        {
            return await httpClient.DeleteAsync(endpoint);
        }
        catch (Exception ex)
        {
            await _dialog.Alert(ex.StackTrace, ex.Message);
            return new HttpResponseMessage();
        }
    }
}
