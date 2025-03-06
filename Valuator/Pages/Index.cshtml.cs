using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConnectionMultiplexer _redis;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Page();
        }
        _logger.LogDebug(text);

        IDatabase db = _redis.GetDatabase();

        string id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;
        // TODO: (pa1) сохранить в БД (Redis) text по ключу textKey
        db.StringSet(textKey, text);

        string rankKey = "RANK-" + id;
        // TODO: (pa1) посчитать rank и сохранить в БД (Redis) по ключу rankKey
        double rank = CalculateRank(text);
        db.StringSet(rankKey, rank);

        string similarityKey = "SIMILARITY-" + id;
        // TODO: (pa1) посчитать similarity и сохранить в БД (Redis) по ключу similarityKey
        double similarity = CalculateSimilarity(db, text, id);
        db.StringSet(similarityKey, similarity);

        return Redirect($"summary?id={id}");
    }
    private double CalculateRank(string text)
    {
        double notAlphabeticCharCount = 0;

        foreach (char c in text)
        {
            if (!Char.IsLetter(c) || 
                (c < 'A' || c > 'Z' && c < 'a' || c > 'z') && 
                (c < 'А' || c > 'Я' && c < 'а' || c > 'я'))
            {
                notAlphabeticCharCount++;
            }
        }

        double rank = (double)notAlphabeticCharCount / text.Length;

        return rank;
    }

    private double CalculateSimilarity(IDatabase db, string text, string id)
    {
        var server = _redis.GetServer("localhost", 6379);
        
        var keys = server.Keys(pattern: "TEXT-*").ToList();
        
        string textKey = "TEXT-" + id;

        foreach (var key in keys)
        {

            if (key == textKey){
                continue;
            }
            if (db.StringGet(key) == text){
                return 1;
            }
        }
        return 0;
    }

}
