using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToolBaoNail.Data;
using ToolBaoNail.DTO;

namespace ToolBaoNail.ViewController
{
    /// <summary>
    /// Interaction logic for CrawlControls.xaml
    /// </summary>
    public partial class CrawlControls : Window
    {
        private readonly IStateScraperService _service;
        private readonly ApplicationDbContext _context;

        public CrawlControls( ApplicationDbContext context, IStateScraperService service)
        {
            InitializeComponent();
            _context = context;
            _service = service;
            
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        private async void Start_CA_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Gọi phương thức GetStatesUSAsync
                var states = await _service.GetStatesCAAsync();

                // Hiển thị thông báo về số lượng bang được lấy
                MessageBox.Show($"Fetched and saved {states.Count} states for CANADA.");
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu có
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

        }

        private void Start_CA_2(object sender, RoutedEventArgs e)
        {

        }

        private void Start_CA_3(object sender, RoutedEventArgs e)
        {

        }

        private async void Start_US_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // Gọi phương thức GetStatesUSAsync
                var states = await _service.GetStatesUSAsync();

                // Hiển thị thông báo về số lượng bang được lấy
                MessageBox.Show($"Fetched and saved {states.Count} states for USA.");
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu có
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

        }

        private async void Start_US_2(object sender, RoutedEventArgs e)
        {
            try
            {
                await _service.FetchAndSaveAllAdsForUSAsync();

                // Hiển thị thông báo về số lượng bang được lấy
                MessageBox.Show($"Fetched and saved full AdInfo for full states for USA.");
            }
            catch (Exception ex)
            {
                // Hiển thị lỗi nếu có
                MessageBox.Show($"An error occurred: {ex.Message}");
            }

        }
        private void Start_US_3(object sender, RoutedEventArgs e)
        {

        } 

        private async void GetData(object sender, RoutedEventArgs e)
        {
            try
            {
                //// URL của API
                //string apiUrl = "http://baonailv1.runasp.net/api/States/AL/adsPage?page=1";

                //// Sử dụng HttpClient để gọi API
                //using (HttpClient client = new HttpClient())
                //{
                //    // Gửi yêu cầu GET
                //    HttpResponseMessage response = await client.GetAsync(apiUrl);

                //    // Kiểm tra xem yêu cầu có thành công không
                //    if (response.IsSuccessStatusCode)
                //    {
                //        // Đọc dữ liệu JSON từ API
                //        var jsonData = await response.Content.ReadAsStringAsync();


                //            nếu json trả về như này :{
                //                "data": [
                //                    {
                //                    "id": 1,
                //"title": "Title 1",
                //"content": "Content 1"
                //},
                //// Chuyển đổi JSON thành danh sách đối tượng (sử dụng Newtonsoft.Json hoặc System.Text.Json)
                //var apiResponse = JsonConvert.DeserializeObject<Api_AdInfo_Response>(jsonData);

                //var historyData = apiResponse.Data;
                //HistoryDataGrid.ItemsSource = historyData;




                //còn nếu trả về như này : [
                //        {
                //                            "id": 1,
                //            "title": "Title 1",
                //            "content": "Content 1"
                //        },
                //        {
                //                            "id": 2,
                //            "title": "Title 2",
                //            "content": "Content 2"
                //        }
                //    ]

                // Chuyển đổi JSON thành danh sách đối tượng
                //var historyData = JsonConvert.DeserializeObject<List<AdInfoDTO>>(jsonData);

                //// Bind dữ liệu vào DataGrid
                //HistoryDataGrid.ItemsSource = historyData;
                //}
                //else
                //{
                //    MessageBox.Show($"API call failed with status code: {response.StatusCode}");
                //    }
                //}


                    // Kiểm tra lựa chọn từ ComboBox
                    string selectedRegion = (RegionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    if (string.IsNullOrEmpty(selectedRegion))
                    {
                        MessageBox.Show("Please select a region (US or CA) before proceeding.");
                        return;
                    }

                    // Xác định URL API dựa trên lựa chọn
                    string apiUrl = selectedRegion == "USA"
                        ? "http://baonailv1.runasp.net/api/States/AL/adsPage?page=1"
                        : "http://baonailv1.runasp.net/api/States/CA/adsPage?page=1";

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(apiUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonData = await response.Content.ReadAsStringAsync();
                            var historyData = JsonConvert.DeserializeObject<List<AdInfoDTO>>(jsonData);

                            HistoryDataGrid.ItemsSource = historyData;
                        }
                        else
                        {
                            MessageBox.Show($"API call failed with status code: {response.StatusCode}");
                        }
                    }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching data from API: {ex.Message}");
            }

        }
    }
}
