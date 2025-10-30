class Ajaximum {
    constructor($, config = {}) {
        this.$ = $;
        this.config = {
            onRenderToast: config.onRenderToast || (() => {}),
            onRenderOffCanvas: config.onRenderOffCanvas || (() => {}),
            onRenderModal: config.onRenderModal || (() => {}),
            onRenderGrid: config.onRenderGrid || (() => {})
        };
        this.grids = {};
        this.options = {};
        this.eventManager = new EventManager($, this);
        this.viewManager = new ViewManager($, this);
        this.requestManager = new RequestManager($, this);
        this.responseHandler = new ResponseHandler($, this);

        this.init();
    }

    init() {
        console.log('Start initialize Ajaximum ');
        this.eventManager.setupEventListeners();
        this.viewManager.setupViews();
    }

    collectGridIds() {
        return Object.keys(this.grids);
    }

    fetchOptionsFromResponse(xhr) {
        const optionsHeader = xhr.getResponseHeader('Ajaximum-Options');
        if (optionsHeader) {
            this.options = JSON.parse(optionsHeader);
        }
    }

    validate(form) {
        const validationInfo = this.$(form).data('ajaximumValidation');
        return !validationInfo || !validationInfo.validate || validationInfo.validate();
    }

    getFunction(code, argNames) {
        let fn = window;
        const parts = (code || '').split('.');
        while (fn && parts.length) {
            fn = fn[parts.shift()];
        }
        if (typeof fn === 'function') {
            return fn;
        }
        argNames.push(code);
        return Function.constructor.apply(null, argNames);
    }
}

class EventManager {
    constructor($, ajaximum) {
        this.$ = $;
        this.ajaximum = ajaximum;
    }

    setupEventListeners() {
        this.$(document).on('click', 'a[data-ajaximum=true]', (evt) => this.handleLinkClick(evt));
        this.$(document).on('click', 'form[data-ajaximum=true] input[type=image]', (evt) => this.handleImageInputClick(evt));
        this.$(document).on('click', 'form[data-ajaximum=true] :submit', (evt) => this.handleSubmitButton(evt));
        this.$(document).on('submit', 'form[data-ajaximum=true]', (evt) => this.handleFormSubmit(evt));
    }

    handleLinkClick(evt) {
        evt.preventDefault();
        const options = {
            url: evt.currentTarget.getAttribute('data-ajaximum-url') || evt.currentTarget.href,
            type: 'GET',
            data: [],
        };
        this.ajaximum.requestManager.asyncRequest(evt.currentTarget, options);
    }

    handleImageInputClick(evt) {
        const name = evt.target.name,
            target = this.$(evt.target),
            form = this.$(target.parents('form')[0]),
            offset = target.offset();

        form.data('ajaximumAjaxClick', [
            { name: name + '.x', value: Math.round(evt.pageX - offset.left) },
            { name: name + '.y', value: Math.round(evt.pageY - offset.top) },
        ]);

        setTimeout(() => form.removeData('ajaximumAjaxClick'), 0);
    }

    handleSubmitButton(evt) {
        const name = evt.currentTarget.name,
            target = this.$(evt.target),
            form = this.$(target.parents('form')[0]);

        form.data('ajaximumAjaxClick', name ? [{ name: name, value: evt.currentTarget.value }] : []);
        form.data('ajaximumAjaxClickTarget', target);

        setTimeout(() => {
            form.removeData('ajaximumAjaxClick');
            form.removeData('ajaximumAjaxClickTarget');
        }, 0);
    }

    handleFormSubmit(evt) {
        const clickInfo = this.$(evt.currentTarget).data('ajaximumAjaxClick') || [],
            clickTarget = this.$(evt.currentTarget).data('ajaximumAjaxClickTarget'),
            isCancel = clickTarget && (clickTarget.hasClass('cancel') || clickTarget.attr('formnovalidate') !== undefined);
        evt.preventDefault();
        if (!isCancel && !this.ajaximum.validate(evt.currentTarget)) return;

        const options = {
            url: evt.currentTarget.action,
            type: evt.currentTarget.method || 'GET',
            data: clickInfo.concat(this.$(evt.currentTarget).serializeArray()),
        };
        this.ajaximum.requestManager.asyncRequest(evt.currentTarget, options);
    }
}

