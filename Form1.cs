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
                // CIDR là dải IP, ví dụ 192.168.100.0/24
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
            string subnetId = "aa9667d2-ca06-4e8b-b9b2-31d78ae05a03"; //ae đừng xóa nhầm cái subnet của nhóm nhé nhom4_net (192.168.1.0/24)

            if (string.IsNullOrEmpty(routerId))
            {
                MessageBox.Show("Vui lòng nhập ID của Router vào ô Target ID trước khi cắm dây!", "Thiếu thông tin");
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

            string result = await osClient.CreateInstanceWithWebAsync(
                txtVmName.Text,
                txtImageId.Text,
                txtFlavorId.Text,
                txtNetId.Text
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

            // Giả sử bạn vừa tạo xong máy ảo số 2 (nhomXX_vm_2) và lấy được IP nội bộ của nó
            // (Trong bài thực hành, bạn có thể nhập tay IP này vào một ô TextBox cho nhanh)
            string newVmIp = "192.168.100.102"; // Thay bằng TextBox chứa IP máy ảo mới
            string subnetId = "ID_SUBNET_CUA_BAN"; // Thay bằng TextBox chứa ID Subnet

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
    }
}
