using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

public class OpenStackClient
{
    private readonly HttpClient _httpClient = new HttpClient();
    public string AuthToken { get; private set; } // Biến lưu Token sau khi đăng nhập

    // Hàm gọi API Đăng nhập - Xác thực
    public async Task<bool> AuthenticateAsync(string identityUrl, string username, string password, string projectName)
    {
        var authPayload = new
        {
            auth = new
            {
                identity = new
                {
                    methods = new[] { "password" },
                    password = new
                    {
                        user = new
                        {
                            name = username,
                            domain = new { name = "Default" },
                            password = password
                        }
                    }
                },
                scope = new
                {
                    project = new
                    {
                        name = projectName,
                        domain = new { name = "Default" }
                    }
                }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(authPayload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{identityUrl}/auth/tokens", content);

        if (response.IsSuccessStatusCode)
        {
            AuthToken = response.Headers.GetValues("X-Subject-Token").FirstOrDefault();

            // Lưu token vào Header mặc định cho các lần gọi API sau
            _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
            _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);
            return true;
        }
        return false;
    }

    // Hàm lấy danh sách Flavor (Cấu hình máy ảo)
    public async Task<string> GetFlavorsAsync()
    {
        _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);

        var response = await _httpClient.GetAsync("https://cloud-compute.uitiot.vn/v2.1/flavors/detail");
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm lấy danh sách Image (Hệ điều hành như Ubuntu, CentOS...)
    public async Task<string> GetImagesAsync()
    {
        _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);

        var response = await _httpClient.GetAsync("https://cloud-compute.uitiot.vn/v2.1/images/detail");
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm tạo Network
    public async Task<string> CreateNetworkAsync(string networkName)
    {
        _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);

        var netPayload = new
        {
            network = new
            {
                name = networkName,
                admin_state_up = true
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(netPayload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://cloud-network.uitiot.vn/v2.0/networks", content);
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm tạo Subnet
    public async Task<string> CreateSubnetAsync(string networkId, string subnetName, string cidr)
    {
        _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);

        var payload = new
        {
            subnet = new
            {
                network_id = networkId,
                name = subnetName,
                ip_version = 4,
                cidr = cidr
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://cloud-network.uitiot.vn/v2.0/subnets", content);
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm tạo Router
    public async Task<string> CreateRouterAsync(string routerName, string extNetworkId)
    {
        _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);

        var payload = new
        {
            router = new
            {
                name = routerName,
                admin_state_up = true,
                external_gateway_info = new { network_id = extNetworkId }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://cloud-network.uitiot.vn/v2.0/routers", content);
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm cắm Subnet vào Router
    public async Task<string> AddInterfaceToRouterAsync(string routerId, string subnetId)
    {
        _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);

        var payload = new
        {
            subnet_id = subnetId
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        //Cắm dây dùng put kh dùng post
        var response = await _httpClient.PutAsync($"https://cloud-network.uitiot.vn/v2.0/routers/{routerId}/add_router_interface", content);
        return await response.Content.ReadAsStringAsync();
    }

    //CÁC HÀM XÓA
    public async Task<string> DeleteNetworkAsync(string networkId)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"https://cloud-network.uitiot.vn/v2.0/networks/{networkId}");
        return response.IsSuccessStatusCode ? "Xóa Network thành công" : "Lỗi xóa Network";
    }

    public async Task<string> DeleteRouterAsync(string routerId)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"https://cloud-network.uitiot.vn/v2.0/routers/{routerId}");
        return response.IsSuccessStatusCode ? "Xóa Router thành công" : "Lỗi xóa Router";
    }

    public async Task<string> DeleteInstanceAsync(string instanceId)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"https://cloud-compute.uitiot.vn/v2.1/servers/{instanceId}");
        return response.IsSuccessStatusCode ? "Xóa Máy ảo thành công" : "Lỗi xóa Máy ảo";
    }

    private void SetAuthHeader()
    {
        _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);
    }

    //TẠO MÁY ẢO & WEB SERVER
    public async Task<string> CreateInstanceWithWebAsync(string vmName, string imageId, string flavorId, string networkId)
    {
        SetAuthHeader();

        // Script tự động cập nhật, cài Apache và in ra trang web chứa IP của máy
        string bashScript = @"#!/bin/bash
                            apt-get update
                            apt-get install -y apache2
                            IP=$(hostname -I | awk '{print $1}')
                            echo ""<h1>Nhom XX - Dia chi IP cua toi la: $IP</h1>"" > /var/www/html/index.html
                            systemctl restart apache2";

        string userDataBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(bashScript));

        var payload = new
        {
            server = new
            {
                name = vmName,
                imageRef = imageId,
                flavorRef = flavorId,
                networks = new[] { new { uuid = networkId } },
                user_data = userDataBase64,
                security_groups = new[] { new { name = "default" } } // Đảm bảo dùng Security Group mặc định
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://cloud-compute.uitiot.vn/v2.1/servers", content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> CreateAndAssignFloatingIpAsync(string extNetId, string portId)
    {
        SetAuthHeader();

        var payload = new
        {
            floatingip = new
            {
                floating_network_id = extNetId,
                port_id = portId
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://cloud-network.uitiot.vn/v2.0/floatingips", content);
        return await response.Content.ReadAsStringAsync();
    }

    //LOAD BALANCER SCALING
    public async Task<string> AddMemberToLoadBalancerPoolAsync(string poolId, string vmIpAddress, string subnetId)
    {
        SetAuthHeader();

        var payload = new
        {
            member = new
            {
                address = vmIpAddress, // IP nội bộ của máy ảo vừa tạo
                protocol_port = 80,    // Port chạy Web
                subnet_id = subnetId,
                admin_state_up = true
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/pools/{poolId}/members", content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> RemoveMemberFromPoolAsync(string poolId, string memberId)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/pools/{poolId}/members/{memberId}");
        return response.IsSuccessStatusCode ? "Đã gỡ VM khỏi Load Balancer" : "Lỗi gỡ VM";
    }

    // Hàm lấy danh sách Router
    public async Task<string> GetRoutersAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("https://cloud-network.uitiot.vn/v2.0/routers");
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm lấy danh sách Máy ảo (Instances)
    public async Task<string> GetInstancesAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("https://cloud-compute.uitiot.vn/v2.1/servers/detail"); //detail để lấy status
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm lấy danh sách Network
    public async Task<string> GetNetworksAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("https://cloud-network.uitiot.vn/v2.0/networks");
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm lấy danh sách Subnet
    public async Task<string> GetSubnetsAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("https://cloud-network.uitiot.vn/v2.0/subnets");
        return await response.Content.ReadAsStringAsync();
    }

}