using HtmlAgilityPack;
using System.Net.Http;
using AngleSharp.Dom;
using Microsoft.EntityFrameworkCore;
using Data.Model;
using ToolBaoNail.Data;
using Data;
using System.Windows;
using ToolBaoNail.DTO;
using PuppeteerSharp;


public interface IStateScraperService
{
    Task<List<StateInfo>> GetStatesUSAsync();
    Task<List<StateInfo>> GetStatesCAAsync();
    Task<List<AdInfo>> GetAdsByStateUSpageAsync(string stateCode,int page);
    Task<AdDetailInfo> GetAdDetailAsync(string adId);
    Task FetchAndSaveAllAdsForUSAsync();
}

public class StateScraperService : IStateScraperService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApplicationDbContext _context;

    public StateScraperService(HttpClient httpClient, IHttpClientFactory httpClientFactory, ApplicationDbContext context)
    {
        _httpClient = httpClient;
        _httpClientFactory = httpClientFactory;
        _context = context;
    }

    #region GetStatesUSAsync()
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


            // Trích xuất State từ Title (nếu tồn tại)
            string? stateCode = null;
            if (!string.IsNullOrEmpty(title) && title.Contains("-"))
            {
                var parts = title.Split('-');
                stateCode = parts.LastOrDefault()?.Trim(); // Lấy phần sau dấu "-"
            }


            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(link))
            {
                // Kiểm tra nếu title đã tồn tại
                var existingState = await _context.StateInfos
                    .FirstOrDefaultAsync(s => s.Title == title && s.Country == "US");

                if (existingState == null)
                {
                    // Thêm mới vào danh sách và lưu vào database
                    var newState = new StateInfo
                    {
                        Country = "US",
                        State = stateCode,
                        Title = title,
                        Name = name,
                        Url = link
                    };

                    states.Add(newState);
                    _context.StateInfos.Add(newState);
                }
            }
        }

        // Lưu các thay đổi vào database
        await _context.SaveChangesAsync();

        return states;
    }
    #endregion

    #region GetStatesCAAsync()
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


            // Trích xuất State từ Title (nếu tồn tại)
            string? stateCode = null;
            if (!string.IsNullOrEmpty(title) && title.Contains("-"))
            {
                var parts = title.Split('-');
                stateCode = parts.LastOrDefault()?.Trim(); // Lấy phần sau dấu "-"
            }


            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(link))
            {
                // Kiểm tra nếu title đã tồn tại
                var existingState = await _context.StateInfos
                    .FirstOrDefaultAsync(s => s.Title == title && s.Country == "CA");

                if (existingState == null)
                {
                    // Thêm mới vào danh sách và lưu vào database
                    var newState = new StateInfo
                    {
                        Country = "CA",
                        State = stateCode,
                        Title = title,
                        Name = name,
                        Url = link
                    };

                    states.Add(newState);
                    _context.StateInfos.Add(newState);
                }
            }
        }

        // Lưu các thay đổi vào database
        await _context.SaveChangesAsync();

        return states;
    }
    #endregion

    #region GetAdsByStateUSpageAsync(string stateCode, int page)
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
    #endregion


    public async Task FetchAndSaveAllAdsForUSAsync()
    {
        try
        {
            // Lấy danh sách tất cả các bang trong Country US từ database
            var usStates = await _context.StateInfos
                                         .Where(s => s.Country == "US")
                                         .ToListAsync();

            foreach (var state in usStates)
            {
                Console.WriteLine($"Fetching ads for state: {state.State}");

                for (int page = 1; page <= 10; page++)
                {
                    // Lấy danh sách quảng cáo từ trang web
                    var ads = await GetAdsByStateUSpageAsync(state.State, page);

                    // Nếu không có dữ liệu, thoát vòng lặp trang
                    if (ads == null || !ads.Any())
                    {
                        Console.WriteLine($"No ads found for state {state.State} on page {page}. Stopping further fetch for this state.");
                        break;
                    }

                    // Lọc các quảng cáo chưa tồn tại trong database
                    var newAds = ads.Where(ad => !_context.AdInfos.Any(dbAd => dbAd.AdId == ad.AdId)).ToList();

                    // Gán StateInfoId cho từng quảng cáo mới
                    foreach (var ad in newAds)
                    {
                        ad.StateInfoId = state.StateInfoId; // Gán StateInfoId từ bảng StateInfo
                    }

                    // Nếu có quảng cáo mới, thêm vào database
                    if (newAds.Any())
                    {
                        _context.AdInfos.AddRange(newAds);
                        await _context.SaveChangesAsync();

                        Console.WriteLine($"Saved {newAds.Count} new ads for state {state.State}, page {page}");



                        // Khu vực để lưu AdDetailsInfo 
                        // Lấy thông tin chi tiết cho từng quảng cáo
                        foreach (var ad in newAds)
                        {
                            //var adDetail = await GetAdDetailAsync(ad.AdId);
                            var adDetail = await GetAdDetailWithPuppeteerAsync(ad.AdId);

                            // Kiểm tra nếu chi tiết quảng cáo đã tồn tại
                            if (!_context.AdDetailInfos.Any(detail => detail.AdId == adDetail.AdId))
                            {
                                // Gán các khóa liên kết
                                adDetail.StateInfoId = ad.StateInfoId;
                                adDetail.AdInfoId = ad.AdInfoId;

                                // Thêm và lưu vào cơ sở dữ liệu
                                _context.AdDetailInfos.Add(adDetail);
                                await _context.SaveChangesAsync();

                                Console.WriteLine($"Saved AdDetail for AdId: {ad.AdId}");
                            }
                            else
                            {
                                Console.WriteLine($"AdDetail for AdId: {ad.AdId} already exists.");
                            }
                        }

                        //Hết // khu vực lưu addedtailinfo 
                    }
                    else
                    {
                        Console.WriteLine($"All ads on state {state.State}, page {page} already exist in the database.");
                    }
                }
            }

            MessageBox.Show("Data fetching and saving completed successfully!");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }
    }

    #region GetAdDetailAsync No Browser Puppeteer
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
    #endregion

    #region GetAdDetailWithPuppeteerAsync No Proxy
    //public async Task<AdDetailInfo> GetAdDetailWithPuppeteerAsync(string adId)
    //{
    //    var url = $"https://baonail.com/index.php?id={adId}";

    //    // Tải xuống trình duyệt nếu chưa có
    //    //await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
    //    await new BrowserFetcher().DownloadAsync();

    //    // Khởi chạy trình duyệt
    //    using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
    //    {
    //        Headless = true, // Chạy trong chế độ headless (ẩn)
    //    });

    //    using var page = await browser.NewPageAsync();

    //    // Điều hướng đến URL
    //    await page.GoToAsync(url);



    //    // Nhấp vào liên kết "Phone ☎" theo Xpath
    //    var contactButtonXpath = "//*[@id='ad_vi']/a";
    //    var contactButton = await page.WaitForXPathAsync(contactButtonXpath, new WaitForSelectorOptions { Timeout = 5000 });
    //    if (contactButton != null)
    //    {
    //        await contactButton.ClickAsync();
    //    }
    //    else
    //    {
    //        throw new Exception("Không tìm thấy nút liên hệ.");
    //    }

    //    // Thêm thời gian chờ ngẫu nhiên giữa các lần truy cập
    //    //var randomWaitTime = new Random().Next(90000, 100000); // Wait for 1-3 seconds
    //    //await Task.Delay(randomWaitTime);

    //    // Đợi các thông tin cần thiết hiển thị sau khi nhấp
    //    await page.WaitForSelectorAsync("#id8472f", new WaitForSelectorOptions { Timeout = 5000 });


    //    // Lấy các trường dữ liệu từ trang
    //    var titleVi = await page.EvaluateExpressionAsync<string>("document.querySelector('#title_ad').innerText.trim()");
    //    var titleEn = await page.EvaluateExpressionAsync<string>("document.querySelector('#title_ad_en').innerText.trim()");
    //    //var adInfoText = await page.EvaluateExpressionAsync<string>("document.querySelector('.xsmall').innerText.trim()");

    //    //// Lấy thông tin ID và ngày cập nhật từ adInfoText
    //    //var adIdValue = adInfoText?.Split(new[] { "Ad ID: " }, StringSplitOptions.None)[1]?.Split(new[] { "Cập nhật:" }, StringSplitOptions.None)[0]?.Trim();
    //    //var lastUpdated = adInfoText?.Split(new[] { "Cập nhật: " }, StringSplitOptions.None)[1]?.Trim();

    //    //var contentVi = await page.EvaluateExpressionAsync<string>("document.querySelector('#ad_vi').innerText.trim()");
    //    //var contentEn = await page.EvaluateExpressionAsync<string>("document.querySelector('#ad_en').innerText.trim()");
    //    // Lấy dữ liệu từng phần tử
    //    var adInfoText = await page.EvaluateExpressionAsync<string>(
    //        "document.querySelector('.xsmall')?.innerText.trim()");
    //    var adIdValue = adInfoText?.Split(new[] { "Ad ID: " }, StringSplitOptions.None)[1]
    //        ?.Split(new[] { "Cập nhật:" }, StringSplitOptions.None)[0]?.Trim();
    //    var lastUpdated = adInfoText?.Split(new[] { "Cập nhật: " }, StringSplitOptions.None)[1]?.Trim();

    //    var contentVi = await page.EvaluateExpressionAsync<string>(
    //        "document.querySelector('#ad_vi')?.innerText.trim()");
    //    var contentEn = await page.EvaluateExpressionAsync<string>(
    //        "document.querySelector('#ad_en')?.innerText.trim()");

    //    var storeAddress = await page.EvaluateExpressionAsync<string>(
    //"document.evaluate('//*[@id=\"id8472f\"]/div[4]', document, null, XPathResult.STRING_TYPE, null).stringValue.trim()");


    //    // Lấy danh sách hình ảnh
    //    var imageUrls = await page.EvaluateExpressionAsync<string[]>(
    //        @"Array.from(document.querySelectorAll('.row.gallery a.xxxyyy'))
    //       .map(a => 'https://baonail.com' + a.getAttribute('href'))");


    //    // Lấy thông tin liên hệ từ các XPath
    //    var contactInfo0 = await page.EvaluateExpressionAsync<string>(
    //        "document.evaluate('//*[@id=\"ad_vi\"]/a', document, null, XPathResult.STRING_TYPE, null).stringValue.trim()");
    //    contactInfo0 = string.IsNullOrWhiteSpace(contactInfo0) ? "Không tìm thấy số điện thoại" : contactInfo0;
    //    // Lấy thông tin liên hệ từ các XPath
    //    var contactInfo1 = await page.EvaluateExpressionAsync<string>(
    //        "document.evaluate('//*[@id=\"ad_vi\"]/a[1]', document, null, XPathResult.STRING_TYPE, null).stringValue.trim()");
    //    contactInfo1 = string.IsNullOrWhiteSpace(contactInfo1) ? "Không tìm thấy số điện thoại" : contactInfo1;
    //    var contactInfo2 = await page.EvaluateExpressionAsync<string>(
    //        "document.evaluate('//*[@id=\"ad_vi\"]/a[2]', document, null, XPathResult.STRING_TYPE, null).stringValue.trim()");
    //    contactInfo2 = string.IsNullOrWhiteSpace(contactInfo2) ? "Không tìm thấy số điện thoại" : contactInfo2;
    //    var contactInfo3 = await page.EvaluateExpressionAsync<string>(
    //       "document.evaluate('//*[@id=\"id8472f\"]/div[4]/a', document, null, XPathResult.STRING_TYPE, null).stringValue.trim()");
    //    contactInfo3 = string.IsNullOrWhiteSpace(contactInfo3) ? "Không tìm thấy số điện thoại" : contactInfo3;


    //    // Xóa tất cả cookies sau khi lấy dữ liệu xong
    //    await page.DeleteCookieAsync();

    //    // Đóng trình duyệt
    //    await browser.CloseAsync();


    //    // Tạo đối tượng AdDetailInfo
    //    var adDetail = new AdDetailInfo
    //    {
    //        TitleVi = titleVi,
    //        TitleEn = titleEn,
    //        AdId = adIdValue,
    //        LastUpdated = lastUpdated,
    //        ContentVi = contentVi,
    //        ContentEn = contentEn,
    //        StoreAddress = storeAddress,
    //        Images = imageUrls.ToList(),
    //        ContactInfo0 = contactInfo0,
    //        ContactInfo1 = contactInfo1,
    //        ContactInfo2 = contactInfo2,
    //        ContactInfo3 = contactInfo3
    //    };

    //    return adDetail;
    //}
    #endregion


    #region GetAdDetailWithPuppeteerAsync Have Proxy
    private List<string> _proxies = new List<string>
    {
        "http://161.35.49.68:80",
        "http://172.105.26.6:8080",
        "http://143.198.226.25:80",
        "http://103.152.112.120:80",
        "http://143.110.232.177:80",
        "http://44.195.247.145:80",
        "http://54.67.125.45:3128",
        "http://184.169.154.119:80"
    };

    private int _currentProxyIndex = 0;
    private int _adCount = 0;

    public async Task<AdDetailInfo> GetAdDetailWithPuppeteerAsync(string adId)
    {
        // Tăng số lượng quảng cáo đã cào
        _adCount++;

        // Chuyển sang proxy mới sau 3 quảng cáo
        if (_adCount % 3 == 0)
        {
            _currentProxyIndex = (_currentProxyIndex + 1) % _proxies.Count;
        }

        // Lấy proxy hiện tại
        var currentProxy = _proxies[_currentProxyIndex];

        // Khởi chạy trình duyệt với proxy
        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = new[] { $"--proxy-server={currentProxy}" }
        });

        using var page = await browser.NewPageAsync();

        // URL của quảng cáo
        var url = $"https://baonail.com/index.php?id={adId}";

        // Điều hướng đến URL
        await page.GoToAsync(url);

        // Nhấp vào liên kết "Phone ☎" theo XPath
        var contactButtonXpath = "//*[@id='ad_vi']/a";
        var contactButton = await page.WaitForXPathAsync(contactButtonXpath, new WaitForSelectorOptions { Timeout = 30000 });
        if (contactButton != null)
        {
            await contactButton.ClickAsync();
        }
        else
        {
            throw new Exception("Không tìm thấy nút liên hệ.");
        }

        // Đợi các thông tin cần thiết hiển thị sau khi nhấp
        await page.WaitForSelectorAsync("#id8472f", new WaitForSelectorOptions { Timeout = 5000 });

        // Lấy các trường dữ liệu từ trang
        var titleVi = await page.EvaluateExpressionAsync<string>("document.querySelector('#title_ad').innerText.trim()");
        var titleEn = await page.EvaluateExpressionAsync<string>("document.querySelector('#title_ad_en').innerText.trim()");
        var adInfoText = await page.EvaluateExpressionAsync<string>("document.querySelector('.xsmall')?.innerText.trim()");
        var adIdValue = adInfoText?.Split(new[] { "Ad ID: " }, StringSplitOptions.None)[1]?.Split(new[] { "Cập nhật:" }, StringSplitOptions.None)[0]?.Trim();
        var lastUpdated = adInfoText?.Split(new[] { "Cập nhật: " }, StringSplitOptions.None)[1]?.Trim();
        var contentVi = await page.EvaluateExpressionAsync<string>("document.querySelector('#ad_vi')?.innerText.trim()");
        var contentEn = await page.EvaluateExpressionAsync<string>("document.querySelector('#ad_en')?.innerText.trim()");
        var storeAddress = await page.EvaluateExpressionAsync<string>("document.evaluate('//*[@id=\"id8472f\"]/div[4]', document, null, XPathResult.STRING_TYPE, null).stringValue.trim()");

        // Lấy thông tin liên hệ
        var contactInfo0 = await page.EvaluateExpressionAsync<string>("document.evaluate('//*[@id=\"ad_vi\"]/a', document, null, XPathResult.STRING_TYPE, null).stringValue.trim()");
        contactInfo0 = string.IsNullOrWhiteSpace(contactInfo0) ? "Không tìm thấy số điện thoại" : contactInfo0;
        var contactInfo1 = await page.EvaluateExpressionAsync<string>("document.evaluate('//*[@id=\"ad_vi\"]/a[1]', document, null, XPathResult.STRING_TYPE, null).stringValue.trim()");
        contactInfo1 = string.IsNullOrWhiteSpace(contactInfo1) ? "Không tìm thấy số điện thoại" : contactInfo1;
        var contactInfo2 = await page.EvaluateExpressionAsync<string>("document.evaluate('//*[@id=\"ad_vi\"]/a[2]', document, null, XPathResult.STRING_TYPE, null).stringValue.trim()");
        contactInfo2 = string.IsNullOrWhiteSpace(contactInfo2) ? "Không tìm thấy số điện thoại" : contactInfo2;
        var contactInfo3 = await page.EvaluateExpressionAsync<string>("document.evaluate('//*[@id=\"id8472f\"]/div[4]/a', document, null, XPathResult.STRING_TYPE, null).stringValue.trim()");
        contactInfo3 = string.IsNullOrWhiteSpace(contactInfo3) ? "Không tìm thấy số điện thoại" : contactInfo3;

        // Xóa cookie sau khi lấy dữ liệu xong
        await page.DeleteCookieAsync();

        // Đóng trình duyệt
        await browser.CloseAsync();

        // Tạo đối tượng AdDetailInfo
        return new AdDetailInfo
        {
            TitleVi = titleVi,
            TitleEn = titleEn,
            AdId = adIdValue,
            LastUpdated = lastUpdated,
            ContentVi = contentVi,
            ContentEn = contentEn,
            StoreAddress = storeAddress,
            ContactInfo0 = contactInfo0,
            ContactInfo1 = contactInfo1,
            ContactInfo2 = contactInfo2,
            ContactInfo3 = contactInfo3
        };
    }
    #endregion


}


