/* ═══════════════════════════════════════════════════
   OilWatch — Application Logic
   ═══════════════════════════════════════════════════ */

'use strict';

// ── Oil Data ──────────────────────────────────────────────────────
const OIL_DATA = [
  {
    id: 'brent',
    name: 'Brent Crude',
    flag: '🌊',
    type: 'crude',
    price: 85.42,
    change: +1.23,
    changePct: +1.46,
    high52: 97.10,
    low52: 71.32,
    volume: 342000,
    api: 38.3,
    sulfur: '0.37%',
    origin: 'North Sea',
    exchange: 'ICE',
    currency: 'USD',
    desc: 'The global benchmark for Atlantic basin crude oil, produced from North Sea fields. Sets the reference price for approximately two-thirds of the world\'s internationally traded crude oil supplies.',
  },
  {
    id: 'wti',
    name: 'WTI Crude',
    flag: '🦅',
    type: 'crude',
    price: 81.17,
    change: +0.88,
    changePct: +1.09,
    high52: 93.67,
    low52: 67.80,
    volume: 410000,
    api: 39.6,
    sulfur: '0.24%',
    origin: 'Cushing, USA',
    exchange: 'NYMEX',
    currency: 'USD',
    desc: 'West Texas Intermediate — the primary benchmark for US-produced oil. Known for its light, sweet characteristics, making it easier and cheaper to refine into gasoline.',
  },
  {
    id: 'dubai',
    name: 'Dubai Crude',
    flag: '🕌',
    type: 'crude',
    price: 82.90,
    change: -0.45,
    changePct: -0.54,
    high52: 96.20,
    low52: 70.15,
    volume: 178000,
    api: 31.0,
    sulfur: '2.00%',
    origin: 'Gulf Emirates',
    exchange: 'DME',
    currency: 'USD',
    desc: 'Dubai/Oman crude serves as the primary benchmark for Middle Eastern oil sold to Asian markets. Heavier and more sour than Brent, requiring more complex refining.',
  },
  {
    id: 'opec',
    name: 'OPEC Basket',
    flag: '🛢️',
    type: 'regional',
    price: 83.58,
    change: +0.62,
    changePct: +0.75,
    high52: 95.50,
    low52: 69.22,
    volume: 290000,
    api: 32.7,
    sulfur: '1.77%',
    origin: 'OPEC Nations',
    exchange: 'OPEC',
    currency: 'USD',
    desc: 'A weighted average of prices for petroleum blends produced by OPEC member countries. Used as a reference point for OPEC\'s price-setting decisions and output agreements.',
  },
  {
    id: 'urals',
    name: 'Urals Blend',
    flag: '🏔️',
    type: 'regional',
    price: 64.30,
    change: -1.10,
    changePct: -1.68,
    high52: 78.40,
    low52: 55.90,
    volume: 195000,
    api: 31.7,
    sulfur: '1.35%',
    origin: 'Russia',
    exchange: 'OTC',
    currency: 'USD',
    desc: 'Russia\'s primary export crude, a blend of heavy sour Urals and lighter western Siberian grades. Trades at a discount to Brent, reflecting its higher sulfur content and geopolitical risk.',
  },
  {
    id: 'lng',
    name: 'Henry Hub LNG',
    flag: '❄️',
    type: 'futures',
    price: 3.21,
    change: +0.08,
    changePct: +2.56,
    high52: 4.80,
    low52: 1.90,
    volume: 525000,
    api: null,
    sulfur: null,
    origin: 'Louisiana, USA',
    exchange: 'NYMEX',
    currency: 'USD/MMBtu',
    desc: 'Henry Hub is the pricing point for natural gas futures. Named after a pipeline hub in Erath, Louisiana, it serves as the national benchmark price for the US natural gas market.',
  },
  {
    id: 'arab-light',
    name: 'Arab Light',
    flag: '🌅',
    type: 'crude',
    price: 84.10,
    change: +0.30,
    changePct: +0.36,
    high52: 96.40,
    low52: 70.50,
    volume: 156000,
    api: 32.8,
    sulfur: '1.77%',
    origin: 'Saudi Arabia',
    exchange: 'Aramco OSP',
    currency: 'USD',
    desc: 'Saudi Arabia\'s main export grade and the cornerstone of Saudi Aramco\'s production. The price is set monthly via Official Selling Prices, influencing Asian and European refinery margins significantly.',
  },
  {
    id: 'bonny',
    name: 'Bonny Light',
    flag: '🌍',
    type: 'regional',
    price: 88.15,
    change: +1.55,
    changePct: +1.79,
    high52: 99.00,
    low52: 73.20,
    volume: 112000,
    api: 35.4,
    sulfur: '0.14%',
    origin: 'Nigeria',
    exchange: 'OTC',
    currency: 'USD',
    desc: 'Nigeria\'s primary export crude, prized for its low sulfur content and high yield of gasoline and other light products. Benchmark for West African crude grades exported to European and Asian markets.',
  },
  {
    id: 'maya',
    name: 'Maya Crude',
    flag: '🦜',
    type: 'crude',
    price: 68.45,
    change: -0.88,
    changePct: -1.27,
    high52: 80.30,
    low52: 56.40,
    volume: 98000,
    api: 21.8,
    sulfur: '3.30%',
    origin: 'Mexico',
    exchange: 'Pemex OSP',
    currency: 'USD',
    desc: 'Mexico\'s flagship heavy crude, exported primarily to the US Gulf Coast and Asia. Its high sulfur and heavy gravity requires specialized upgrading refineries but is priced at a significant discount.',
  },
  {
    id: 'tapis',
    name: 'Tapis Crude',
    flag: '🌺',
    type: 'crude',
    price: 92.30,
    change: +2.10,
    changePct: +2.33,
    high52: 103.50,
    low52: 77.80,
    volume: 45000,
    api: 45.9,
    sulfur: '0.03%',
    origin: 'Malaysia',
    exchange: 'OTC',
    currency: 'USD',
    desc: 'The lightest and sweetest crude in Asia, produced offshore Malaysia. Tapis commands a premium over Brent due to its exceptional quality, yielding high volumes of jet fuel and gasoline.',
  },
  {
    id: 'murban',
    name: 'Murban Crude',
    flag: '🌟',
    type: 'futures',
    price: 86.72,
    change: +0.95,
    changePct: +1.11,
    high52: 98.20,
    low52: 72.60,
    volume: 88000,
    api: 38.7,
    sulfur: '0.70%',
    origin: 'Abu Dhabi',
    exchange: 'Intercontinental (ICE)',
    currency: 'USD',
    desc: 'ADNOC\'s flagship crude, now a freely-traded futures contract on ICE since 2021. Murban\'s FOB pricing and physical deliverability make it increasingly important as a Middle East benchmark.',
  },
  {
    id: 'es-sider',
    name: 'Es Sider',
    flag: '🏺',
    type: 'regional',
    price: 84.95,
    change: -0.22,
    changePct: -0.26,
    high52: 96.80,
    low52: 70.90,
    volume: 67000,
    api: 36.7,
    sulfur: '0.45%',
    origin: 'Libya',
    exchange: 'OTC',
    currency: 'USD',
    desc: 'Libya\'s most exported crude grade, named after the Es Sider oil terminal — one of the largest oil terminals in the world. Production levels subject to geopolitical disruptions.',
  },
];

