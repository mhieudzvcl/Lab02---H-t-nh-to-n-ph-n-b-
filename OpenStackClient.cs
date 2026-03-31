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
                cidr = cidr,
                dns_nameservers = new[] { "8.8.8.8", "1.1.1.1" }
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

    public async Task<string> GetKeyPairsAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("https://cloud-compute.uitiot.vn/v2.1/os-keypairs");
        return await response.Content.ReadAsStringAsync();
    }

    private void SetAuthHeader()
    {
        _httpClient.DefaultRequestHeaders.Remove("X-Auth-Token");
        _httpClient.DefaultRequestHeaders.Add("X-Auth-Token", AuthToken);
    }

    //TẠO MÁY ẢO & WEB SERVER
    public async Task<string> CreateInstanceWithWebAsync(string vmName, string imageId, string flavorId, string networkId, string keyPairName = null, int volumeSize = 4)
    {
        SetAuthHeader();

        // Script tự động cập nhật, cài Apache và in ra trang web chứa IP của máy
        string bashScript = @"#!/bin/bash
set -eux
exec > >(tee /var/log/nhom04-init.log) 2>&1

sleep 10
resolvectl dns ens3 8.8.8.8 1.1.1.1 || true
resolvectl domain ens3 '~.' || true
printf 'nameserver 8.8.8.8\nnameserver 1.1.1.1\n' > /etc/resolv.conf || true

apt-get update
DEBIAN_FRONTEND=noninteractive apt-get install -y apache2 curl
systemctl enable apache2

IP=$(hostname -I | awk '{print $1}')
cat > /var/www/html/index.html <<EOF
<h1>Nhom 04 - Dia chi IP cua toi la: $IP</h1>
EOF

systemctl restart apache2
systemctl status apache2 --no-pager || true
curl -I http://127.0.0.1 || true";

        string userDataBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(bashScript));

        var payload = new
        {
            server = new
            {
                name = vmName,
                imageRef = imageId,
                flavorRef = flavorId,
                key_name = string.IsNullOrWhiteSpace(keyPairName) ? null : keyPairName,
                networks = new[] { new { uuid = networkId } },
                user_data = userDataBase64,
                security_groups = new[] { new { name = "default" } }, // Đảm bảo dùng Security Group mặc định
                block_device_mapping_v2 = new[]
                {
                    new
                    {
                        uuid = imageId, // ID của Image (giống imageId)
                        source_type = "image",
                        destination_type = "volume",
                        boot_index = 0,
                        volume_size = volumeSize.ToString() // Kích thước Volume (GB)
                    }
                }
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
    public async Task<string> AddMemberToLoadBalancerPoolAsync(string poolId, string vmIpAddress, string subnetId, int protocolPort = 80)
    {
        SetAuthHeader();

        var payload = new
        {
            member = new 
            {
                address = vmIpAddress,      // IP nội bộ của máy ảo vừa tạo
                protocol_port = protocolPort,// Port chạy Web
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

    // Hàm lấy Port Interface của Instance
    public async Task<string> GetPortInterfaceAsync(string serverId)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"https://cloud-compute.uitiot.vn/v2.1/servers/{serverId}/os-interface");
        return await response.Content.ReadAsStringAsync();
    }

    // ===== LOAD BALANCER METHODS =====

    // Hàm tạo LoadBalancer
    public async Task<string> CreateLoadBalancerAsync(string lbName, string vipSubnetId)
    {
        SetAuthHeader();

        var payload = new
        {
            loadbalancer = new
            {
                name = lbName,
                vip_subnet_id = vipSubnetId,
                admin_state_up = true
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/loadbalancers", content);
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm tạo Listener
    public async Task<string> CreateListenerAsync(string lbId, string name, string protocol, int port)
    {
        SetAuthHeader();

        var payload = new
        {
            listener = new
            {
                name = name,
                loadbalancer_id = lbId,
                protocol = protocol.ToUpper(), // HTTP, HTTPS, TCP, UDP
                protocol_port = port,
                admin_state_up = true
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/listeners", content);
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm tạo Pool (nhóm backend servers)
    public async Task<string> CreatePoolAsync(string name, string listenerId, string protocol)
    {
        SetAuthHeader();

        var payload = new
        {
            pool = new
            {
                name = name,
                listener_id = listenerId,
                protocol = protocol.ToUpper(),
                lb_algorithm = "ROUND_ROBIN", // Cân bằng tải kiểu Round Robin
                admin_state_up = true
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/pools", content);
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm tạo Health Monitor
    public async Task<string> CreateHealthMonitorAsync(string poolId, string type = "HTTP", int delay = 5, int timeout = 5, int maxRetries = 3)
    {
        SetAuthHeader();

        var payload = new
        {
            healthmonitor = new
            {
                pool_id = poolId,
                type = type,
                delay = delay,          // Thời gian chờ giữa các lần kiểm tra (giây)
                timeout = timeout,      // Timeout cho mỗi lần kiểm tra (giây)
                max_retries = maxRetries, // Số lần retry trước khi coi là down
                admin_state_up = true
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/healthmonitors", content);
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm lấy danh sách LoadBalancer
    public async Task<string> GetLoadBalancersAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/loadbalancers");
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetPoolsAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/pools");
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetListenersAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/listeners");
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm lấy chi tiết 1 LoadBalancer
    public async Task<string> GetLoadBalancerDetailsAsync(string lbId)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/loadbalancers/{lbId}");
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm xóa LoadBalancer
    public async Task<string> DeleteLoadBalancerAsync(string lbId)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/loadbalancers/{lbId}");
        return response.IsSuccessStatusCode ? "Xóa LoadBalancer thành công" : "Lỗi xóa LoadBalancer";
    }

    // Hàm xóa Listener
    public async Task<string> DeleteListenerAsync(string listenerId)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/listeners/{listenerId}");
        return response.IsSuccessStatusCode ? "Xóa Listener thành công" : "Lỗi xóa Listener";
    }

    // Hàm xóa Pool
    public async Task<string> DeletePoolAsync(string poolId)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"https://cloud-loadbalancer.uitiot.vn/v2.0/lbaas/pools/{poolId}");
        return response.IsSuccessStatusCode ? "Xóa Pool thành công" : "Lỗi xóa Pool";
    }

    // Hàm gắn Floating IP cho LoadBalancer
    public async Task<string> AssignFloatingIpToLoadBalancerAsync(string lbId, string extNetworkId)
    {
        SetAuthHeader();

        // Lấy VIP Port ID từ LoadBalancer details
        var lbDetails = await GetLoadBalancerDetailsAsync(lbId);
        JObject lbJson = JObject.Parse(lbDetails);
        string vipPortId = lbJson["loadbalancer"]["vip_port_id"].ToString();

        // Gắn Floating IP vào VIP Port
        var payload = new
        {
            floatingip = new
            {
                floating_network_id = extNetworkId,
                port_id = vipPortId
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://cloud-network.uitiot.vn/v2.0/floatingips", content);
        return await response.Content.ReadAsStringAsync();
    }

    // Hàm lấy Network ID từ Subnet ID
    public async Task<string> GetNetworkIdFromSubnetAsync(string subnetId)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"https://cloud-network.uitiot.vn/v2.0/subnets/{subnetId}");
        string result = await response.Content.ReadAsStringAsync();

        JObject json = JObject.Parse(result);
        return json["subnet"]["network_id"].ToString();
    }

}
