using BirdsiteLive.DAL.Contracts;
using BirdsiteLive.DAL.Models;

namespace BirdsiteLive.Wikidata;

public class WikidataService
{
    private ITwitterUserDal _dal;

    private const string FediHandleQuery = """
                                       SELECT ?item ?username ?username2 ?linkcount ?itemLabel
                                       WHERE
                                       {
                                         ?item wdt:P2002 ?username.
                                         ?item wdt:P4033 ?username2.
                                               ?item wikibase:sitelinks ?linkcount .
                                         SERVICE wikibase:label { bd:serviceParam wikibase:language "[AUTO_LANGUAGE],en". } # Helps get the label in your language, if not, then en language
                                       } ORDER BY DESC(?linkcount) LIMIT 5000
                                       """;

    private const string HandleQuery = """
                                       SELECT ?item ?handle 
                                       WHERE
                                       {
                                         ?item wdt:P2002 ?handle
                                       }
                                       """;
    public WikidataService(ITwitterUserDal twitterUserDal)
    {
        _dal = twitterUserDal;
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

        var client = new HttpClient();

        Console.WriteLine("Making Wikidata Query");
        client.DefaultRequestHeaders.Add("Accept", "text/csv");
        client.DefaultRequestHeaders.Add("User-Agent", "BirdMakeup/1.0 (https://bird.makeup; https://sr.ht/~cloutier/bird.makeup/) BirdMakeup/1.0");
        var response = await client.GetAsync($"https://query.wikidata.org/sparql?query={Uri.EscapeDataString(HandleQuery)}");
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Done with Wikidata Query");


        foreach (string n in content.Split("\n"))
        {
            var s = n.Split(",");
            if (n.Length < 2)
                continue;

            var qcode = s[0].Replace("http://www.wikidata.org/entity/", "");
            var acct = s[1].ToLower().Trim().TrimEnd( '\r', '\n' );;
            //await _dal.UpdateTwitterUserFediAcctAsync(acct, fedi);
            //await _dal.UpdateUserExtradataAsync(acct, "qcode", qcode);

            if (twitterUser.Contains(acct))
            {
                Console.WriteLine($"{acct} with {qcode}");
                await _dal.UpdateUserExtradataAsync(acct, "qcode", qcode);
            }
        }
    }
    public async Task SyncFedi()
    {
        
        var twitterUser = new HashSet<string>();
        var twitterUserQuery = await _dal.GetAllTwitterUsersAsync();
        Console.WriteLine("Loading twitter users");
        foreach (SyncTwitterUser user in twitterUserQuery)
        {
            twitterUser.Add(user.Acct);
        }
        Console.WriteLine("Done loading twitter users");

        var client = new HttpClient();

        client.DefaultRequestHeaders.Add("Accept", "text/csv");
        client.DefaultRequestHeaders.Add("User-Agent", "BirdMakeup/1.0 (https://bird.makeup; https://sr.ht/~cloutier/bird.makeup/) BirdMakeup/1.0");
        var response = await client.GetAsync($"https://query.wikidata.org/sparql?query={Uri.EscapeDataString(FediHandleQuery)}");
        var content = await response.Content.ReadAsStringAsync();

        // Console.WriteLine(content);

        foreach (string n in content.Split("\n"))
        {
            var s = n.Split(",");
            if (n.Length < 2)
                continue;
            
            var acct = s[1].ToLower();
            var fedi = "@" + s[2];
            //await _dal.UpdateTwitterUserFediAcctAsync(acct, fedi);
            await _dal.UpdateUserExtradataAsync(acct, "fediaccount", fedi);
            if (twitterUser.Contains(acct))
                Console.WriteLine(fedi);
        }
    }
}