// ── Utility functions ──────────────────────────────────────────────

function generateHistory(basePrice, days, volatility = 0.015) {
  const data = [];
  let price = basePrice * (1 - (days / 365) * 0.05 + Math.random() * 0.04);
  const now = Date.now();
  for (let i = days; i >= 0; i--) {
    const date = new Date(now - i * 86400000);
    const change = (Math.random() - 0.48) * volatility * price;
    price = Math.max(price + change, basePrice * 0.5);
    data.push({ date, price: +price.toFixed(2) });
  }
  // Ensure last point matches current price
  data[data.length - 1].price = basePrice;
  return data;
}

function generateVolume(baseVol, days) {
  return Array.from({ length: days + 1 }, (_, i) => ({
    day: i,
    vol: Math.floor(baseVol * (0.7 + Math.random() * 0.6))
  }));
}

function fmt(n, decimals = 2) {
  return n.toLocaleString('en-US', {
    minimumFractionDigits: decimals,
    maximumFractionDigits: decimals
  });
}

function fmtPrice(oil) {
  return `$${fmt(oil.price)}`;
}

function changeClass(v) { return v >= 0 ? 'up' : 'down'; }
function changeArrow(v) { return v >= 0 ? '▲' : '▼'; }

// ── State ─────────────────────────────────────────────────────────
let selectedOilId = 'brent';
let selectedRange  = 30;
let activeFilter   = 'all';
let priceChart, volumeChart, volatilityChart;

