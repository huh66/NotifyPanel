namespace NotifyPanel
{
    partial class MessageForm
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblHeader;
        private Label lblSubject;
        private Label lblReferenz;
        private Button btnOk;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageForm));
            lblHeader = new Label();
            lblSubject = new Label();
            lblReferenz = new Label();
            btnOk = new Button();
            lblMessage = new Label();
            SuspendLayout();
            // 
            // lblHeader
            // 
            lblHeader.Font = new Font("Segoe UI", 14F);
            lblHeader.Location = new Point(20, 20);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(440, 30);
            lblHeader.TabIndex = 0;
            lblHeader.Text = "HEADER";
            // 
            // lblSubject
            // 
            lblSubject.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSubject.Location = new Point(20, 60);
            lblSubject.Name = "lblSubject";
            lblSubject.Size = new Size(440, 25);
            lblSubject.TabIndex = 1;
            lblSubject.Text = "SUBJECT";
            // 
            // lblReferenz
            // 
            lblReferenz.Font = new Font("Segoe UI", 12F);
            lblReferenz.ForeColor = Color.Red;
            lblReferenz.Location = new Point(20, 95);
            lblReferenz.Name = "lblReferenz";
            lblReferenz.Size = new Size(440, 25);
            lblReferenz.TabIndex = 2;
            lblReferenz.Text = "REFERENZ";
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(20, 250);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(440, 30);
            btnOk.TabIndex = 4;
            btnOk.Text = Properties.Resources.MeesageBoxForm_OkButton_Text;
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += BtnOk_Click;
            // 
            // lblMessage
            // 
            lblMessage.BackColor = SystemColors.ControlLight;
            lblMessage.Font = new Font("Segoe UI", 11F, FontStyle.Bold | FontStyle.Italic);
            lblMessage.Location = new Point(20, 120);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(440, 127);
            lblMessage.TabIndex = 5;
            lblMessage.Text = "MESSAGE";
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MessageForm
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(480, 300);
            Controls.Add(lblMessage);
            Controls.Add(lblHeader);
            Controls.Add(lblSubject);
            Controls.Add(lblReferenz);
            Controls.Add(btnOk);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MessageForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "New Message";
            TopMost = true;
            ResumeLayout(false);
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private Label lblMessage;
    }
}