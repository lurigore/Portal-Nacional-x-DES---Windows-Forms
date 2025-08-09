using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Portal_Nacional_x_DES
{
    public partial class Form1 : Form
    {
        static string versaoDES = "VERSÃO301 BUILD152";
        static string pathBancoIMs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BancoInscricoes.csv");

        Dictionary<string, string> inscricoes = new Dictionary<string, string>();

        //GLOBAIS PORTAL NACIONAL
        static string XML_FOLDER = @""; // Pasta onde estão os XMLs
        static string imTomadorGlobal = "";
        static List<string> chaveSubstituida = new List<string>();

        //GLOBAIS SP NFSE 
        static string CSV_FOLDER = @"";

        public Form1()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;
            versao_DES.Text = $"DES: {versaoDES}";

            inscricoesListView.Columns.Add("Empresa", 200);
            inscricoesListView.Columns.Add("CNPJ", 150);
            inscricoesListView.Columns.Add("Inscricao Municipal", 150);

            var linhas = File.ReadAllLines(pathBancoIMs, Encoding.UTF8);
            foreach (var linha in linhas)
            {
                var coluna = linha.Split(";");

                if (!inscricoes.ContainsKey(coluna[2]))
                {
                    inscricoes.Add(coluna[2], $"{coluna[0]};{coluna[1]};{coluna[2]}");

                    var item = new ListViewItem(coluna[0]);
                    item.SubItems.Add(TratarCNPJ(coluna[1]));
                    item.SubItems.Add(TratarIm(coluna[2]));
                    inscricoesListView.Items.Add(item);
                }
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Selecione uma pasta";
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    XML_FOLDER = folderBrowserDialog.SelectedPath;
                    textBox1.Text = XML_FOLDER;
                    string[] arquivos = Directory.GetFiles(XML_FOLDER);
                    listBox1.Items.Clear();
                    foreach (string arquivo in arquivos)
                    {
                        if (arquivo.EndsWith(".xml"))
                        {
                            listBox1.Items.Add(Path.GetFileName(arquivo));
                        }
                    }
                }
            }
            registros1.Text = "Total: " + listBox1.Items.Count.ToString();
        }

        private void processButton1_Click(object sender, EventArgs e)
        {
            ProcessarPasta(XML_FOLDER);
        }

        public void ProcessarPasta(string xmlFolder)
        {
            if (!Directory.Exists(xmlFolder))
            {
                MessageBox.Show("Selecione uma pasta", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var arquivosXml = Directory.GetFiles(xmlFolder, "*.xml");
                if (arquivosXml.Length == 0)
                {
                    MessageBox.Show("Nenhum arquivo .xml encontrado na pasta.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var registrosH = new Dictionary<string, string>();
                    var registrosR = new Dictionary<string, string>();
                    var listaRelatorio = new List<Relatorio>();
                    var index = new List<int>();
                    foreach (var path in arquivosXml)
                    {
                        try
                        {
                            var (h, r, cnpj, dados) = GerarRegistros(path);
                            if (!registrosH.ContainsKey(cnpj))
                                registrosH[cnpj] = h;
                            registrosR.Add(Path.GetFileName(path).Substring(0, Path.GetFileName(path).Length - 4), r);
                            listaRelatorio.Add(dados);
                            foreach (string chave in chaveSubstituida)
                            {
                                if (chave != "")
                                {
                                    if (registrosR.Keys.ToList().IndexOf(chave) >= 0)
                                    {
                                        index.Add(registrosR.Keys.ToList().IndexOf(chave));
                                    }
                                    if (chave != "" && registrosR.ContainsKey(chave))
                                    {
                                        MessageBox.Show($"Nota {chave} desconsiderada: Nota Substituída",
                                            "Nota Substituída", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        registrosR.Remove(chave);
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro ao processar o arquivo: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    foreach (int posicao in index)
                    {
                        listaRelatorio.RemoveAt(posicao);
                    }

                    string arquivoSaida = Path.Combine(xmlFolder, "1 - ARQUIVO PARA IMPORTAÇÃO - DES.txt");

                    string pastaLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                    Directory.CreateDirectory(pastaLog);
                    string relatorio = Path.Combine(pastaLog, $"relatorio_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                    using (StreamWriter sw = new StreamWriter(relatorio))
                    {
                        sw.WriteLine("- /// RELATÓRIO DE NOTAS PROCESSADAS /// -");
                        sw.WriteLine($"Tomador: {listaRelatorio.FirstOrDefault()?.tomador} | {listaRelatorio.FirstOrDefault()?.cnpjTomador} \n");
                        sw.WriteLine("Número da Nota | Nome do Emitente | Estado | Município | Valor | Alíquota\n");
                        foreach (var item in listaRelatorio)
                        {
                            sw.WriteLine($"{item.numeroNota} | {item.nomeEmitente} | {item.estado} | {item.municipio} | {item.valor} | {item.aliquota}");
                        }
                        sw.WriteLine($"\nTotal de Notas Processadas: {listaRelatorio.Count}");
                    }
                    File.WriteAllLines(arquivoSaida, registrosH.Values.Concat(registrosR.Values));

                    abrirPastaProcessada(XML_FOLDER, false);
                }
            }

        }

        public (string registroH, string registroR, string cnpjTomador, Relatorio dados) GerarRegistros(string xmlPath)
        {
            try
            {

                XDocument doc = XDocument.Load(xmlPath);
                XNamespace nf = "http://www.sped.fazenda.gov.br/nfse";

                var toma = doc.Descendants(nf + "toma").FirstOrDefault();
                var emit = doc.Descendants(nf + "emit").FirstOrDefault();
                var prest = doc.Descendants(nf + "prest").FirstOrDefault();
                var regime = doc.Descendants(nf + "regTrib").FirstOrDefault();
                string opcaoSimples = prest?.Descendants(nf + "opSimpNac").FirstOrDefault()?.Value ?? "";
                var infDPS = doc.Descendants(nf + "infDPS").FirstOrDefault();
                var valores = doc.Descendants(nf + "valores").FirstOrDefault();
                var valorServ = doc.Descendants(nf + "vServPrest").FirstOrDefault();
                var localIncid = doc.Descendants(nf + "cLocIncid").FirstOrDefault();
                var trib = doc.Descendants(nf + "tribMun").FirstOrDefault();
                var serv = doc.Descendants(nf + "cServ").FirstOrDefault();
                var subst = doc.Descendants(nf + "subst").FirstOrDefault();

                //possibilidades
                string[] sociedadeCGC = {
                    "19179789000118",
                    "65165649000108"
                };
                string[] construcao = {
                    "070201",
                    "070202",
                    "070203",
                    "070204",
                    "070205",
                    "070206",
                    "070207",
                    "070208",
                    "070209",
                    "0702010"
                };
                string[] propaganda = {
                    "170601",
                    "170602",
                    "170603",
                    "170604",
                    "170605",
                    "170606",
                    "170607",
                    "170608",
                    "170609",
                    "170610"

                };

                string chaveSubst = subst?.Element(nf + "chSubstda")?.Value ?? "";
                chaveSubstituida.Add(chaveSubst);


                string nomeArquivo = Path.GetFileName(xmlPath);
                string imXml = toma?.Element(nf + "IM")?.Value ?? "";
                if (imTomadorGlobal == "")
                {
                    imTomadorGlobal = imXml;

                }
                string imTomador = imTomadorGlobal;
                if (imTomador == "")
                {
                    MessageBox.Show("Inscrição Municipal do Tomador não encontrada. Por favor, insira manualmente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    while (true)
                    {
                        imTomadorGlobal = Interaction.InputBox("Insira a Inscrição Municipal do Tomador (Sem pontuação):", "Inscrição Municipal");

                        if (imTomadorGlobal.Length == 11)
                            break;

                        MessageBox.Show("Inscrição Municipal Inválida.",
                        "Formato inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (imXml != "" && imXml != imTomadorGlobal)
                {
                    imTomadorGlobal = imXml;
                }
                showInscrição.Text = "Inscrição Municipal: " + imTomadorGlobal;


                string cnpjTomador = toma?.Element(nf + "CNPJ")?.Value ?? "";
                string xNomeTomador = toma?.Element(nf + "xNome")?.Value ?? "";

                string dataAtual = DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss");

                string registroH = $"H|{dataAtual}||{versaoDES}|{imTomadorGlobal}|{cnpjTomador}||{xNomeTomador}|{xNomeTomador}|||0|2|2|2|||2|2|null";

                // === Registro R ===
                string dhEmissao = infDPS?.Element(nf + "dhEmi")?.Value ?? "";
                string dataEmissao = "";
                if (DateTime.TryParse(dhEmissao, null, DateTimeStyles.AdjustToUniversal, out DateTime dt))
                    dataEmissao = dt.ToString("ddMMyyyy");


                string serie = "0";
                string numeroNF = doc.Descendants(nf + "nNFSe").FirstOrDefault()?.Value ?? "";
                string valorTotal = valorServ?.Element(nf + "vServ")?.Value ?? "";

                var tomaEnd = toma?.Descendants(nf + "endNac").FirstOrDefault();
                var prestEnd = emit?.Descendants(nf + "enderNac").FirstOrDefault();

                bool isMei = (opcaoSimples == "2");
                /*
                DES - 
                1 - Simples Nacional
                2 - Não Optante
                3 - MEI

                Portal Nacional - 
                1 - Não Optante
                2 - MEI
                3 - Simples Nacional
                */
                string opcao = opcaoSimples switch
                {
                    "3" => "1",
                    "2" => "3",
                    "1" => "2",
                    _ => "2"
                };
                string modelo = isMei ? "28" : "5";

                string situacaoResponsabilidade = "1";
                string codServ = serv?.Element(nf + "cTribNac")?.Value ?? "";
                foreach (string servico in construcao)
                {
                    situacaoResponsabilidade = (codServ == servico) ? "3" : "1";
                    break;
                }
                foreach (string servico in propaganda)
                {
                    situacaoResponsabilidade = (codServ == servico) ? "5" : "1";
                    break;
                }


                string ufEmitente = prestEnd?.Element(nf + "UF")?.Value ?? "";
                string codMunEmitente = prestEnd?.Element(nf + "cMun")?.Value ?? "";
                string localIncidencia = localIncid?.Value ?? "";
                string cep = prestEnd?.Element(nf + "CEP")?.Value ?? "";

                if (cep == "")
                    cep = FaltandoCEP($"{prestEnd?.Element(nf + "xLgr")?.Value ?? ""}, {prestEnd?.Element(nf + "nro")?.Value ?? ""} - {prestEnd?.Element(nf + "xBairro")?.Value ?? ""}");



                string aliquotaIss = trib?.Element(nf + "pAliq")?.Value ?? "0.00";
                bool isRetido = aliquotaIss != "0.00";
                string retencao = isRetido ? "1" : "2";

                string motivoNaoRetencao = "1";
                if (regime?.Element(nf + "regEspTrib")?.Value == "6")
                {
                    motivoNaoRetencao = "6";
                }
                else
                {
                    if (isMei)
                    {
                        motivoNaoRetencao = "14";
                    }
                    else if (!isMei && !isRetido)
                    {
                        motivoNaoRetencao = "1";
                        string semPrefixo = numeroNF.Substring(2);
                        numeroNF = "2025" + semPrefixo.TrimStart('0');
                    }
                    else if (isRetido)
                    {
                        motivoNaoRetencao = "16";
                        string semPrefixo = numeroNF.Substring(2);
                        numeroNF = "2025" + semPrefixo.TrimStart('0');
                    }
                    foreach (string cadastro in sociedadeCGC)
                    {
                        if (emit?.Element(nf + "CNPJ")?.Value == cadastro)
                        {
                            motivoNaoRetencao = "6";
                        }
                    }
                }


                var camposR = new List<string>
            {
                "R",
                dataEmissao,
                dataEmissao,
                modelo,
                serie,
                "",
                situacaoResponsabilidade,
                motivoNaoRetencao,
                localIncidencia,
                retencao,
                numeroNF,
                valorTotal,
                valorTotal,
                aliquotaIss,
                opcao,
                "",
                emit?.Element(nf + "CNPJ")?.Value ?? "",
                "",
                emit?.Element(nf + "xNome")?.Value ?? "",
                prestEnd?.Element(nf + "xLgr")?.Value ?? "",
                prestEnd?.Element(nf + "nro")?.Value ?? "",
                "",
                prestEnd?.Element(nf + "xBairro")?.Value ?? "",
                codMunEmitente,
                "1058",
                cep,
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                localIncidencia,
                localIncidencia,
                "1058",
                "",
                "",
                ""
            };

                string registroR = string.Join("|", camposR);
                CadastrarIM(xNomeTomador, cnpjTomador, imTomadorGlobal);

                var nota = new Relatorio
                {
                    tomador = xNomeTomador,
                    cnpjTomador = cnpjTomador,
                    numeroNota = numeroNF,
                    nomeEmitente = emit?.Element(nf + "xNome")?.Value ?? "",
                    estado = ufEmitente,
                    municipio = codMunEmitente,
                    valor = valorTotal,
                    aliquota = aliquotaIss
                };

                return (registroH, registroR, cnpjTomador, nota);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar o arquivo: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw new Exception($"Erro ao processar {xmlPath}: {ex.Message}");
            }
        }

        public class Relatorio
        {
            public string? tomador { get; set; }
            public string? cnpjTomador { get; set; }
            public string? numeroNota { get; set; }
            public string? nomeEmitente { get; set; }
            public string? estado { get; set; }
            public string? municipio { get; set; }
            public string? valor { get; set; }
            public string? aliquota { get; set; }
        }

        private void logButton_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log"));
        }

        //SP NFSE

        private void button2_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            nomeTomadorText.Text = "Empresa:";
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Selecione um arquivo .CSV";
                openFileDialog.Filter = "Arquivos CSV (*.csv)|*.csv";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string caminhoCSV = openFileDialog.FileName;
                    CSV_FOLDER = caminhoCSV;
                    textBox2.Text = caminhoCSV;
                }
            }
        }

        private void processButton2_Click(object sender, EventArgs e)
        {
            var registrosH = new Dictionary<string, string>();
            var registrosR = new Dictionary<string, string>();
            var listaRelatorio = new List<RelatorioSP>();
            var index = new List<int>();

            if (string.IsNullOrWhiteSpace(CSV_FOLDER) || !File.Exists(CSV_FOLDER))
            {
                MessageBox.Show("Selecione um arquivo CSV válido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!CSV_FOLDER.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("O arquivo selecionado não é um .csv.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {

                    var registros = GerarRegistrosSp();
                    foreach (var (h, r, cnpj, dados) in registros)
                    {
                        if (!registrosH.ContainsKey(cnpj))
                            registrosH[cnpj] = h;
                        registrosR.Add(dados.numeroNota ?? "CHAVE", r);
                        listaRelatorio.Add(dados);
                    }
                    foreach (string chave in chaveSubstituida)
                    {
                        //FAZER COM QUE DESCONSIDERE NOTA CANCELADA
                        if (chave != "")
                        {
                            if (registrosR.Keys.ToList().IndexOf(chave) >= 0)
                            {
                                index.Add(registrosR.Keys.ToList().IndexOf(chave));
                            }
                            if (chave != "" && registrosR.ContainsKey(chave))
                            {
                                MessageBox.Show($"Nota {chave} desconsiderada: Nota Substituída",
                                    "Nota Substituída", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                registrosR.Remove(chave);
                            }

                        }
                    }

                    foreach (int posicao in index)
                    {
                        listaRelatorio.RemoveAt(posicao);
                    }

                    string arquivoSaida = Path.Combine(Path.GetDirectoryName(CSV_FOLDER) ?? "", "1 - ARQUIVO PARA IMPORTAÇÃO - DES.txt");

                    string pastaLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                    Directory.CreateDirectory(pastaLog);
                    string relatorio = Path.Combine(pastaLog, $"relatorio_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                    using (StreamWriter sw = new StreamWriter(relatorio))
                    {
                        sw.WriteLine("- /// RELATÓRIO DE NOTAS PROCESSADAS /// -");
                        sw.WriteLine($"Tomador: {listaRelatorio.FirstOrDefault()?.tomador} | {listaRelatorio.FirstOrDefault()?.cnpjTomador} \n");
                        sw.WriteLine("Número da Nota | Nome do Emitente | Estado | Município | Valor | Base de Cálculo\n");
                        foreach (var item in listaRelatorio)
                        {
                            sw.WriteLine($"{item.numeroNota} | {item.nomeEmitente} | {item.estado} | {item.municipio} | {item.valor} | {item.baseCalc}");
                        }
                        sw.WriteLine($"\nTotal de Notas Processadas: {listaRelatorio.Count}");
                    }
                    File.WriteAllLines(arquivoSaida, registrosH.Values.Concat(registrosR.Values));

                    abrirPastaProcessada(CSV_FOLDER, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao processar o arquivo: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public List<(string registroH, string registroR, string cnpjTomador, RelatorioSP dados)> GerarRegistrosSp()
        {
            var resultados = new List<(string, string, string, RelatorioSP)>();
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var linhas = File.ReadAllLines(CSV_FOLDER, Encoding.GetEncoding("Windows-1252"));
                var linhasSemUltima = linhas.Take(linhas.Length - 1).ToArray();
                foreach (var linha in linhasSemUltima.Skip(1))
                {
                    var colunas = linha.Split(';');
                    //registro h
                    string dataAtual = DateTime.Now.ToString("dd/MM/yyyyHH:mm:ss");
                    string cnpjTomador = colunas[34].Replace(".", "").Replace("/", "").Replace("-", "");
                    string imTomadorBanco = "";

                    string nomeTomador = colunas[37].ToUpper();
                    bool imTomadorExiste = inscricoes.Values.Any(l =>
                    {
                        var coluna = l.Split(';');
                        if (coluna[1] == cnpjTomador)
                        {
                            imTomadorBanco = coluna[2];
                            textBox3.Text = coluna[2];
                            nomeTomadorText.Text = $"Empresa: {coluna[0]} | {coluna[1]}";
                            return true;
                        }
                        return false;
                    });


                    if (textBox3.Text.Length == 0 && !imTomadorExiste)
                        throw new Exception("Por favor, insira a inscrição municipal do tomador.");
                    else if (textBox3.Text.Length != 11 && !imTomadorExiste)
                        throw new Exception("Inscrição Municipal do tomador inválida.");

                    string imTomador = textBox3.Text.ToUpper();

                    if (imTomador != "" && imTomador != imTomadorBanco && imTomadorExiste)
                    {
                        throw new Exception("Inscrição Municipal não encontrada");
                    }

                    //registro r
                    string dataEmissão = colunas[2].Substring(0, colunas[2].Length - 9).Replace("/", "");

                    string codServ = colunas[28];

                    string situacao = "1";

                    if (codServ == "1015" || codServ == "1023")
                    {
                        situacao = "3";
                    }
                    else if (codServ == "2496")
                    {
                        situacao = "5";
                    }

                    string numeroNF = colunas[1];

                    string valorServ = colunas[26];
                    string novaBaseCalc = colunas[26];
                    if (valorServ == "0,00")
                    {
                        novaBaseCalc = "0";
                        valorServ = colunas[68];
                        situacao = "2";
                    }
                    valorServ = valorServ.Replace(".", "").Replace(",", ".");
                    novaBaseCalc = novaBaseCalc.Replace(".", "").Replace(",", ".");

                    string opcaoSimples = (colunas[21] != "0") ? "1" : "2";

                    string cnpjEmit = colunas[10].Replace(".", "").Replace("/", "").Replace("-", "");

                    string nomeEmit = colunas[11];

                    string logradouro = colunas[13];

                    string num = colunas[14];

                    string bairro = colunas[16];

                    string cep = colunas[19].Replace("-", "");

                    if (cep == "")
                        cep = FaltandoCEP($"{logradouro}, {num} - {bairro}");

                    var camposR = new List<string> {
                    "R",
                    dataEmissão,
                    dataEmissão,
                    "16",
                    "0",
                    "",
                    situacao,
                    "5",
                    "3550308",
                    "2",
                    numeroNF,
                    valorServ,
                    novaBaseCalc,
                    "0.00",
                    opcaoSimples,
                    "",
                    cnpjEmit,
                    "",
                    nomeEmit,
                    logradouro,
                    num,
                    "",
                    bairro,
                    "3550308",
                    "1058",
                    cep,
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "3550308",
                    "3550308",
                    "1058",
                    "",
                    "",
                    ""
                    };

                    string registroR = string.Join("|", camposR);
                    string registroH = $"H|{dataAtual}||{versaoDES}|{imTomador}|{cnpjTomador}||{nomeTomador}|{nomeTomador}|||0|2|2|2|||2|2|null";

                    CadastrarIM(nomeTomador, cnpjTomador, imTomador);

                    var nota = new RelatorioSP
                    {
                        tomador = nomeTomador,
                        cnpjTomador = cnpjTomador,
                        numeroNota = numeroNF,
                        nomeEmitente = nomeEmit,
                        estado = "São Paulo",
                        municipio = "3550308",
                        valor = valorServ,
                        baseCalc = novaBaseCalc,
                    };
                    bool situacaoCancelado = (colunas[22] == "C") ? true : false;
                    if (!situacaoCancelado)
                        resultados.Add((registroH, registroR, cnpjTomador, nota));
                }
                return resultados;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar {CSV_FOLDER}: {ex.Message}");
            }

        }
        public class RelatorioSP
        {
            public string? tomador { get; set; }
            public string? cnpjTomador { get; set; }
            public string? numeroNota { get; set; }
            public string? nomeEmitente { get; set; }
            public string? estado { get; set; }
            public string? municipio { get; set; }
            public string? valor { get; set; }
            public string? baseCalc { get; set; }
        }

        private void abrirPastaProcessada(string path, bool isArquivo)
        {
            try
            {
                var result = MessageBox.Show("Arquivo gerado com sucesso. Deseja acessar a pasta?", "Sucesso!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                string folder = isArquivo ? Path.GetDirectoryName(path) ?? "" : path;
                if (!string.IsNullOrWhiteSpace(path) && result == DialogResult.Yes)
                {
                    Process.Start("explorer.exe", folder);
                }
                else if (string.IsNullOrWhiteSpace(path))
                {
                    throw new Exception("Pasta Inválida");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível abrir a pasta selecionada: {ex.Message}");
            }
        }

        private void CadastrarIM(string nome, string cnpj, string im)
        {
            try
            {
                if (!inscricoes.ContainsKey(im))
                {
                    inscricoes.Add(im, $"{nome};{cnpj};{im}");

                    var item = new ListViewItem(nome);
                    item.SubItems.Add(TratarCNPJ(cnpj));
                    item.SubItems.Add(TratarIm(im));
                    inscricoesListView.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ERRO: {ex.Message}");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var linhas = File.ReadAllLines(pathBancoIMs, Encoding.UTF8).ToList();
            if (linhas.Count == 0)
            {
                File.AppendAllLines(pathBancoIMs, inscricoes.Values);
                return;
            }
            foreach (var im in inscricoes)
            {
                bool jaExiste = linhas.Any(l =>
                {
                    var coluna = l.Split(';');
                    return coluna.Length > 2 && coluna[2] == im.Key;
                });
                if (!jaExiste)
                {
                    File.AppendAllText(pathBancoIMs, im.Value + Environment.NewLine);
                }
            }
        }

        string TratarCNPJ(string cnpj) => Convert.ToUInt64(cnpj).ToString(@"00\.000\.000\/0000\-00");
        string TratarIm(string im) => $"{Convert.ToUInt64(im.Substring(0, 10)).ToString(@"0\.000\.000\/000\")}-{im.Substring(10, 1)}";
        string FaltandoCEP(string endereço) => Interaction.InputBox($"Não consta CEP no XML processado, gentileza inserir manualmente.\nEndereço: {endereço}", "CEP Faltando");

        private void manualButton_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Manual de Uso - Conversor XML – DES.pdf")) { UseShellExecute = true });
        }

    }
}