// Simulate live price updates
function jitter(oil) {
  const delta = (Math.random() - 0.49) * 0.12;
  oil.price      = +(oil.price + delta).toFixed(2);
  oil.change     = +(oil.change + delta * 0.1).toFixed(2);
  oil.changePct  = +(oil.change / (oil.price - oil.change) * 100).toFixed(2);
}

// ── Clock ─────────────────────────────────────────────────────────
function startClock() {
  const el = document.getElementById('live-clock');
  function tick() {
    const now = new Date();
    el.textContent = now.toLocaleTimeString('en-US', { hour12: false });
  }
  tick();
  setInterval(tick, 1000);
}

// ── Tab Navigation ────────────────────────────────────────────────
function initTabs() {
  document.querySelectorAll('.tab-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
      document.querySelectorAll('.tab-panel').forEach(p => p.classList.remove('active'));
      btn.classList.add('active');
      document.getElementById(`tab-${btn.dataset.tab}`).classList.add('active');
      if (btn.dataset.tab === 'charts') {
        setTimeout(() => updateCharts(), 50);
      }
    });
  });
}

// ── Filter Pills ──────────────────────────────────────────────────
function initFilters() {
  document.querySelectorAll('.filter-pill').forEach(pill => {
    pill.addEventListener('click', () => {
      document.querySelectorAll('.filter-pill').forEach(p => p.classList.remove('active'));
      pill.classList.add('active');
      activeFilter = pill.dataset.filter;
      renderCards();
    });
  });
}

// ── Ticker ────────────────────────────────────────────────────────
function renderTicker() {
  const track = document.getElementById('ticker-track');
  const items = [...OIL_DATA, ...OIL_DATA]; // duplicate for seamless loop
  track.innerHTML = items.map(o => `
    <span class="ticker-item">
      <span class="ticker-name">${o.name}</span>
      <span class="ticker-price">$${fmt(o.price)}</span>
      <span class="ticker-${changeClass(o.changePct)}">${changeArrow(o.changePct)} ${Math.abs(o.changePct).toFixed(2)}%</span>
    </span>
  `).join('');
}

// ── Sparkline ─────────────────────────────────────────────────────
function drawSparkline(canvas, oil) {
  const data = generateHistory(oil.price, 14, 0.012);
  const prices = data.map(d => d.price);
  const min = Math.min(...prices);
  const max = Math.max(...prices);
  const W = canvas.width  = canvas.offsetWidth  || 220;
  const H = canvas.height = 36;
  const ctx = canvas.getContext('2d');
  ctx.clearRect(0, 0, W, H);

  const color = oil.changePct >= 0 ? '#2ecc8a' : '#e84b4b';
  const pts = prices.map((p, i) => ({
    x: (i / (prices.length - 1)) * W,
    y: H - ((p - min) / (max - min || 1)) * (H - 4) - 2
  }));

  // Fill
  const grad = ctx.createLinearGradient(0, 0, 0, H);
  grad.addColorStop(0, color + '30');
  grad.addColorStop(1, color + '00');
  ctx.beginPath();
  ctx.moveTo(pts[0].x, H);
  pts.forEach(p => ctx.lineTo(p.x, p.y));
  ctx.lineTo(pts[pts.length - 1].x, H);
  ctx.fillStyle = grad;
  ctx.fill();

  // Line
  ctx.beginPath();
  ctx.moveTo(pts[0].x, pts[0].y);
  pts.forEach(p => ctx.lineTo(p.x, p.y));
  ctx.strokeStyle = color;
  ctx.lineWidth = 1.5;
  ctx.stroke();
}

// ── Cards ─────────────────────────────────────────────────────────
function renderCards() {
  const grid = document.getElementById('cards-grid');
  const filtered = activeFilter === 'all'
    ? OIL_DATA
    : OIL_DATA.filter(o => o.type === activeFilter);

  grid.innerHTML = filtered.map(oil => `
    <div class="price-card" data-id="${oil.id}">
      <div class="card-flag">${oil.flag}</div>
      <div class="card-top">
        <span class="card-name">${oil.name}</span>
        <span class="card-type-badge">${oil.type.toUpperCase()}</span>
      </div>
      <div class="card-price-row">
        <span class="card-price">$${fmt(oil.price)}</span>
        <span class="card-unit">/ bbl</span>
      </div>
      <span class="card-change ${changeClass(oil.changePct)}">
        ${changeArrow(oil.changePct)} ${Math.abs(oil.changePct).toFixed(2)}% &nbsp;${changeArrow(oil.changePct)} $${Math.abs(oil.change).toFixed(2)}
      </span>
      <div class="card-meta">
        <span>${oil.exchange}</span>
        <span>${oil.origin}</span>
      </div>
      <canvas class="card-sparkline" data-id="${oil.id}"></canvas>
    </div>
  `).join('');

  grid.querySelectorAll('.price-card').forEach(card => {
    const oil = OIL_DATA.find(o => o.id === card.dataset.id);
    const canvas = card.querySelector('.card-sparkline');
    setTimeout(() => drawSparkline(canvas, oil), 0);
    card.addEventListener('click', () => openModal(oil));
  });
}

