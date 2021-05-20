namespace Lommeregner_til_aflevering
{
    partial class LommeregnerForm
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
            this.InputText = new System.Windows.Forms.TextBox();
            this.CalculateButton = new System.Windows.Forms.Button();
            this.Result = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // InputText
            // 
            this.InputText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.InputText.Location = new System.Drawing.Point(146, 61);
            this.InputText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.InputText.Name = "InputText";
            this.InputText.Size = new System.Drawing.Size(377, 24);
            this.InputText.TabIndex = 0;
            this.InputText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.InputText_KeyUp);
            // 
            // CalculateButton
            // 
            this.CalculateButton.AutoSize = true;
            this.CalculateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.CalculateButton.Location = new System.Drawing.Point(537, 61);
            this.CalculateButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CalculateButton.Name = "CalculateButton";
            this.CalculateButton.Size = new System.Drawing.Size(76, 27);
            this.CalculateButton.TabIndex = 1;
            this.CalculateButton.Text = "Calculate";
            this.CalculateButton.UseVisualStyleBackColor = true;
            this.CalculateButton.UseWaitCursor = true;
            this.CalculateButton.Click += new System.EventHandler(this.CalculateButton_Click);
            // 
            // Result
            // 
            this.Result.AutoSize = true;
            this.Result.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.Result.Location = new System.Drawing.Point(310, 104);
            this.Result.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Result.Name = "Result";
            this.Result.Size = new System.Drawing.Size(125, 17);
            this.Result.TabIndex = 2;
            this.Result.Text = "ResultatGoesHere";
            // 
            // LommeregnerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Result);
            this.Controls.Add(this.CalculateButton);
            this.Controls.Add(this.InputText);
            this.Name = "LommeregnerForm";
            this.Text = "Roalds Lommeregner";
            this.Load += new System.EventHandler(this.FrontPage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox InputText;
        private System.Windows.Forms.Button CalculateButton;
        private System.Windows.Forms.Label Result;
    }
}