class ViewManager {
    constructor($, ajaximum) {
        this.$ = $;
        this.ajaximum = ajaximum;
    }

    setupViews() {
        // Ensure grid initialization after HTML response
        this.initGrids();
        const selector = 'article[data-ajaximum=true], div[data-ajaximum=true], section[data-ajaximum=true]';
        this.$(selector).each((index, element) => {
            const options = {
                url: element.getAttribute('data-ajaximum-url') || undefined,
                type: element.getAttribute('data-ajaximum-method') || 'GET',
                data: [],
            };
            this.ajaximum.requestManager.asyncRequest(element, options);
        });
    }

    initGrids() {
        const gridSelector = '.ajaximum-grid';
        this.$(gridSelector).each((index, element) => {
            this.initGrid(element);
        });
    }
    
    initGrid(element) {
        const gridId = element.getAttribute('id');
        const dataSourceUrl = element.getAttribute('data-ajaximum-datasource-url');

        // Check if the gridId and dataSourceUrl are present and if the grid has not already been initialized
        if (gridId && dataSourceUrl && !this.ajaximum.grids[gridId]) {
            const columns = window.AjaximumGrids[gridId];

            // Initialize the DataTable
            const table = this.$(`#${gridId}-table`).DataTable({
                processing: true,
                serverSide: true,
                ajax: {
                    url: dataSourceUrl,
                    type: 'POST'
                },
                columns: columns
            });

            // Store the table instance and dataSourceUrl to indicate it has been initialized
            this.ajaximum.grids[gridId] = {
                table: table,
                dataSourceUrl: dataSourceUrl
            };
        } else {
            console.log(`Grid with ID ${gridId} is already initialized.`);
        }
    }


    refreshGrids(gridIds) {
        gridIds.forEach(gridId => {
            if (this.ajaximum.grids[gridId]) {
                const gridConfig = this.ajaximum.grids[gridId];
                gridConfig.table.ajax.url(gridConfig.dataSourceUrl).load();
            }
        });
    }
}

class RequestManager {
    constructor($, ajaximum) {
        this.$ = $;
        this.ajaximum = ajaximum;
    }

    asyncRequest(element, options) {
        const confirm = element.getAttribute('data-ajaximum-confirm');
        if (confirm && !window.confirm(confirm)) return;

        const loading = this.$(element.getAttribute('data-ajaximum-loading-target'));
        const duration = parseInt(element.getAttribute('data-ajaximum-loading-duration'), 10) || 0;

        this.$.extend(options, {
            type: element.getAttribute('data-ajaximum-method') || undefined,
            url: element.getAttribute('data-ajaximum-url') || undefined,
            cache: (element.getAttribute('data-ajaximum-cache') || '').toLowerCase() === 'true',
            headers: {
                'Ajaximum': true,
                'Ajaximum-Update-Type': element.getAttribute('data-ajaximum-update-type') || 'Html',
                'Ajaximum-Correlation-Id': element.getAttribute('data-ajaximum-correlation-id') || undefined,
                'Ajaximum-Update-Target': element.getAttribute('data-ajaximum-update-target'),
                'Ajaximum-Grid-Ids': this.ajaximum.collectGridIds().join(',')
            },
            beforeSend: (xhr) => {
                this.asyncOnBeforeSend(xhr, options.type);
                const result = this.ajaximum.getFunction(element.getAttribute('data-ajaximum-begin'), ['xhr']).apply(element, arguments);
                loading.show(duration);
                return result !== false ? result : false;
            },
            complete: () => {
                loading.hide(duration);
                this.ajaximum.getFunction(element.getAttribute('data-ajaximum-complete'), ['xhr', 'status']).apply(element, arguments);
            },
            success: (data, status, xhr) => {
                this.ajaximum.fetchOptionsFromResponse(xhr);
                this.ajaximum.responseHandler.handleSuccess(element, data, xhr);
                this.ajaximum.getFunction(element.getAttribute('data-ajaximum-success'), ['data', 'status', 'xhr']).apply(element, arguments);
            },
            error: () => {
                this.ajaximum.getFunction(element.getAttribute('data-ajaximum-failure'), ['xhr', 'status', 'error']).apply(element, arguments);
            },
        });

        options.data.push({ name: 'X-Requested-With', value: 'XMLHttpRequest' });

        if (!this.isMethodProxySafe(options.type.toUpperCase())) {
            options.type = 'POST';
            options.data.push({ name: 'X-HTTP-Method-Override', value: options.type });
        }

        const $element = this.$(element);
        if ($element.is('form') && $element.attr('enctype') === 'multipart/form-data') {
            const formData = new FormData();
            options.data.forEach(v => formData.append(v.name, v.value));
            this.$('input[type=file]', $element).each(function () {
                Array.from(this.files).forEach(file => formData.append(this.name, file));
            });
            this.$.extend(options, {
                processData: false,
                contentType: false,
                data: formData,
            });
        }

        this.$.ajax(options);
    }