// ── Modal ─────────────────────────────────────────────────────────
function openModal(oil) {
  const content = document.getElementById('modal-content');
  content.innerHTML = `
    <div class="modal-flag">${oil.flag}</div>
    <div class="modal-oil-name">${oil.name}</div>
    <div class="modal-price">$${fmt(oil.price)}</div>
    <span class="card-change ${changeClass(oil.changePct)}" style="margin-bottom:16px;display:inline-flex">
      ${changeArrow(oil.changePct)} ${Math.abs(oil.changePct).toFixed(2)}%&nbsp;&nbsp;${changeArrow(oil.changePct)} $${Math.abs(oil.change).toFixed(2)} today
    </span>
    <p class="modal-desc">${oil.desc}</p>
    <div class="modal-grid">
      <div class="modal-stat">
        <div class="modal-stat-label">52W High</div>
        <div class="modal-stat-value">$${fmt(oil.high52)}</div>
      </div>
      <div class="modal-stat">
        <div class="modal-stat-label">52W Low</div>
        <div class="modal-stat-value">$${fmt(oil.low52)}</div>
      </div>
      <div class="modal-stat">
        <div class="modal-stat-label">Volume (kbd)</div>
        <div class="modal-stat-value">${(oil.volume / 1000).toFixed(0)}k</div>
      </div>
      <div class="modal-stat">
        <div class="modal-stat-label">Exchange</div>
        <div class="modal-stat-value">${oil.exchange}</div>
      </div>
      ${oil.api ? `
      <div class="modal-stat">
        <div class="modal-stat-label">API Gravity</div>
        <div class="modal-stat-value">${oil.api}°</div>
      </div>
      <div class="modal-stat">
        <div class="modal-stat-label">Sulfur Content</div>
        <div class="modal-stat-value">${oil.sulfur}</div>
      </div>` : ''}
      <div class="modal-stat" style="grid-column:1/-1">
        <div class="modal-stat-label">Origin</div>
        <div class="modal-stat-value">${oil.origin}</div>
      </div>
    </div>
  `;
  document.getElementById('modal-overlay').classList.add('open');
}

function initModal() {
  document.getElementById('modal-close').addEventListener('click', closeModal);
  document.getElementById('modal-overlay').addEventListener('click', e => {
    if (e.target === document.getElementById('modal-overlay')) closeModal();
  });
}

function closeModal() {
  document.getElementById('modal-overlay').classList.remove('open');
}

// ── Oil Selector (Charts Tab) ─────────────────────────────────────
function renderOilSelector() {
  const container = document.getElementById('oil-selector');
  container.innerHTML = OIL_DATA.map(oil => `
    <div class="oil-opt ${oil.id === selectedOilId ? 'active' : ''}" data-id="${oil.id}">
      <span class="oil-opt-name">${oil.flag} ${oil.name}</span>
      <span class="oil-opt-chg ${changeClass(oil.changePct)}">${changeArrow(oil.changePct)}${Math.abs(oil.changePct).toFixed(1)}%</span>
    </div>
  `).join('');

  container.querySelectorAll('.oil-opt').forEach(opt => {
    opt.addEventListener('click', () => {
      selectedOilId = opt.dataset.id;
      renderOilSelector();
      updateCharts();
    });
  });
}

