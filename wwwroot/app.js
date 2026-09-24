const API = "/api/smartphones";

const lista = document.getElementById("lista");
const log = document.getElementById("log");
const formCadastro = document.getElementById("form-cadastro");
const errosCadastro = document.getElementById("erros-cadastro");
const template = document.getElementById("tpl-celular");

function registrar(mensagem, falha = false) {
  const item = document.createElement("li");
  item.textContent = mensagem;
  if (falha) item.classList.add("falha");
  log.prepend(item);
}

// Junta as mensagens de erro que o backend devolve
async function lerErro(resposta) {
  try {
    const corpo = await resposta.json();
    if (corpo.errors) return Object.values(corpo.errors).flat().join("\n");
    if (corpo.mensagem) return corpo.mensagem;
  } catch { /* resposta sem corpo */ }
  return resposta.status === 404 ? "Celular não encontrado." : `Erro ${resposta.status}.`;
}

function montarCelular(celular) {
  const card = template.content.firstElementChild.cloneNode(true);
  card.dataset.id = celular.id;
  card.dataset.marca = celular.marca;
  card.querySelector(".marca").textContent = celular.marca;
  card.querySelector(".modelo").textContent = celular.modelo;
  card.querySelector(".numero").textContent = celular.numero;
  card.querySelector(".imei").textContent = celular.imei;
  card.querySelector(".memoria").textContent = `${celular.memoria} GB`;

  const apps = card.querySelector(".apps");
  if (celular.aplicativos.length === 0) {
    const nenhum = document.createElement("li");
    nenhum.className = "nenhum";
    nenhum.textContent = "Nenhum instalado";
    apps.append(nenhum);
  }
  for (const nome of celular.aplicativos) {
    const li = document.createElement("li");
    li.textContent = nome;
    apps.append(li);
  }
  return card;
}

async function carregar() {
  const resposta = await fetch(API);
  const celulares = await resposta.json();
  lista.replaceChildren(...celulares.map(montarCelular));
  if (celulares.length === 0) {
    const vazio = document.createElement("p");
    vazio.className = "vazio";
    vazio.textContent = "Nenhum celular cadastrado.";
    lista.append(vazio);
  }
}

formCadastro.addEventListener("submit", async (evento) => {
  evento.preventDefault();
  errosCadastro.textContent = "";
  const dados = Object.fromEntries(new FormData(formCadastro));
  dados.memoria = Number(dados.memoria);

  const resposta = await fetch(API, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(dados),
  });

  if (!resposta.ok) {
    errosCadastro.textContent = await lerErro(resposta);
    return;
  }

  const celular = await resposta.json();
  registrar(`${celular.marca} ${celular.modelo} cadastrado.`);
  formCadastro.reset();
  await carregar();
});

lista.addEventListener("click", async (evento) => {
  const botao = evento.target.closest("button[data-acao]");
  if (!botao) return;
  const card = botao.closest(".celular");
  const id = card.dataset.id;
  const acao = botao.dataset.acao;

  if (acao === "remover") {
    const modelo = card.querySelector(".modelo").textContent;
    if (!confirm(`Remover o ${modelo}?`)) return;
    const resposta = await fetch(`${API}/${id}`, { method: "DELETE" });
    if (resposta.ok) registrar(`${modelo} removido.`);
    else registrar(await lerErro(resposta), true);
    await carregar();
    return;
  }

  const resposta = await fetch(`${API}/${id}/${acao}`, { method: "POST" });
  if (resposta.ok) registrar((await resposta.json()).mensagem);
  else registrar(await lerErro(resposta), true);
});

lista.addEventListener("submit", async (evento) => {
  evento.preventDefault();
  const form = evento.target;
  const id = form.closest(".celular").dataset.id;
  const nome = form.elements.nome.value;

  const resposta = await fetch(`${API}/${id}/aplicativos`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ nome }),
  });

  if (!resposta.ok) {
    registrar(await lerErro(resposta), true);
    return;
  }

  registrar((await resposta.json()).mensagem);
  await carregar();
});

// Avisa o programa que esta aba está aberta (evita abrir abas repetidas)
new EventSource("/api/presenca");

carregar().catch(() => registrar("Não foi possível conectar ao servidor.", true));