    asyncOnBeforeSend(xhr, method) {
        if (!this.isMethodProxySafe(method)) {
            xhr.setRequestHeader('X-HTTP-Method-Override', method);
        }
    }

    isMethodProxySafe(method) {
        return method === 'GET' || method === 'POST';
    }
}

class ResponseHandler {
    constructor($, ajaximum) {
        this.$ = $;
        this.ajaximum = ajaximum;
    }

    handleSuccess(element, data, xhr) {
        this.ajaximum.fetchOptionsFromResponse(xhr);
        const correlationId = xhr.getResponseHeader('Ajaximum-Correlation-Id');
        if (correlationId) {
            data.forEach(item => {
                switch (item.ResponseType) {
                    case 'AjaximumHtmlResponse':
                        this.handleHtmlResponse(element, item, xhr);
                        break;
                    case 'AjaximumHideOffCanvasResponse':
                        this.asyncHideOffCanvas(element, item);
                        break;
                    case 'AjaximumGridResponse':
                        this.asyncOnGridSuccess(element, item);
                        break;
                    case 'AjaximumRefreshGridResponse':
                        this.ajaximum.viewManager.refreshGrids(item.Ids);
                        break;
                    case 'AjaximumHideModalResponse':
                        this.asyncHideModal(element, item);
                        break;
                    case 'AjaximumToastResponse':
                        this.asyncToast(element, item);
                        break;
                    case 'AjaximumRefreshPageResponse':
                        window.location.reload();
                        break;
                }
            });
        } else {
            this.handleHtmlResponse(element, data, xhr);
        }
    }

    handleHtmlResponse(element, item, xhr) {
        const contentType = xhr.getResponseHeader('Content-Type') || 'text/html';
        switch (item.UpdateType) {
            case 'OffCanvas':
                this.asyncOnOffCanvasSuccess(element, item, contentType);
                break;
            case 'Modal':
                this.asyncOnModalSuccess(element, item, contentType);
                break;
            default:
                this.asyncOnHtmlSuccess(element, item, contentType);
                break;
        }
        this.ajaximum.viewManager.initGrids();
    }

    asyncHideOffCanvas(element, data) {
        this.$('.offcanvas').each((i, update) => {
            const bootstrapCanvas = bootstrap.Offcanvas.getOrCreateInstance(update);
            bootstrapCanvas.hide();
        });
    }

    asyncHideModal(element, data) {
        this.$('.modal').each((i, update) => {
            const bootstrapCanvas = bootstrap.Modal.getOrCreateInstance(update);
            bootstrapCanvas.hide();
        });
    }

    asyncOnModalSuccess(element, data, contentType) {
        if (contentType.includes('application/x-javascript')) return;

        this.$(element.getAttribute('data-ajaximum-update-target')).each((i, update) => {
            let modalElement = this.$(update).find('.modal');
            if (modalElement.length === 0) {
                modalElement = this.createModalElement(data);
                this.$(update).append(modalElement);

                const modal = new bootstrap.Modal(modalElement);
                modal.show();

                modalElement.on('hidden.bs.modal', () => this.$(update).html(''));
            } else {
                modalElement.find('.modal-content').html(data.Html);
            }
        });

        this.ajaximum.config.onRenderModal(data);
    }

