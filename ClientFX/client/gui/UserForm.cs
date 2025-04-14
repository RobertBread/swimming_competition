using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Lab8Csharp.model;
using Lab8Csharp.services;

namespace ClientFX.client.gui
{
    public partial class UserForm : Form, IObserver
    {
        private IServices _service;

        private DataGridView dgvProbes;
        private DataGridView dgvParticipants;
        private TextBox txtName;
        private TextBox txtAge;
        private Button btnRegister;
        private Button btnLogout;

        public UserForm()
        {
            InitializeComponent();
            SetupUI();
        }

        public void SetService(IServices service)
        {
            _service = service;
            _service.AddObserver(this);
            LoadProbes();
        }

        private void SetupUI()
        {
            // this.Text = "User Panel";
            // this.Size = new System.Drawing.Size(900, 600);
            //
            // dgvProbes = new DataGridView { Width = 400, Height = 200, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = true };
            // dgvParticipants = new DataGridView { Width = 400, Height = 200, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            //
            // txtName = new TextBox { PlaceholderText = "Nume" };
            // txtAge = new TextBox { PlaceholderText = "Vârstă" };
            //
            // btnRegister = new Button { Text = "Înscrie" };
            // btnLogout = new Button { Text = "Logout" };
            //
            // btnRegister.Click += BtnRegister_Click;
            // btnLogout.Click += BtnLogout_Click;
            // dgvProbes.SelectionChanged += DgvProbes_SelectionChanged;
            //
            // var topLayout = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            // topLayout.Controls.Add(dgvProbes);
            // topLayout.Controls.Add(dgvParticipants);
            //
            // var midLayout = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            // midLayout.Controls.Add(new Label { Text = "Nume:" });
            // midLayout.Controls.Add(txtName);
            // midLayout.Controls.Add(new Label { Text = "Vârstă:" });
            // midLayout.Controls.Add(txtAge);
            //
            // var bottomLayout = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            // bottomLayout.Controls.Add(btnRegister);
            // bottomLayout.Controls.Add(btnLogout);
            //
            // var mainLayout = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, Dock = DockStyle.Fill, AutoScroll = true };
            // mainLayout.Controls.Add(topLayout);
            // mainLayout.Controls.Add(midLayout);
            // mainLayout.Controls.Add(bottomLayout);
            //
            // this.Controls.Add(mainLayout);
        }

        private void LoadProbes()
        {
            var probes = _service.GetAllProbe().ToList();
            dgvProbes.DataSource = null;
            dgvProbes.DataSource = probes;

            if (dgvProbes.Columns["Id"] != null)
                dgvProbes.Columns["Id"].Visible = false;
        }

        private void LoadParticipantsForProba(long probaId)
        {
            var participants = _service.GetParticipantsForProba(probaId);
            dgvParticipants.DataSource = null;
            dgvParticipants.DataSource = participants;
            
            if (dgvParticipants.Columns["Id"] != null)
                dgvParticipants.Columns["Id"].Visible = false;
        }

        private void DgvProbes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProbes.SelectedRows.Count > 0)
            {
                var lastSelected = (ProbaDTO)dgvProbes.SelectedRows[dgvProbes.SelectedRows.Count - 1].DataBoundItem;
                LoadParticipantsForProba(lastSelected.Id);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string ageText = txtAge.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(ageText))
            {
                MessageBox.Show("Completați toate câmpurile!");
                return;
            }

            if (dgvProbes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selectați cel puțin o probă!");
                return;
            }

            if (!int.TryParse(ageText, out int age))
            {
                MessageBox.Show("Vârsta trebuie să fie un număr!");
                return;
            }

            var selectedProbeIds = dgvProbes.SelectedRows
                .Cast<DataGridViewRow>()
                .Select(r => ((ProbaDTO)r.DataBoundItem).Id)
                .ToList();

            _service.RegisterParticipantToProba(name, age, selectedProbeIds);

            MessageBox.Show("Participantul a fost înscris cu succes!");
            LoadProbes();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void ParticipantAdded(List<ProbaDTO> lista)
        {
            if (dgvProbes.InvokeRequired)
            {
                dgvProbes.BeginInvoke(new Action(() => ParticipantAdded(lista)));
                return;
            }

            dgvProbes.DataSource = null;
            dgvProbes.DataSource = lista;

            if (dgvProbes.Columns["Id"] != null)
                dgvProbes.Columns["Id"].Visible = false;
        }

    }
}
