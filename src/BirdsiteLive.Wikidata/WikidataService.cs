using BirdsiteLive.DAL.Contracts;
using BirdsiteLive.DAL.Models;

namespace BirdsiteLive.Wikidata;

public class WikidataService
{
    private ITwitterUserDal _dal;
    private readonly string _endpoint;
    private HttpClient _client = new ();

    // notable work could be interesting: https://www.wikidata.org/wiki/Property:P800
    private const string HandleQuery = """
                                       SELECT ?item ?handle ?fediHandle ?itemLabel ?itemDescription
                                       WHERE
                                       {
                                         ?item wdt:P2002 ?handle
                                          OPTIONAL {?item wdt:P4033 ?fediHandle} 
                                          SERVICE wikibase:label { bd:serviceParam wikibase:language "en". }
                                       } # LIMIT 10 
                                       """;
    public WikidataService(ITwitterUserDal twitterUserDal)
    {
        _dal = twitterUserDal;

        string? key = Environment.GetEnvironmentVariable("semantic");
        if (key is null)
        {
            _endpoint = "https://query.wikidata.org/sparql?query=";
        }
        else
        {
            _endpoint = "https://query.semantic.builders/sparql?query=";
            _client.DefaultRequestHeaders.Add("api-key", key);   
        }
        _client.DefaultRequestHeaders.Add("Accept", "text/csv");
        _client.DefaultRequestHeaders.Add("User-Agent", "BirdMakeup/1.0 (https://bird.makeup; https://sr.ht/~cloutier/bird.makeup/) BirdMakeup/1.0");
        _client.Timeout = Timeout.InfiniteTimeSpan;
    }

    public async Task SyncQcodes()
    {
        
        var twitterUser = new HashSet<string>();
        var twitterUserQuery = await _dal.GetAllTwitterUsersAsync();
        Console.WriteLine("Loading twitter users");
        foreach (SyncTwitterUser user in twitterUserQuery)
        {
            twitterUser.Add(user.Acct);
        }
        Console.WriteLine($"Done loading {twitterUser.Count} twitter users");


        Console.WriteLine("Making Wikidata Query to " + _endpoint);
        var response = await _client.GetAsync(_endpoint + Uri.EscapeDataString(HandleQuery));
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Done with Wikidata Query");


        foreach (string n in content.Split("\n"))
        {
            var s = n.Split(",");
            if (n.Length < 2)
                continue;

            var qcode = s[0].Replace("http://www.wikidata.org/entity/", "");
            var acct = s[1].ToLower().Trim().TrimEnd( '\r', '\n' );
            var fediHandle = s[2];
            var label = s[3];
            var description = s[4].Trim().TrimEnd( '\r', '\n');
            //await _dal.UpdateTwitterUserFediAcctAsync(acct, fedi);
            //await _dal.UpdateUserExtradataAsync(acct, "qcode", qcode);

            if (twitterUser.Contains(acct))
            {
                Console.WriteLine($"{acct} with {qcode}");
                await _dal.UpdateUserExtradataAsync(acct, "qcode", qcode);
                if (fediHandle != "")
                    await _dal.UpdateUserExtradataAsync(acct, "fedihandle", fediHandle);
                if (label != "")
                    await _dal.UpdateUserExtradataAsync(acct, "label", label);
                if (description != "")
                    await _dal.UpdateUserExtradataAsync(acct, "description", description);
            }
        }
    }
}