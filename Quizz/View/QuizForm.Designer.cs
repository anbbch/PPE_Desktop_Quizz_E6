namespace Quizz
{
    partial class QuizForm
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
            lblQuestion = new Label();
            panelAnswers = new Panel();
            SuspendLayout();
            // 
            // lblQuestion
            // 
            lblQuestion.AutoSize = true;
            lblQuestion.Location = new Point(326, 105);
            lblQuestion.Name = "lblQuestion";
            lblQuestion.Size = new Size(0, 30);
            lblQuestion.TabIndex = 0;
            // 
            // panelAnswers
            // 
            panelAnswers.Location = new Point(296, 157);
            panelAnswers.Name = "panelAnswers";
            panelAnswers.Size = new Size(710, 395);
            panelAnswers.TabIndex = 1;
            // 
            // QuizForm
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1229, 875);
            Controls.Add(panelAnswers);
            Controls.Add(lblQuestion);
            Name = "QuizForm";
            Text = "QuizForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblQuestion;
        private Panel panelAnswers;
    }
}