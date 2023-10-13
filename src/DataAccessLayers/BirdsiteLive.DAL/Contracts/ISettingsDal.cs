using System.Text.Json;
using System.Threading.Tasks;

namespace BirdsiteLive.DAL.Contracts;

public interface ISettingsDal
{
    Task<JsonElement?> Get(string key);
    Task Set(string key, JsonElement? value);
}