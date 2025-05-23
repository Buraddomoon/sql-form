using System;
using System.Windows.Forms;
using GerenciadorTarefas.Data;
using GerenciadorTarefas.Models;

namespace GerenciadorTarefas
{
    public partial class Form1 : Form
    {
        private readonly DatabaseContext _dbContext;
        private int _tarefaIdSelecionada = -1;

        public Form1()
        {
            InitializeComponent();
            _dbContext = new DatabaseContext();
            CarregarTarefas();
        }

        private void InitializeComponent()
        {
            this.txtNome = new TextBox();
            this.dtpData = new DateTimePicker();
            this.cmbStatus = new ComboBox();
            this.dgvTarefas = new DataGridView();
            this.btnSalvar = new Button();
            this.btnExcluir = new Button();
            this.btnLimpar = new Button();
            this.label1 = new Label();
            this.label2 = new Label();
            this.label3 = new Label();

            // Configuração do formulário
            this.Text = "Gerenciador de Tarefas";
            this.Size = new System.Drawing.Size(800, 600);

            // Configuração dos controles
            this.label1.Text = "Nome:";
            this.label1.Location = new System.Drawing.Point(20, 20);
            this.label1.AutoSize = true;

            this.txtNome.Location = new System.Drawing.Point(20, 40);
            this.txtNome.Size = new System.Drawing.Size(300, 20);

            this.label2.Text = "Data:";
            this.label2.Location = new System.Drawing.Point(20, 70);
            this.label2.AutoSize = true;

            this.dtpData.Location = new System.Drawing.Point(20, 90);
            this.dtpData.Size = new System.Drawing.Size(200, 20);
            this.dtpData.Format = DateTimePickerFormat.Short;

            this.label3.Text = "Status:";
            this.label3.Location = new System.Drawing.Point(20, 120);
            this.label3.AutoSize = true;

            this.cmbStatus.Location = new System.Drawing.Point(20, 140);
            this.cmbStatus.Size = new System.Drawing.Size(200, 20);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new object[] { "Pendente", "Em Andamento", "Concluída" });
            this.cmbStatus.SelectedIndex = 0;

            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.Location = new System.Drawing.Point(20, 180);
            this.btnSalvar.Click += new EventHandler(btnSalvar_Click);

            this.btnExcluir.Text = "Excluir";
            this.btnExcluir.Location = new System.Drawing.Point(120, 180);
            this.btnExcluir.Click += new EventHandler(btnExcluir_Click);

            this.btnLimpar.Text = "Limpar";
            this.btnLimpar.Location = new System.Drawing.Point(220, 180);
            this.btnLimpar.Click += new EventHandler(btnLimpar_Click);

            this.dgvTarefas.Location = new System.Drawing.Point(20, 220);
            this.dgvTarefas.Size = new System.Drawing.Size(740, 300);
            this.dgvTarefas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTarefas.MultiSelect = false;
            this.dgvTarefas.AllowUserToAddRows = false;
            this.dgvTarefas.AllowUserToDeleteRows = false;
            this.dgvTarefas.ReadOnly = true;
            this.dgvTarefas.SelectionChanged += new EventHandler(dgvTarefas_SelectionChanged);

            this.Controls.AddRange(new Control[] {
                this.label1, this.txtNome,
                this.label2, this.dtpData,
                this.label3, this.cmbStatus,
                this.btnSalvar, this.btnExcluir, this.btnLimpar,
                this.dgvTarefas
            });
        }

        private void CarregarTarefas()
        {
            dgvTarefas.DataSource = _dbContext.ObterTarefas();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Por favor, informe o nome da tarefa.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var tarefa = new Tarefa
            {
                Id = _tarefaIdSelecionada,
                Nome = txtNome.Text,
                Data = dtpData.Value,
                Status = cmbStatus.SelectedItem.ToString()
            };

            if (_tarefaIdSelecionada == -1)
            {
                _dbContext.InserirTarefa(tarefa);
            }
            else
            {
                _dbContext.AtualizarTarefa(tarefa);
            }

            LimparCampos();
            CarregarTarefas();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (_tarefaIdSelecionada == -1)
            {
                MessageBox.Show("Por favor, selecione uma tarefa para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Tem certeza que deseja excluir esta tarefa?", "Confirmação", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _dbContext.ExcluirTarefa(_tarefaIdSelecionada);
                LimparCampos();
                CarregarTarefas();
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }

        private void dgvTarefas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTarefas.SelectedRows.Count > 0)
            {
                var row = dgvTarefas.SelectedRows[0];
                _tarefaIdSelecionada = Convert.ToInt32(row.Cells["Id"].Value);
                txtNome.Text = row.Cells["Nome"].Value.ToString();
                dtpData.Value = Convert.ToDateTime(row.Cells["Data"].Value);
                cmbStatus.SelectedItem = row.Cells["Status"].Value.ToString();
            }
        }

        private void LimparCampos()
        {
            _tarefaIdSelecionada = -1;
            txtNome.Clear();
            dtpData.Value = DateTime.Now;
            cmbStatus.SelectedIndex = 0;
            dgvTarefas.ClearSelection();
        }

        private TextBox txtNome;
        private DateTimePicker dtpData;
        private ComboBox cmbStatus;
        private DataGridView dgvTarefas;
        private Button btnSalvar;
        private Button btnExcluir;
        private Button btnLimpar;
        private Label label1;
        private Label label2;
        private Label label3;
    }
}
