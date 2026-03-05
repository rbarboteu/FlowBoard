const API = 'http://localhost:5291';

const token    = sessionStorage.getItem('token');
const userEmail = sessionStorage.getItem('email');
const userRole  = sessionStorage.getItem('role');

if (!token) window.location.href = 'index.html';

// Header
document.getElementById('userEmail').textContent = userEmail;
document.getElementById('userRole').textContent  = userRole;
document.getElementById('userAvatar').textContent = userEmail?.[0]?.toUpperCase() || 'U';

if (userRole === 'Admin') {
  const btn = document.getElementById('btnAddUser');
  if (btn) btn.style.display = 'block';
}

let projetoAtivo = null;
let projetoEditandoId = null;

// ── Helpers ───────────────────────────────────────────────────────────────
function headers() {
  return {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  };
}

async function api(method, path, body = null) {
  const res = await fetch(`${API}${path}`, {
    method,
    headers: headers(),
    body: body ? JSON.stringify(body) : null
  });
  if (res.status === 401) { sessionStorage.clear(); window.location.href = 'index.html'; return; }
  if (res.status === 204) return null;
  return res.json();
}

function sair() { sessionStorage.clear(); window.location.href = 'index.html'; }
function fecharModal(id) { document.getElementById(id).classList.remove('aberto'); }

// ── Projetos ──────────────────────────────────────────────────────────────
async function carregarProjetos() {
  const projetos = await api('GET', '/api/projects');
  const lista = document.getElementById('listaProjetos');

  if (!projetos || projetos.length === 0) {
    lista.innerHTML = '<div style="font-size:12px;color:#374151;text-align:center;padding:16px 0;">Nenhum projeto ainda</div>';
    return;
  }

  lista.innerHTML = projetos.map(p => `
    <div class="projeto-item ${projetoAtivo?.id === p.id ? 'ativo' : ''}"
         onclick="selecionarProjeto(${JSON.stringify(p).replace(/"/g, '&quot;')})">
      <div class="projeto-item-left">
        <div class="projeto-dot"></div>
        <div class="projeto-item-name">${p.name}</div>
      </div>
      <div class="projeto-item-actions" onclick="event.stopPropagation()">
        <button class="btn-icon-sm" onclick="abrirEdicaoProjeto('${p.id}','${p.name}','${p.description || ''}')">✏</button>
        <button class="btn-icon-sm del" onclick="excluirProjeto('${p.id}')">✕</button>
      </div>
    </div>
  `).join('');
}

function toggleFormProjeto() {
  const form = document.getElementById('formProjeto');
  form.style.display = form.style.display === 'none' ? 'block' : 'none';
  if (form.style.display === 'block') document.getElementById('nomeProj').focus();
}

async function criarProjeto() {
  const nome = document.getElementById('nomeProj').value.trim();
  const desc = document.getElementById('descProj').value.trim();
  if (!nome) return;
  await api('POST', '/api/projects', { name: nome, description: desc });
  document.getElementById('nomeProj').value = '';
  document.getElementById('descProj').value = '';
  document.getElementById('formProjeto').style.display = 'none';
  await carregarProjetos();
}

function abrirEdicaoProjeto(id, nome, desc) {
  projetoEditandoId = id;
  document.getElementById('editNomeProj').value = nome;
  document.getElementById('editDescProj').value = desc;
  document.getElementById('modalEditarProjeto').classList.add('aberto');
}

async function salvarEdicaoProjeto() {
  const nome = document.getElementById('editNomeProj').value.trim();
  const desc = document.getElementById('editDescProj').value.trim();
  if (!nome) return;
  await api('PUT', `/api/projects/${projetoEditandoId}`, { name: nome, description: desc });
  if (projetoAtivo?.id === projetoEditandoId) {
    projetoAtivo.name = nome;
    document.getElementById('topbarTitle').textContent = nome;
  }
  fecharModal('modalEditarProjeto');
  await carregarProjetos();
}

async function excluirProjeto(id) {
  if (!confirm('Excluir este projeto e todas as suas tarefas?')) return;
  await api('DELETE', `/api/projects/${id}`);
  if (projetoAtivo?.id === id) {
    projetoAtivo = null;
    document.getElementById('topbarTitle').textContent = 'Selecione um projeto';
    document.getElementById('topbarActions').style.display = 'none';
    document.getElementById('boardArea').innerHTML = `
      <div class="empty-state">
        <div class="empty-state-icon">📋</div>
        <div class="empty-state-text">Selecione um projeto na barra lateral</div>
      </div>`;
  }
  await carregarProjetos();
}

// ── Tarefas ───────────────────────────────────────────────────────────────
async function selecionarProjeto(projeto) {
  projetoAtivo = projeto;
  document.getElementById('topbarTitle').textContent = projeto.name;
  document.getElementById('topbarActions').style.display = 'flex';
  await carregarProjetos();
  await carregarTarefas();
}

