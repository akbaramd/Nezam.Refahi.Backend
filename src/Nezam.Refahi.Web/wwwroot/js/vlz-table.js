(function() {
    'use strict';

    // Initialize all VlzTable instances on the page
    function initVlzTables() {
        document.querySelectorAll('table.vlz-data-table').forEach(initTable);
    }

    // Initialize a single table instance
    function initTable(tbl) {
        if (!tbl || tbl._vlzInitialized) return;
        tbl._vlzInitialized = true;

        const tableId = tbl.id;
        const wrapper = tbl.closest('.table-responsive');
        // Allow either data-show-filter-row or data-show-filter
        const showFilter = (tbl.dataset.showFilterRow === 'true') || (tbl.dataset.showFilter === 'true');
        const formId = tbl.dataset.filterFormId;
        const form = formId ? document.getElementById(formId) : null;
        const enableSorting = tbl.dataset.enableSorting !== 'false';
        const enablePagination = tbl.dataset.enablePagination !== 'false';
        const pageSize = parseInt(tbl.dataset.pageSize || '10', 10);
        const ajaxUrl = tbl.dataset.ajaxUrl;
        const method = (tbl.dataset.method || 'GET').toUpperCase();

        console.log(`[VlzTable] initTable: id=${tableId}, showFilter=${showFilter}, sorting=${enableSorting}, pagination=${enablePagination}, pageSize=${pageSize}`);

        // Columns config
        const inputs = Array.from(wrapper.querySelectorAll('input.vlz-column-config'));
        const columns = inputs.map(inp => {
            const col = {};
            Object.entries(inp.dataset).forEach(([k, v]) => {
                if (k === 'parentId') return;
                if (v === 'true') col[k] = true;
                else if (v === 'false') col[k] = false;
                else if (!isNaN(v) && v.trim() !== '') col[k] = Number(v);
                else col[k] = v;
            });
            return col;
        }).sort((a, b) => (a.order || 0) - (b.order || 0));

        // Build table header
        const thead = document.createElement('thead');
        const headerRow = document.createElement('tr');
        const filterRow = document.createElement('tr');
        
        // Ensure rows take full width
        headerRow.style.width = '100%';
        filterRow.style.width = '100%';
        
        // Add a class to the table for full-width rows
        tbl.classList.add('vlz-full-width-rows');

        columns.forEach(c => {
            const th = document.createElement('th');
            th.dataset.colName = c.name;
            if (c.cssClass) th.className = c.cssClass;
            if (c.width) th.style.width = c.width;
            if (c.visible === false) th.style.display = 'none';
            if (c.templateId) th.setAttribute('data-template-id', c.templateId);
            if (c.tooltip) th.title = c.tooltip;
            if (c.sortable) th.setAttribute('data-sortable', 'true');
            if (c.dataType) th.setAttribute('data-type', c.dataType);
            if (c.fixed) th.setAttribute('data-fixed', 'true');
            th.textContent = c.title;
            headerRow.appendChild(th);

            const thf = document.createElement('th');
            if (showFilter && c.filterable) {
                const inp = document.createElement('input');
                inp.type = c.filterType === 'number' ? 'number' : 'text';
                inp.name = c.name;
                inp.className = 'form-control form-control-sm';
                inp.placeholder = 'جستجو…';
                
                // Set minimum width for filter inputs
                inp.style.minWidth = '80px';
                inp.style.width = '100%';
                
                // If the column has a width, apply it to the filter cell too
                if (c.width) {
                    thf.style.width = c.width;
                }
                
                thf.appendChild(inp);
            }
            filterRow.appendChild(thf);
        });

        thead.appendChild(headerRow);
        if (showFilter) thead.appendChild(filterRow);
        if (tbl.tHead) tbl.replaceChild(thead, tbl.tHead);
        else tbl.insertBefore(thead, tbl.firstChild);

        tbl.vlzColumns = columns;
        const body = tbl.tBodies[0];
        let sortState = { column: null, direction: 'asc' };
        let activeFilters = {};
        let currentPage = 1;
        let filterTimeout;
        const filterDelay = 300;

        // Render rows function
        function renderRows(items) {
            console.log(`[VlzTable] renderRows: items.length=${items.length}`);
            body.innerHTML = '';
            if (!items || !items.length) {
                body.insertAdjacentHTML('beforeend',
                    `<tr style="width:100%"><td colspan="${columns.length}" class="text-center py-4">موردی یافت نشد.</td></tr>`);
                return;
            }
            items.forEach((item, idx) => {
                const tr = document.createElement('tr');
                tr.dataset.rowIndex = idx;
                tr.dataset.rowData = JSON.stringify(item);
                tr.style.width = '100%'; // Ensure data rows take full width
                columns.forEach(c => {
                    if (c.visible === false) return;
                    const td = document.createElement('td');
                    td.dataset.colName = c.name;
                    if (c.width) td.style.width = c.width;
                    if (c.cssClass) td.className = c.cssClass;
                    let val = getValueCaseInsensitive(item, c.name) ?? c.defaultValue ?? '';
                    td.dataset.value = val;
                    // Check if column has a template
                    if (c.templateId || c.templateContent) {
                        let templateHtml = '';
                        
                        // First priority: Use template from DOM if templateId is provided
                        if (c.templateId) {
                            const template = document.getElementById(c.templateId);
                            if (template) {
                                templateHtml = template.innerHTML;
                            } else {
                                console.warn(`Template with ID ${c.templateId} not found`);
                            }
                        }
                        // Second priority: Use inline template content if provided
                        else if (c.templateContent) {
                            templateHtml = c.templateContent;
                        }
                        
                        // If we have a template, process it
                        if (templateHtml) {
                            // Replace all {{property}} placeholders with actual values
                            Object.keys(item).forEach(key => {
                                const regex = new RegExp(`\{\{${key}\}\}`, 'g');
                                const value = item[key] !== undefined && item[key] !== null ? item[key] : '';
                                templateHtml = templateHtml.replace(regex, value);
                            });
                            
                            // Add data attributes to the cell for potential event handling
                            Object.keys(item).forEach(key => {
                                td.dataset[`row${key.charAt(0).toUpperCase() + key.slice(1)}`] = item[key] || '';
                            });
                            
                            td.innerHTML = templateHtml;
                        } else {
                            td.textContent = val || '';
                        }
                    } 
                    // Otherwise format based on data type
                    else {
                        switch ((c.dataType || '').toLowerCase()) {
                            case 'number':
                                td.textContent = c.format && !isNaN(+val)
                                    ? (+val).toLocaleString() : val;
                                break;
                            case 'price':
                                if (!isNaN(+val)) {
                                    td.dataset.rawValue = val;
                                    td.textContent = (+val).toLocaleString();
                                } else td.textContent = val;
                                break;
                            case 'date':
                                const d = new Date(val);
                                td.textContent = isNaN(d) ? val : d.toLocaleDateString();
                                break;
                            default:
                                td.textContent = val;
                        }
                    }
                    tr.appendChild(td);
                });
                body.appendChild(tr);
            });
        }

        // Helper: case-insensitive key lookup
        function getValueCaseInsensitive(obj, key) {
            if (obj[key] !== undefined) return obj[key];
            const lowerKey = key.toLowerCase();
            for (const k in obj) if (k.toLowerCase() === lowerKey) return obj[k];
            return undefined;
        }

        // Load data via AJAX
        async function loadData(page = 1) {
            currentPage = page;
            console.log(`[VlzTable] loadData: page=${page}, sort=${sortState.column}:${sortState.direction}, filters=${JSON.stringify(activeFilters)}`);
            const params = new URLSearchParams();
            // Filters
            Object.entries(activeFilters).forEach(([k, v]) => { if (v) params.set(k, v); });
            if (form) new FormData(form).forEach((v, k) => { if (v && !params.has(k)) params.set(k, v); });
            // Sorting
            if (sortState.column) {
                params.set('sortColumn', sortState.column);
                params.set('sortDirection', sortState.direction);
            }
            // Pagination
            params.set('pageSize', pageSize);
            params.set('page', page);

            // Update URL
            const base = ajaxUrl.split('?')[0];
            const requestUrl = `${base}?${params.toString()}`;
            window.history.replaceState(null, '', requestUrl);

            // Loading indicator
            body.innerHTML = `<tr style="width:100%"><td colspan="${columns.length}" class="text-center py-4">در حال بارگذاری…</td></tr>`;

            try {
                const headers = { 'X-Requested-With': 'XMLHttpRequest' };
                if (method !== 'GET') headers['Content-Type'] = 'application/json';
                const rsp = await fetch(requestUrl, { method, headers });
                if (!rsp.ok) throw new Error(`HTTP ${rsp.status}`);
                const json = await rsp.json();
                const items = json.items || json.data || [];
                renderRows(items);
                if (enablePagination && json.totalPages > 1) updatePagination(json.currentPage || page, json.totalPages, json.totalItems);
            } catch (e) {
                console.error('[VlzTable] loadData error:', e);
                body.innerHTML = `<tr><td colspan="${columns.length}" class="text-center py-4 text-danger">خطا در بارگذاری داده‌ها.</td></tr>`;
            }
        }

        // Pagination controls
        function updatePagination(current, total, totalItems) {
            let container = document.getElementById(`${tableId}-pagination`);
            if (!container) {
                container = document.createElement('div');
                container.id = `${tableId}-pagination`;
                container.className = 'vlz-pagination d-flex justify-content-between align-items-center mt-3';
                tbl.parentNode.insertBefore(container, tbl.nextSibling);
            }
            const showFrom = Math.min((current - 1) * pageSize + 1, totalItems);
            const showTo = Math.min(current * pageSize, totalItems);
            let html = `<div class="pagination-info">نمایش ${showFrom} تا ${showTo} از ${totalItems} مورد</div><ul class="pagination mb-0">`;
            // Previous
            html += `<li class="page-item ${current === 1 ? 'disabled' : ''}"><a class="page-link" href="#" data-page="${current-1}">&laquo;</a></li>`;
            const maxPages = 5;
            let start = Math.max(1, current - Math.floor(maxPages/2));
            let end = Math.min(total, start + maxPages - 1);
            if (end - start + 1 < maxPages && start > 1) start = Math.max(1, end - maxPages + 1);
            if (start > 1) { html += `<li class="page-item"><a class="page-link" href="#" data-page="1">1</a></li>`; if (start > 2) html += `<li class="page-item disabled"><span class="page-link">...</span></li>`; }
            for (let i = start; i <= end; i++) html += `<li class="page-item ${i === current ? 'active' : ''}"><a class="page-link" href="#" data-page="${i}">${i}</a></li>`;
            if (end < total) { if (end < total-1) html += `<li class="page-item disabled"><span class="page-link">...</span></li>`; html += `<li class="page-item"><a class="page-link" href="#" data-page="${total}">${total}</a></li>`; }
            html += `<li class="page-item ${current === total ? 'disabled' : ''}"><a class="page-link" href="#" data-page="${current+1}">&raquo;</a></li></ul>`;
            container.innerHTML = html;
            container.querySelectorAll('.page-link').forEach(link => {
                link.addEventListener('click', e => {
                    e.preventDefault();
                    const p = parseInt(e.currentTarget.dataset.page, 10);
                    if (!isNaN(p) && p > 0 && p <= total) loadData(p);
                });
            });
        }

        // Sorting setup
        if (enableSorting) {
            tbl.querySelectorAll('th[data-sortable]').forEach(th => {
                th.classList.add('sortable');
                th.addEventListener('click', () => {
                    const col = th.dataset.colName;
                    sortState.direction = (sortState.column === col && sortState.direction === 'asc') ? 'desc' : 'asc';
                    sortState.column = col;
                    console.log(`[VlzTable] sorting: ${col} ${sortState.direction}`);
                    tbl.querySelectorAll('th.sort-asc, th.sort-desc').forEach(x => x.classList.remove('sort-asc','sort-desc'));
                    th.classList.add(`sort-${sortState.direction}`);
                    loadData(1);
                });
            });
        }

        // Filtering inputs
        if (showFilter) {
            const headerInputs = Array.from(tbl.querySelectorAll('thead input'));
            const formInputs = form ? Array.from(form.querySelectorAll('input, select')) : [];
            const allInputs = headerInputs.concat(formInputs);
            allInputs.forEach(input => {
                if (input.name && input.value) activeFilters[input.name] = input.value;
                input.addEventListener('input', () => {
                    clearTimeout(filterTimeout);
                    filterTimeout = setTimeout(() => {
                        if (input.name) {
                            if (input.value) activeFilters[input.name] = input.value;
                            else delete activeFilters[input.name];
                        }
                        console.log(`[VlzTable] filter change: ${input.name}=${input.value}`);
                        loadData(1);
                    }, filterDelay);
                });
            });
            if (form) {
                form.addEventListener('submit', e => { e.preventDefault(); loadData(1); });
            }
        }

        // Initial data load
        loadData(1);
    }

    // Export to global scope
    window.VlzTable = { init: initVlzTables, initTable };
    document.addEventListener('DOMContentLoaded', initVlzTables);
})();
