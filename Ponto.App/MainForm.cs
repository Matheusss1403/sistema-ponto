using Ponto.Data;
using Ponto.Domain;
using Ponto.App;
using System.Data;

namespace Ponto.App;

public partial class MainForm : Form
{
    private readonly PontoRepository _pontoRepository;
    private Label lblUsuario;
    private Label lblCargo;
    private Label lblHorario;
    private Label lblStatus;
    private Button btnEntradaManha;
    private Button btnSaidaManha;
    private Button btnEntradaTarde;
    private Button btnSaidaTarde;
    private Button btnRelatorio;
    private Usuario _usuarioAtual;
    private Button btnEditarRegistro;
    private Button btnAdicionarManual;

    public MainForm()
    {
        InitializeComponent();
        _pontoRepository = new PontoRepository();
        DatabaseConfig.InitializeDatabase();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        CarregarUsuario();
        AtualizarInterface();
    }

    private void CarregarUsuario()
    {
        // Aqui você pode implementar um login ou usar um usuário fixo
        _usuarioAtual = _pontoRepository.ObterUsuarioPorCodigo("60");

        if (_usuarioAtual == null)
        {
            _usuarioAtual = new Usuario
            {
                Id = 1,
                Nome = "Matheus Siede dos Santos",
                Codigo = "60",
                Departamento = "MATRIZ",
                Cargo = "AUXILIAR SUPORTE TÉCNICO",
                HorarioPadrao = "Segunda a Sexta das 8:00 às 12:00 e das 13:30 às 18:00"
            };
        }

        lblUsuario.Text = $"{_usuarioAtual.Codigo} - {_usuarioAtual.Nome}";
        lblCargo.Text = _usuarioAtual.Cargo;
        lblHorario.Text = _usuarioAtual.HorarioPadrao;
    }

    private void AtualizarInterface()
    {
        var hoje = DateTime.Today;
        var registroHoje = _pontoRepository.ObterRegistrosPorPeriodo(_usuarioAtual.Id, hoje, hoje)
            .FirstOrDefault();

        btnEntradaManha.Enabled = registroHoje?.HoraEntradaManha == null;
        btnSaidaManha.Enabled = registroHoje?.HoraEntradaManha != null && registroHoje?.HoraSaidaManha == null;
        btnEntradaTarde.Enabled = registroHoje?.HoraSaidaManha != null && registroHoje?.HoraEntradaTarde == null;
        btnSaidaTarde.Enabled = registroHoje?.HoraEntradaTarde != null && registroHoje?.HoraSaidaTarde == null;

        lblStatus.Text = ObterStatusAtual(registroHoje);
    }

    private string ObterStatusAtual(RegistroPonto registro)
    {
        if (registro == null) return "Aguardando entrada (manhã)";

        if (registro.HoraEntradaManha != null && registro.HoraSaidaManha == null)
            return $"Trabalhando (manhã) - Entrada: {registro.HoraEntradaManha.Value:hh\\:mm}";

        if (registro.HoraSaidaManha != null && registro.HoraEntradaTarde == null)
            return $"Intervalo almoço - Saída: {registro.HoraSaidaManha.Value:hh\\:mm}";

        if (registro.HoraEntradaTarde != null && registro.HoraSaidaTarde == null)
            return $"Trabalhando (tarde) - Entrada: {registro.HoraEntradaTarde.Value:hh\\:mm}";

        return $"Dia encerrado - Total: {registro.CalcularHorasTrabalhadas()?.ToString("hh\\:mm") ?? "N/A"}";
    }

    private void RegistrarPonto(TimeSpan? entradaManha, TimeSpan? saidaManha, TimeSpan? entradaTarde, TimeSpan? saidaTarde)
    {
        var registro = new RegistroPonto
        {
            UsuarioId = _usuarioAtual.Id,
            Data = DateTime.Today,
            HoraEntradaManha = entradaManha,
            HoraSaidaManha = saidaManha,
            HoraEntradaTarde = entradaTarde,
            HoraSaidaTarde = saidaTarde
        };

        _pontoRepository.RegistrarPonto(registro);
        AtualizarInterface();
    }

