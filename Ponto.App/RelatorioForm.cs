using Ponto.Data;
using Ponto.Domain;
using System.Globalization;

namespace Ponto.App;

public partial class RelatorioForm : Form
{
    private DataGridView dgvRelatorio;
    private Label lblTotalHoras;
    private Label lblHorasNormais;
    private Label lblHorasExtras;
    private Label lblNome;
    private Label lblCodigo;
    private Label lblCargo;
    private Label lblMesAno;
    private Button btnImprimir;
    private Button btnFechar;
    private Button btnEditar; // Novo botão de edição

    private readonly Usuario _usuario;
    private readonly PontoRepository _pontoRepository;

    public RelatorioForm(Usuario usuario, PontoRepository pontoRepository)
    {
        InitializeComponent();
        _usuario = usuario;
        _pontoRepository = pontoRepository;
        CarregarRelatorio();
    }

    private void InitializeComponent()
    {
        this.dgvRelatorio = new DataGridView();
        this.lblTotalHoras = new Label();
        this.lblHorasNormais = new Label();
        this.lblHorasExtras = new Label();
        this.lblNome = new Label();
        this.lblCodigo = new Label();
        this.lblCargo = new Label();
        this.lblMesAno = new Label();
        this.btnImprimir = new Button();
        this.btnFechar = new Button();
        this.btnEditar = new Button(); // Inicializa o botão de edição

        // Configuração dos controles
        this.SuspendLayout();

        // dgvRelatorio
        this.dgvRelatorio.Location = new Point(20, 100);
        this.dgvRelatorio.Size = new Size(800, 400);
        this.dgvRelatorio.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.dgvRelatorio.CellDoubleClick += dgvRelatorio_CellDoubleClick; // Evento de duplo clique

        // Configurar o formulário
        this.Text = "Relatório Mensal";
        this.Size = new Size(850, 600);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Configurar posições e tamanhos dos controles
        lblNome.Location = new Point(20, 20);
        lblNome.AutoSize = true;

        lblCodigo.Location = new Point(20, 50);
        lblCodigo.AutoSize = true;

        lblCargo.Location = new Point(250, 20);
        lblCargo.AutoSize = true;

        lblMesAno.Location = new Point(250, 50);
        lblMesAno.AutoSize = true;
        lblMesAno.Font = new Font(Font, FontStyle.Bold);

        // Labels de totais
        lblTotalHoras.Location = new Point(20, 520);
        lblHorasNormais.Location = new Point(150, 520);
        lblHorasExtras.Location = new Point(280, 520);

        // Botão Imprimir
        btnImprimir.Location = new Point(530, 520);
        btnImprimir.Size = new Size(80, 30);
        btnImprimir.Text = "Imprimir";
        btnImprimir.Click += btnImprimir_Click;

        // Botão Editar
        btnEditar.Location = new Point(620, 520);
        btnEditar.Size = new Size(80, 30);
        btnEditar.Text = "Editar";
        btnEditar.Click += btnEditar_Click;

        // Botão Fechar
        btnFechar.Location = new Point(740, 520);
        btnFechar.Size = new Size(80, 30);
        btnFechar.Text = "Fechar";
        btnFechar.Click += btnFechar_Click;

        // Adicionar controles ao formulário
        this.Controls.AddRange(new Control[] {
            dgvRelatorio,
            lblNome, lblCodigo, lblCargo, lblMesAno,
            lblTotalHoras, lblHorasNormais, lblHorasExtras,
            btnImprimir, btnEditar, btnFechar
        });

        this.ResumeLayout(true);
    }