    createModalElement(data) {
        const modalElement = this.$('<div>', { class: 'modal fade', tabindex: -1, 'aria-hidden': true, style: 'display: none;' });
        const modalDialog = this.$('<div>', { class: 'modal-dialog' });
        const modalContent = this.$('<div>', { class: 'modal-content' }).html(data.Html);

        modalDialog.append(modalContent);
        modalElement.append(modalDialog);

        return modalElement;
    }

    asyncOnOffCanvasSuccess(element, data, contentType) {
        if (contentType.includes('application/x-javascript')) return;

        this.$(element.getAttribute('data-ajaximum-update-target')).each((i, update) => {
            let offCanvasElement = this.$(update).find('.offcanvas');
            if (offCanvasElement.length === 0) {
                offCanvasElement = this.createOffCanvasElement(data);
                this.$(update).append(offCanvasElement);

                const bootstrapCanvas = new bootstrap.Offcanvas(offCanvasElement);
                bootstrapCanvas.show();

                offCanvasElement.on('hidden.bs.offcanvas', () => this.$(update).html(''));
            } else {
                offCanvasElement.find('.offcanvas-body').html(data.Html);
            }
        });

        this.ajaximum.config.onRenderOffCanvas(data);
    }

    createOffCanvasElement(data) {
        const offCanvasElement = this.$('<div>', { class: 'offcanvas offcanvas-start', tabindex: -1 });
        const offCanvasHeader = this.$('<div>', { class: 'offcanvas-header' });
        const offCanvasTitle = this.$('<h5>', { class: 'offcanvas-title', id: 'offcanvasExampleLabel' }).text(data.Title);
        const closeButton = this.$('<button>', { type: 'button', class: 'btn-close text-reset', 'data-bs-dismiss': 'offcanvas', 'aria-label': 'Close' });
        const offCanvasBody = this.$('<div>', { class: 'offcanvas-body' }).html(data.Html);

        offCanvasHeader.append(offCanvasTitle).append(closeButton);
        offCanvasElement.append(offCanvasHeader).append(offCanvasBody);

        return offCanvasElement;
    }

    asyncOnGridSuccess(element, data) {
        if (data.UpdateType === 'RefreshGrid') {
            this.ajaximum.viewManager.refreshGrids(data.Ids);
            return;
        }
    }

    asyncOnHtmlSuccess(element, data, contentType) {
        if (contentType.includes('application/x-javascript')) return;

        const mode = (element.getAttribute('data-ajaximum-mode') || '').toUpperCase();
        this.$(element.getAttribute('data-ajaximum-update-target')).each((i, update) => {
            switch (mode) {
                case 'BEFORE':
                    this.$(update).prepend(data.Html);
                    break;
                case 'AFTER':
                    this.$(update).append(data.Html);
                    break;
                case 'REPLACE-WITH':
                    this.$(update).replaceWith(data.Html);
                    break;
                default:
                    this.$(update).html(data.Html);
                    break;
            }
        });

        // Ensure grid initialization after HTML response
      
    }

    asyncToast(element, item) {
        this.ajaximum.config.onRenderToast(item);
    }
}



// navmenu.js
// navmenu.js

// navmenu.js

