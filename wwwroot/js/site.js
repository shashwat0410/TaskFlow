// ============================================================
// TaskFlow — Main JavaScript
// ============================================================

// Auto-dismiss toast after animation
document.addEventListener('DOMContentLoaded', () => {
    const toast = document.getElementById('toast');
    if (toast) {
        setTimeout(() => toast.remove(), 3500);
    }
});

// ── Color Swatch Picker ──────────────────────────────────────
function initColorPicker() {
    const swatches = document.querySelectorAll('.color-swatch');
    const colorInput = document.getElementById('colorInput');
    if (!swatches.length || !colorInput) return;

    swatches.forEach(swatch => {
        swatch.addEventListener('click', () => {
            swatches.forEach(s => s.classList.remove('selected'));
            swatch.classList.add('selected');
            colorInput.value = swatch.dataset.color;
        });
        if (swatch.dataset.color === colorInput.value) {
            swatch.classList.add('selected');
        }
    });
}
document.addEventListener('DOMContentLoaded', initColorPicker);


// ── AJAX: Update Task Status from Kanban ─────────────────────
async function updateTaskStatus(taskId, status) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    try {
        const res = await fetch('/Tasks/UpdateStatus', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token || ''
            },
            body: JSON.stringify({ taskId, status })
        });

        if (!res.ok) throw new Error('Failed to update');
        const data = await res.json();
        showNotification(`Task moved to "${status}"`, 'success');
        return data;
    } catch (err) {
        showNotification('Failed to update task status', 'error');
        console.error(err);
    }
}

// ── Kanban Drag & Drop ───────────────────────────────────────
function initKanban() {
    const cards = document.querySelectorAll('.kanban-card[draggable="true"]');
    const columns = document.querySelectorAll('.kanban-tasks');

    let dragging = null;

    cards.forEach(card => {
        card.addEventListener('dragstart', e => {
            dragging = card;
            card.style.opacity = '0.5';
        });
        card.addEventListener('dragend', () => {
            card.style.opacity = '1';
            dragging = null;
        });
    });

    columns.forEach(col => {
        col.addEventListener('dragover', e => {
            e.preventDefault();
            col.style.background = 'rgba(99,102,241,0.05)';
        });
        col.addEventListener('dragleave', () => {
            col.style.background = '';
        });
        col.addEventListener('drop', async e => {
            e.preventDefault();
            col.style.background = '';
            if (!dragging) return;

            const newStatus = col.closest('.kanban-col').dataset.status;
            const taskId = parseInt(dragging.dataset.taskId);

            col.appendChild(dragging);

            // Update counts
            document.querySelectorAll('.kanban-col').forEach(c => {
                const count = c.querySelector('.kanban-tasks').children.length;
                c.querySelector('.kanban-count').textContent = count;
            });

            await updateTaskStatus(taskId, newStatus);
        });
    });
}
document.addEventListener('DOMContentLoaded', initKanban);


// ── Notification ─────────────────────────────────────────────
function showNotification(message, type = 'success') {
    const existing = document.getElementById('dynamic-toast');
    if (existing) existing.remove();

    const toast = document.createElement('div');
    toast.id = 'dynamic-toast';
    toast.className = `alert-toast ${type}`;
    toast.innerHTML = `<i class="bi bi-${type === 'success' ? 'check-circle-fill' : 'exclamation-circle-fill'}"></i> ${message}`;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 3500);
}


// ── Dashboard Charts ──────────────────────────────────────────
function initDashboardCharts(tasksByStatus, tasksByPriority) {
    // Status Doughnut
    const statusCtx = document.getElementById('statusChart');
    if (statusCtx && tasksByStatus) {
        new Chart(statusCtx, {
            type: 'doughnut',
            data: {
                labels: Object.keys(tasksByStatus),
                datasets: [{
                    data: Object.values(tasksByStatus),
                    backgroundColor: ['#e5e7eb', '#3b82f6', '#f59e0b', '#10b981'],
                    borderWidth: 0,
                    hoverOffset: 4
                }]
            },
            options: {
                responsive: true, maintainAspectRatio: false,
                cutout: '68%',
                plugins: {
                    legend: { position: 'bottom', labels: { padding: 12, font: { size: 12, family: "'Plus Jakarta Sans'" } } }
                }
            }
        });
    }

    // Priority Bar
    const priorityCtx = document.getElementById('priorityChart');
    if (priorityCtx && tasksByPriority) {
        new Chart(priorityCtx, {
            type: 'bar',
            data: {
                labels: Object.keys(tasksByPriority),
                datasets: [{
                    label: 'Tasks',
                    data: Object.values(tasksByPriority),
                    backgroundColor: ['#bbf7d0', '#fde68a', '#fed7aa', '#fecaca'],
                    borderRadius: 6,
                    borderSkipped: false
                }]
            },
            options: {
                responsive: true, maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    y: { beginAtZero: true, ticks: { stepSize: 1 }, grid: { color: '#f1f5f9' } },
                    x: { grid: { display: false } }
                }
            }
        });
    }
}
