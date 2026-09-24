using System.Net;
using HtmlAgilityPack;
using Yukari.Core.Models;
using Yukari.Core.Sources;
using Yukari.Core.Sources.Http;

namespace Yukari.Plugin.MangaLivre;

[ComicSourceMetadata(
    "MangaLivre",
    "1.0.0+core2.5.0",
    "https://github.com/Yukari-App/Plugin.MangaLivre/releases",
    "https://mangalivre.blog/favicon.ico",
    ""
)]

public class MangaLivreSource : IComicSource, IRequiresHttpClient
{
    private const int DefaultPageSize = 32;
    private const string SourceLanguage = "pt-BR";

    private static IReadOnlyList<Filter>? _filters;
    private static IReadOnlyDictionary<string, string>? _languages;

    public IReadOnlyList<Filter> Filters =>
        _filters ??= [
            new Filter(
                Key: "status-select",
                DisplayName: "Status",
                [
                    new FilterOption("cancelado", "cancelado", false),
                    new FilterOption("completo", "completo", false),
                    new FilterOption("em-andamento", "em-andamento", false),
                    new FilterOption("em-lancamento", "em-lancamento", false),
                    new FilterOption("hiato", "hiato", false),
                ],
                AllowMultiple: true
            ),

            new Filter(
                Key: "genre-select",
                DisplayName: "Genre",
                [
                    new FilterOption("4-koma", "4-koma", false),
                    new FilterOption("acao", "acao", false),
                    new FilterOption("action", "action", false),
                    new FilterOption("adaptation", "adaptation", false),
                    new FilterOption("adulto", "adulto", false),
                    new FilterOption("adventure", "adventure", false),
                    new FilterOption("ahegao", "ahegao", false),
                    new FilterOption("aliens", "aliens", false),
                    new FilterOption("animals", "animals", false),
                    new FilterOption("anthology", "anthology", false),
                    new FilterOption("apocalipse", "apocalipse", false),
                    new FilterOption("artes-marciais", "artes-marciais", false),
                    new FilterOption("aventura", "aventura", false),
                    new FilterOption("award-winning", "award-winning", false),
                    new FilterOption("boquete", "boquete", false),
                    new FilterOption("boys-love", "boys-love", false),
                    new FilterOption("bunda-grande", "bunda-grande", false),
                    new FilterOption("colegial", "colegial", false),
                    new FilterOption("comedia", "comedia", false),
                    new FilterOption("comedy", "comedy", false),
                    new FilterOption("comic", "comic", false),
                    new FilterOption("cooking", "cooking", false),
                    new FilterOption("cotidiano", "cotidiano", false),
                    new FilterOption("creampie", "creampie", false),
                    new FilterOption("crime", "crime", false),
                    new FilterOption("crossdressing", "crossdressing", false),
                    new FilterOption("delinquents", "delinquents", false),
                    new FilterOption("demonios", "demonios", false),
                    new FilterOption("demons", "demons", false),
                    new FilterOption("doujinshi", "doujinshi", false),
                    new FilterOption("drama", "drama", false),
                    new FilterOption("dungeon", "dungeon", false),
                    new FilterOption("ecchi", "ecchi", false),
                    new FilterOption("erotica", "erotica", false),
                    new FilterOption("escola", "escola", false),
                    new FilterOption("escolar", "escolar", false),
                    new FilterOption("esportes", "esportes", false),
                    new FilterOption("fantasia", "fantasia", false),
                    new FilterOption("fantasy", "fantasy", false),
                    new FilterOption("ficcao-cientifica", "ficcao-cientifica", false),
                    new FilterOption("full-color", "full-color", false),
                    new FilterOption("genderswap", "genderswap", false),
                    new FilterOption("ghosts", "ghosts", false),
                    new FilterOption("girls-love", "girls-love", false),
                    new FilterOption("gore", "gore", false),
                    new FilterOption("grupal", "grupal", false),
                    new FilterOption("guerra", "guerra", false),
                    new FilterOption("gyaru", "gyaru", false),
                    new FilterOption("harem", "harem", false),
                    new FilterOption("hentai", "hentai", false),
                    new FilterOption("historical", "historical", false),
                    new FilterOption("historico", "historico", false),
                    new FilterOption("horror", "horror", false),
                    new FilterOption("incest", "incest", false),
                    new FilterOption("incesto", "incesto", false),
                    new FilterOption("isekai", "isekai", false),
                    new FilterOption("iyashikey", "iyashikey", false),
                    new FilterOption("jogo", "jogo", false),
                    new FilterOption("jogos", "jogos", false),
                    new FilterOption("josei", "josei", false),
                    new FilterOption("lingerie", "lingerie", false),
                    new FilterOption("loli", "loli", false),
                    new FilterOption("long-strip", "long-strip", false),
                    new FilterOption("mafia", "mafia", false),
                    new FilterOption("magia", "magia", false),
                    new FilterOption("magic", "magic", false),
                    new FilterOption("magical-girls", "magical-girls", false),
                    new FilterOption("martial-arts", "martial-arts", false),
                    new FilterOption("masturbacao", "masturbacao", false),
                    new FilterOption("mecha", "mecha", false),
                    new FilterOption("medical", "medical", false),
                    new FilterOption("milf", "milf", false),
                    new FilterOption("military", "military", false),
                    new FilterOption("misterio", "misterio", false),
                    new FilterOption("monster-girls", "monster-girls", false),
                    new FilterOption("monsters", "monsters", false),
                    new FilterOption("monstros", "monstros", false),
                    new FilterOption("murim", "murim", false),
                    new FilterOption("music", "music", false),
                    new FilterOption("mystery", "mystery", false),
                    new FilterOption("ninja", "ninja", false),
                    new FilterOption("oculos", "oculos", false),
                    new FilterOption("office-workers", "office-workers", false),
                    new FilterOption("official-colored", "official-colored", false),
                    new FilterOption("oneshot", "oneshot", false),
                    new FilterOption("oral", "oral", false),
                    new FilterOption("paizuri", "paizuri", false),
                    new FilterOption("parodia", "parodia", false),
                    new FilterOption("peak-arte", "peak-arte", false),
                    new FilterOption("peitoes", "peitoes", false),
                    new FilterOption("philosophical", "philosophical", false),
                    new FilterOption("police", "police", false),
                    new FilterOption("pornhwa", "pornhwa", false),
                    new FilterOption("post-apocalyptic", "post-apocalyptic", false),
                    new FilterOption("prostituicao", "prostituicao", false),
                    new FilterOption("psicologico", "psicologico", false),
                    new FilterOption("psychological", "psychological", false),
                    new FilterOption("raio-x", "raio-x", false),
                    new FilterOption("realidade-virtual", "realidade-virtual", false),
                    new FilterOption("reencarnacao", "reencarnacao", false),
                    new FilterOption("regressao", "regressao", false),
                    new FilterOption("reincarnation", "reincarnation", false),
                    new FilterOption("retorno", "retorno", false),
                    new FilterOption("reverse-harem", "reverse-harem", false),
                    new FilterOption("romance", "romance", false),
                    new FilterOption("samurai", "samurai", false),
                    new FilterOption("school-life", "school-life", false),
                    new FilterOption("sci-fi", "sci-fi", false),
                    new FilterOption("seinen", "seinen", false),
                    new FilterOption("self-published", "self-published", false),
                    new FilterOption("sexual-violence", "sexual-violence", false),
                    new FilterOption("shota", "shota", false),
                    new FilterOption("shoujo", "shoujo", false),
                    new FilterOption("shounen", "shounen", false),
                    new FilterOption("shounen-ai", "shounen-ai", false),
                    new FilterOption("sistema", "sistema", false),
                    new FilterOption("slice-of-life", "slice-of-life", false),
                    new FilterOption("sobrenatural", "sobrenatural", false),
                    new FilterOption("sports", "sports", false),
                    new FilterOption("super-poderes", "super-poderes", false),
                    new FilterOption("superhero", "superhero", false),
                    new FilterOption("supernatural", "supernatural", false),
                    new FilterOption("survival", "survival", false),
                    new FilterOption("suspense", "suspense", false),
                    new FilterOption("tela-de-sistema", "tela-de-sistema", false),
                    new FilterOption("thriller", "thriller", false),
                    new FilterOption("time-travel", "time-travel", false),
                    new FilterOption("tomboy", "tomboy", false),
                    new FilterOption("traditional-games", "traditional-games", false),
                    new FilterOption("tragedia", "tragedia", false),
                    new FilterOption("tragedy", "tragedy", false),
                    new FilterOption("vampires", "vampires", false),
                    new FilterOption("viagem-no-tempo", "viagem-no-tempo", false),
                    new FilterOption("vida-escolar", "vida-escolar", false),
                    new FilterOption("villainess", "villainess", false),
                    new FilterOption("virgem", "virgem", false),
                    new FilterOption("virtual-reality", "virtual-reality", false),
                    new FilterOption("web-comic", "web-comic", false),
                    new FilterOption("wuxia", "wuxia", false),
                    new FilterOption("yaoi", "yaoi", false),
                    new FilterOption("zombies", "zombies", false),
                ],
                AllowMultiple: true
            ),

            new Filter(
                Key: "rating-select",
                DisplayName: "rating",
                [
                    new FilterOption("0", "0", false),
                    new FilterOption("1", "1", false),
                    new FilterOption("2", "2", false),
                    new FilterOption("3", "3", false),
                    new FilterOption("4", "4", false),
                    new FilterOption("5", "5", false),
                    new FilterOption("6", "6", false),
                    new FilterOption("7", "7", false),
                    new FilterOption("8", "8", false),
                    new FilterOption("9", "9", false),
                    new FilterOption("10", "10", false),
                ],
                AllowMultiple: true
            ),
        ];

