/**
 * A highly flexible, enhanced Grid.js wrapper that supports various data sources,
 * including local or remote. It provides strong structure, easy configuration,
 * and exposes a rich API for controlling the grid at runtime.
 *
 * Usage:
 *
 *   const grid = createGridComponent({
 *     selector: '#myGrid',
 *     columns: [
 *       { name: 'ID', field: 'id' },
 *       { name: 'Name', field: 'name' },
 *       // ...
 *     ],
 *     dataSource: {
 *       type: 'remote',  // or 'local'
 *       url: '/api/users',
 *       method: 'GET',   // default is GET
 *       headers: { 'Authorization': 'Bearer ...' },
 *       data: null,      // used for 'local' or if you want to prefetch
 *       transformResponse: (rawData) => { ... return your array ... },
 *     },
 *     pagination: { limit: 5, server: true },
 *     sort: { multiColumn: false, server: true },
 *     filters: { status: 'active' },  // or a string, etc.
 *     language: { ... },
 *     events: {
 *       onRowClick: (row, event) => { ... },
 *       onLoad: () => { ... },
 *       ...
 *     },
 *     gridOptions: { ...any gridjs config here... }
 *   });
 *
 *   grid.render();
 *   // later: grid.setFilters({ status: 'inactive' });
 *   // or: grid.destroy();
 */