    private void CarregarRelatorio()
    {
        var hoje = DateTime.Today;
        var primeiroDiaMes = new DateTime(hoje.Year, hoje.Month, 1);
        var ultimoDiaMes = primeiroDiaMes.AddMonths(1).AddDays(-1);

        var registros = _pontoRepository.ObterRegistrosPorPeriodo(_usuario.Id, primeiroDiaMes, ultimoDiaMes)
            .OrderBy(r => r.Data)
            .ToList();

        // Configurar a DataGridView
        dgvRelatorio.AutoGenerateColumns = false;
        dgvRelatorio.Columns.Clear();

        dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Data",
            DataPropertyName = "Data",
            DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
        });

        dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Dia",
            DataPropertyName = "DiaSemana"
        });

        dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Entrada Manhã",
            DataPropertyName = "HoraEntradaManhaFormatada"
        });

        dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Saída Manhã",
            DataPropertyName = "HoraSaidaManhaFormatada"
        });

        dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Entrada Tarde",
            DataPropertyName = "HoraEntradaTardeFormatada"
        });

        dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Saída Tarde",
            DataPropertyName = "HoraSaidaTardeFormatada"
        });

        dgvRelatorio.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Total",
            DataPropertyName = "TotalHorasFormatado"
        });

        // Criar lista para binding
        var dadosRelatorio = registros.Select(r => new
        {
            r.Data,
            DiaSemana = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetDayName(r.Data.DayOfWeek).ToUpper(),
            HoraEntradaManhaFormatada = r.HoraEntradaManha?.ToString(@"hh\:mm") ?? "-",
            HoraSaidaManhaFormatada = r.HoraSaidaManha?.ToString(@"hh\:mm") ?? "-",
            HoraEntradaTardeFormatada = r.HoraEntradaTarde?.ToString(@"hh\:mm") ?? "-",
            HoraSaidaTardeFormatada = r.HoraSaidaTarde?.ToString(@"hh\:mm") ?? "-",
            TotalHorasFormatado = r.CalcularHorasTrabalhadas()?.ToString(@"hh\:mm") ?? "-",
            TotalHoras = r.CalcularHorasTrabalhadas() ?? TimeSpan.Zero
        }).ToList();

        dgvRelatorio.DataSource = dadosRelatorio;

        // Calcular totais
        var totalHoras = dadosRelatorio.Sum(r => r.TotalHoras.TotalHours);
        var horasNormais = Math.Min(totalHoras, 220);
        var horasExtras = Math.Max(0, totalHoras - 220);

        lblTotalHoras.Text = $"Total: {TimeSpan.FromHours(totalHoras):hh\\:mm}";
        lblHorasNormais.Text = $"Normais: {TimeSpan.FromHours(horasNormais):hh\\:mm}";
        lblHorasExtras.Text = $"Extras: {TimeSpan.FromHours(horasExtras):hh\\:mm}";

        // Configurar cabeçalho
        lblNome.Text = _usuario.Nome;
        lblCodigo.Text = _usuario.Codigo;
        lblCargo.Text = _usuario.Cargo;
        lblMesAno.Text = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetMonthName(hoje.Month).ToUpper() + "/" + hoje.Year;
    }

    private void dgvRelatorio_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            EditarRegistroSelecionado();
        }
    }

    private void btnEditar_Click(object sender, EventArgs e)
    {
        if (dgvRelatorio.CurrentRow != null)
        {
            EditarRegistroSelecionado();
        }
        else
        {
            MessageBox.Show("Selecione um registro para editar", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void EditarRegistroSelecionado()
    {
        if (dgvRelatorio.CurrentRow == null) return;

        try
        {
            // Acessa o objeto de dados diretamente com verificação de null
            var rowData = dgvRelatorio.CurrentRow.DataBoundItem;
            if (rowData == null) return;

            // Usando reflexão para maior segurança que dynamic
            var dataProperty = rowData.GetType().GetProperty("Data");
            if (dataProperty == null)
            {
                MessageBox.Show("Não foi possível encontrar a propriedade 'Data' no registro.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dataSelecionada = (DateTime)dataProperty.GetValue(rowData);

            var registro = _pontoRepository.ObterRegistrosPorPeriodo(_usuario.Id, dataSelecionada, dataSelecionada)
                .FirstOrDefault();

            if (registro != null)
            {
                var formEdicao = new EditarPontoForm(registro, _pontoRepository);
                if (formEdicao.ShowDialog() == DialogResult.OK)
                {
                    CarregarRelatorio(); // Atualiza o grid após edição
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao editar registro: {ex.Message}", "Erro",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnImprimir_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Funcionalidade de impressão será implementada aqui.");
    }

    private void btnFechar_Click(object sender, EventArgs e)
    {
        Close();
    }
}