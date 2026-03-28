namespace OpenStackManager
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtProjectName = new System.Windows.Forms.TextBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.btnCreateNet = new System.Windows.Forms.Button();
            this.btnGetImages = new System.Windows.Forms.Button();
            this.btnGetFlavors = new System.Windows.Forms.Button();
            this.btnCreateRouter = new System.Windows.Forms.Button();
            this.btnAttachRouter = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.txtVmName = new System.Windows.Forms.TextBox();
            this.txtImageId = new System.Windows.Forms.TextBox();
            this.txtPoolId = new System.Windows.Forms.TextBox();
            this.txtNetId = new System.Windows.Forms.TextBox();
            this.txtFlavorId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnCreateVM = new System.Windows.Forms.Button();
            this.btnScaleUp = new System.Windows.Forms.Button();
            this.btnScaleDown = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.txtPortId = new System.Windows.Forms.TextBox();
            this.txtTargetId = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnDeleteVM = new System.Windows.Forms.Button();
            this.btnAssignFip = new System.Windows.Forms.Button();
            this.btnDeleteRouter = new System.Windows.Forms.Button();
            this.btnDeleteNet = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.btnGetRouters = new System.Windows.Forms.Button();
            this.btnGetInstances = new System.Windows.Forms.Button();
            this.btnGetNetworks = new System.Windows.Forms.Button();
            this.btnGetSubnets = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Palatino Linotype", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label1.Location = new System.Drawing.Point(7, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 33);
            this.label1.TabIndex = 0;
            this.label1.Text = "Indentity URL:";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Palatino Linotype", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label2.Location = new System.Drawing.Point(7, 74);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 33);
            this.label2.TabIndex = 1;
            this.label2.Text = "Username:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Palatino Linotype", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label3.Location = new System.Drawing.Point(7, 125);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(129, 33);
            this.label3.TabIndex = 2;
            this.label3.Text = "Password:";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Palatino Linotype", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label4.Location = new System.Drawing.Point(7, 176);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(172, 28);
            this.label4.TabIndex = 3;
            this.label4.Text = "Project Name:";
            // 
            // btnLogin
            // 
            this.btnLogin.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnLogin.Location = new System.Drawing.Point(950, 20);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(192, 51);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtUrl
            // 
            this.txtUrl.Font = new System.Drawing.Font("Palatino Linotype", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUrl.Location = new System.Drawing.Point(197, 22);
            this.txtUrl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(742, 30);
            this.txtUrl.TabIndex = 5;
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Palatino Linotype", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.Location = new System.Drawing.Point(197, 74);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(742, 30);
            this.txtUsername.TabIndex = 6;
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Palatino Linotype", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(197, 128);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(742, 30);
            this.txtPassword.TabIndex = 7;
            // 
            // txtProjectName
            // 
            this.txtProjectName.Font = new System.Drawing.Font("Palatino Linotype", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProjectName.Location = new System.Drawing.Point(197, 174);
            this.txtProjectName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Size = new System.Drawing.Size(742, 30);
            this.txtProjectName.TabIndex = 8;
            // 
            // rtbLog
            // 
            this.rtbLog.Location = new System.Drawing.Point(12, 321);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(1517, 324);
            this.rtbLog.TabIndex = 9;
            this.rtbLog.Text = "";
            // 
            // btnCreateNet
            // 
            this.btnCreateNet.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnCreateNet.Location = new System.Drawing.Point(498, 53);
            this.btnCreateNet.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateNet.Name = "btnCreateNet";
            this.btnCreateNet.Size = new System.Drawing.Size(192, 51);
            this.btnCreateNet.TabIndex = 10;
            this.btnCreateNet.Text = "Create Network";
            this.btnCreateNet.UseVisualStyleBackColor = true;
            this.btnCreateNet.Click += new System.EventHandler(this.btnCreateNet_Click);
            // 
            // btnGetImages
            // 
            this.btnGetImages.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnGetImages.Location = new System.Drawing.Point(950, 127);
            this.btnGetImages.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetImages.Name = "btnGetImages";
            this.btnGetImages.Size = new System.Drawing.Size(192, 51);
            this.btnGetImages.TabIndex = 11;
            this.btnGetImages.Text = "Get Images";
            this.btnGetImages.UseVisualStyleBackColor = true;
            this.btnGetImages.Click += new System.EventHandler(this.btnGetImages_Click);
            // 
            // btnGetFlavors
            // 
            this.btnGetFlavors.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnGetFlavors.Location = new System.Drawing.Point(950, 74);
            this.btnGetFlavors.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetFlavors.Name = "btnGetFlavors";
            this.btnGetFlavors.Size = new System.Drawing.Size(192, 51);
            this.btnGetFlavors.TabIndex = 12;
            this.btnGetFlavors.Text = "Get Flavors";
            this.btnGetFlavors.UseVisualStyleBackColor = true;
            this.btnGetFlavors.Click += new System.EventHandler(this.btnGetFlavors_Click);
            // 
            // btnCreateRouter
            // 
            this.btnCreateRouter.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnCreateRouter.Location = new System.Drawing.Point(1298, 0);
            this.btnCreateRouter.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateRouter.Name = "btnCreateRouter";
            this.btnCreateRouter.Size = new System.Drawing.Size(192, 51);
            this.btnCreateRouter.TabIndex = 13;
            this.btnCreateRouter.Text = "Create Router";
            this.btnCreateRouter.UseVisualStyleBackColor = true;
            this.btnCreateRouter.Click += new System.EventHandler(this.btnCreateRouter_Click);
            // 
            // btnAttachRouter
            // 
            this.btnAttachRouter.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnAttachRouter.Location = new System.Drawing.Point(698, 54);
            this.btnAttachRouter.Margin = new System.Windows.Forms.Padding(4);
            this.btnAttachRouter.Name = "btnAttachRouter";
            this.btnAttachRouter.Size = new System.Drawing.Size(192, 51);
            this.btnAttachRouter.TabIndex = 14;
            this.btnAttachRouter.Text = "Attach Router";
            this.btnAttachRouter.UseVisualStyleBackColor = true;
            this.btnAttachRouter.Click += new System.EventHandler(this.btnAttachRouter_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(126, 66);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(8, 8);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 32);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(0, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 32);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(0, 0);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Location = new System.Drawing.Point(12, 12);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1521, 307);
            this.tabControl2.TabIndex = 16;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label1);
            this.tabPage4.Controls.Add(this.label2);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.label4);
            this.tabPage4.Controls.Add(this.btnGetFlavors);
            this.tabPage4.Controls.Add(this.txtUrl);
            this.tabPage4.Controls.Add(this.btnGetImages);
            this.tabPage4.Controls.Add(this.txtUsername);
            this.tabPage4.Controls.Add(this.txtPassword);
            this.tabPage4.Controls.Add(this.txtProjectName);
            this.tabPage4.Controls.Add(this.btnLogin);
            this.tabPage4.Location = new System.Drawing.Point(4, 32);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(1513, 271);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Tab 1 - Xác Thực";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.btnGetSubnets);
            this.tabPage5.Controls.Add(this.btnGetNetworks);
            this.tabPage5.Controls.Add(this.btnGetInstances);
            this.tabPage5.Controls.Add(this.btnGetRouters);
            this.tabPage5.Controls.Add(this.label12);
            this.tabPage5.Controls.Add(this.btnDeleteNet);
            this.tabPage5.Controls.Add(this.btnDeleteRouter);
            this.tabPage5.Controls.Add(this.btnAssignFip);
            this.tabPage5.Controls.Add(this.btnDeleteVM);
            this.tabPage5.Controls.Add(this.btnAttachRouter);
            this.tabPage5.Controls.Add(this.label11);
            this.tabPage5.Controls.Add(this.btnCreateNet);
            this.tabPage5.Controls.Add(this.btnCreateRouter);
            this.tabPage5.Controls.Add(this.label10);
            this.tabPage5.Controls.Add(this.txtTargetId);
            this.tabPage5.Controls.Add(this.txtPortId);
            this.tabPage5.Location = new System.Drawing.Point(4, 32);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(1513, 271);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Tab 2 - Mạng";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnScaleDown);
            this.tabPage3.Controls.Add(this.btnScaleUp);
            this.tabPage3.Controls.Add(this.btnCreateVM);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.txtFlavorId);
            this.tabPage3.Controls.Add(this.txtNetId);
            this.tabPage3.Controls.Add(this.txtPoolId);
            this.tabPage3.Controls.Add(this.txtImageId);
            this.tabPage3.Controls.Add(this.txtVmName);
            this.tabPage3.Location = new System.Drawing.Point(4, 32);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1513, 271);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Tab 3 - Máy ảo & LB";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // txtVmName
            // 
            this.txtVmName.Location = new System.Drawing.Point(117, 20);
            this.txtVmName.Name = "txtVmName";
            this.txtVmName.Size = new System.Drawing.Size(382, 30);
            this.txtVmName.TabIndex = 0;
            // 
            // txtImageId
            // 
            this.txtImageId.Location = new System.Drawing.Point(117, 69);
            this.txtImageId.Name = "txtImageId";
            this.txtImageId.Size = new System.Drawing.Size(382, 30);
            this.txtImageId.TabIndex = 1;
            // 
            // txtPoolId
            // 
            this.txtPoolId.Location = new System.Drawing.Point(117, 216);
            this.txtPoolId.Name = "txtPoolId";
            this.txtPoolId.Size = new System.Drawing.Size(382, 30);
            this.txtPoolId.TabIndex = 2;
            // 
            // txtNetId
            // 
            this.txtNetId.Location = new System.Drawing.Point(117, 170);
            this.txtNetId.Name = "txtNetId";
            this.txtNetId.Size = new System.Drawing.Size(382, 30);
            this.txtNetId.TabIndex = 3;
            // 
            // txtFlavorId
            // 
            this.txtFlavorId.Location = new System.Drawing.Point(117, 120);
            this.txtFlavorId.Name = "txtFlavorId";
            this.txtFlavorId.Size = new System.Drawing.Size(382, 30);
            this.txtFlavorId.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 23);
            this.label5.TabIndex = 5;
            this.label5.Text = "VM Name:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 23);
            this.label6.TabIndex = 6;
            this.label6.Text = "Image ID:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 123);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 23);
            this.label7.TabIndex = 7;
            this.label7.Text = "Flavor ID:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 173);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(64, 23);
            this.label8.TabIndex = 8;
            this.label8.Text = "Net ID:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 219);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 23);
            this.label9.TabIndex = 9;
            this.label9.Text = "Pool ID:";
            // 
            // btnCreateVM
            // 
            this.btnCreateVM.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnCreateVM.Location = new System.Drawing.Point(506, 67);
            this.btnCreateVM.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateVM.Name = "btnCreateVM";
            this.btnCreateVM.Size = new System.Drawing.Size(192, 51);
            this.btnCreateVM.TabIndex = 10;
            this.btnCreateVM.Text = "Create VM";
            this.btnCreateVM.UseVisualStyleBackColor = true;
            this.btnCreateVM.Click += new System.EventHandler(this.btnCreateVM_Click);
            // 
            // btnScaleUp
            // 
            this.btnScaleUp.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnScaleUp.Location = new System.Drawing.Point(506, 8);
            this.btnScaleUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnScaleUp.Name = "btnScaleUp";
            this.btnScaleUp.Size = new System.Drawing.Size(192, 51);
            this.btnScaleUp.TabIndex = 11;
            this.btnScaleUp.Text = "Scale Up VM";
            this.btnScaleUp.UseVisualStyleBackColor = true;
            this.btnScaleUp.Click += new System.EventHandler(this.btnScaleUp_Click);
            // 
            // btnScaleDown
            // 
            this.btnScaleDown.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnScaleDown.Location = new System.Drawing.Point(706, 8);
            this.btnScaleDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnScaleDown.Name = "btnScaleDown";
            this.btnScaleDown.Size = new System.Drawing.Size(192, 51);
            this.btnScaleDown.TabIndex = 12;
            this.btnScaleDown.Text = "Scale Down VM";
            this.btnScaleDown.UseVisualStyleBackColor = true;
            this.btnScaleDown.Click += new System.EventHandler(this.btnScaleDown_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // txtPortId
            // 
            this.txtPortId.Font = new System.Drawing.Font("Palatino Linotype", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPortId.Location = new System.Drawing.Point(132, 65);
            this.txtPortId.Margin = new System.Windows.Forms.Padding(4);
            this.txtPortId.Name = "txtPortId";
            this.txtPortId.Size = new System.Drawing.Size(356, 30);
            this.txtPortId.TabIndex = 6;
            // 
            // txtTargetId
            // 
            this.txtTargetId.Font = new System.Drawing.Font("Palatino Linotype", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTargetId.Location = new System.Drawing.Point(132, 18);
            this.txtTargetId.Margin = new System.Windows.Forms.Padding(4);
            this.txtTargetId.Name = "txtTargetId";
            this.txtTargetId.Size = new System.Drawing.Size(356, 30);
            this.txtTargetId.TabIndex = 7;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Palatino Linotype", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label10.Location = new System.Drawing.Point(7, 20);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(99, 37);
            this.label10.TabIndex = 8;
            this.label10.Text = "Target ID:";
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Palatino Linotype", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label11.Location = new System.Drawing.Point(7, 67);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(130, 37);
            this.label11.TabIndex = 9;
            this.label11.Text = "Port ID:";
            // 
            // btnDeleteVM
            // 
            this.btnDeleteVM.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnDeleteVM.Location = new System.Drawing.Point(498, -3);
            this.btnDeleteVM.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteVM.Name = "btnDeleteVM";
            this.btnDeleteVM.Size = new System.Drawing.Size(192, 51);
            this.btnDeleteVM.TabIndex = 15;
            this.btnDeleteVM.Text = "Delete VM";
            this.btnDeleteVM.UseVisualStyleBackColor = true;
            this.btnDeleteVM.Click += new System.EventHandler(this.btnDeleteVM_Click);
            // 
            // btnAssignFip
            // 
            this.btnAssignFip.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnAssignFip.Location = new System.Drawing.Point(1098, 0);
            this.btnAssignFip.Margin = new System.Windows.Forms.Padding(4);
            this.btnAssignFip.Name = "btnAssignFip";
            this.btnAssignFip.Size = new System.Drawing.Size(192, 51);
            this.btnAssignFip.TabIndex = 16;
            this.btnAssignFip.Text = "Assign Floating IP";
            this.btnAssignFip.UseVisualStyleBackColor = true;
            this.btnAssignFip.Click += new System.EventHandler(this.btnAssignFip_Click);
            // 
            // btnDeleteRouter
            // 
            this.btnDeleteRouter.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnDeleteRouter.Location = new System.Drawing.Point(698, 0);
            this.btnDeleteRouter.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteRouter.Name = "btnDeleteRouter";
            this.btnDeleteRouter.Size = new System.Drawing.Size(192, 51);
            this.btnDeleteRouter.TabIndex = 17;
            this.btnDeleteRouter.Text = "Delete Router";
            this.btnDeleteRouter.UseVisualStyleBackColor = true;
            this.btnDeleteRouter.Click += new System.EventHandler(this.btnDeleteRouter_Click);
            // 
            // btnDeleteNet
            // 
            this.btnDeleteNet.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnDeleteNet.Location = new System.Drawing.Point(898, 0);
            this.btnDeleteNet.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteNet.Name = "btnDeleteNet";
            this.btnDeleteNet.Size = new System.Drawing.Size(192, 51);
            this.btnDeleteNet.TabIndex = 18;
            this.btnDeleteNet.Text = "Delete Net";
            this.btnDeleteNet.UseVisualStyleBackColor = true;
            this.btnDeleteNet.Click += new System.EventHandler(this.btnDeleteNet_Click);
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Palatino Linotype", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label12.Location = new System.Drawing.Point(494, 112);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(275, 133);
            this.label12.TabIndex = 19;
            this.label12.Text = "Lưu ý: Xóa Máy ảo trước sau đó mở giao diện web ngắt kết nối giữa Router và Subne" +
    "t. Tiếp theo xóa Router và sau đó xóa Network\r\n\r\n\r\n";
            this.label12.Click += new System.EventHandler(this.label12_Click);
            // 
            // btnGetRouters
            // 
            this.btnGetRouters.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnGetRouters.Location = new System.Drawing.Point(898, 53);
            this.btnGetRouters.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetRouters.Name = "btnGetRouters";
            this.btnGetRouters.Size = new System.Drawing.Size(192, 51);
            this.btnGetRouters.TabIndex = 20;
            this.btnGetRouters.Text = "Get Routers";
            this.btnGetRouters.UseVisualStyleBackColor = true;
            this.btnGetRouters.Click += new System.EventHandler(this.btnGetRouters_Click);
            // 
            // btnGetInstances
            // 
            this.btnGetInstances.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnGetInstances.Location = new System.Drawing.Point(1298, 53);
            this.btnGetInstances.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetInstances.Name = "btnGetInstances";
            this.btnGetInstances.Size = new System.Drawing.Size(192, 51);
            this.btnGetInstances.TabIndex = 21;
            this.btnGetInstances.Text = "Get Intances";
            this.btnGetInstances.UseVisualStyleBackColor = true;
            this.btnGetInstances.Click += new System.EventHandler(this.btnGetInstances_Click);
            // 
            // btnGetNetworks
            // 
            this.btnGetNetworks.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnGetNetworks.Location = new System.Drawing.Point(898, 112);
            this.btnGetNetworks.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetNetworks.Name = "btnGetNetworks";
            this.btnGetNetworks.Size = new System.Drawing.Size(192, 51);
            this.btnGetNetworks.TabIndex = 22;
            this.btnGetNetworks.Text = "Get Networks";
            this.btnGetNetworks.UseVisualStyleBackColor = true;
            this.btnGetNetworks.Click += new System.EventHandler(this.btnGetNetworks_Click);
            // 
            // btnGetSubnets
            // 
            this.btnGetSubnets.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.btnGetSubnets.Location = new System.Drawing.Point(1098, 54);
            this.btnGetSubnets.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetSubnets.Name = "btnGetSubnets";
            this.btnGetSubnets.Size = new System.Drawing.Size(192, 51);
            this.btnGetSubnets.TabIndex = 23;
            this.btnGetSubnets.Text = "Get Subnets";
            this.btnGetSubnets.UseVisualStyleBackColor = true;
            this.btnGetSubnets.Click += new System.EventHandler(this.btnGetSubnets_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1545, 642);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.rtbLog);
            this.Font = new System.Drawing.Font("Palatino Linotype", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button btnCreateNet;
        private System.Windows.Forms.Button btnGetImages;
        private System.Windows.Forms.Button btnGetFlavors;
        private System.Windows.Forms.Button btnCreateRouter;
        private System.Windows.Forms.Button btnAttachRouter;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtFlavorId;
        private System.Windows.Forms.TextBox txtNetId;
        private System.Windows.Forms.TextBox txtPoolId;
        private System.Windows.Forms.TextBox txtImageId;
        private System.Windows.Forms.TextBox txtVmName;
        private System.Windows.Forms.Button btnScaleDown;
        private System.Windows.Forms.Button btnScaleUp;
        private System.Windows.Forms.Button btnCreateVM;
        private System.Windows.Forms.Button btnDeleteNet;
        private System.Windows.Forms.Button btnDeleteRouter;
        private System.Windows.Forms.Button btnAssignFip;
        private System.Windows.Forms.Button btnDeleteVM;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtTargetId;
        private System.Windows.Forms.TextBox txtPortId;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnGetInstances;
        private System.Windows.Forms.Button btnGetRouters;
        private System.Windows.Forms.Button btnGetSubnets;
        private System.Windows.Forms.Button btnGetNetworks;
    }
}

