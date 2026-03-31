using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenStackManager
{
    public partial class Form1 : Form
    {
        private OpenStackClient osClient = new OpenStackClient();
        private string _selectedInstanceFixedIp;
        private string _selectedInstanceSubnetId;
        private class SelectItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public override string ToString()
            {
                if (string.IsNullOrWhiteSpace(Id)) return string.IsNullOrWhiteSpace(Name) ? "None" : Name;
                if (string.IsNullOrWhiteSpace(Name)) return Id;
                if (string.Equals(Name, Id, StringComparison.OrdinalIgnoreCase)) return Name;
                return $"{Name} ({Id})";
            }
        }
        public Form1()
        {
            InitializeComponent();
            LoadProtocolDefaults();
            txtListenerProtocol.SelectedIndexChanged += txtListenerProtocol_SelectedIndexChanged;
            txtPoolProtocol.SelectedIndexChanged += txtPoolProtocol_SelectedIndexChanged;
            tabControl2.SelectedIndexChanged += tabControl2_SelectedIndexChanged;
        }

        private void LoadProtocolDefaults()
        {
            BindItems(txtListenerProtocol, new[]
            {
                new SelectItem { Name = "HTTP", Id = "HTTP" },
                new SelectItem { Name = "TCP", Id = "TCP" },
                new SelectItem { Name = "HTTPS", Id = "HTTPS" },
                new SelectItem { Name = "UDP", Id = "UDP" },
                new SelectItem { Name = "SCTP", Id = "SCTP" }
            });

            BindItems(txtPoolProtocol, new[]
            {
                new SelectItem { Name = "HTTP", Id = "HTTP" },
                new SelectItem { Name = "TCP", Id = "TCP" },
                new SelectItem { Name = "HTTPS", Id = "HTTPS" },
                new SelectItem { Name = "UDP", Id = "UDP" },
                new SelectItem { Name = "SCTP", Id = "SCTP" }
            });

            BindItems(txtHealthCheckType, new[]
            {
                new SelectItem { Name = "HTTP", Id = "HTTP" },
                new SelectItem { Name = "PING", Id = "PING" },
                new SelectItem { Name = "TCP", Id = "TCP" },
                new SelectItem { Name = "UDP", Id = "UDP" }
            });

            if (txtListenerProtocol.Items.Count > 0) txtListenerProtocol.SelectedIndex = 0;
            if (txtPoolProtocol.Items.Count > 0) txtPoolProtocol.SelectedIndex = 0;
            if (txtHealthCheckType.Items.Count > 0) txtHealthCheckType.SelectedIndex = 0;
        }

        private void txtListenerProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            var protocol = SelectedId(txtListenerProtocol);
            txtListenerPort.Text = GetDefaultPortForProtocol(protocol);
        }

        private void txtPoolProtocol_SelectedIndexChanged(object sender, EventArgs e)
        {
            var protocol = SelectedId(txtPoolProtocol);
            txtPoolPort.Text = GetDefaultPortForProtocol(protocol);
        }

        private string GetDefaultPortForProtocol(string protocol)
        {
            switch ((protocol ?? string.Empty).ToUpperInvariant())
            {
                case "HTTP":
                    return "80";
                case "":
                case "HTTPS":
                case "TCP":
                case "UDP":
                case "SCTP":
                default:
                    return string.Empty;
            }
        }

        private void BindItems(System.Windows.Forms.ComboBox combo, IEnumerable<SelectItem> items)
        {
            combo.BeginUpdate();
            combo.Items.Clear();
            combo.Items.Add(new SelectItem { Name = "None", Id = string.Empty });
            foreach (var item in items) combo.Items.Add(item);
            combo.SelectedIndex = 0;
            combo.EndUpdate();
        }

        private void ResetComboToNone(System.Windows.Forms.ComboBox combo)
        {
            if (combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
        }

        private async void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetComboToNone(txtNetworkId);
            ResetComboToNone(txtTargetId);
            ResetComboToNone(txtidInterface);
            ResetComboToNone(txtLBSubnetId);
            ResetComboToNone(txtLBId);
            ResetComboToNone(txtListenerId);
            ResetComboToNone(txtPoolId);
            ResetComboToNone(txtLbPoolId);

            // Keep VM selection and port mapping stable across tabs.
            if (!string.IsNullOrWhiteSpace(SelectedId(txtInstanceId)))
            {
                await LoadPortsForSelectedInstanceAsync();
            }
        }

        private string SelectedId(System.Windows.Forms.ComboBox combo)
        {
            var selected = combo.SelectedItem as SelectItem;
            if (selected != null && !string.IsNullOrWhiteSpace(selected.Id))
            {
                return selected.Id;
            }

            var text = combo.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var open = text.LastIndexOf('(');
            var close = text.EndsWith(")") ? text.Length - 1 : -1;
            if (open >= 0 && close > open)
            {
                var candidate = text.Substring(open + 1, close - open - 1).Trim();
                if (!string.IsNullOrWhiteSpace(candidate))
                {
                    return candidate;
                }
            }

            return text;
        }

        private async Task RefreshRouterListAsync()
        {
            var result = await osClient.GetRoutersAsync();
            var json = JObject.Parse(result);
            var items = new List<SelectItem>();
            foreach (var r in json["routers"]) items.Add(new SelectItem { Name = r["name"]?.ToString(), Id = r["id"]?.ToString() });
            BindItems(txtTargetId, items);
        }

        private async Task RefreshNetworkAndSubnetListsAsync()
        {
            var netResult = await osClient.GetNetworksAsync();
            var netJson = JObject.Parse(netResult);
            var nets = new List<SelectItem>();
            var extNets = new List<SelectItem>();
            foreach (var n in netJson["networks"]) nets.Add(new SelectItem { Name = n["name"]?.ToString(), Id = n["id"]?.ToString() });
            foreach (var n in netJson["networks"])
            {
                if (n["router:external"]?.Value<bool>() == true)
                {
                    extNets.Add(new SelectItem { Name = n["name"]?.ToString(), Id = n["id"]?.ToString() });
                }
            }
            BindItems(txtNetworkId, nets);
            BindItems(txtNetId, nets);
            BindItems(txtExternalNetworkId, extNets);

            var subResult = await osClient.GetSubnetsAsync();
            var subJson = JObject.Parse(subResult);
            var subs = new List<SelectItem>();
            foreach (var s in subJson["subnets"]) subs.Add(new SelectItem { Name = s["name"]?.ToString(), Id = s["id"]?.ToString() });
            BindItems(txtidInterface, subs);
            BindItems(txtLBSubnetId, subs);
        }

        private async Task RefreshInstanceAndPortListsAsync()
        {
            var result = await osClient.GetInstancesAsync();
            var json = JObject.Parse(result);
            var items = new List<SelectItem>();
            foreach (var s in json["servers"]) items.Add(new SelectItem { Name = s["name"]?.ToString(), Id = s["id"]?.ToString() });
            BindItems(txtInstanceId, items);
            if (txtInstanceId.Items.Count > 0)
            {
                await LoadPortsForSelectedInstanceAsync();
            }
        }

        private async Task LoadPortsForSelectedInstanceAsync()
        {
            var targetId = SelectedId(txtInstanceId);
            if (string.IsNullOrWhiteSpace(targetId))
            {
                _selectedInstanceFixedIp = null;
                _selectedInstanceSubnetId = null;
                BindItems(txtPortId, new List<SelectItem>());
                return;
            }

            var result = await osClient.GetPortInterfaceAsync(targetId);
            var json = JObject.Parse(result);
            var interfaces = json["interfaceAttachments"];
            var ports = new List<SelectItem>();
            _selectedInstanceFixedIp = null;
            _selectedInstanceSubnetId = null;

            if (interfaces != null && interfaces.HasValues)
            {
                foreach (var iface in interfaces)
                {
                    var portId = iface["port_id"]?.ToString();
                    var mac = iface["mac_addr"]?.ToString();
                    var fixedIps = iface["fixed_ips"];
                    var ip = fixedIps != null && fixedIps.HasValues ? fixedIps.First?["ip_address"]?.ToString() : null;
                    var subnetId = fixedIps != null && fixedIps.HasValues ? fixedIps.First?["subnet_id"]?.ToString() : null;
                    if (!string.IsNullOrWhiteSpace(portId))
                    {
                        if (string.IsNullOrWhiteSpace(_selectedInstanceFixedIp) && !string.IsNullOrWhiteSpace(ip))
                        {
                            _selectedInstanceFixedIp = ip;
                            _selectedInstanceSubnetId = subnetId;
                        }
                        ports.Add(new SelectItem { Name = string.IsNullOrWhiteSpace(ip) ? portId : $"{ip}", Id = portId });
                    }
                }
            }

            BindItems(txtPortId, ports);
            if (txtPortId.Items.Count > 1)
            {
                txtPortId.SelectedIndex = 1;
            }
        }

        private async Task RefreshLoadBalancerListsAsync()
        {
            var result = await osClient.GetLoadBalancersAsync();
            var json = JObject.Parse(result);
            var lbs = new List<SelectItem>();
            foreach (var lb in json["loadbalancers"]) lbs.Add(new SelectItem { Name = lb["name"]?.ToString(), Id = lb["id"]?.ToString() });
            BindItems(txtLBId, lbs);
        }

        private async Task RefreshPoolListAsync()
        {
            var result = await osClient.GetPoolsAsync();
            var json = JObject.Parse(result);
            var pools = new List<SelectItem>();
            if (json["pools"] != null)
            {
                foreach (var p in json["pools"])
                {
                    pools.Add(new SelectItem { Name = p["name"]?.ToString(), Id = p["id"]?.ToString() });
                }
            }
            BindItems(txtPoolId, pools);
            BindItems(txtLbPoolId, pools);
        }

        private async Task RefreshListenerListAsync()
        {
            var result = await osClient.GetListenersAsync();
            var json = JObject.Parse(result);
            var listeners = new List<SelectItem>();
            if (json["listeners"] != null)
            {
                foreach (var l in json["listeners"])
                {
                    listeners.Add(new SelectItem { Name = l["name"]?.ToString(), Id = l["id"]?.ToString() });
                }
            }
            BindItems(txtListenerId, listeners);
        }

        private async Task LoadAllAfterLoginAsync()
        {
            await Task.WhenAll(
                RefreshNetworkAndSubnetListsAsync(),
                RefreshRouterListAsync(),
                RefreshInstanceAndPortListsAsync(),
                RefreshLoadBalancerListsAsync(),
                RefreshPoolListAsync(),
                RefreshListenerListAsync()
            );

            await LoadFlavorsAsync();
            await LoadImagesAsync();
            await LoadKeyPairsAsync();
        }

        private async Task LoadFlavorsAsync()
        {
            var result = await osClient.GetFlavorsAsync();
            var json = JObject.Parse(result);
            var items = new List<SelectItem>();
            foreach (var f in json["flavors"]) items.Add(new SelectItem { Name = f["name"]?.ToString(), Id = f["id"]?.ToString() });
            BindItems(txtFlavorId, items);
        }

        private async Task LoadImagesAsync()
        {
            var result = await osClient.GetImagesAsync();
            var json = JObject.Parse(result);
            var items = new List<SelectItem>();
            foreach (var img in json["images"]) items.Add(new SelectItem { Name = img["name"]?.ToString(), Id = img["id"]?.ToString() });
            BindItems(txtImageId, items);
        }

        private async Task LoadKeyPairsAsync()
        {
            var result = await osClient.GetKeyPairsAsync();
            var json = JObject.Parse(result);
            var items = new List<SelectItem>();
            if (json["keypairs"] != null)
            {
                foreach (var kp in json["keypairs"])
                {
                    var keypair = kp["keypair"];
                    if (keypair != null)
                    {
                        var name = keypair["name"]?.ToString();
                        items.Add(new SelectItem { Name = name, Id = name });
                    }
                }
            }
            BindItems(txtKeyPair, items);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            rtbLog.Text = "Đang gửi yêu cầu đăng nhập lên OpenStack...\r\n";
            btnLogin.Enabled = false;

            try
            {
                bool isSuccess = await osClient.AuthenticateAsync(
                    txtUrl.Text,
                    txtUsername.Text,
                    txtPassword.Text,
                    txtProjectName.Text
                );

                if (isSuccess)
                {
                    rtbLog.Text += "Đăng nhập THÀNH CÔNG!\r\n";
                    rtbLog.Text += "Mã Token của bạn là: " + osClient.AuthToken + "\r\n";
                    await LoadAllAfterLoginAsync();
                }
                else
                {
                    rtbLog.Text += "Đăng nhập THẤT BẠI. Hãy kiểm tra lại thông tin User/Pass/Project Name.\r\n";
                }
            }
            catch (Exception ex)
            {
                rtbLog.Text += "Lỗi hệ thống: " + ex.Message + "\r\n";
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private async void btnGetFlavors_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            rtbLog.Text = "Đang lấy danh sách cấu hình (Flavor)...\r\n";
            string result = await osClient.GetFlavorsAsync();
            try
            {
                JObject json = JObject.Parse(result);
                var flavors = json["flavors"];
                rtbLog.Text = "[DANH SÁCH FLAVORS]\r\n";
                int i = 1;
                foreach (var f in flavors)
                {
                    rtbLog.Text += $"{i}. Tên: {f["name"]} | ID: {f["id"]} | RAM: {f["ram"]}MB | vCPU: {f["vcpus"]}\r\n";
                    i++;
                }
                await LoadFlavorsAsync();
            }
            catch { rtbLog.Text += result + "\r\n"; }
        }

        private async void btnGetImages_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            rtbLog.Text = "Đang lấy danh sách hệ điều hành (Image)...\r\n";
            string result = await osClient.GetImagesAsync();
            try
            {
                JObject json = JObject.Parse(result);
                var images = json["images"];
                rtbLog.Text = "[DANH SÁCH IMAGES]\r\n";
                int i = 1;
                foreach (var img in images)
                {
                    rtbLog.Text += $"{i}. Tên: {img["name"]} | ID: {img["id"]} | Status: {img["status"]}\r\n";
                    i++;
                }
                await LoadImagesAsync();
            }
            catch { rtbLog.Text += result + "\r\n"; }
        }

        private async void btnCreateNet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken))
            {
                MessageBox.Show("Vui lòng đăng nhập trước!"); return;
            }

            rtbLog.Text = "1. Đang tạo Network...\r\n";
            string networkName = string.IsNullOrWhiteSpace(txtNetworkName.Text) ? "test-net" : txtNetworkName.Text.Trim();
            string subnetName = string.IsNullOrWhiteSpace(txtSubnetName.Text) ? "test-subnet" : txtSubnetName.Text.Trim();
            string cidr = string.IsNullOrWhiteSpace(txtCidr.Text) ? "192.168.100.0/24" : txtCidr.Text.Trim();

            string netResult = await osClient.CreateNetworkAsync(networkName);

            try
            {
                // Trích xuất cái ID của Network từ chuỗi JSON trả về
                JObject netJson = JObject.Parse(netResult);
                string networkId = netJson["network"]["id"].ToString();
                rtbLog.Text += $"=> Tạo Net thành công! ID: {networkId}\r\n\r\n";

                // Dùng ID đó để tạo Subnet
                rtbLog.Text += "2. Đang tạo Subnet cho Network này...\r\n";
                // CIDR là dải IP - 192.168.100.0/24
                string subResult = await osClient.CreateSubnetAsync(networkId, subnetName, cidr);

                rtbLog.Text += "=> Tạo Subnet thành công!\r\n";
                await RefreshNetworkAndSubnetListsAsync();
            }
            catch (Exception ex)
            {
                rtbLog.Text += "Lỗi bóc tách JSON: " + ex.Message + "\r\n";
                rtbLog.Text += "Chi tiết phản hồi: " + netResult + "\r\n";
            }
        }

        private async void btnCreateRouter_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken))
            {
                MessageBox.Show("Vui lòng đăng nhập trước!"); return;
            }

            rtbLog.Text = "Đang tạo Router nối ra Internet...\r\n";

            //public_net
            string routerName = string.IsNullOrWhiteSpace(txtRouterName.Text) ? "test-router" : txtRouterName.Text.Trim();
            string extNetworkId = "c3455e8f-ea16-4f5d-ad5e-5c4292015a0d"; // public_net

            string result = await osClient.CreateRouterAsync(routerName, extNetworkId);
            rtbLog.Text += result + "\r\n";
            await RefreshRouterListAsync();
        }

        private async void btnAttachRouter_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }

            string routerId = SelectedId(txtTargetId);
            string subnetId = SelectedId(txtidInterface);

            if (string.IsNullOrEmpty(routerId))
            {
                MessageBox.Show("Vui lòng nhập ID của Router vào ô Target ID và ID Subnet vào ô ID Interface!", "Thiếu thông tin");
                return;
            }

            rtbLog.Text = $"Đang cắm Router ({routerId}) vào Subnet nội bộ...\r\n";

            try
            {
                string result = await osClient.AddInterfaceToRouterAsync(routerId, subnetId);
                rtbLog.Text += "Kết quả nối mạng:\r\n" + result + "\r\n";
            }
            catch (Exception ex)
            {
                rtbLog.Text += "Lỗi khi cắm dây: " + ex.Message + "\r\n";
            }
        }

        private async void btnCreateVM_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken))
            {
                MessageBox.Show("Vui lòng đăng nhập trước!"); return;
            }

            rtbLog.Text += "Đang gửi lệnh tạo máy ảo. Quá trình này có thể mất 1-2 phút...\r\n";

            // Lấy Volume Size từ TextBox (default 4GB)
            int volumeSize = 4;
            if (!string.IsNullOrEmpty(txtVolumeSize.Text) && int.TryParse(txtVolumeSize.Text, out int size))
            {
                volumeSize = size;
            }

            var imageId = SelectedId(txtImageId);
            var flavorId = SelectedId(txtFlavorId);
            var netId = SelectedId(txtNetId);

            if (string.IsNullOrEmpty(imageId) || string.IsNullOrEmpty(flavorId) || string.IsNullOrEmpty(netId))
            {
                MessageBox.Show("Vui lòng chọn Image, Flavor và Net hợp lệ!", "Thiếu thông tin");
                return;
            }

            string result = await osClient.CreateInstanceWithWebAsync(
                txtVmName.Text,
                imageId,
                flavorId,
                netId,
                SelectedId(txtKeyPair),
                volumeSize
            );

            rtbLog.Text += "Kết quả khởi tạo:\r\n" + result + "\r\n";
            rtbLog.Text += "=> Đợi khoảng 1 phút rồi vào OpenStack kiểm tra xem Web Server đã chạy chưa nhé!\r\n";
        }

        private async void btnScaleUp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken))
            {
                MessageBox.Show("Vui lòng đăng nhập trước!"); return;
            }

            string newVmIp = _selectedInstanceFixedIp;
            string subnetId = _selectedInstanceSubnetId;
            int memberPort = 80;
            if (!string.IsNullOrWhiteSpace(txtPoolPort.Text))
            {
                if (!int.TryParse(txtPoolPort.Text.Trim(), out memberPort))
                {
                    MessageBox.Show("Port thành viên phải là số hợp lệ!", "Thiếu thông tin");
                    return;
                }
            }

            if (string.IsNullOrEmpty(newVmIp) || string.IsNullOrEmpty(subnetId))
            {
                MessageBox.Show("Vui lòng chọn Instance có IP nội bộ và Subnet hợp lệ!", "Thiếu thông tin");
                return;
            }

            rtbLog.Text += $"Đang đưa VM ({newVmIp}) vào Load Balancer...\r\n";

            string result = await osClient.AddMemberToLoadBalancerPoolAsync(SelectedId(txtPoolId), newVmIp, subnetId, memberPort);

            rtbLog.Text += "Kết quả Tăng máy ảo (Scale Up):\r\n" + result + "\r\n";
        }

        private async void btnScaleDown_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken))
            {
                MessageBox.Show("Vui lòng đăng nhập trước!"); return;
            }

            var poolId = SelectedId(txtPoolId);
            if (string.IsNullOrEmpty(poolId))
            {
                MessageBox.Show("Vui lòng chọn Pool ID hợp lệ!", "Thiếu thông tin");
                return;
            }

            string memberIdToRemove = txtMemberId.Text.Trim();
            if (string.IsNullOrEmpty(memberIdToRemove))
            {
                MessageBox.Show("Vui lòng nhập Member ID cần xóa trong ô Member ID!", "Thiếu thông tin");
                return;
            }

            rtbLog.Text += "Đang rút máy ảo khỏi Load Balancer...\r\n";

            string result = await osClient.RemoveMemberFromPoolAsync(poolId, memberIdToRemove);

            rtbLog.Text += "Kết quả Giảm máy ảo (Scale Down):\r\n" + result + "\r\n";
            rtbLog.Text += "=> (Tùy chọn) Bạn có thể gọi thêm hàm DeleteInstanceAsync() để xóa hẳn máy ảo này cho đỡ tốn tài nguyên.\r\n";
        }

        private async void btnDeleteVM_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var targetId = SelectedId(txtInstanceId);
            if (string.IsNullOrEmpty(targetId)) { MessageBox.Show("Vui lòng chọn ID của Máy ảo cần xóa"); return; }

            rtbLog.Text += $"Đang gửi lệnh xóa Máy ảo (ID: {targetId})...\r\n";
            string result = await osClient.DeleteInstanceAsync(targetId);
            rtbLog.Text += $"Kết quả: {result}\r\n";
            await RefreshInstanceAndPortListsAsync();
        }

        private async void btnDeleteRouter_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var targetId = SelectedId(txtTargetId);
            if (string.IsNullOrEmpty(targetId)) { MessageBox.Show("Vui lòng chọn ID của Router cần xóa"); return; }

            rtbLog.Text += $"Đang gửi lệnh xóa Router...\r\n";
            string result = await osClient.DeleteRouterAsync(targetId);
            rtbLog.Text += $"Kết quả: {result}\r\n";
            await RefreshRouterListAsync();
        }

        private async void btnDeleteNet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var targetId = SelectedId(txtNetworkId);
            if (string.IsNullOrEmpty(targetId)) { MessageBox.Show("Vui lòng chọn ID của Network cần xóa"); return; }

            rtbLog.Text += $"Đang gửi lệnh xóa Network...\r\n";
            string result = await osClient.DeleteNetworkAsync(targetId);
            rtbLog.Text += $"Kết quả: {result}\r\n";
            await RefreshNetworkAndSubnetListsAsync();
        }

        private async void btnAssignFip_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var portId = SelectedId(txtPortId);
            if (string.IsNullOrEmpty(portId)) { MessageBox.Show("Vui lòng chọn Port ID của máy ảo!"); return; }

            rtbLog.Text += "Đang xin cấp Floating IP và gắn vào máy ảo...\r\n";

            //public_net
            string extNetworkId = "c3455e8f-ea16-4f5d-ad5e-5c4292015a0d";

            try
            {
                string result = await osClient.CreateAndAssignFloatingIpAsync(extNetworkId, portId);
                JObject fipJson = JObject.Parse(result);
                var floatingIp = fipJson["floatingip"];
                if (floatingIp == null)
                {
                    rtbLog.Text += "Không gắn được Floating IP.\r\n";
                    rtbLog.Text += "Response:\r\n" + result + "\r\n";
                    return;
                }

                string floatingIpAddress = floatingIp["floating_ip_address"]?.ToString();
                if (string.IsNullOrWhiteSpace(floatingIpAddress))
                {
                    rtbLog.Text += "Floating IP response không chứa địa chỉ IP.\r\n";
                    rtbLog.Text += "Response:\r\n" + result + "\r\n";
                    return;
                }

                rtbLog.Text += $"=> THÀNH CÔNG! Máy ảo của bạn đã có thể truy cập qua IP: {floatingIpAddress}\r\n";
            }
            catch (Exception ex)
            {
                rtbLog.Text += "Lỗi gắn Floating IP: " + ex.Message + "\r\n";
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private async void btnGetRouters_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }

            rtbLog.Text = "Đang lấy danh sách Router...\r\n";
            string result = await osClient.GetRoutersAsync();

            try
            {
                JObject json = JObject.Parse(result);
                var routers = json["routers"];
                rtbLog.Text = "[DANH SÁCH ROUTERS]\r\n";
                int i = 1;
                foreach (var r in routers)
                {
                    rtbLog.Text += $"{i}. Tên: {r["name"]} | ID: {r["id"]} | Status: {r["status"]}\r\n";
                    i++;
                }
            }
            catch { rtbLog.Text += result + "\r\n"; }
        }

        private async void btnGetInstances_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }

            rtbLog.Text = "Đang lấy danh sách Máy ảo (Instances)...\r\n";
            string result = await osClient.GetInstancesAsync();

            try
            {
                JObject json = JObject.Parse(result);
                var servers = json["servers"];
                rtbLog.Text = "[DANH SÁCH MÁY ẢO]\r\n";
                int i = 1;
                foreach (var s in servers)
                {
                    rtbLog.Text += $"{i}. Tên: {s["name"]} | ID: {s["id"]} | Status: {s["status"]}\r\n";
                    if (s["addresses"].HasValues)
                    {
                        rtbLog.Text += $"   -> Địa chỉ mạng:\r\n{s["addresses"].ToString(Newtonsoft.Json.Formatting.Indented)}\r\n";
                    }
                    rtbLog.Text += "--------------------------------------\r\n";
                    i++;
                }
            }
            catch { rtbLog.Text += result + "\r\n"; }

            await RefreshInstanceAndPortListsAsync();
        }

        private async void btnGetNetworks_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }

            rtbLog.Text = "Đang lấy danh sách Network...\r\n";
            string result = await osClient.GetNetworksAsync();

            try
            {
                JObject json = JObject.Parse(result);
                var networks = json["networks"];
                rtbLog.Text = "[DANH SÁCH NETWORKS]\r\n";
                int i = 1;
                foreach (var net in networks)
                {
                    rtbLog.Text += $"{i}. Tên: {net["name"]} | ID: {net["id"]} | Status: {net["status"]}\r\n";
                    i++;
                }
            }
            catch { rtbLog.Text += result + "\r\n"; }
        }

        private async void btnGetSubnets_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }

            rtbLog.Text = "Đang lấy danh sách Subnet...\r\n";
            string result = await osClient.GetSubnetsAsync();

            try
            {
                JObject json = JObject.Parse(result);
                var subnets = json["subnets"];
                rtbLog.Text = "[DANH SÁCH SUBNETS]\r\n";
                int i = 1;
                foreach (var sub in subnets)
                {
                    rtbLog.Text += $"{i}. Tên: {sub["name"]} | ID: {sub["id"]} | CIDR: {sub["cidr"]}\r\n";
                    i++;
                }
            }
            catch { rtbLog.Text += result + "\r\n"; }
        }

        private void rtbLog_TextChanged(object sender, EventArgs e)
        {

        }

        private async void butgetPortPC_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var targetId = SelectedId(txtInstanceId);
            if (string.IsNullOrEmpty(targetId)) { MessageBox.Show("Vui lòng chọn ID của Instance!"); return; }

            rtbLog.Text = "Đang lấy Port Interface của Instance...\r\n";
            string result = await osClient.GetPortInterfaceAsync(targetId);

            try
            {
                JObject json = JObject.Parse(result);
                var interfaces = json["interfaceAttachments"];
                rtbLog.Text = "[PORT INTERFACE CỦA INSTANCE]\r\n";

                if (interfaces == null || !interfaces.HasValues)
                {
                    rtbLog.Text += "Không có interface attachment nào hoặc instance chưa sẵn sàng.\r\n";
                    rtbLog.Text += "Response:\r\n" + result + "\r\n";
                    return;
                }

                int i = 1;
                foreach (var iface in interfaces)
                {
                    if (iface == null)
                    {
                        continue;
                    }

                    rtbLog.Text += $"{i}. Port ID: {iface["port_id"]}\r\n";
                    rtbLog.Text += $"   Mac Address: {iface["mac_addr"]}\r\n";

                    var fixedIps = iface["fixed_ips"];
                    if (fixedIps != null && fixedIps.HasValues)
                    {
                        rtbLog.Text += $"   Fixed IPs:\r\n";
                        foreach (var ip in fixedIps)
                        {
                            rtbLog.Text += $"      - IP: {ip["ip_address"]} | Subnet ID: {ip["subnet_id"]}\r\n";
                        }
                    }
                    rtbLog.Text += "--------------------------------------\r\n";
                    i++;
                }
            }
            catch (Exception ex)
            {
                rtbLog.Text += "Lỗi lấy Port Interface: " + ex.Message + "\r\n";
                rtbLog.Text += "Response: " + result + "\r\n";
            }
        }

        private async void btnCreateLB_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            if (string.IsNullOrEmpty(txtLBName.Text)) { MessageBox.Show("Vui lòng nhập tên LoadBalancer!"); return; }
            var subnetId = SelectedId(txtLBSubnetId);
            if (string.IsNullOrEmpty(subnetId)) { MessageBox.Show("Vui lòng chọn Subnet ID!"); return; }

            rtbLog.Text = "Đang tạo LoadBalancer...\r\n";

            try
            {
                string result = await osClient.CreateLoadBalancerAsync(txtLBName.Text, subnetId);
                rtbLog.Text += "Response: " + result + "\r\n";

                JObject json = JObject.Parse(result);
                if (json["loadbalancer"] != null)
                {
                    string lbId = json["loadbalancer"]["id"].ToString();
                    rtbLog.Text += $"=> Tạo LoadBalancer thành công! ID: {lbId}\r\n";
                    await RefreshLoadBalancerListsAsync();
                }
                else
                {
                    rtbLog.Text += "Lỗi: Response không chứa loadbalancer\r\n";
                }
            }
            catch (Exception ex)
            {
                rtbLog.Text += "Lỗi tạo LoadBalancer: " + ex.Message + "\r\n";
            }
        }

        private async void btnCreateListener_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var lbId = SelectedId(txtLBId);
            if (string.IsNullOrEmpty(lbId)) { MessageBox.Show("Vui lòng chọn ID LoadBalancer!"); return; }
            if (string.IsNullOrEmpty(txtListenerName.Text)) { MessageBox.Show("Vui lòng nhập tên Listener!"); return; }
            var protocol = SelectedId(txtListenerProtocol);
            if (string.IsNullOrEmpty(protocol)) { MessageBox.Show("Vui lòng chọn Protocol!"); return; }
            if (string.IsNullOrEmpty(txtListenerPort.Text) || !int.TryParse(txtListenerPort.Text, out int port)) { MessageBox.Show("Vui lòng nhập Port hợp lệ!"); return; }

            rtbLog.Text = "Đang tạo Listener...\r\n";

            try
            {
                string result = await osClient.CreateListenerAsync(lbId, txtListenerName.Text, protocol, port);
                rtbLog.Text += "Response: " + result + "\r\n";

                JObject json = JObject.Parse(result);
                if (json["listener"] != null)
                {
                    string listenerId = json["listener"]["id"].ToString();
                    rtbLog.Text += $"=> Tạo Listener thành công! ID: {listenerId}\r\n";
                    await RefreshListenerListAsync();
                }
                else
                {
                    rtbLog.Text += "Lỗi: Response không chứa listener\r\n";
                }
            }
            catch (Exception ex)
            {
                rtbLog.Text += "Lỗi tạo Listener: " + ex.Message + "\r\n";
            }
        }

        private async void btnCreatePool_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var listenerId = SelectedId(txtListenerId);
            if (string.IsNullOrEmpty(listenerId)) { MessageBox.Show("Vui lòng chọn ID Listener!"); return; }
            if (string.IsNullOrEmpty(txtPoolName.Text)) { MessageBox.Show("Vui lòng nhập tên Pool!"); return; }
            var poolProtocol = SelectedId(txtPoolProtocol);
            if (string.IsNullOrEmpty(poolProtocol)) { MessageBox.Show("Vui lòng chọn Protocol!"); return; }

            rtbLog.Text = "Đang tạo Pool...\r\n";

            try
            {
                string result = await osClient.CreatePoolAsync(txtPoolName.Text, listenerId, poolProtocol);
                rtbLog.Text += "Response: " + result + "\r\n";

                JObject json = JObject.Parse(result);
                if (json["pool"] != null)
                {
                    string poolId = json["pool"]["id"].ToString();
                    rtbLog.Text += $"=> Tạo Pool thành công! ID: {poolId}\r\n";
                    await RefreshPoolListAsync();
                }
                else
                {
                    rtbLog.Text += "Lỗi: Response không chứa pool\r\n";
                }
            }
            catch (Exception ex)
            {
                rtbLog.Text += "Lỗi tạo Pool: " + ex.Message + "\r\n";
            }
        }

        private async void btnCreateHealthMonitor_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var poolId = SelectedId(txtLbPoolId);
            if (string.IsNullOrEmpty(poolId)) { MessageBox.Show("Vui lòng chọn ID Pool!"); return; }

            rtbLog.Text = "Đang tạo Health Monitor...\r\n";

            try
            {
                string hcType = string.IsNullOrEmpty(SelectedId(txtHealthCheckType)) ? "HTTP" : SelectedId(txtHealthCheckType);
                int delay = string.IsNullOrEmpty(txtHealthCheckDelay.Text) ? 5 : int.Parse(txtHealthCheckDelay.Text);
                int timeout = string.IsNullOrEmpty(txtHealthCheckTimeout.Text) ? 5 : int.Parse(txtHealthCheckTimeout.Text);
                int maxRetries = string.IsNullOrEmpty(txtHealthCheckRetries.Text) ? 3 : int.Parse(txtHealthCheckRetries.Text);

                string result = await osClient.CreateHealthMonitorAsync(poolId, hcType, delay, timeout, maxRetries);
                rtbLog.Text += "Response: " + result + "\r\n";

                JObject json = JObject.Parse(result);
                if (json["healthmonitor"] != null)
                {
                    string hmId = json["healthmonitor"]["id"].ToString();
                    rtbLog.Text += $"=> Tạo Health Monitor thành công! ID: {hmId}\r\n";
                }
                else
                {
                    rtbLog.Text += "Lỗi: Response không chứa healthmonitor\r\n";
                }
            }
            catch (Exception ex)
            {
                rtbLog.Text += "Lỗi tạo Health Monitor: " + ex.Message + "\r\n";
            }
        }

        private async void btnGetLoadBalancers_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }

            rtbLog.Text = "Đang lấy danh sách LoadBalancer...\r\n";
            string result = await osClient.GetLoadBalancersAsync();

            try
            {
                JObject json = JObject.Parse(result);
                var loadbalancers = json["loadbalancers"];
                rtbLog.Text = "[DANH SÁCH LOADBALANCERS]\r\n";
                int i = 1;
                foreach (var lb in loadbalancers)
                {
                    rtbLog.Text += $"{i}. Tên: {lb["name"]} | ID: {lb["id"]} | Status: {lb["provisioning_status"]}\r\n";
                    i++;
                }
            }
            catch { rtbLog.Text += result + "\r\n"; }
        }

        private async void btnGetPools_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }

            rtbLog.Text = "Đang lấy danh sách Pool...\r\n";
            string result = await osClient.GetPoolsAsync();

            try
            {
                JObject json = JObject.Parse(result);
                var pools = json["pools"];
                rtbLog.Text = "[DANH SÁCH POOLS]\r\n";
                int i = 1;
                foreach (var p in pools)
                {
                    rtbLog.Text += $"{i}. Tên: {p["name"]} | ID: {p["id"]} | Status: {p["status"]}\r\n";
                    i++;
                }
                await RefreshPoolListAsync();
            }
            catch { rtbLog.Text += result + "\r\n"; }
        }

        private async void btnGetLBDetails_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var lbId = SelectedId(txtLBId);
            if (string.IsNullOrEmpty(lbId)) { MessageBox.Show("Vui lòng chọn ID LoadBalancer!"); return; }

            rtbLog.Text = "Đang lấy chi tiết LoadBalancer...\r\n";
            string result = await osClient.GetLoadBalancerDetailsAsync(lbId);

            try
            {
                JObject json = JObject.Parse(result);
                var lb = json["loadbalancer"];
                if (lb == null)
                {
                    rtbLog.Text = "Không lấy được chi tiết LoadBalancer.\r\n";
                    rtbLog.Text += "Response:\r\n" + result + "\r\n";
                    return;
                }
                rtbLog.Text = "[CHI TIẾT LOADBALANCER]\r\n";
                rtbLog.Text += $"Tên: {lb["name"]}\r\n";
                rtbLog.Text += $"ID: {lb["id"]}\r\n";
                rtbLog.Text += $"Status: {lb["provisioning_status"]}\r\n";
                if (lb["vip_address"] != null)
                {
                    rtbLog.Text += $"VIP Address: {lb["vip_address"]}\r\n";
                }
                rtbLog.Text += "--------------------------------------\r\n";
            }
            catch { rtbLog.Text += result + "\r\n"; }
        }

        private async void btnDeleteLB_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var lbId = SelectedId(txtLBId);
            if (string.IsNullOrEmpty(lbId)) { MessageBox.Show("Vui lòng chọn ID LoadBalancer cần xóa!"); return; }

            rtbLog.Text = $"Đang xóa LoadBalancer (ID: {lbId})...\r\n";
            string result = await osClient.DeleteLoadBalancerAsync(lbId);
            rtbLog.Text += $"Kết quả: {result}\r\n";
            await RefreshLoadBalancerListsAsync();
        }

        private async void txtInstanceId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) return;
            await LoadPortsForSelectedInstanceAsync();
        }

        private async void txtTargetId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (osClient == null || string.IsNullOrEmpty(osClient.AuthToken))
            {
                return;
            }
        }

        private async void btnDeleteListener_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var listenerId = SelectedId(txtListenerId);
            if (string.IsNullOrEmpty(listenerId)) { MessageBox.Show("Vui lòng chọn ID Listener cần xóa!"); return; }

            rtbLog.Text = $"Đang xóa Listener (ID: {listenerId})...\r\n";
            string result = await osClient.DeleteListenerAsync(listenerId);
            rtbLog.Text += $"Kết quả: {result}\r\n";
            await RefreshListenerListAsync();
        }

        private async void btnDeletePool_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var poolId = SelectedId(txtLbPoolId);
            if (string.IsNullOrEmpty(poolId)) { MessageBox.Show("Vui lòng chọn ID Pool cần xóa!"); return; }

            rtbLog.Text = $"Đang xóa Pool (ID: {poolId})...\r\n";
            string result = await osClient.DeletePoolAsync(poolId);
            rtbLog.Text += $"Kết quả: {result}\r\n";
            await RefreshPoolListAsync();
        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private async void btnAssignFloatingIpLB_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            var lbId = SelectedId(txtLBId);
            var extNetworkId = SelectedId(txtExternalNetworkId);
            if (string.IsNullOrEmpty(lbId)) { MessageBox.Show("Vui lòng chọn ID LoadBalancer!"); return; }
            if (string.IsNullOrEmpty(extNetworkId)) { MessageBox.Show("Vui lòng chọn External Network để cấp Floating IP!"); return; }

            rtbLog.Text = "Đang gắn Floating IP cho LoadBalancer...\r\n";

            try
            {
                rtbLog.Text += $"1. External Network ID: {extNetworkId}\r\n";
                rtbLog.Text += "2. Gắn Floating IP cho LoadBalancer...\r\n";
                string result = await osClient.AssignFloatingIpToLoadBalancerAsync(lbId, extNetworkId);
                rtbLog.Text += "Response: " + result + "\r\n";

                JObject json = JObject.Parse(result);
                if (json["floatingip"] != null)
                {
                    string floatingIp = json["floatingip"]["floating_ip_address"].ToString();
                    rtbLog.Text += $"=> Gắn Floating IP thành công!\r\n";
                    rtbLog.Text += $"   - Floating IP: {floatingIp}\r\n";
                    rtbLog.Text += $"=> Bây giờ bạn có thể truy cập LoadBalancer qua IP này!\r\n";
                }
                else
                {
                    rtbLog.Text += "Lỗi: Response không chứa floatingip\r\n";
                }
            }
            catch (Exception ex)
            {
                rtbLog.Text += "Lỗi gắn Floating IP: " + ex.Message + "\r\n";
            }
        }

        private void txtidInterface_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void txtLBId_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

