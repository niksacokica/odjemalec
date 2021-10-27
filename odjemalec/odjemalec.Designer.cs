
namespace odjemalec{
    partial class odjemalec{
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose( bool disposing ){
            if ( disposing && ( components != null ) )
                components.Dispose();

            base.Dispose( disposing );
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent(){
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(odjemalec));
            this.horizontal_split = new System.Windows.Forms.SplitContainer();
            this.stats_lab = new System.Windows.Forms.Label();
            this.stats = new System.Windows.Forms.TextBox();
            this.log_lab = new System.Windows.Forms.Label();
            this.log = new System.Windows.Forms.TextBox();
            this.chat = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.horizontal_split)).BeginInit();
            this.horizontal_split.Panel1.SuspendLayout();
            this.horizontal_split.Panel2.SuspendLayout();
            this.horizontal_split.SuspendLayout();
            this.SuspendLayout();
            // 
            // horizontal_split
            // 
            this.horizontal_split.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.horizontal_split.IsSplitterFixed = true;
            this.horizontal_split.Location = new System.Drawing.Point(12, 12);
            this.horizontal_split.Name = "horizontal_split";
            this.horizontal_split.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // horizontal_split.Panel1
            // 
            this.horizontal_split.Panel1.Controls.Add(this.stats_lab);
            this.horizontal_split.Panel1.Controls.Add(this.stats);
            // 
            // horizontal_split.Panel2
            // 
            this.horizontal_split.Panel2.Controls.Add(this.log_lab);
            this.horizontal_split.Panel2.Controls.Add(this.log);
            this.horizontal_split.Panel2.Controls.Add(this.chat);
            this.horizontal_split.Size = new System.Drawing.Size(982, 697);
            this.horizontal_split.SplitterDistance = 150;
            this.horizontal_split.TabIndex = 1;
            // 
            // stats_lab
            // 
            this.stats_lab.AutoSize = true;
            this.stats_lab.Location = new System.Drawing.Point(0, 0);
            this.stats_lab.Name = "stats_lab";
            this.stats_lab.Size = new System.Drawing.Size(53, 17);
            this.stats_lab.TabIndex = 1;
            this.stats_lab.Text = "STATS";
            // 
            // stats
            // 
            this.stats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stats.Enabled = false;
            this.stats.Location = new System.Drawing.Point(3, 3);
            this.stats.Multiline = true;
            this.stats.Name = "stats";
            this.stats.ReadOnly = true;
            this.stats.ShortcutsEnabled = false;
            this.stats.Size = new System.Drawing.Size(976, 144);
            this.stats.TabIndex = 0;
            this.stats.TabStop = false;
            // 
            // log_lab
            // 
            this.log_lab.AutoSize = true;
            this.log_lab.Location = new System.Drawing.Point(0, 0);
            this.log_lab.Name = "log_lab";
            this.log_lab.Size = new System.Drawing.Size(38, 17);
            this.log_lab.TabIndex = 2;
            this.log_lab.Text = "LOG";
            // 
            // log
            // 
            this.log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.log.CausesValidation = false;
            this.log.Location = new System.Drawing.Point(4, 4);
            this.log.Multiline = true;
            this.log.Name = "log";
            this.log.ReadOnly = true;
            this.log.Size = new System.Drawing.Size(975, 508);
            this.log.TabIndex = 1;
            this.log.TabStop = false;
            this.log.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chat
            // 
            this.chat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chat.Location = new System.Drawing.Point(4, 518);
            this.chat.Name = "chat";
            this.chat.Size = new System.Drawing.Size(975, 22);
            this.chat.TabIndex = 0;
            this.chat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.chat_KeyPress);
            // 
            // odjemalec
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 721);
            this.Controls.Add(this.horizontal_split);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "odjemalec";
            this.Text = "Odjemalec";
            this.horizontal_split.Panel1.ResumeLayout(false);
            this.horizontal_split.Panel1.PerformLayout();
            this.horizontal_split.Panel2.ResumeLayout(false);
            this.horizontal_split.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.horizontal_split)).EndInit();
            this.horizontal_split.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.SplitContainer horizontal_split;
        private System.Windows.Forms.TextBox stats;
        private System.Windows.Forms.TextBox log;
        private System.Windows.Forms.TextBox chat;
        private System.Windows.Forms.Label stats_lab;
        private System.Windows.Forms.Label log_lab;
    }
}

