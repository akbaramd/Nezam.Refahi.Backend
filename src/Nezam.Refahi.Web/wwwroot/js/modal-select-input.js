/* ---------------------------------------------------------------------------
   Modal-Select · Simplified JS · 2025-01-27
   Uses Bootstrap defaults with minimal custom styling
--------------------------------------------------------------------------- */
(function ($) {
    'use strict';

    /* debounce helper */
    const debounce = (fn, ms = 300) => { 
        let t; 
        return function () { 
            clearTimeout(t); 
            t = setTimeout(() => fn.apply(this, arguments), ms); 
        }; 
    };

    /* ======================== CLASS ====================================== */
    function ModalSelect($wrap) {

        /* ---------- data + helpers ----------------------------------------- */
        const ds = $wrap.data();
        const fieldName = ds.fieldName || 'RecipientIds';
        
        const addQS = url => {
            const qs = ($wrap.attr('data-extra-filters') || '').trim();
            return qs ? url + (url.includes('?') ? '&' : '?') + qs : url;
        };

        /* ---------- key nodes --------------------------------------------- */
        const $display = $('#' + ds.inputId + '_display');
        const $holder = $('#' + ds.inputId + '_holder');
        const $modal = $wrap.find('#' + ds.inputId + '_modal');
        const $body = $modal.find('.modal-body');
        const $commit = $modal.find('.modal-select-commit');
        const $search = $body.find('.search-box');
        const $sBtn = $body.find('.search-btn');
        const $tbl = $body.find('.tbl-body');
        const $info = $body.find('.tbl-info');
        const $pager = $body.find('.pager');
        const $showSelectedBtn = $modal.find('.show-selected-btn');

        /* ---------- component state --------------------------------------- */
        const st = { 
            page: 1, 
            size: parseInt(ds.pageSize) || 10, 
            total: 0, 
            term: '', 
            rows: [], 
            sel: [], 
            xhr: null,
            showSelectedFirst: ds.showSelectedFirst === 'true',
            allowDeselect: ds.allowDeselect === 'true',
            minSearchLength: parseInt(ds.minSearchLength) || 2,
            showingSelectedOnly: false,
            allSelectedItems: []
        };

        /* ---------- restore selections (edit page) ------------------------ */
        (function restore() {
            try {
                const src = ds.selectedItems;
                let ids = [];
                
                if (src) {
                    if (Array.isArray(src)) {
                        ids = src;
                    } else if (typeof src === 'string') {
                        if (src.trim().startsWith('[')) {
                            ids = JSON.parse(src);
                        } else {
                            ids = src.split(',').filter(Boolean).map(id => id.trim());
                        }
                    }
                }
                
                if (ids.length > 0) {
                    fetchByIds(ids);
                }
            } catch (error) {
                console.error('Error restoring selections:', error);
            }
        })();

        /* ---------- events ------------------------------------------------ */
        $sBtn.on('click', () => {
            const searchTerm = $.trim($search.val());
            if (searchTerm.length >= st.minSearchLength || searchTerm.length === 0) {
                load(1, searchTerm);
            }
        });
        
        // Debounced search
        const debouncedSearch = debounce(() => {
            const searchTerm = $.trim($search.val());
            if (searchTerm.length >= st.minSearchLength || searchTerm.length === 0) {
                load(1, searchTerm);
            }
        }, 500);
        
        $search.on('input', debouncedSearch);
        $search.on('keydown', e => { 
            if (e.key === 'Enter') { 
                e.preventDefault(); 
                e.stopImmediatePropagation(); 
                $sBtn.click(); 
            }
        });
        
        $commit.on('click', commit);
        $modal.on('shown.bs.modal', () => { 
            load(1); 
            $search.focus();
        });

        // Show selected items button
        $showSelectedBtn.on('click', toggleShowSelected);

        /* ---------- ajax -------------------------------------------------- */
        function load(page, term = st.term) {
            if (st.showingSelectedOnly) {
                showSelectedItemsOnly();
                return;
            }

            st.page = page; 
            st.term = term;
            st.xhr?.abort(); 
            renderLoading();

            const params = { 
                page, 
                pageSize: st.size, 
                search: term 
            };

            st.xhr = $.getJSON(addQS(ds.apiUrl), params)
                .done(res => { 
                    st.rows = res.items || []; 
                    st.total = res.totalItems || st.rows.length;
                    
                    // Always sort selected items to top
                    sortSelectedItemsToTop();
                    
                    renderTable(); 
                    renderPager(res.totalPages || 1); 
                })
                .fail((xhr, status, error) => { 
                    if (status !== 'abort') {
                        console.error('API Error:', error);
                        renderError('خطا در بارگذاری اطلاعات: ' + (xhr.responseJSON?.message || error));
                    }
                });
        }

        function fetchByIds(ids) {
            if (!ids || ids.length === 0) return;
            
            $.getJSON(addQS(ds.apiUrl.replace(/\/[^/]+$/, '/byIds')), { ids })
                .done(r => { 
                    st.sel = r.items || []; 
                    st.allSelectedItems = [...st.sel]; // Keep a copy of all selected items
                    updateDisplay();
                    // Update table if it's already rendered
                    if (st.rows.length > 0) {
                        sortSelectedItemsToTop();
                        renderTable();
                    }
                })
                .fail((xhr, status, error) => {
                    console.error('Error fetching selected items:', error);
                });
        }

        /* ---------- show selected functionality --------------------------- */
        function toggleShowSelected() {
            if (st.showingSelectedOnly) {
                // Switch back to normal view
                st.showingSelectedOnly = false;
                $showSelectedBtn.html('<i class="ri-checkbox-multiple-line"></i> نمایش انتخاب شده‌ها');
                $showSelectedBtn.removeClass('btn-primary').addClass('btn-outline-primary');
                load(1, st.term);
            } else {
                // Show only selected items
                showSelectedItemsOnly();
            }
        }

        function showSelectedItemsOnly() {
            if (st.sel.length === 0) {
                renderError('هیچ موردی انتخاب نشده است');
                $info.text('هیچ موردی انتخاب نشده است');
                $pager.empty();
                return;
            }

            st.showingSelectedOnly = true;
            $showSelectedBtn.html('<i class="ri-list-check"></i> نمایش همه');
            $showSelectedBtn.removeClass('btn-outline-primary').addClass('btn-primary');

            // Display all selected items
            st.rows = [...st.sel];
            st.total = st.sel.length;
            
            renderTable();
            $info.text(`نمایش ${st.sel.length} مورد انتخاب شده`);
            $pager.empty(); // No pagination for selected items view
        }

        /* ---------- sorting functions ------------------------------------ */
        function sortSelectedItemsToTop() {
            if (st.sel.length === 0) return;
            
            // Sort rows: selected items first, then unselected
            st.rows.sort((a, b) => {
                const aSelected = st.sel.some(s => s.id == a.id);
                const bSelected = st.sel.some(s => s.id == b.id);
                
                if (aSelected && !bSelected) return -1;
                if (!aSelected && bSelected) return 1;
                return 0;
            });
        }

        /* ---------- renderers -------------------------------------------- */
        const renderLoading = () => $tbl.html('<tr><td colspan="99" class="text-center py-4"><div class="spinner-border"></div><div class="mt-2">در حال بارگذاری...</div></td></tr>');
        const renderError = msg => $tbl.html(`<tr><td colspan="99" class="text-center py-4 text-danger"><i class="ri-error-warning-line fs-2"></i><div class="mt-2">${msg}</div></td></tr>`);

        function renderTable() {
            if (!st.rows.length) {
                return renderError('موردی یافت نشد');
            }
            
            $tbl.html(st.rows.map(r => {
                const isSelected = st.sel.some(x => x.id == r.id);
                const rowClass = isSelected ? 'table-success' : '';
                const checkboxDisabled = !st.allowDeselect && isSelected ? 'disabled' : '';
                
                return `
                <tr class="${rowClass}" data-id="${r.id}">
                  <td class="text-center">
                    <div class="form-check d-flex justify-content-center">
                      <input type="checkbox" class="form-check-input chk" 
                             data-id="${r.id}" data-text="${r.text}"
                             ${isSelected ? 'checked' : ''} ${checkboxDisabled}>
                    </div>
                  </td>
                  <td>${r.text}</td>
                  <td>${r.description || ''}</td>
                </tr>`;
            }).join(''));
            
            $tbl.find('.chk').on('change', function () {
                const item = { id: $(this).data('id'), text: $(this).data('text') };
                toggle(item, this.checked);
            });
            
            if (!st.showingSelectedOnly) {
                const start = (st.page - 1) * st.size + 1;
                const end = start + st.rows.length - 1;
                $info.text(`نمایش ${start} تا ${end} از ${st.total} مورد`);
            }
        }

        function updateTableRowStyles() {
            // Update row styling based on current selections
            $tbl.find('tr').each(function() {
                const $row = $(this);
                const rowId = $row.data('id');
                const isSelected = st.sel.some(s => s.id == rowId);
                
                if (isSelected) {
                    $row.addClass('table-success');
                    $row.find('.chk').prop('checked', true);
                } else {
                    $row.removeClass('table-success');
                    $row.find('.chk').prop('checked', false);
                }
            });
        }

        function renderPager(pgs) {
            st.totalPages = pgs; 
            $pager.empty(); 
            if (pgs <= 1) return;
            
            const li = (t, p, d, a) => `<li class="page-item ${d ? 'disabled' : ''} ${a ? 'active' : ''}">
                <a class="page-link" href="#" data-pg="${p}">${t}</a></li>`;
            
            $pager.append(li('«', st.page - 1, st.page === 1, 0));
            
            for (let p = Math.max(1, st.page - 2); p <= Math.min(pgs, st.page + 2); p++) {
                $pager.append(li(p, p, 0, p === st.page));
            }
            
            $pager.append(li('»', st.page + 1, st.page === pgs, 0));
            $pager.find('a').on('click', e => { 
                e.preventDefault(); 
                load(+$(e.target).data('pg')); 
            });
        }

        /* ---------- selection management --------------------------------- */
        const toggle = (it, on) => {
            if (on) {
                if (ds.selectionMode === 'single') {
                    st.sel = [it];
                } else if (!st.sel.some(x => x.id == it.id)) {
                    st.sel.push(it);
                }
            } else {
                st.sel = st.sel.filter(x => x.id != it.id);
            }
            
            // Update allSelectedItems array
            st.allSelectedItems = [...st.sel];
            
            // If showing selected only and item was deselected, remove from view
            if (st.showingSelectedOnly && !on) {
                st.rows = st.rows.filter(r => r.id != it.id);
                st.total = st.rows.length;
                if (st.rows.length === 0) {
                    renderError('هیچ موردی انتخاب نشده است');
                    $info.text('هیچ موردی انتخاب نشده است');
                } else {
                    renderTable();
                    $info.text(`نمایش ${st.rows.length} مورد انتخاب شده`);
                }
            } else {
                // Sort selected items to top and re-render table
                sortSelectedItemsToTop();
                renderTable();
            }
            
            updateDisplay();
        };

        function commit() {
            $holder.empty();
            st.sel.forEach(s =>
                $('<input>', { type: 'hidden', name: fieldName, value: s.id })
                    .appendTo($holder));
            updateDisplay(); 
            bootstrap.Modal.getInstance($modal[0]).hide();
        }

        const updateDisplay = () => {
            const displayText = st.sel.length > 0 
                ? st.sel.map(x => x.text).join(', ') 
                : '';
            $display.val(displayText);
            
            // Update button text to show count
            const $btn = $('#' + ds.inputId + '_button');
            const btnText = st.sel.length > 0 
                ? `${ds.buttonText || 'انتخاب'} (${st.sel.length})`
                : (ds.buttonText || 'انتخاب');
            $btn.html(`<i class="ri-search-line"></i> ${btnText}`);
        };
    }

    /* autoboot */
    $(document).ready(() => {
        $('.modal-select-container').each(function () { 
            new ModalSelect($(this)); 
        });
    });

})(jQuery);