// ── Charts ─────────────────────────────────────────────────────────
function updateCharts() {
  const oil = OIL_DATA.find(o => o.id === selectedOilId);
  if (!oil) return;

  // Header
  document.getElementById('chart-oil-name').textContent = oil.name;
  document.getElementById('chart-oil-desc').textContent  = oil.desc.slice(0, 100) + '…';
  document.getElementById('chart-current-price').textContent = `$${fmt(oil.price)}`;
  const badge = document.getElementById('chart-change-badge');
  badge.textContent  = `${changeArrow(oil.changePct)} ${Math.abs(oil.changePct).toFixed(2)}%  ${changeArrow(oil.change)} $${Math.abs(oil.change).toFixed(2)}`;
  badge.className    = `chart-change-badge ${changeClass(oil.changePct)}`;

  const history = generateHistory(oil.price, selectedRange);
  const labels  = history.map(d => d.date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }));
  const prices  = history.map(d => d.price);
  const volData = generateVolume(oil.volume, selectedRange);
  const vols    = volData.map(d => +(d.vol / 1000).toFixed(1));

  const gridColor  = 'rgba(42,47,62,0.8)';
  const tickColor  = '#5a6278';
  const tooltipBg  = '#1a1e28';
  const isUp       = oil.changePct >= 0;
  const lineColor  = isUp ? '#2ecc8a' : '#e84b4b';

  // ── Price Chart ──────────────────────────────────────────────
  const priceCtx = document.getElementById('price-chart').getContext('2d');
  const grad = priceCtx.createLinearGradient(0, 0, 0, 300);
  grad.addColorStop(0, lineColor + '40');
  grad.addColorStop(1, lineColor + '00');

  if (priceChart) priceChart.destroy();
  priceChart = new Chart(priceCtx, {
    type: 'line',
    data: {
      labels,
      datasets: [{
        label: oil.name,
        data: prices,
        borderColor: lineColor,
        backgroundColor: grad,
        borderWidth: 2,
        pointRadius: 0,
        pointHoverRadius: 5,
        tension: 0.3,
        fill: true,
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: true,
      aspectRatio: 3,
      interaction: { intersect: false, mode: 'index' },
      plugins: {
        legend: { display: false },
        tooltip: {
          backgroundColor: tooltipBg,
          borderColor: '#363d52',
          borderWidth: 1,
          titleColor: '#8892a4',
          bodyColor: '#f0f2f8',
          padding: 10,
          callbacks: {
            label: ctx => ` $${fmt(ctx.raw)}`
          }
        }
      },
      scales: {
        x: {
          grid: { color: gridColor },
          ticks: {
            color: tickColor,
            font: { family: 'DM Mono', size: 10 },
            maxTicksLimit: 8,
          }
        },
        y: {
          grid: { color: gridColor },
          ticks: {
            color: tickColor,
            font: { family: 'DM Mono', size: 10 },
            callback: v => `$${v.toFixed(0)}`
          }
        }
      }
    }
  });

  // ── Volume Chart ─────────────────────────────────────────────
  const volCtx = document.getElementById('volume-chart').getContext('2d');
  if (volumeChart) volumeChart.destroy();
  volumeChart = new Chart(volCtx, {
    type: 'bar',
    data: {
      labels,
      datasets: [{
        label: 'Volume (kbd)',
        data: vols,
        backgroundColor: 'rgba(201,168,76,0.3)',
        borderColor: '#c9a84c',
        borderWidth: 1,
        borderRadius: 2,
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: true,
      aspectRatio: 2.5,
      plugins: { legend: { display: false }, tooltip: {
        backgroundColor: tooltipBg, borderColor: '#363d52', borderWidth: 1,
        titleColor: '#8892a4', bodyColor: '#f0f2f8',
      }},
      scales: {
        x: { grid: { display: false }, ticks: { display: false } },
        y: {
          grid: { color: gridColor },
          ticks: { color: tickColor, font: { family: 'DM Mono', size: 10 }, callback: v => `${v}k` }
        }
      }
    }
  });

  // ── Volatility Chart ─────────────────────────────────────────
  const volatilityData = prices.map((p, i) => {
    if (i === 0) return 0;
    return +Math.abs((p - prices[i - 1]) / prices[i - 1] * 100).toFixed(3);
  });
  const volGrad = priceCtx.createLinearGradient(0, 0, 0, 150);
  volGrad.addColorStop(0, '#4b8ae840');
  volGrad.addColorStop(1, '#4b8ae800');

  const vixCtx = document.getElementById('volatility-chart').getContext('2d');
  if (volatilityChart) volatilityChart.destroy();
  volatilityChart = new Chart(vixCtx, {
    type: 'line',
    data: {
      labels,
      datasets: [{
        label: 'Volatility %',
        data: volatilityData,
        borderColor: '#4b8ae8',
        backgroundColor: volGrad,
        borderWidth: 1.5,
        pointRadius: 0,
        tension: 0.4,
        fill: true,
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: true,
      aspectRatio: 2.5,
      plugins: { legend: { display: false }, tooltip: {
        backgroundColor: tooltipBg, borderColor: '#363d52', borderWidth: 1,
        titleColor: '#8892a4', bodyColor: '#f0f2f8',
        callbacks: { label: ctx => ` ${ctx.raw}%` }
      }},
      scales: {
        x: { grid: { display: false }, ticks: { display: false } },
        y: {
          grid: { color: gridColor },
          ticks: { color: tickColor, font: { family: 'DM Mono', size: 10 }, callback: v => `${v}%` }
        }
      }
    }
  });

  // ── Stats Panel ───────────────────────────────────────────────
  const avg = prices.reduce((a, b) => a + b, 0) / prices.length;
  const hi  = Math.max(...prices);
  const lo  = Math.min(...prices);
  const avgVol = vols.reduce((a, b) => a + b, 0) / vols.length;

  document.getElementById('stats-panel').innerHTML = `
    <div class="stat-row">
      <span class="stat-label">Period Avg</span>
      <span class="stat-value">$${fmt(avg)}</span>
    </div>
    <div class="stat-row">
      <span class="stat-label">Period High</span>
      <span class="stat-value" style="color:var(--green)">$${fmt(hi)}</span>
    </div>
    <div class="stat-row">
      <span class="stat-label">Period Low</span>
      <span class="stat-value" style="color:var(--red)">$${fmt(lo)}</span>
    </div>
    <div class="stat-row">
      <span class="stat-label">Avg Volume</span>
      <span class="stat-value">${fmt(avgVol, 0)}k kbd</span>
    </div>
    <div class="stat-row">
      <span class="stat-label">52W High</span>
      <span class="stat-value">$${fmt(oil.high52)}</span>
    </div>
    <div class="stat-row">
      <span class="stat-label">52W Low</span>
      <span class="stat-value">$${fmt(oil.low52)}</span>
    </div>
  `;

  // ── Info Table ────────────────────────────────────────────────
  const rows = [
    ['Origin',    oil.origin],
    ['Exchange',  oil.exchange],
    ['Currency',  oil.currency],
    ...(oil.api    ? [['API Gravity', `${oil.api}°`]] : []),
    ...(oil.sulfur ? [['Sulfur Content', oil.sulfur]]   : []),
    ['Grade Type', oil.type.toUpperCase()],
    ['Last Price', `$${fmt(oil.price)}`],
    ['Change', `${changeArrow(oil.change)} $${Math.abs(oil.change).toFixed(2)} (${changeArrow(oil.changePct)}${Math.abs(oil.changePct).toFixed(2)}%)`],
  ];

  document.querySelector('#info-table tbody').innerHTML = rows.map(([label, val]) => `
    <tr><td>${label}</td><td>${val}</td></tr>
  `).join('');
}

// ── Range Buttons ──────────────────────────────────────────────────
function initRangeBtns() {
  document.querySelectorAll('.range-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('.range-btn').forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      selectedRange = +btn.dataset.range;
      updateCharts();
    });
  });
}

// ── Live Updates ──────────────────────────────────────────────────
function startLiveUpdates() {
  setInterval(() => {
    OIL_DATA.forEach(oil => jitter(oil));
    renderTicker();
    renderCards();
    renderOilSelector();
    // Update chart header price only (not full chart redraw)
    const oil = OIL_DATA.find(o => o.id === selectedOilId);
    if (oil) {
      document.getElementById('chart-current-price').textContent = `$${fmt(oil.price)}`;
      const badge = document.getElementById('chart-change-badge');
      badge.textContent = `${changeArrow(oil.changePct)} ${Math.abs(oil.changePct).toFixed(2)}%  ${changeArrow(oil.change)} $${Math.abs(oil.change).toFixed(2)}`;
      badge.className   = `chart-change-badge ${changeClass(oil.changePct)}`;
    }
  }, 4000);
}

// ── Boot ──────────────────────────────────────────────────────────
function init() {
  startClock();
  initTabs();
  initFilters();
  initModal();
  initRangeBtns();
  renderTicker();
  renderCards();
  renderOilSelector();
  updateCharts();
  startLiveUpdates();
}

document.addEventListener('DOMContentLoaded', init);