async function carregarTarefas() {
  if (!projetoAtivo) return;
  const tarefas = await api('GET', `/api/tasks/project/${projetoAtivo.id}`);
  const board = document.getElementById('boardArea');

  const todo     = tarefas.filter(t => t.status === 'Todo');
  const progress = tarefas.filter(t => t.status === 'InProgress');
  const done     = tarefas.filter(t => t.status === 'Done');

  board.innerHTML = `
    <div id="formTarefa" class="form-new-task">
      <input type="text" id="nomeTarefa" placeholder="Descreva a tarefa..." />
      <div class="form-new-task-actions">
        <button onclick="toggleFormTarefa()" style="background:rgba(255,255,255,0.05);color:#6B7280;">Cancelar</button>
        <button onclick="criarTarefa()" style="background:linear-gradient(135deg,#6366F1,#8B5CF6);color:white;">Criar tarefa</button>
      </div>
    </div>

    <div class="kanban">
      <div class="kanban-col">
        <div class="kanban-col-head">
          <div class="kanban-col-title">
            <div class="col-dot col-dot-todo"></div>
            A Fazer
          </div>
          <span class="col-count">${todo.length}</span>
        </div>
        <div class="kanban-col-body">
          ${todo.length === 0 ? '<div class="col-vazio">Nenhuma tarefa</div>' : todo.map(cardTarefa).join('')}
        </div>
      </div>

      <div class="kanban-col">
        <div class="kanban-col-head">
          <div class="kanban-col-title">
            <div class="col-dot col-dot-progress"></div>
            Em Progresso
          </div>
          <span class="col-count">${progress.length}</span>
        </div>
        <div class="kanban-col-body">
          ${progress.length === 0 ? '<div class="col-vazio">Nenhuma tarefa</div>' : progress.map(cardTarefa).join('')}
        </div>
      </div>

      <div class="kanban-col">
        <div class="kanban-col-head">
          <div class="kanban-col-title">
            <div class="col-dot col-dot-done"></div>
            Concluído
          </div>
          <span class="col-count">${done.length}</span>
        </div>
        <div class="kanban-col-body">
          ${done.length === 0 ? '<div class="col-vazio">Nenhuma tarefa</div>' : done.map(cardTarefa).join('')}
        </div>
      </div>
    </div>
  `;
}

function cardTarefa(t) {
  const moves = {
    'Todo':       [['InProgress', '→ Iniciar']],
    'InProgress': [['Todo', '← Voltar'], ['Done', '✓ Concluir']],
    'Done':       [['InProgress', '↺ Reabrir']],
  };

  const botoes = (moves[t.status] || [])
    .map(([s, l]) => `<button class="btn-move" onclick="mudarStatus('${t.id}','${s}')">${l}</button>`)
    .join('');

  return `
    <div class="task-card">
      <div class="task-card-header">
        <div class="task-title">${t.title}</div>
        <button class="btn-del-task" onclick="excluirTarefa('${t.id}')">✕</button>
      </div>
      <div class="task-actions">${botoes}</div>
    </div>
  `;
}

function toggleFormTarefa() {
  const form = document.getElementById('formTarefa');
  if (!form) return;
  const isOpen = form.style.display === 'block';
  form.style.display = isOpen ? 'none' : 'block';
  if (!isOpen) document.getElementById('nomeTarefa').focus();
}

async function criarTarefa() {
  const titulo = document.getElementById('nomeTarefa').value.trim();
  if (!titulo) return;
  await api('POST', '/api/tasks', { projectId: projetoAtivo.id, title: titulo });
  document.getElementById('nomeTarefa').value = '';
  document.getElementById('formTarefa').style.display = 'none';
  await carregarTarefas();
}

async function mudarStatus(taskId, novoStatus) {
  await api('PATCH', `/api/tasks/${taskId}/status`, { status: novoStatus });
  await carregarTarefas();
}

async function excluirTarefa(id) {
  if (!confirm('Excluir esta tarefa?')) return;
  await api('DELETE', `/api/tasks/${id}`);
  await carregarTarefas();
}

// ── Usuários ──────────────────────────────────────────────────────────────
function abrirModalUsuario() {
  document.getElementById('sucessoUsuario').style.display = 'none';
  document.getElementById('erroUsuario').style.display = 'none';
  document.getElementById('novoEmail').value = '';
  document.getElementById('novaSenha').value = '';
  document.getElementById('modalUsuario').classList.add('aberto');
}

async function criarUsuario() {
  const email = document.getElementById('novoEmail').value.trim();
  const senha = document.getElementById('novaSenha').value;
  const role  = document.getElementById('novoRole').value;
  const erro  = document.getElementById('erroUsuario');
  const suc   = document.getElementById('sucessoUsuario');

  erro.style.display = 'none';
  suc.style.display = 'none';

  if (!email || !senha) {
    erro.textContent = 'Preencha todos os campos.';
    erro.style.display = 'block';
    return;
  }

  const res = await api('POST', '/api/auth/register-user', { email, password: senha, role });
  if (res?.error) { erro.textContent = res.error; erro.style.display = 'block'; return; }

  suc.style.display = 'block';
  document.getElementById('novoEmail').value = '';
  document.getElementById('novaSenha').value = '';
}

// ── Init ──────────────────────────────────────────────────────────────────
carregarProjetos();