    private void btnEntradaManha_Click(object sender, EventArgs e)
    {
        RegistrarPonto(DateTime.Now.TimeOfDay, null, null, null);
    }

    private void btnSaidaManha_Click(object sender, EventArgs e)
    {
        var registro = _pontoRepository.ObterRegistrosPorPeriodo(_usuarioAtual.Id, DateTime.Today, DateTime.Today)
            .FirstOrDefault();

        RegistrarPonto(registro?.HoraEntradaManha, DateTime.Now.TimeOfDay, null, null);
    }

    private void btnEntradaTarde_Click(object sender, EventArgs e)
    {
        var registro = _pontoRepository.ObterRegistrosPorPeriodo(_usuarioAtual.Id, DateTime.Today, DateTime.Today)
            .FirstOrDefault();

        RegistrarPonto(registro?.HoraEntradaManha, registro?.HoraSaidaManha, DateTime.Now.TimeOfDay, null);
    }

    private void btnSaidaTarde_Click(object sender, EventArgs e)
    {
        var registro = _pontoRepository.ObterRegistrosPorPeriodo(_usuarioAtual.Id, DateTime.Today, DateTime.Today)
            .FirstOrDefault();

        RegistrarPonto(registro?.HoraEntradaManha, registro?.HoraSaidaManha, registro?.HoraEntradaTarde, DateTime.Now.TimeOfDay);
    }