export function createGridComponent(options) {
    // Destructure options with defaults
    const {
        selector,
        columns = [],
        dataSource = {
            type: 'remote',
            url: '',
            method: 'GET',
            headers: {},
            data: null,
            transformResponse: null,
        },
        pagination = { limit: 5, server: true },
        sort = { multiColumn: false, server: true },
        filters = {},   // can be object or string
        language = {},
        events = {},
        enableRowSelection = false,
        gridOptions = {},
    } = options;

    let gridInstance;
    let currentFilters = filters;

    // -------------------------------------
    // Get Nested Value (for dotted fields)
    // -------------------------------------
    function getNestedValue(obj, path) {
        if (!path) return null;
        return path.split('.').reduce((acc, part) => acc && acc[part], obj);
    }

    // -------------------------------------
    // Default transformResponse
    // for remote data
    // -------------------------------------
    function defaultTransformResponse(data) {
        // Assuming server returns { data: [], total: number }
        // Map it to an array-of-arrays
        if (!data || !data.data) return [];
        return data.data.map((item) =>
            columns.map((col) => (col.field ? getNestedValue(item, col.field) : null))
        );
    }

    // -------------------------------------
    // Build request config for remote
    // (URL, Query Params, Method, Headers)
    // -------------------------------------
    function buildRequestConfig({ page = 0, limit = 5, sortColumns = [] }) {
        const {
            url,
            method = 'GET',
            headers = {},
        } = dataSource;

        // Base URL
        const fullUrl = new URL(url, window.location.origin);

        // Pagination
        if (pagination.server) {
            fullUrl.searchParams.set('offset', page * limit);
            fullUrl.searchParams.set('limit', limit);
        }

        // Sorting
        if (sort.server && sortColumns && sortColumns.length > 0) {
            const sortCol = sortColumns[0]; // Grid.js only sends the first if multiColumn=false
            const sortDirection = sortCol.direction === 1 ? 'asc' : 'desc';
            const sortField =
                columns[sortCol.index]?.sortField || columns[sortCol.index]?.field;
            if (sortField) {
                fullUrl.searchParams.set('sortColumn', sortField);
                fullUrl.searchParams.set('sortDirection', sortDirection);
            }
        }

        // Filters
        if (currentFilters) {
            // If filters is an object, set each key/value in query params
            if (typeof currentFilters === 'object') {
                Object.entries(currentFilters).forEach(([key, val]) => {
                    fullUrl.searchParams.set(key, val);
                });
            }
            // If it's a string, set as 'filters'
            else if (typeof currentFilters === 'string') {
                fullUrl.searchParams.set('filters', currentFilters);
            }
        }

        // Return an object describing how we'll fetch the data
        return {
            url: fullUrl.toString(),
            options: {
                method,
                headers,
            },
        };
    }

    // -------------------------------------
    // Build final columns
    // including row selection if needed
    // -------------------------------------
    function buildColumns() {
        if (!enableRowSelection) {
            return columns.map((col) => ({
                name: col.name,
                hidden: col.hidden || false,
                width: col.width || null,
                formatter: col.formatter || undefined,
            }));
        }

        // If we want row selection, prepend a checkbox column
        return [
            {
                id: '_selection',
                name: '',
                width: '40px',
                formatter: (cell, row) => {
                    // Example: store row ID in data attribute
                    const rowId = row?.cells?.[0]?.data;
                    return gridjs.html(
                        `<input type="checkbox" data-row-id="${rowId}" />`
                    );
                },
            },
            ...columns.map((col) => ({
                name: col.name,
                hidden: col.hidden || false,
                width: col.width || null,
                formatter: col.formatter || undefined,
            })),
        ];
    }

    // -------------------------------------
    // Initialize & Render the Grid
    // -------------------------------------
    function initialize() {
        if (!selector) {
            console.error('Grid selector is not specified!');
            return;
        }

        // Prepare language overrides
        const defaultLanguage = {
            search: { placeholder: 'جستجو...' },
            pagination: {
                previous: 'قبلی',
                next: 'بعدی',
                showing: 'نمایش',
                results: () => 'نتیجه',
            },
            loading: 'در حال بارگذاری...',
            noRecordsFound: 'رکوردی یافت نشد',
            error: 'خطا در بارگذاری داده‌ها',
        };
        const finalLanguage = {
            ...defaultLanguage,
            ...language,
            pagination: {
                ...defaultLanguage.pagination,
                ...(language.pagination || {}),
            },
            search: {
                ...defaultLanguage.search,
                ...(language.search || {}),
            },
        };

        // Configure pagination
        let paginationConfig = false;
        if (pagination && pagination.limit) {
            // If server-based pagination
            if (pagination.server) {
                paginationConfig = {
                    limit: pagination.limit,
                    server: {
                        url: (_prev, page, limit) =>
                            buildRequestConfig({ page, limit }).url,
                        // Optionally, you could handle fetch yourself if needed
                    },
                };
            } else {
                // Client-side pagination
                paginationConfig = { limit: pagination.limit };
            }
        }

        // Configure sorting
        let sortConfig = false;
        if (sort) {
            // If server-based sorting
            if (sort.server) {
                sortConfig = {
                    multiColumn: sort.multiColumn || false,
                    server: {
                        url: (_prev, sortColumns) =>
                            buildRequestConfig({ page: 0, limit: pagination.limit, sortColumns })
                                .url,
                    },
                };
            } else {
                sortConfig = {
                    multiColumn: sort.multiColumn || false,
                };
            }
        }

        // Server config if dataSource.type = 'remote'
        let serverConfig;
        if (dataSource.type === 'remote') {
            serverConfig = {
                // The default URL (on first load) is derived from buildRequestConfig
                url: buildRequestConfig({ page: 0, limit: pagination.limit }).url,
                then: (rawData) => {
                    if (typeof dataSource.transformResponse === 'function') {
                        return dataSource.transformResponse(rawData, columns);
                    }
                    // else use our default
                    return defaultTransformResponse(rawData);
                },
                total: (rawData) => rawData.total,
            };
        }

        // If dataSource.type === 'local', we can pass the array directly to data:
        let localData = undefined;
        if (dataSource.type === 'local' && Array.isArray(dataSource.data)) {
            localData = dataSource.data.map((item) => {
                return columns.map((col) =>
                    col.field ? getNestedValue(item, col.field) : null
                );
            });
        }

        // Create the grid
        gridInstance = new gridjs.Grid({
            columns: buildColumns(),
            data: dataSource.type === 'local' ? localData : undefined, // only if local
            server: dataSource.type === 'remote' ? serverConfig : undefined,
            pagination: paginationConfig,
            sort: sortConfig,
            language: finalLanguage,
            ...gridOptions,
        });

        // Render
        gridInstance.render(document.querySelector(selector));
        console.log(gridInstance);
        console.log(selector);
        // Attach events
        attachEvents();
    }

    // -------------------------------------
    // Attach Event Handlers
    // -------------------------------------
    function attachEvents() {
        if (!gridInstance || !events) return;

        const {
            onRowClick,
            onCellClick,
            onLoad,
            onReady,
            onDestroy,
            onError,
        } = events;

        // Row Click
        if (typeof onRowClick === 'function') {
            gridInstance.on('rowClick', onRowClick);
        }

        // Cell Click
        if (typeof onCellClick === 'function') {
            gridInstance.on('cellClick', onCellClick);
        }

        // On Load
        if (typeof onLoad === 'function') {
            gridInstance.on('load', onLoad);
        }

        // On Ready
        if (typeof onReady === 'function') {
            gridInstance.on('ready', onReady);
        }

        // On Destroy
        if (typeof onDestroy === 'function') {
            gridInstance.on('destroy', onDestroy);
        }

        // On Error
        if (typeof onError === 'function') {
            gridInstance.on('error', onError);
        }
    }

    // -------------------------------------
    // Public API
    // -------------------------------------

    // 1. Render / Force re-render
    function render() {
        if (!gridInstance) {
            initialize();
        } else {
            gridInstance.forceRender();
        }
    }

    // 2. Destroy
    function destroy() {
        if (gridInstance) {
            gridInstance.destroy();
            gridInstance = null;
        }
    }

    // 3. Set Filters (object or string) & re-render
    function setFilters(newFilters) {
        currentFilters = newFilters;
        if (gridInstance) {
            // If we're remote, we want to re-fetch from the server
            // A quick way is to call `forceRender()`, which triggers the server call
            gridInstance.forceRender();
        }
    }

    // 4. Go to specific page
    function goToPage(pageNumber) {
        if (gridInstance && gridInstance.updateConfig) {
            gridInstance
                .updateConfig((cfg) => {
                    if (!cfg.pagination) return cfg;
                    return {
                        ...cfg,
                        pagination: {
                            ...cfg.pagination,
                            page: pageNumber,
                        },
                    };
                })
                .forceRender();
        }
    }

    // 5. Get the underlying grid instance
    function getInstance() {
        return gridInstance;
    }

    // Return all public methods
    return {
        render,
        destroy,
        setFilters,
        goToPage,
        getInstance,
    };
}
