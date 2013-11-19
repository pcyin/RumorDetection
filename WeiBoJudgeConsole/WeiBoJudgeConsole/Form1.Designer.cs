namespace WeiBoJudgeConsole
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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.news = new System.Windows.Forms.RichTextBox();
            this.vector = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(335, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 25);
            this.button1.TabIndex = 0;
            this.button1.Text = "获取向量与相关新闻";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(26, 27);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(295, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "http://weibo.com/1188552450/zE4R89EXf";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // news
            // 
            this.news.Location = new System.Drawing.Point(23, 87);
            this.news.Name = "news";
            this.news.Size = new System.Drawing.Size(439, 245);
            this.news.TabIndex = 2;
            this.news.Text = "";
            // 
            // vector
            // 
            this.vector.Location = new System.Drawing.Point(23, 57);
            this.vector.Name = "vector";
            this.vector.Size = new System.Drawing.Size(439, 20);
            this.vector.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 345);
            this.Controls.Add(this.vector);
            this.Controls.Add(this.news);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RichTextBox news;
        private System.Windows.Forms.TextBox vector;
    }
}

