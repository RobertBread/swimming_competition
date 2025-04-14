using System.ComponentModel;

namespace ClientFX.client.gui;

partial class UserForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.Label lblName;
    private System.Windows.Forms.Label lblAge;
    private System.Windows.Forms.Label lblProbes;
    private System.Windows.Forms.Label lblParticipants;

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
            this.components = new System.ComponentModel.Container();

            this.dgvProbes = new System.Windows.Forms.DataGridView();
            this.dgvProbes.SelectionChanged += new EventHandler(this.DgvProbes_SelectionChanged);
            this.dgvParticipants = new System.Windows.Forms.DataGridView();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtAge = new System.Windows.Forms.TextBox();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.lblAge = new System.Windows.Forms.Label();
            this.lblProbes = new System.Windows.Forms.Label();
            this.lblParticipants = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.dgvProbes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParticipants)).BeginInit();

            this.SuspendLayout();

            // lblProbes
            this.lblProbes.Text = "Lista Probelor";
            this.lblProbes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblProbes.Location = new System.Drawing.Point(50, 20);

            // dgvProbes
            this.dgvProbes.Location = new System.Drawing.Point(50, 50);
            this.dgvProbes.Size = new System.Drawing.Size(400, 200);
            this.dgvProbes.ReadOnly = true;
            this.dgvProbes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProbes.MultiSelect = true;
            this.dgvProbes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            // lblParticipants
            this.lblParticipants.Text = "Lista Participanților";
            this.lblParticipants.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblParticipants.Location = new System.Drawing.Point(480, 20);

            // dgvParticipants
            this.dgvParticipants.Location = new System.Drawing.Point(480, 50);
            this.dgvParticipants.Size = new System.Drawing.Size(400, 200);
            this.dgvParticipants.ReadOnly = true;
            this.dgvParticipants.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvParticipants.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;

            // lblName
            this.lblName.Text = "Nume:";
            this.lblName.AutoSize = true; 
            this.lblName.Location = new System.Drawing.Point(50, 280);

            // txtName
            this.txtName.Location = new System.Drawing.Point(100, 275);
            this.txtName.Size = new System.Drawing.Size(150, 23);

            // lblAge
            this.lblAge.Text = "Vârstă:";
            this.lblAge.AutoSize = true; 
            this.lblAge.Location = new System.Drawing.Point(270, 280);

            // txtAge
            this.txtAge.Location = new System.Drawing.Point(320, 275);
            this.txtAge.Size = new System.Drawing.Size(80, 23);

            // btnRegister
            this.btnRegister.Text = "Înscrie";
            this.btnRegister.Location = new System.Drawing.Point(480, 270);
            this.btnRegister.Size = new System.Drawing.Size(100, 30);
            this.btnRegister.Click += new System.EventHandler(this.BtnRegister_Click);

            // btnLogout
            this.btnLogout.Text = "Logout";
            this.btnLogout.Location = new System.Drawing.Point(600, 270);
            this.btnLogout.Size = new System.Drawing.Size(100, 30);
            this.btnLogout.Click += new System.EventHandler(this.BtnLogout_Click);

            // UserForm
            this.ClientSize = new System.Drawing.Size(900, 350);
            this.Controls.Add(this.lblProbes);
            this.Controls.Add(this.dgvProbes);
            this.Controls.Add(this.lblParticipants);
            this.Controls.Add(this.dgvParticipants);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblAge);
            this.Controls.Add(this.txtAge);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.btnLogout);
            this.Text = "User Panel";

            ((System.ComponentModel.ISupportInitialize)(this.dgvProbes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParticipants)).EndInit();

            this.ResumeLayout(false);
            this.PerformLayout();
        }

    #endregion
}