    public IReadOnlyDictionary<string, string> Languages =>
        _languages ??= new Dictionary<string, string> { { "pt-BR", "Português (BR)" } };
    private const string BaseUrl = "https://mangalivre.blog";

    private ISharedHttpClient? _httpClient;

    public void SetHttpClient(ISharedHttpClient httpClient) => _httpClient = httpClient;

    public async Task<IReadOnlyList<Comic>> SearchAsync(
        string query,
        IReadOnlyDictionary<string, IReadOnlyList<string>> filters,
        int page = 1,
        CancellationToken ct = default
    )
    {
        string searchUrl = $"{BaseUrl}?s={Uri.EscapeDataString(query)}";

        var html = await GetHTMLAsync(searchUrl, ct);
        if (html == null)
            return Array.Empty<Comic>();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var mangaCards = doc.DocumentNode.SelectNodes("//div[contains(@class, 'manga-card')]");
        if (mangaCards is not { Count: > 0 })
            return Array.Empty<Comic>();

        var comics = new List<Comic>();
        foreach (var mangaCard in mangaCards)
        {
            var linkNode = mangaCard.SelectSingleNode(".//a[contains(@class, 'manga-card-link')]");

            string? comicUrl = linkNode?.GetAttributeValue("href", null);
            if (string.IsNullOrEmpty(comicUrl))
                continue;

            string? comicId = ExtractIdFromUrl(comicUrl);
            if (string.IsNullOrEmpty(comicId))
                continue;

            var titleNode = mangaCard.SelectSingleNode(".//h3[contains(@class, 'manga-card-title')]");
            string title = titleNode?.InnerText.Trim() ?? "Unknown Title";

            var imageNode = mangaCard.SelectSingleNode(".//img[contains(@class, 'manga-cover-img')]");
            string? coverUrl = imageNode?.GetAttributeValue("src", null);

            comics.Add(
                new Comic(
                    Id: comicId,
                    ComicUrl: comicUrl,
                    Title: title,
                    Author: null,
                    Description: null,
                    Tags: [],
                    Year: null,
                    CoverImageUrl: coverUrl,
                    Langs: ["pt-BR"]
                )
            );

        }
        return comics;
    }

