const statusEl = document.getElementById('status');
const pingBtn = document.getElementById('ping');
const reconnectBtn = document.getElementById('reconnect');
const asInput = document.getElementById('as');
const meEl = document.getElementById('me');
const logEl = document.getElementById('log');
const rttEl = document.getElementById('rtt');
const canvas = document.getElementById('view');
const ctx = canvas.getContext('2d');

// --- world state, populated from server packets ---

let socket = null;
let lastPingClientTime = 0;
let myEntityId = null;
let camera = { x: 0, z: 0 }; // follows my position
let lastSnapshot = { tick: 0, lastSeq: 0, players: [] };
const PIXELS_PER_METER = 6;

// --- input state ---

const pressed = new Set();
let moveSequence = 0;
const MOVE_TICK_MS = 50; // matches server 20Hz
let moveTimer = null;

// --- log helpers ---

function log(kind, text) {
  const entry = document.createElement('div');
  entry.className = `entry ${kind}`;
  const ts = new Date().toISOString().slice(11, 23);
  entry.innerHTML = `<span class="ts">${ts}</span> <pre>${escapeHtml(text)}</pre>`;
  logEl.appendChild(entry);
  // Cap log size so a long session doesn't murder the DOM.
  while (logEl.childNodes.length > 200) logEl.removeChild(logEl.firstChild);
  logEl.scrollTop = logEl.scrollHeight;
}

