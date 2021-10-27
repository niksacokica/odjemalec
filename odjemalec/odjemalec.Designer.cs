
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
            this.horizontal_split.Location = new System.Drawing.Point(9, 10);
            this.horizontal_split.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.horizontal_split.Size = new System.Drawing.Size(736, 566);
            this.horizontal_split.SplitterDistance = 121;
            this.horizontal_split.SplitterWidth = 3;
            this.horizontal_split.TabIndex = 1;
            // 
            // stats_lab
            // 
            this.stats_lab.AutoSize = true;
            this.stats_lab.Location = new System.Drawing.Point(0, 0);
            this.stats_lab.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.stats_lab.Name = "stats_lab";
            this.stats_lab.Size = new System.Drawing.Size(42, 13);
            this.stats_lab.TabIndex = 1;
            this.stats_lab.Text = "STATS";
            // 
            // stats
            // 
            this.stats.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stats.Enabled = false;
            this.stats.Location = new System.Drawing.Point(2, 2);
            this.stats.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.stats.Multiline = true;
            this.stats.Name = "stats";
            this.stats.ReadOnly = true;
            this.stats.ShortcutsEnabled = false;
            this.stats.Size = new System.Drawing.Size(733, 117);
            this.stats.TabIndex = 0;
            this.stats.TabStop = false;
            // 
            // log_lab
            // 
            this.log_lab.AutoSize = true;
            this.log_lab.Location = new System.Drawing.Point(0, 0);
            this.log_lab.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.log_lab.Name = "log_lab";
            this.log_lab.Size = new System.Drawing.Size(29, 13);
            this.log_lab.TabIndex = 2;
            this.log_lab.Text = "LOG";
            // 
            // log
            // 
            this.log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.log.CausesValidation = false;
            this.log.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.log.Location = new System.Drawing.Point(3, 3);
            this.log.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.log.Multiline = true;
            this.log.Name = "log";
            this.log.ReadOnly = true;
            this.log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.log.Size = new System.Drawing.Size(732, 415);
            this.log.TabIndex = 1;
            this.log.TabStop = false;
            this.log.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // chat
            // 
            this.chat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chat.Location = new System.Drawing.Point(3, 422);
            this.chat.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chat.Name = "chat";
            this.chat.Size = new System.Drawing.Size(732, 20);
            this.chat.TabIndex = 0;
            this.chat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.chat_KeyPress);
            // 
            // odjemalec
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 586);
            this.Controls.Add(this.horizontal_split);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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