    public async Task<IReadOnlyList<Comic>> GetTrendingAsync(
        IReadOnlyDictionary<string, IReadOnlyList<string>> filters,
        int page = 1,
        CancellationToken ct = default
    )
    {
        var trendingFilters = new Dictionary<string, IReadOnlyList<string>>(filters)
        {
            ["order"] = ["Descending"],
            ["sort"] = ["Popularity"],
        };

        return await SearchAsync(string.Empty, trendingFilters, page, ct);
    }

    public async Task<Comic?> GetDetailsAsync(string comicId, CancellationToken ct = default)
    {
        string detailsUrl = $"{BaseUrl}/manga/{comicId}";

        var html = await GetHTMLAsync(detailsUrl, ct);
        if (html == null)
            return null;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var titleNode = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'manga-title')]");
        string title = titleNode?.InnerText.Trim() ?? "Unknown Title";

        var authorNodes = doc.DocumentNode.SelectNodes("//div[@class='manga-meta-grid']//div[contains(@class, 'manga-meta-item')][span[@class='meta-label' and contains(., 'Autor:')]]/span[@class='meta-value']");
        string[] authors =
            authorNodes?.Select(a => a.InnerText.Trim()).ToArray() ?? Array.Empty<string>();

        var tagNodes = doc.DocumentNode.SelectNodes("//span[contains(@class, 'manga-tag')]//a");
        string[] tags =
            tagNodes?.Select(t => t.InnerText.Trim()).ToArray() ?? Array.Empty<string>();

        var statusNode = doc.DocumentNode.SelectSingleNode("//div[@class='manga-meta-grid']//div[contains(@class, 'manga-meta-item')][span[@class='meta-label' and contains(., 'status:')]]/span[@class='meta-value']");
        ComicStatus status = GetComicStatus(statusNode?.InnerText.Trim());

        var yearNode = doc.DocumentNode.SelectSingleNode("//div[@class='manga-meta-grid']//div[contains(@class, 'manga-meta-item')][span[@class='meta-label' and contains(., 'ano:')]]/span[@class='meta-value']");
        string yearStr = yearNode?.InnerText.Trim() ?? "";
        int? year = int.TryParse(yearStr, out int y) ? y : null;

        var descriptionNode = doc.DocumentNode.SelectSingleNode(
            "//div[contains(@class, 'manga-synopsis')]//div[@class='synopsis-content']"
        );
        string? description = descriptionNode?.InnerText.Trim();

        string? coverUrl = GetCoverUrl(comicId);

        return new Comic(
            Id: comicId,
            ComicUrl: detailsUrl,
            Title: title,
            Author: authors.FirstOrDefault(),
            Description: description,
            Tags: tags,
            Year: year,
            coverUrl,
            Langs: [SourceLanguage],
            Status: status
        );
    }

    public async Task<IReadOnlyList<Chapter>> GetAllChaptersAsync(
            string comicId,
            string language,
            CancellationToken ct = default
        )
        {
            var chaptersUrl = $"{BaseUrl}/manga/{comicId}";

            var html = await GetHTMLAsync(chaptersUrl, ct);
            if (html == null)
                return Array.Empty<Chapter>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var chapterNodes = doc.DocumentNode.SelectNodes(
                "//article[contains(@class, 'chapter-grid-item')]"
            );

            if (chapterNodes is not { Count: > 0 })
                return Array.Empty<Chapter>();

            var chapters = new List<Chapter>();

            foreach (var node in chapterNodes)
            {
                string? chapterNumberString =
                    node.GetAttributeValue("data-chapter-number", null);

                string title =
                    node.GetAttributeValue("data-chapter-title", "Unknown");

                var linkNode = node.SelectSingleNode(
                    ".//a[contains(@class, 'chapter-grid-number')]"
                );

                string? chapterUrl =
                    linkNode?.GetAttributeValue("href", null);

                string? chapterId =
                    ExtractChapterIdFromUrl(chapterUrl);

                if (string.IsNullOrEmpty(chapterId))
                    continue;

                double? number = null;

                if (double.TryParse(
                    chapterNumberString,
                    out double parsedNumber))
                {
                    number = parsedNumber;
                }

                chapters.Add(
                    new Chapter(
                        Id: chapterId,
                        Title: title,
                        Number: null,
                        Volume: null,
                        Language: language,
                        Groups: Array.Empty<string>(),
                        LastUpdate: null,
                        Pages: null
                    )
                );
            }

            return chapters;
        }

    private async Task<string?> GetHTMLAsync(string url, CancellationToken ct = default)
    {
        using var response = await _httpClient!.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, url),
            ct
        );

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new HttpRequestException(
                "WeebCentral Rate Limit Exceeded. Try again later.",
                null,
                HttpStatusCode.TooManyRequests
            );

        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
            return default;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(ct);
    }

    private ComicStatus GetComicStatus(string? status) =>
        status switch
        {
            "Ongoing" => ComicStatus.Ongoing,
            "Complete" => ComicStatus.Completed,
            "Hiatus" => ComicStatus.Hiatus,
            "Canceled" => ComicStatus.Cancelled,
            _ => ComicStatus.Unknown,
        };

    private string? GetCoverUrl(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return null;
        return $"https://temp.compsci88.com/cover/normal/{id}.webp";
    }

    private string? ExtractIdFromUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return null;
        var parts = url.TrimEnd('/').Split('/');
        return parts.Length >= 2 ? parts[^2] : null;
    }

    private string? ExtractChapterIdFromUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return null;
        var parts = url.TrimEnd('/').Split('/');
        return parts.Length > 0 ? parts[^1] : null;
    }

    private static string ToQueryString(Dictionary<string, string[]> source) =>
        string.Join(
            "&",
            source.SelectMany(kvp =>
                kvp.Value.Select(v => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(v)}")
            )
        );
}