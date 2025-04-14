using System.ComponentModel;

namespace Lab8Csharp.client.gui;

partial class LoginForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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
    private Label lblTitle;
    private TextBox txtUsername;
    private TextBox txtPassword;
    private Button btnLogin;

    private void InitializeComponent()
    {
        this.lblTitle = new Label();
        this.txtUsername = new TextBox();
        this.txtPassword = new TextBox();
        this.btnLogin = new Button();

        // 
        // lblTitle
        // 
        this.lblTitle.Text = "Autentificare";
        this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        this.lblTitle.Location = new Point(80, 20);
        this.lblTitle.Size = new Size(150, 30);

        // 
        // txtUsername
        // 
        this.txtUsername.PlaceholderText = "Utilizator";
        this.txtUsername.Location = new Point(50, 60);
        this.txtUsername.Size = new Size(200, 23);

        // 
        // txtPassword
        // 
        this.txtPassword.PlaceholderText = "Parola";
        this.txtPassword.Location = new Point(50, 100);
        this.txtPassword.Size = new Size(200, 23);
        this.txtPassword.UseSystemPasswordChar = true;

        // 
        // btnLogin
        // 
        this.btnLogin.Text = "Login";
        this.btnLogin.Location = new Point(50, 140);
        this.btnLogin.Size = new Size(200, 30);
        this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

        // 
        // LoginForm
        // 
        this.ClientSize = new Size(300, 200);
        this.Controls.Add(this.lblTitle);
        this.Controls.Add(this.txtUsername);
        this.Controls.Add(this.txtPassword);
        this.Controls.Add(this.btnLogin);
        this.Text = "Login";
    }


    #endregion
}