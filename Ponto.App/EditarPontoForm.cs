using Ponto.Data;
using Ponto.Domain;
using System;
using System.Windows.Forms;

namespace Ponto.App
{
    public partial class EditarPontoForm : Form
    {
        private RegistroPonto _registro;
        private readonly PontoRepository _pontoRepository;

        // Controles
        private DateTimePicker dtpData;
        private TextBox txtEntradaManha;
        private TextBox txtSaidaManha;
        private TextBox txtEntradaTarde;
        private TextBox txtSaidaTarde;
        private TextBox txtObservacao;
        private Button btnSalvar;
        private Button btnCancelar;

        // Labels
        private Label lblData;
        private Label lblManha;
        private Label lblTarde;
        private Label lblObservacao;

        public EditarPontoForm(RegistroPonto registro, PontoRepository pontoRepository)
        {
            InitializeComponent();
            _pontoRepository = pontoRepository ?? throw new ArgumentNullException(nameof(pontoRepository));

            // Inicializa um novo registro se for null
            _registro = registro ?? new RegistroPonto
            {
                Data = DateTime.Today,
                UsuarioId = _pontoRepository.ObterUsuarioPorCodigo("60")?.Id ?? 0 // Assume usuário padrão
            };

            CarregarDados();
        }

        private void CarregarDados()
        {
            dtpData.Value = _registro?.Data ?? DateTime.Today;
            txtEntradaManha.Text = _registro?.HoraEntradaManha?.ToString(@"hh\:mm") ?? "";
            txtSaidaManha.Text = _registro?.HoraSaidaManha?.ToString(@"hh\:mm") ?? "";
            txtEntradaTarde.Text = _registro?.HoraEntradaTarde?.ToString(@"hh\:mm") ?? "";
            txtSaidaTarde.Text = _registro?.HoraSaidaTarde?.ToString(@"hh\:mm") ?? "";
            txtObservacao.Text = _registro?.Observacao ?? "";
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_registro == null)
                    _registro = new RegistroPonto();

                _registro.HoraEntradaManha = TimeSpan.TryParse(txtEntradaManha.Text, out var entradaManha)
                    ? entradaManha : (TimeSpan?)null;
                _registro.HoraSaidaManha = TimeSpan.TryParse(txtSaidaManha.Text, out var saidaManha)
                    ? saidaManha : (TimeSpan?)null;
                _registro.HoraEntradaTarde = TimeSpan.TryParse(txtEntradaTarde.Text, out var entradaTarde)
                    ? entradaTarde : (TimeSpan?)null;
                _registro.HoraSaidaTarde = TimeSpan.TryParse(txtSaidaTarde.Text, out var saidaTarde)
                    ? saidaTarde : (TimeSpan?)null;
                _registro.Observacao = txtObservacao.Text;
                _registro.Data = dtpData.Value;

                if (_registro.Id == 0)
                {
                    _pontoRepository.AdicionarRegistroManual(_registro);
                    MessageBox.Show("Registro adicionado com sucesso!");
                }
                else
                {
                    _pontoRepository.AtualizarRegistroPonto(_registro);
                    MessageBox.Show("Registro atualizado com sucesso!");
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}");
            }
        }

        private void InitializeComponent()
        {
            // Configuração do formulário
            this.ClientSize = new System.Drawing.Size(300, 250);
            this.Text = _registro?.Id == 0 ? "Adicionar Registro Manual" : "Editar Registro";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Criação dos controles
            this.lblData = new Label { Text = "Data:", Location = new Point(20, 20), AutoSize = true };
            this.dtpData = new DateTimePicker
            {
                Location = new Point(80, 20),
                Size = new Size(120, 20),
                Format = DateTimePickerFormat.Short
            };

            this.lblManha = new Label { Text = "Manhã:", Location = new Point(20, 50), AutoSize = true };
            this.txtEntradaManha = new TextBox
            {
                Location = new Point(80, 50),
                Size = new Size(50, 20),
                PlaceholderText = "HH:mm"
            };
            this.txtSaidaManha = new TextBox
            {
                Location = new Point(150, 50),
                Size = new Size(50, 20),
                PlaceholderText = "HH:mm"
            };

            this.lblTarde = new Label { Text = "Tarde:", Location = new Point(20, 80), AutoSize = true };
            this.txtEntradaTarde = new TextBox
            {
                Location = new Point(80, 80),
                Size = new Size(50, 20),
                PlaceholderText = "HH:mm"
            };
            this.txtSaidaTarde = new TextBox
            {
                Location = new Point(150, 80),
                Size = new Size(50, 20),
                PlaceholderText = "HH:mm"
            };

            this.lblObservacao = new Label { Text = "Observação:", Location = new Point(20, 110), AutoSize = true };
            this.txtObservacao = new TextBox
            {
                Location = new Point(20, 130),
                Size = new Size(250, 20),
                PlaceholderText = "Observações"
            };

            this.btnSalvar = new Button
            {
                Location = new Point(70, 170),
                Size = new Size(80, 30),
                Text = "Salvar"
            };

            this.btnCancelar = new Button
            {
                Location = new Point(160, 170),
                Size = new Size(80, 30),
                Text = "Cancelar"
            };

            // Eventos
            this.btnSalvar.Click += btnSalvar_Click;
            this.btnCancelar.Click += (s, e) => this.Close();

            // Adiciona todos os controles ao formulário
            this.Controls.AddRange(new Control[] {
                lblData, dtpData,
                lblManha, txtEntradaManha, txtSaidaManha,
                lblTarde, txtEntradaTarde, txtSaidaTarde,
                lblObservacao, txtObservacao,
                btnSalvar, btnCancelar
            });
        }
    }
}