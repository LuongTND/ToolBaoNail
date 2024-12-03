using HtmlAgilityPack;
using System.Net.Http;
using AngleSharp.Dom;
using Data.Model;


public interface IStateScraperService
{
    Task<List<StateInfo>> GetStatesUSAsync();
    Task<List<StateInfo>> GetStatesCAAsync();
    Task<List<AdInfo>> GetAdsByStateUSAsync(string stateCode);
    Task<List<AdInfo>> GetAdsByStateUSpageAsync(string stateCode,int page);
    Task<AdDetailInfo> GetAdDetailAsync(string adId);
}

public class StateScraperService : IStateScraperService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public StateScraperService(HttpClient httpClient, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<StateInfo>> GetStatesUSAsync()
    {
        var url = "https://baonail.com/index.php?func=homepage&country=us";
        var response = await _httpClient.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(response);

        var stateNodes = doc.DocumentNode.SelectNodes("//div[@class='row citybg tab_box']//div[@class='col-lg-2 col-md-3 col-sm-4 col-xs-4 h28 ellipsis']");

        var states = new List<StateInfo>();
        foreach (var node in stateNodes)
        {
            var title = node.GetAttributeValue("title", "");
            var name = node.SelectSingleNode(".//a")?.InnerText;
            var link = node.SelectSingleNode(".//a")?.GetAttributeValue("href", "");

            if (name != null && link != null)
            {
                states.Add(new StateInfo
                {
                    Title = title,
                    Name = name,
                    Url = link
                });
            }
        }

        return states;
    }

    public async Task<List<StateInfo>> GetStatesCAAsync()
    {
        var url = "https://baonail.com/index.php?func=homepage&country=ca";
        var response = await _httpClient.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(response);

        var stateNodes = doc.DocumentNode.SelectNodes("//div[@class='row citybg tab_box']//div[@class='col-lg-2 col-md-3 col-sm-4 col-xs-4 h28 ellipsis']");

        var states = new List<StateInfo>();
        foreach (var node in stateNodes)
        {
            var title = node.GetAttributeValue("title", "");
            var name = node.SelectSingleNode(".//a")?.InnerText;
            var link = node.SelectSingleNode(".//a")?.GetAttributeValue("href", "");

            if (name != null && link != null)
            {
                states.Add(new StateInfo
                {
                    Title = title,
                    Name = name,
                    Url = link
                });
            }
        }

        return states;
    }

    public async Task<List<AdInfo>> GetAdsByStateUSAsync(string stateCode)
    {
        if (string.IsNullOrWhiteSpace(stateCode))
            throw new ArgumentException("State code cannot be null or empty.", nameof(stateCode));

        var url = $"https://baonail.com/index.php?country=US&state={stateCode}";
        var response = await _httpClient.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(response);

        var adNodes = doc.DocumentNode.SelectNodes("//div[@class='col-xs-12 col-sm-6 col-md-4']");

        var ads = new List<AdInfo>();
        if (adNodes != null)
        {
            foreach (var node in adNodes)
            {
                var adSmallIn = node.SelectSingleNode(".//div[contains(@class, 'adsmall_in')]");
                if (adSmallIn == null)
                    continue;

                // Lấy thông tin liên kết cửa hàng
                var linkNode = adSmallIn.SelectSingleNode(".//div[contains(@class, 'ellipsis height30')]/a[1]");
                var storeName = linkNode?.InnerText.Trim();
                var storeUrl = linkNode?.GetAttributeValue("href", "");

                // Lấy thông tin địa điểm
                var locationNode = adSmallIn.SelectSingleNode(".//div[contains(@class, 'ellipsis height30')]/a[2]");
                var location = locationNode?.InnerText.Trim();

                // Lấy thông tin mã tiểu bang
                var stateNode = adSmallIn.SelectSingleNode(".//div[contains(@class, 'ellipsis height30')]/a[3]");
                var state = stateNode?.InnerText.Trim();

                // Lấy tiêu đề quảng cáo
                var adBodyNode = node.SelectSingleNode(".//div[contains(@class, 'small_ad_body')]");
                var adTitle = adBodyNode?.SelectSingleNode(".//div[contains(@class, 'ellipsis')]/b")?.InnerText.Trim();

                // Lấy nội dung quảng cáo
                var adMessage = adBodyNode?.SelectSingleNode(".//div[contains(@class, 'hidden-xs ads_msg height80')]")?.InnerText.Trim();

                // Lấy ID quảng cáo từ thuộc tính onclick
                var onclickAttr = adBodyNode?.GetAttributeValue("onclick", "");
                string adId = null;
                if (!string.IsNullOrEmpty(onclickAttr))
                {
                    var start = onclickAttr.IndexOf("'");
                    var end = onclickAttr.LastIndexOf("'");
                    if (start >= 0 && end > start)
                    {
                        adId = onclickAttr.Substring(start + 1, end - start - 1);
                    }
                }

                ads.Add(new AdInfo
                {
                    StoreName = storeName,
                    StoreUrl = storeUrl,
                    Location = location,
                    State = state,
                    AdTitle = adTitle,
                    AdMessage = adMessage,
                    AdId = adId
                });
            }
        }

        return ads;
    }

    public async Task<List<AdInfo>> GetAdsByStateUSpageAsync(string stateCode, int page)
    {
        if (string.IsNullOrWhiteSpace(stateCode))
            throw new ArgumentException("State code cannot be null or empty.", nameof(stateCode));
        

        var url = $"https://baonail.com/index.php?state={stateCode}&page={page}";
        var response = await _httpClient.GetStringAsync(url);

        var doc = new HtmlDocument();
        doc.LoadHtml(response);

        var adNodes = doc.DocumentNode.SelectNodes("//div[@class='col-xs-12 col-sm-6 col-md-4']");

        var ads = new List<AdInfo>();
        if (adNodes != null)
        {
            foreach (var node in adNodes)
            {
                var adSmallIn = node.SelectSingleNode(".//div[contains(@class, 'adsmall_in')]");
                if (adSmallIn == null)
                    continue;

                // Lấy thông tin liên kết cửa hàng
                var linkNode = adSmallIn.SelectSingleNode(".//div[contains(@class, 'ellipsis height30')]/a[1]");
                var storeName = linkNode?.InnerText.Trim();
                var storeUrl = linkNode?.GetAttributeValue("href", "");

                // Lấy thông tin địa điểm
                var locationNode = adSmallIn.SelectSingleNode(".//div[contains(@class, 'ellipsis height30')]/a[2]");
                var location = locationNode?.InnerText.Trim();

                // Lấy thông tin mã tiểu bang
                var stateNode = adSmallIn.SelectSingleNode(".//div[contains(@class, 'ellipsis height30')]/a[3]");
                var state = stateNode?.InnerText.Trim();

                // Lấy tiêu đề quảng cáo
                var adBodyNode = node.SelectSingleNode(".//div[contains(@class, 'small_ad_body')]");
                var adTitle = adBodyNode?.SelectSingleNode(".//div[contains(@class, 'ellipsis')]/b")?.InnerText.Trim();

                // Lấy nội dung quảng cáo
                var adMessage = adBodyNode?.SelectSingleNode(".//div[contains(@class, 'hidden-xs ads_msg height80')]")?.InnerText.Trim();

                // Lấy ID quảng cáo từ thuộc tính onclick
                var onclickAttr = adBodyNode?.GetAttributeValue("onclick", "");
                string adId = null;
                if (!string.IsNullOrEmpty(onclickAttr))
                {
                    var start = onclickAttr.IndexOf("'");
                    var end = onclickAttr.LastIndexOf("'");
                    if (start >= 0 && end > start)
                    {
                        adId = onclickAttr.Substring(start + 1, end - start - 1);
                    }
                }

                ads.Add(new AdInfo
                {
                    StoreName = storeName,
                    StoreUrl = storeUrl,
                    Location = location,
                    State = state,
                    AdTitle = adTitle,
                    AdMessage = adMessage,
                    AdId = adId
                });
            }
        }

        return ads;
    }

    public async Task<AdDetailInfo> GetAdDetailAsync(string adId)
    {
        

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetStringAsync($"https://baonail.com/index.php?id={adId}");

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(response);

        var imageUrls = new List<string>();
        var galleryNodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='row gallery']//a[@class='xxxyyy']");

        if (galleryNodes != null)
        {
            foreach (var node in galleryNodes)
            {
                var imageUrl = node.Attributes["href"]?.Value;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    // Concatenate base URL if necessary
                    imageUrls.Add($"https://baonail.com{imageUrl}");
                }
            }
        }
        // Truy xuất toàn bộ văn bản từ phần tử chứa cả Ad ID và ngày cập nhật
        var adInfoText = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(@class, 'xsmall')]")?.InnerText.Trim();

        var adDetail = new AdDetailInfo
        {
            TitleVi = htmlDocument.DocumentNode.SelectSingleNode("//span[@id='title_ad']").InnerText.Trim(),
            TitleEn = htmlDocument.DocumentNode.SelectSingleNode("//span[@id='title_ad_en']").InnerText.Trim(),
            //AdId = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(text(), 'Ad ID:')]").InnerText.Trim().Split(':')[1].Trim(),
            //LastUpdated = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(text(), 'Cập nhật:')]").InnerText.Trim().Split(':')[1].Trim(),
            AdId = adInfoText?.Split(new[] { "Ad ID: " }, StringSplitOptions.None)[1]?.Split(new[] { "Cập nhật:" }, StringSplitOptions.None)[0]?.Replace("&nbsp;", "").Trim(),
            LastUpdated = adInfoText?.Split(new[] { "Cập nhật: " }, StringSplitOptions.None)[1]?.Trim(),
            ContentVi = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='ad_vi']").InnerText.Trim(),
            ContentEn = htmlDocument.DocumentNode.SelectSingleNode("//div[@id='ad_en']").InnerText.Trim(),
            StoreAddress = htmlDocument.DocumentNode.SelectSingleNode("//*[@id='id8472f']/div[4]").InnerText.Trim(),
            Images = imageUrls,
            ContactInfo1 = htmlDocument.DocumentNode.SelectSingleNode("//a[contains(@class, 'contact_info')]")?.InnerText.Trim() ?? "Không tìm thấy số điện thoại",
            ContactInfo2 = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"ad_vi\"]/a[1]")?.InnerText.Trim() ?? "Không tìm thấy số điện thoại",
            ContactInfo3 = htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"ad_vi\"]/a[2]")?.InnerText.Trim() ?? "Không tìm thấy số điện thoại"
        };

        return adDetail;
    }



}