    private void InitializeComponent()
    {
        lblUsuario = new Label();
        lblCargo = new Label();
        lblHorario = new Label();
        lblStatus = new Label();
        btnEntradaManha = new Button();
        btnSaidaManha = new Button();
        btnEntradaTarde = new Button();
        btnSaidaTarde = new Button();
        btnRelatorio = new Button();
        btnEditarRegistro = new Button();
        btnAdicionarManual = new Button();
        SuspendLayout();
        // 
        // lblUsuario
        // 
        lblUsuario.AutoSize = true;
        lblUsuario.Location = new Point(20, 20);
        lblUsuario.Name = "lblUsuario";
        lblUsuario.Size = new Size(50, 15);
        lblUsuario.TabIndex = 0;
        lblUsuario.Text = "Usuário:";
        // 
        // lblCargo
        // 
        lblCargo.AutoSize = true;
        lblCargo.Location = new Point(20, 50);
        lblCargo.Name = "lblCargo";
        lblCargo.Size = new Size(42, 15);
        lblCargo.TabIndex = 1;
        lblCargo.Text = "Cargo:";
        // 
        // lblHorario
        // 
        lblHorario.AutoSize = true;
        lblHorario.Location = new Point(20, 80);
        lblHorario.Name = "lblHorario";
        lblHorario.Size = new Size(50, 15);
        lblHorario.TabIndex = 2;
        lblHorario.Text = "Horário:";
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Microsoft Sans Serif", 8.25F);
        lblStatus.Location = new Point(20, 110);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(138, 13);
        lblStatus.TabIndex = 3;
        lblStatus.Text = "Status: Aguardando registro";
        // 
        // btnEntradaManha
        // 
        btnEntradaManha.BackColor = SystemColors.ControlLight;
        btnEntradaManha.FlatStyle = FlatStyle.Flat;
        btnEntradaManha.Location = new Point(20, 150);
        btnEntradaManha.Name = "btnEntradaManha";
        btnEntradaManha.Size = new Size(120, 40);
        btnEntradaManha.TabIndex = 4;
        btnEntradaManha.Text = "Entrada Manhã";
        btnEntradaManha.UseVisualStyleBackColor = false;
        // 
        // btnSaidaManha
        // 
        btnSaidaManha.BackColor = SystemColors.ControlLight;
        btnSaidaManha.Enabled = false;
        btnSaidaManha.FlatStyle = FlatStyle.Flat;
        btnSaidaManha.Location = new Point(150, 150);
        btnSaidaManha.Name = "btnSaidaManha";
        btnSaidaManha.Size = new Size(120, 40);
        btnSaidaManha.TabIndex = 5;
        btnSaidaManha.Text = "Saída Manhã";
        btnSaidaManha.UseVisualStyleBackColor = false;
        // 
        // btnEntradaTarde
        // 
        btnEntradaTarde.BackColor = SystemColors.ControlLight;
        btnEntradaTarde.FlatStyle = FlatStyle.Flat;
        btnEntradaTarde.Location = new Point(20, 200);
        btnEntradaTarde.Name = "btnEntradaTarde";
        btnEntradaTarde.Size = new Size(120, 40);
        btnEntradaTarde.TabIndex = 6;
        btnEntradaTarde.Text = "Entrada Tarde";
        btnEntradaTarde.UseVisualStyleBackColor = false;
        // 
        // btnSaidaTarde
        // 
        btnSaidaTarde.BackColor = SystemColors.ControlLight;
        btnSaidaTarde.FlatStyle = FlatStyle.Flat;
        btnSaidaTarde.Location = new Point(150, 200);
        btnSaidaTarde.Name = "btnSaidaTarde";
        btnSaidaTarde.Size = new Size(120, 40);
        btnSaidaTarde.TabIndex = 7;
        btnSaidaTarde.Text = "Saída Tarde";
        btnSaidaTarde.UseVisualStyleBackColor = false;
        // 
        // btnRelatorio
        // 
        btnRelatorio.BackColor = SystemColors.ControlLight;
        btnRelatorio.FlatStyle = FlatStyle.Flat;
        btnRelatorio.Location = new Point(250, 250);
        btnRelatorio.Name = "btnRelatorio";
        btnRelatorio.Size = new Size(100, 30);
        btnRelatorio.TabIndex = 8;
        btnRelatorio.Text = "Relatório";
        btnRelatorio.UseVisualStyleBackColor = false;
        //
        // btnEditarRegistro
        // 
        btnEditarRegistro.Location = new Point(20, 300);
        btnEditarRegistro.Size = new Size(150, 30);
        btnEditarRegistro.Text = "Editar Registro";
        btnEditarRegistro.Click += btnEditarRegistro_Click;
        // 
        // btnAdicionarManual
        // 
        btnAdicionarManual.Location = new Point(180, 300);
        btnAdicionarManual.Size = new Size(150, 30);
        btnAdicionarManual.Text = "Adicionar Manual";
        btnAdicionarManual.Click += btnAdicionarManual_Click;
        // 
        // MainForm
        // 
        ClientSize = new Size(384, 400);
        Controls.Add(btnRelatorio);
        Controls.Add(btnSaidaTarde);
        Controls.Add(btnEntradaTarde);
        Controls.Add(btnSaidaManha);
        Controls.Add(btnEntradaManha);
        Controls.Add(lblStatus);
        Controls.Add(lblHorario);
        Controls.Add(lblCargo);
        Controls.Add(lblUsuario);
        Controls.Add(btnEditarRegistro);
        Controls.Add(btnAdicionarManual);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Sistema de Ponto";
        ResumeLayout(false);
        PerformLayout();

        // Vincular eventos
        btnEntradaManha.Click += btnEntradaManha_Click;
        btnSaidaManha.Click += btnSaidaManha_Click;
        btnEntradaTarde.Click += btnEntradaTarde_Click;
        btnSaidaTarde.Click += btnSaidaTarde_Click;
        btnRelatorio.Click += btnRelatorio_Click;

        this.Load += MainForm_Load;
    }

    private void btnRelatorio_Click(object sender, EventArgs e)
    {
        var relatorioForm = new RelatorioForm(_usuarioAtual, _pontoRepository);
        relatorioForm.ShowDialog();
    }

    private void btnEditarRegistro_Click(object sender, EventArgs e)
    {
        var dataSelecionada = DateTime.Today; // Ou pegue de um controle de data
        var registro = _pontoRepository.ObterRegistroPorData(_usuarioAtual.Id, dataSelecionada);

        // Agora pode passar null que o form tratará
        var editarForm = new EditarPontoForm(registro, _pontoRepository);
        if (editarForm.ShowDialog() == DialogResult.OK)
        {
            AtualizarInterface();
        }
    }

    private void btnAdicionarManual_Click(object sender, EventArgs e)
    {
        // Pode passar null que será criado um novo registro
        var editarForm = new EditarPontoForm(null, _pontoRepository);
        if (editarForm.ShowDialog() == DialogResult.OK)
        {
            AtualizarInterface();
        }
    }
}