// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function applyFilter(query, tableId, columns) {
    query = query.toLowerCase();
    const rows = document.querySelectorAll(`#${tableId} tbody tr`);

    rows.forEach(row => {
        const cells = Array.from(row.cells);
        const text = columns.map(i => cells[i].innerText.toLowerCase()).join(" ");
        row.style.display = text.includes(query) ? "" : "none";
    });
}



// Generic multi-select color + text filter for any table
function initColorFilter({ wrapper, table, search }) {
    const root = document.querySelector(wrapper);
    if (!root) return;

    const selected = new Set();
    const tableEl = document.querySelector(table);
    const searchEl = search ? document.querySelector(search) : null;

    const refresh = () => {
        const q = (searchEl?.value || '').trim().toLowerCase();
        tableEl.querySelectorAll('tbody tr').forEach(tr => {
            const matchesText = !q || tr.innerText.toLowerCase().includes(q);
            const matchesStatus = selected.size === 0 || selected.has(tr.dataset.status);
            tr.style.display = (matchesText && matchesStatus) ? '' : 'none';
        });
    };

    root.querySelectorAll('[data-filter]').forEach(btn => {
        btn.addEventListener('click', () => {
            const s = btn.dataset.filter;
            if (selected.has(s)) { selected.delete(s); btn.setAttribute('aria-pressed', 'false'); }
            else { selected.add(s); btn.setAttribute('aria-pressed', 'true'); }
            refresh();
        });
    });

    if (searchEl) searchEl.addEventListener('keyup', refresh);

    // optional "Clear" button inside wrapper
    const clearBtn = root.querySelector('[data-clear]');
    if (clearBtn) clearBtn.addEventListener('click', () => {
        selected.clear();
        root.querySelectorAll('[data-filter]').forEach(b => b.setAttribute('aria-pressed', 'false'));
        if (searchEl) searchEl.value = '';
        refresh();
    });

    refresh();
}

// expose it globally for your views
window.initColorFilter = initColorFilter;