$(document).ready(function () {

    const $container = $('.navmenu-container').first();
    const onItemClickFnName = $container.data('on-item-click') || "";

    // Toggle submenus for parent items
    $('.navmenu-list li.has-children > .parent-item').on('click', function (e) {
        e.preventDefault();
        $(this).siblings('.sub-menu').slideToggle('fast');
        $(this).parent('li').toggleClass('open');
    });

    // Item click -> call user-defined function
    $('.navmenu-list .navmenu-item-content').on('click', function (e) {
        e.preventDefault(); // stops default link or parent toggle
        const $this = $(this);
        const text = $this.find('.navmenu-item-text').text().trim();
        let url = "";

        if ($this.hasClass('leaf-item')) {
            url = $this.attr('href');
            // If you want links to actually navigate, comment out `e.preventDefault()`.
        }
        callOnItemClicked($this, text, url);
    });

    function callOnItemClicked($item, text, url) {
        if (!onItemClickFnName) return;
        let fn = window[onItemClickFnName];
        if (typeof fn === 'function') {
            fn(text, url, $item);
        }
    }

    // === SEARCH logic ===
    // 1) We'll only do text matching on Normal items
    // 2) If a Normal item matches, show it and its parent chain
    // 3) If the search is empty, show everything (and collapse submenus).

    // On each keystroke in the search box:
    $('.navmenu-search').on('keyup', function () {
        const filter = $(this).val().toUpperCase().trim();

        // If search is empty, reset everything
        if (!filter) {
            resetMenu();
            return;
        }

        // Hide everything initially
        $('.navmenu-list li').hide().removeClass('open');
        $('.navmenu-list .sub-menu').hide();

        // For each Normal item, check if it matches filter
        const $normalLis = $('.navmenu-list li[data-item-type="Normal"]');
        $normalLis.each(function () {
            const $li = $(this);
            const txt = $li.text().toUpperCase();

            if (txt.indexOf(filter) > -1) {
                // Show this matching Normal item
                $li.show();

                // Expand all parent <li> up the chain, so it's visible
                // i.e. if it's inside a group header or parent item
                $li.parents('li').show();

                // For each parent .has-children, add 'open' to keep the submenu expanded
                $li.parents('li.has-children').addClass('open');

                // Also show the sub-menu containers
                $li.parents('ul.sub-menu').show();
            }
        });
    });

    // Reset everything to the default collapsed state (no filtering)
    function resetMenu() {
        // Show all li
        $('.navmenu-list li').show().removeClass('open');
        // Hide submenus
        $('.navmenu-list .sub-menu').hide();
    }
});

// MdiTabsClass.js

// MdiTabsClass.js

class MdiTabs {
    constructor(containerSelector) {
        this.$container = $(containerSelector).first();
        this.$tabList = this.$container.find('.mdi-tab-list');
        this.$contentArea = this.$container.find('.mdi-tab-content');

        // We'll store the default content from the data attribute
        this.defaultContent = this.$contentArea.data('default-content') || "";

        this.tabs = [];
    }

    init() {
        // Click on tab (excluding close)
        this.$tabList.on('click', '.mdi-tab', (e) => {
            if ($(e.target).hasClass('mdi-tab-close')) {
                return;
            }
            this.activateTab($(e.currentTarget));
        });

        // Close button
        this.$tabList.on('click', '.mdi-tab-close', (e) => {
            e.stopPropagation();
            const $tab = $(e.currentTarget).closest('.mdi-tab');
            this.closeTab($tab);
        });
    }

    addTab(title, url, isActive = false) {
        const $tab = $(`
          <li class="mdi-tab" data-url="${url}">
            <span class="mdi-tab-title">${title}</span>
            <span class="mdi-tab-close">&times;</span>
          </li>
        `);
        this.$tabList.append($tab);
        this.tabs.push({ title, url, $tab });

        if (isActive) {
            this.activateTab($tab);
        }
    }

    activateTab($tab) {
        this.$tabList.find('.mdi-tab').removeClass('active');
        $tab.addClass('active');
        this.loadTabContent($tab);
    }

    closeTab($tab) {
        const wasActive = $tab.hasClass('active');
        $tab.remove();
        this.tabs = this.tabs.filter(t => t.$tab[0] !== $tab[0]);

        if (wasActive) {
            const $remaining = this.$tabList.find('.mdi-tab');
            if ($remaining.length > 0) {
                this.activateTab($remaining.first());
            } else {
                // No tabs left => show the default fallback
                this.$contentArea.html(this.defaultContent);
            }
        }
    }

    loadTabContent($tab) {
        const url = $tab.data('url');
        if (!url) {
            this.$contentArea.html("<p>No URL provided.</p>");
            return;
        }

        const cached = $tab.data('cachedContent');
        if (cached) {
            this.$contentArea.html(cached);
            return;
        }

        this.$contentArea.html("<p>Loading...</p>");
        $.get(url)
            .done((resp) => {
                this.$contentArea.html(resp);
                $tab.data('cachedContent', resp);
            })
            .fail(() => {
                this.$contentArea.html("<p>Error loading tab content.</p>");
            });
    }
}

// Expose globally
window.MdiTabs = MdiTabs;

var mdiTab = new MdiTabs('.mdi-tabs-container');
mdiTab.init();
