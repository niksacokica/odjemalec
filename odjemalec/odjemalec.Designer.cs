namespace odjemalec{
    partial class odjemalec{
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose( bool disposing ){
            if( disposing && ( components != null ) )
                components.Dispose();

            base.Dispose( disposing );
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(odjemalec));
            this.chat = new System.Windows.Forms.TextBox();
            this.log = new System.Windows.Forms.TextBox();
            this.horizontal_split = new System.Windows.Forms.SplitContainer();
            this.vertical_split_right = new System.Windows.Forms.SplitContainer();
            this.log_label = new System.Windows.Forms.Label();
            this.online = new System.Windows.Forms.TextBox();
            this.online_lab = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.horizontal_split)).BeginInit();
            this.horizontal_split.Panel1.SuspendLayout();
            this.horizontal_split.Panel2.SuspendLayout();
            this.horizontal_split.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vertical_split_right)).BeginInit();
            this.vertical_split_right.Panel1.SuspendLayout();
            this.vertical_split_right.Panel2.SuspendLayout();
            this.vertical_split_right.SuspendLayout();
            this.SuspendLayout();
            // 
            // chat
            // 
            this.chat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chat.Location = new System.Drawing.Point(3, 5);
            this.chat.Name = "chat";
            this.chat.Size = new System.Drawing.Size(624, 20);
            this.chat.TabIndex = 0;
            this.chat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.chat_KeyPress);
            // 
            // log
            // 
            this.log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.log.CausesValidation = false;
            this.log.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.log.HideSelection = false;
            this.log.Location = new System.Drawing.Point(3, 3);
            this.log.Multiline = true;
            this.log.Name = "log";
            this.log.ReadOnly = true;
            this.log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.log.Size = new System.Drawing.Size(624, 657);
            this.log.TabIndex = 1;
            this.log.TabStop = false;
            this.log.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // horizontal_split
            // 
            this.horizontal_split.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontal_split.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.horizontal_split.IsSplitterFixed = true;
            this.horizontal_split.Location = new System.Drawing.Point(12, 12);
            this.horizontal_split.Name = "horizontal_split";
            // 
            // horizontal_split.Panel1
            // 
            this.horizontal_split.Panel1.Controls.Add(this.online_lab);
            this.horizontal_split.Panel1.Controls.Add(this.online);
            // 
            // horizontal_split.Panel2
            // 
            this.horizontal_split.Panel2.Controls.Add(this.vertical_split_right);
            this.horizontal_split.Size = new System.Drawing.Size(982, 697);
            this.horizontal_split.SplitterDistance = 350;
            this.horizontal_split.SplitterWidth = 2;
            this.horizontal_split.TabIndex = 4;
            // 
            // vertical_split_right
            // 
            this.vertical_split_right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vertical_split_right.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.vertical_split_right.IsSplitterFixed = true;
            this.vertical_split_right.Location = new System.Drawing.Point(0, 0);
            this.vertical_split_right.Name = "vertical_split_right";
            this.vertical_split_right.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // vertical_split_right.Panel1
            // 
            this.vertical_split_right.Panel1.Controls.Add(this.log_label);
            this.vertical_split_right.Panel1.Controls.Add(this.log);
            // 
            // vertical_split_right.Panel2
            // 
            this.vertical_split_right.Panel2.Controls.Add(this.chat);
            this.vertical_split_right.Size = new System.Drawing.Size(630, 697);
            this.vertical_split_right.SplitterDistance = 663;
            this.vertical_split_right.TabIndex = 0;
            // 
            // log_label
            // 
            this.log_label.AutoSize = true;
            this.log_label.Location = new System.Drawing.Point(3, 3);
            this.log_label.Name = "log_label";
            this.log_label.Size = new System.Drawing.Size(29, 13);
            this.log_label.TabIndex = 2;
            this.log_label.Text = "LOG";
            // 
            // online
            // 
            this.online.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.online.Font = new System.Drawing.Font("Courier New", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.online.HideSelection = false;
            this.online.Location = new System.Drawing.Point(3, 3);
            this.online.Multiline = true;
            this.online.Name = "online";
            this.online.ReadOnly = true;
            this.online.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.online.Size = new System.Drawing.Size(344, 689);
            this.online.TabIndex = 3;
            this.online.TabStop = false;
            this.online.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // online_lab
            // 
            this.online_lab.AutoSize = true;
            this.online_lab.Location = new System.Drawing.Point(3, 0);
            this.online_lab.Name = "online_lab";
            this.online_lab.Size = new System.Drawing.Size(47, 13);
            this.online_lab.TabIndex = 4;
            this.online_lab.Text = "ONLINE";
            // 
            // odjemalec
            // 
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.horizontal_split);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "odjemalec";
            this.Text = "Odjemalec";
            this.horizontal_split.Panel1.ResumeLayout(false);
            this.horizontal_split.Panel1.PerformLayout();
            this.horizontal_split.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.horizontal_split)).EndInit();
            this.horizontal_split.ResumeLayout(false);
            this.vertical_split_right.Panel1.ResumeLayout(false);
            this.vertical_split_right.Panel1.PerformLayout();
            this.vertical_split_right.Panel2.ResumeLayout(false);
            this.vertical_split_right.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.vertical_split_right)).EndInit();
            this.vertical_split_right.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TextBox chat;
        private System.Windows.Forms.TextBox log;
        private System.Windows.Forms.SplitContainer horizontal_split;
        private System.Windows.Forms.SplitContainer vertical_split_right;
        private System.Windows.Forms.Label log_label;
        private System.Windows.Forms.Label online_lab;
        private System.Windows.Forms.TextBox online;
    }
}

