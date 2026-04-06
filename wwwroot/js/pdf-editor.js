const state = {
  pdfDoc: null,
  currentPage: 1,
  zoom: 1,
  fabric: null,
  docUrl: null
};

const pdfCanvas = document.getElementById('pdfCanvas');
const overlayCanvas = document.getElementById('editorOverlay');
const ctx = pdfCanvas?.getContext('2d');

const uploadInput = document.getElementById('pdfUpload');
const zoomLabel = document.getElementById('zoomLabel');
const pageLabel = document.getElementById('pageLabel');
const thumbnails = document.getElementById('thumbnails');
const insights = document.getElementById('docInsights');

async function loadPdfFromUrl(url) {
  const { getDocument } = globalThis.pdfjsLib || {};
  if (!getDocument) return;

  state.docUrl = url;
  state.pdfDoc = await getDocument(url).promise;
  state.currentPage = 1;
  await renderPage();
  await buildThumbnails();
}

async function renderPage() {
  if (!state.pdfDoc || !ctx) return;
  const page = await state.pdfDoc.getPage(state.currentPage);
  const viewport = page.getViewport({ scale: state.zoom });

  pdfCanvas.height = viewport.height;
  pdfCanvas.width = viewport.width;
  overlayCanvas.height = viewport.height;
  overlayCanvas.width = viewport.width;

  await page.render({ canvasContext: ctx, viewport }).promise;

  if (!state.fabric) {
    state.fabric = new fabric.Canvas('editorOverlay', {
      isDrawingMode: false,
      selection: true
    });
  } else {
    state.fabric.setHeight(viewport.height);
    state.fabric.setWidth(viewport.width);
  }

  zoomLabel.textContent = `${Math.round(state.zoom * 100)}%`;
  pageLabel.textContent = `Page ${state.currentPage}/${state.pdfDoc.numPages}`;
}

async function buildThumbnails() {
  if (!state.pdfDoc || !thumbnails) return;
  thumbnails.innerHTML = '';

  for (let i = 1; i <= state.pdfDoc.numPages; i++) {
    const div = document.createElement('div');
    div.className = 'thumbnail';
    div.textContent = `Page ${i}`;
    div.addEventListener('click', async () => {
      state.currentPage = i;
      await renderPage();
    });
    thumbnails.appendChild(div);
  }
}

async function uploadPdf(file) {
  const form = new FormData();
  form.append('file', file);

  const res = await fetch('/api/pdf/upload', { method: 'POST', body: form });
  if (!res.ok) {
    alert('Upload failed');
    return;
  }

  const payload = await res.json();
  await loadPdfFromUrl(payload.url);
}

function initToolbar() {
  document.getElementById('zoomIn')?.addEventListener('click', async () => {
    state.zoom = Math.min(3, state.zoom + 0.1);
    await renderPage();
  });

  document.getElementById('zoomOut')?.addEventListener('click', async () => {
    state.zoom = Math.max(0.5, state.zoom - 0.1);
    await renderPage();
  });

  document.getElementById('prevPage')?.addEventListener('click', async () => {
    if (!state.pdfDoc || state.currentPage <= 1) return;
    state.currentPage -= 1;
    await renderPage();
  });

  document.getElementById('nextPage')?.addEventListener('click', async () => {
    if (!state.pdfDoc || state.currentPage >= state.pdfDoc.numPages) return;
    state.currentPage += 1;
    await renderPage();
  });

  document.getElementById('themeToggle')?.addEventListener('click', () => {
    const html = document.documentElement;
    const current = html.getAttribute('data-theme');
    html.setAttribute('data-theme', current === 'dark' ? 'light' : 'dark');
  });

  uploadInput?.addEventListener('change', async (e) => {
    const file = e.target.files?.[0];
    if (file) await uploadPdf(file);
  });

  document.querySelectorAll('[data-element]').forEach(btn => {
    btn.addEventListener('click', () => addElement(btn.getAttribute('data-element')));
  });

  document.getElementById('aiSummaryBtn')?.addEventListener('click', async () => {
    const text = await extractText();
    insights.textContent = summarizeText(text);
  });

  document.getElementById('docSearch')?.addEventListener('change', async (e) => {
    const text = await extractText();
    const q = e.target.value?.toLowerCase() ?? '';
    const found = text.toLowerCase().includes(q);
    insights.textContent = found ? `Match found for: ${q}` : `No match for: ${q}`;
  });
}

function addElement(type) {
  if (!state.fabric) return;

  if (type === 'text' || type === 'field-text') {
    state.fabric.add(new fabric.IText(type === 'text' ? 'Edit me' : 'Text field', {
      left: 60, top: 60, fontSize: 18, fill: '#111827'
    }));
  } else if (type === 'shape') {
    state.fabric.add(new fabric.Rect({
      left: 80, top: 80, width: 120, height: 80, fill: 'rgba(37,99,235,0.25)', stroke: '#2563eb'
    }));
  } else if (type === 'signature') {
    state.fabric.add(new fabric.IText('Signed: Your Name', {
      left: 100, top: 120, fontFamily: 'cursive', fontSize: 24, fill: '#111827'
    }));
  } else {
    state.fabric.add(new fabric.IText(`[${type}]`, { left: 100, top: 100, fontSize: 14 }));
  }
}

async function extractText() {
  if (!uploadInput?.files?.[0]) return 'No document loaded.';

  const form = new FormData();
  form.append('file', uploadInput.files[0]);
  const res = await fetch('/api/pdf/extract-text', { method: 'POST', body: form });
  const payload = await res.json();
  return payload.text || '';
}

function summarizeText(text) {
  if (!text || text.length < 20) return 'Not enough text to summarize.';
  const first = text.split(/\s+/).slice(0, 30).join(' ');
  return `AI Summary (placeholder): ${first}...`;
}

initToolbar();