function escapeHtml(s) {
  return String(s).replace(/[&<>]/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;' }[c]));
}

function setStatus(connected) {
  statusEl.textContent = connected ? 'connected' : 'disconnected';
  statusEl.className = `status ${connected ? 'connected' : 'disconnected'}`;
  pingBtn.disabled = !connected;
}

// --- networking ---

function connect() {
  if (socket && socket.readyState !== WebSocket.CLOSED) socket.close();
  const subject = asInput.value.trim() || 'player_1';
  const url = `ws://${location.host}/ws?as=${encodeURIComponent(subject)}`;
  log('info', `connecting as ${subject}`);
  myEntityId = null;
  meEl.textContent = '';
  socket = new WebSocket(url);

  socket.onopen = () => { setStatus(true); log('info', 'open'); };
  socket.onclose = (e) => {
    setStatus(false);
    log('info', `close code=${e.code} reason="${e.reason || ''}"`);
    stopMoveLoop();
  };
  socket.onerror = () => { log('error', 'socket error'); };
  socket.onmessage = (e) => handleMessage(e.data);
}

function send(packet) {
  if (!socket || socket.readyState !== WebSocket.OPEN) return;
  socket.send(JSON.stringify(packet));
}

function handleMessage(raw) {
  let env;
  try { env = JSON.parse(raw); }
  catch { log('error', `non-JSON message: ${raw}`); return; }

  switch (env.Type) {
    case 'pong': {
      const rtt = Date.now() - lastPingClientTime;
      rttEl.textContent = `rtt=${rtt}ms`;
      log('recv', `← pong ${JSON.stringify(env.Data)}`);
      break;
    }
    case 'welcome': {
      const d = env.Data;
      myEntityId = d.YourEntityId.Value;
      camera.x = d.SpawnPosition.X;
      camera.z = d.SpawnPosition.Z;
      meEl.textContent = `you are E${myEntityId} (${d.YourName}) on realm ${d.RealmId} (${d.RealmName})`;
      log('recv', `← welcome ${JSON.stringify(d)}`);
      startMoveLoop();
      break;
    }
    case 'world_snapshot': {
      lastSnapshot = {
        tick: env.Data.Tick,
        lastSeq: env.Data.LastProcessedSequence,
        players: env.Data.Players || [],
      };
      const me = lastSnapshot.players.find(p => p.Id.Value === myEntityId);
      if (me) { camera.x = me.Position.X; camera.z = me.Position.Z; }
      // Don't log every snapshot — 20/sec drowns the log.
      break;
    }
    default:
      log('recv', `← ${JSON.stringify(env)}`);
  }
}

// --- input ---

window.addEventListener('keydown', (e) => {
  // Don't capture WASD when typing into the input box.
  if (document.activeElement === asInput) return;
  const k = e.key.toLowerCase();
  if ('wasd'.includes(k)) { pressed.add(k); e.preventDefault(); }
});
window.addEventListener('keyup', (e) => {
  const k = e.key.toLowerCase();
  if ('wasd'.includes(k)) pressed.delete(k);
});
window.addEventListener('blur', () => pressed.clear());

function computeDirection() {
  let dx = 0, dz = 0;
  if (pressed.has('w')) dz -= 1;
  if (pressed.has('s')) dz += 1;
  if (pressed.has('a')) dx -= 1;
  if (pressed.has('d')) dx += 1;
  // Server normalizes; we could pre-normalize here but it doesn't matter.
  return { dx, dz };
}

function startMoveLoop() {
  stopMoveLoop();
  moveTimer = setInterval(() => {
    const { dx, dz } = computeDirection();
    // Send every tick whether moving or not — server clears DesiredMove
    // each tick, so we have to keep poking it to stay in motion. Zero
    // direction is a no-op that just keeps the sequence counter alive.
    moveSequence++;
    send(Packets.moveIntent(dx, dz, 0, moveSequence));
  }, MOVE_TICK_MS);
}

function stopMoveLoop() {
  if (moveTimer) { clearInterval(moveTimer); moveTimer = null; }
}

// --- rendering ---

function render() {
  const w = canvas.width, h = canvas.height;
  ctx.fillStyle = '#14181d';
  ctx.fillRect(0, 0, w, h);

  // 50m spatial grid lines, snapped to world cells.
  const cellPx = 50 * PIXELS_PER_METER;
  ctx.strokeStyle = '#1f262d';
  ctx.lineWidth = 1;
  const camPx = { x: w / 2 - camera.x * PIXELS_PER_METER, z: h / 2 - camera.z * PIXELS_PER_METER };
  const startX = camPx.x % cellPx;
  const startZ = camPx.z % cellPx;
  for (let x = startX; x < w; x += cellPx) {
    ctx.beginPath(); ctx.moveTo(x, 0); ctx.lineTo(x, h); ctx.stroke();
  }
  for (let z = startZ; z < h; z += cellPx) {
    ctx.beginPath(); ctx.moveTo(0, z); ctx.lineTo(w, z); ctx.stroke();
  }
  // World origin marker.
  const origin = worldToScreen(0, 0);
  ctx.strokeStyle = '#2a3a4d';
  ctx.beginPath(); ctx.arc(origin.x, origin.y, 4, 0, Math.PI * 2); ctx.stroke();

  // Entities.
  for (const p of lastSnapshot.players) {
    const isMe = p.Id.Value === myEntityId;
    const { x, y } = worldToScreen(p.Position.X, p.Position.Z);
    ctx.fillStyle = isMe ? '#7fe0e0' : '#e09b6f';
    ctx.beginPath(); ctx.arc(x, y, 6, 0, Math.PI * 2); ctx.fill();
    ctx.fillStyle = '#d4d4d4';
    ctx.font = '11px ui-monospace, Menlo, monospace';
    ctx.textAlign = 'center';
    ctx.fillText(p.Name, x, y - 10);
  }

  // HUD: tick + last processed seq.
  ctx.fillStyle = '#6b7280';
  ctx.font = '11px ui-monospace, Menlo, monospace';
  ctx.textAlign = 'left';
  ctx.fillText(`tick ${lastSnapshot.tick}  ack-seq ${lastSnapshot.lastSeq}  players ${lastSnapshot.players.length}`, 8, h - 8);

  requestAnimationFrame(render);
}

function worldToScreen(x, z) {
  return {
    x: canvas.width / 2 + (x - camera.x) * PIXELS_PER_METER,
    y: canvas.height / 2 + (z - camera.z) * PIXELS_PER_METER,
  };
}

// --- buttons ---

pingBtn.addEventListener('click', () => {
  lastPingClientTime = Date.now();
  send(Packets.ping(lastPingClientTime));
  log('send', `→ ping ${lastPingClientTime}`);
});

reconnectBtn.addEventListener('click', connect);

// --- boot ---

requestAnimationFrame(render);
connect();
