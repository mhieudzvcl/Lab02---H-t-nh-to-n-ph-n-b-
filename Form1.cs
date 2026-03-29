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
        public Form1()
        {
            InitializeComponent();
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
            string netResult = await osClient.CreateNetworkAsync("nhom04_net");

            try
            {
                // Trích xuất cái ID của Network từ chuỗi JSON trả về
                JObject netJson = JObject.Parse(netResult);
                string networkId = netJson["network"]["id"].ToString();
                rtbLog.Text += $"=> Tạo Net thành công! ID: {networkId}\r\n\r\n";

                // Dùng ID đó để tạo Subnet
                rtbLog.Text += "2. Đang tạo Subnet cho Network này...\r\n";
                // CIDR là dải IP - 192.168.100.0/24
                string subResult = await osClient.CreateSubnetAsync(networkId, "nhom04_subnet", "192.168.100.0/24");

                rtbLog.Text += "=> Tạo Subnet thành công!\r\n";
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
            string extNetworkId = "c3455e8f-ea16-4f5d-ad5e-5c4292015a0d"; //192.168.120.0/23

            string result = await osClient.CreateRouterAsync("nhom04_router", extNetworkId);
            rtbLog.Text += result + "\r\n";
        }

        private async void btnAttachRouter_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }

            string routerId = txtTargetId.Text.Trim();
            string subnetId = txtidInterface.Text.Trim(); //ae đừng xóa nhầm cái subnet của nhóm nhé nhom4_net (192.168.1.0/24)

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

            string result = await osClient.CreateInstanceWithWebAsync(
                txtVmName.Text,
                txtImageId.Text,
                txtFlavorId.Text,
                txtNetId.Text,
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

            string newVmIp = txtTargetId.Text.Trim(); // TextBox chứa IP máy ảo mới
            string subnetId = txtNetId.Text.Trim(); // TextBox chứa ID Subnet

            if (string.IsNullOrEmpty(newVmIp) || string.IsNullOrEmpty(subnetId))
            {
                MessageBox.Show("Vui lòng nhập IP của máy ảo mới vào ô Target ID và ID Subnet vào ô Net ID!", "Thiếu thông tin");
                return;
            }

            rtbLog.Text += $"Đang đưa VM ({newVmIp}) vào Load Balancer...\r\n";

            string result = await osClient.AddMemberToLoadBalancerPoolAsync(txtPoolId.Text, newVmIp, subnetId);

            rtbLog.Text += "Kết quả Tăng máy ảo (Scale Up):\r\n" + result + "\r\n";
        }

        private async void btnScaleDown_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken))
            {
                MessageBox.Show("Vui lòng đăng nhập trước!"); return;
            }

            // Yêu cầu nhập Member ID của máy ảo trong Load Balancer (lấy từ API GET pool members)
            string memberIdToRemove = "ID_CUA_MEMBER_CAN_XOA";

            rtbLog.Text += "Đang rút máy ảo khỏi Load Balancer...\r\n";

            string result = await osClient.RemoveMemberFromPoolAsync(txtPoolId.Text, memberIdToRemove);

            rtbLog.Text += "Kết quả Giảm máy ảo (Scale Down):\r\n" + result + "\r\n";
            rtbLog.Text += "=> (Tùy chọn) Bạn có thể gọi thêm hàm DeleteInstanceAsync() để xóa hẳn máy ảo này cho đỡ tốn tài nguyên.\r\n";
        }

        private async void btnDeleteVM_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            if (string.IsNullOrEmpty(txtTargetId.Text)) { MessageBox.Show("Vui lòng nhập ID của Máy ảo cần xóa vào ô Target ID"); return; }

            rtbLog.Text += $"Đang gửi lệnh xóa Máy ảo (ID: {txtTargetId.Text})...\r\n";
            string result = await osClient.DeleteInstanceAsync(txtTargetId.Text);
            rtbLog.Text += $"Kết quả: {result}\r\n";
        }

        private async void btnDeleteRouter_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            if (string.IsNullOrEmpty(txtTargetId.Text)) { MessageBox.Show("Vui lòng nhập ID của Router cần xóa vào ô Target ID"); return; }

            rtbLog.Text += $"Đang gửi lệnh xóa Router...\r\n";
            string result = await osClient.DeleteRouterAsync(txtTargetId.Text);
            rtbLog.Text += $"Kết quả: {result}\r\n";
        }

        private async void btnDeleteNet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            if (string.IsNullOrEmpty(txtTargetId.Text)) { MessageBox.Show("Vui lòng nhập ID của Network cần xóa vào ô Target ID"); return; }

            rtbLog.Text += $"Đang gửi lệnh xóa Network...\r\n";
            string result = await osClient.DeleteNetworkAsync(txtTargetId.Text);
            rtbLog.Text += $"Kết quả: {result}\r\n";
        }

        private async void btnAssignFip_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            if (string.IsNullOrEmpty(txtPortId.Text)) { MessageBox.Show("Vui lòng nhập Port ID của máy ảo!"); return; }

            rtbLog.Text += "Đang xin cấp Floating IP và gắn vào máy ảo...\r\n";

            //public_net
            string extNetworkId = "c3455e8f-ea16-4f5d-ad5e-5c4292015a0d";

            try
            {
                string result = await osClient.CreateAndAssignFloatingIpAsync(extNetworkId, txtPortId.Text);

                JObject fipJson = JObject.Parse(result);
                string floatingIpAddress = fipJson["floatingip"]["floating_ip_address"].ToString();

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
            if (string.IsNullOrEmpty(txtTargetId.Text)) { MessageBox.Show("Vui lòng nhập ID của Instance vào ô Target ID!"); return; }

            rtbLog.Text = "Đang lấy Port Interface của Instance...\r\n";
            string result = await osClient.GetPortInterfaceAsync(txtTargetId.Text);

            try
            {
                JObject json = JObject.Parse(result);
                var interfaces = json["interfaceAttachments"];
                rtbLog.Text = "[PORT INTERFACE CỦA INSTANCE]\r\n";
                int i = 1;
                foreach (var iface in interfaces)
                {
                    rtbLog.Text += $"{i}. Port ID: {iface["port_id"]}\r\n";
                    rtbLog.Text += $"   Mac Address: {iface["mac_addr"]}\r\n";
                    if (iface["fixed_ips"].HasValues)
                    {
                        rtbLog.Text += $"   Fixed IPs:\r\n";
                        foreach (var ip in iface["fixed_ips"])
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
            if (string.IsNullOrEmpty(txtLBSubnetId.Text)) { MessageBox.Show("Vui lòng nhập Subnet ID!"); return; }

            rtbLog.Text = "Đang tạo LoadBalancer...\r\n";

            try
            {
                string result = await osClient.CreateLoadBalancerAsync(txtLBName.Text, txtLBSubnetId.Text);
                rtbLog.Text += "Response: " + result + "\r\n";

                JObject json = JObject.Parse(result);
                if (json["loadbalancer"] != null)
                {
                    string lbId = json["loadbalancer"]["id"].ToString();
                    rtbLog.Text += $"=> Tạo LoadBalancer thành công! ID: {lbId}\r\n";
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
            if (string.IsNullOrEmpty(txtLBId.Text)) { MessageBox.Show("Vui lòng nhập ID LoadBalancer!"); return; }
            if (string.IsNullOrEmpty(txtListenerName.Text)) { MessageBox.Show("Vui lòng nhập tên Listener!"); return; }
            if (string.IsNullOrEmpty(txtListenerProtocol.Text)) { MessageBox.Show("Vui lòng nhập Protocol!"); return; }
            if (string.IsNullOrEmpty(txtListenerPort.Text) || !int.TryParse(txtListenerPort.Text, out int port)) { MessageBox.Show("Vui lòng nhập Port hợp lệ!"); return; }

            rtbLog.Text = "Đang tạo Listener...\r\n";

            try
            {
                string result = await osClient.CreateListenerAsync(txtLBId.Text, txtListenerName.Text, txtListenerProtocol.Text, port);
                rtbLog.Text += "Response: " + result + "\r\n";

                JObject json = JObject.Parse(result);
                if (json["listener"] != null)
                {
                    string listenerId = json["listener"]["id"].ToString();
                    rtbLog.Text += $"=> Tạo Listener thành công! ID: {listenerId}\r\n";
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
            if (string.IsNullOrEmpty(txtListenerId.Text)) { MessageBox.Show("Vui lòng nhập ID Listener!"); return; }
            if (string.IsNullOrEmpty(txtPoolName.Text)) { MessageBox.Show("Vui lòng nhập tên Pool!"); return; }
            if (string.IsNullOrEmpty(txtPoolProtocol.Text)) { MessageBox.Show("Vui lòng nhập Protocol!"); return; }

            rtbLog.Text = "Đang tạo Pool...\r\n";

            try
            {
                string result = await osClient.CreatePoolAsync(txtPoolName.Text, txtListenerId.Text, txtPoolProtocol.Text);
                rtbLog.Text += "Response: " + result + "\r\n";

                JObject json = JObject.Parse(result);
                if (json["pool"] != null)
                {
                    string poolId = json["pool"]["id"].ToString();
                    rtbLog.Text += $"=> Tạo Pool thành công! ID: {poolId}\r\n";
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
            if (string.IsNullOrEmpty(txtPoolId.Text)) { MessageBox.Show("Vui lòng nhập ID Pool!"); return; }

            rtbLog.Text = "Đang tạo Health Monitor...\r\n";

            try
            {
                string hcType = string.IsNullOrEmpty(txtHealthCheckType.Text) ? "HTTP" : txtHealthCheckType.Text;
                int delay = string.IsNullOrEmpty(txtHealthCheckDelay.Text) ? 5 : int.Parse(txtHealthCheckDelay.Text);
                int timeout = string.IsNullOrEmpty(txtHealthCheckTimeout.Text) ? 5 : int.Parse(txtHealthCheckTimeout.Text);
                int maxRetries = string.IsNullOrEmpty(txtHealthCheckRetries.Text) ? 3 : int.Parse(txtHealthCheckRetries.Text);

                string result = await osClient.CreateHealthMonitorAsync(txtPoolId.Text, hcType, delay, timeout, maxRetries);
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

        private async void btnGetLBDetails_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            if (string.IsNullOrEmpty(txtLBId.Text)) { MessageBox.Show("Vui lòng nhập ID LoadBalancer!"); return; }

            rtbLog.Text = "Đang lấy chi tiết LoadBalancer...\r\n";
            string result = await osClient.GetLoadBalancerDetailsAsync(txtLBId.Text);

            try
            {
                JObject json = JObject.Parse(result);
                var lb = json["loadbalancer"];
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
            if (string.IsNullOrEmpty(txtLBId.Text)) { MessageBox.Show("Vui lòng nhập ID LoadBalancer cần xóa!"); return; }

            rtbLog.Text = $"Đang xóa LoadBalancer (ID: {txtLBId.Text})...\r\n";
            string result = await osClient.DeleteLoadBalancerAsync(txtLBId.Text);
            rtbLog.Text += $"Kết quả: {result}\r\n";
        }

        private async void btnDeleteListener_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            if (string.IsNullOrEmpty(txtListenerId.Text)) { MessageBox.Show("Vui lòng nhập ID Listener cần xóa!"); return; }

            rtbLog.Text = $"Đang xóa Listener (ID: {txtListenerId.Text})...\r\n";
            string result = await osClient.DeleteListenerAsync(txtListenerId.Text);
            rtbLog.Text += $"Kết quả: {result}\r\n";
        }

        private async void btnDeletePool_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            if (string.IsNullOrEmpty(txtPoolId.Text)) { MessageBox.Show("Vui lòng nhập ID Pool cần xóa!"); return; }

            rtbLog.Text = $"Đang xóa Pool (ID: {txtPoolId.Text})...\r\n";
            string result = await osClient.DeletePoolAsync(txtPoolId.Text);
            rtbLog.Text += $"Kết quả: {result}\r\n";
        }

        private async void btnAssignFloatingIpLB_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(osClient.AuthToken)) { MessageBox.Show("Vui lòng đăng nhập trước!"); return; }
            if (string.IsNullOrEmpty(txtLBId.Text)) { MessageBox.Show("Vui lòng nhập ID LoadBalancer!"); return; }
            if (string.IsNullOrEmpty(txtLBSubnetId.Text)) { MessageBox.Show("Vui lòng nhập Subnet ID của LoadBalancer!"); return; }

            rtbLog.Text = "Đang gắn Floating IP cho LoadBalancer...\r\n";

            try
            {
                // Lấy Network ID từ Subnet ID
                rtbLog.Text += "1. Lấy Network ID từ Subnet...\r\n";
                string networkId = await osClient.GetNetworkIdFromSubnetAsync(txtLBSubnetId.Text);
                rtbLog.Text += $"   => Network ID: {networkId}\r\n\r\n";

                // Gắn Floating IP vào LoadBalancer
                rtbLog.Text += "2. Gắn Floating IP cho LoadBalancer...\r\n";
                string result = await osClient.AssignFloatingIpToLoadBalancerAsync(txtLBId.Text, networkId);
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


    }
}

