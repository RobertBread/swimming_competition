using System;
using System.Windows.Forms;
using ClientFX.client.gui;
using Lab8Csharp.model;
using Lab8Csharp.services;

namespace Lab8Csharp.client.gui
{
    public partial class LoginForm : Form
    {
        private IServices _service;

        public LoginForm()
        {
            InitializeComponent();
        }

        public void SetService(IServices service)
        {
            _service = service;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            try
            {
                string hashedPassword = Hashing.HashPassword(password);
                var user = new User(username, hashedPassword);

                _service.Login(user);

                // deschide fereastra principală
                var userForm = new UserForm(); // trebuie să o creezi
                userForm.SetService(_service);
                userForm.Show();

                this.Hide(); // sau this.Close(); dacă nu vrei să mai rămână deschisă
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Autentificare eșuată", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}