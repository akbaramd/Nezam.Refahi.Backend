/**
 * Enhanced Select2 Component
 *
 * A wrapper around Select2 that supports local or remote data, modal-awareness,
 * multiple options, tagging, placeholders, and event callbacks.
 *
 * @param {string} selector - CSS selector for the select element.
 * @param {Object} config - Configuration object for Select2.
 *
 * // General Config
 * @param {boolean} [config.allowClear=true] - Whether to allow clearing the selection.
 * @param {boolean} [config.multiple=false] - Enable multi-select if true.
 * @param {boolean} [config.tags=false] - Enable tagging (free-text) if true.
 * @param {string} [config.placeholder='انتخاب کنید'] - Placeholder text.
 * @param {number} [config.minimumInputLength=0] - Characters required to start searching.
 *
 * // Data Source
 * @param {Object} config.dataSource - Defines how data is provided.
 * @param {string} [config.dataSource.type='remote'] - 'remote' or 'local'.
 * @param {string} [config.dataSource.url] - API endpoint for remote fetching.
 * @param {number} [config.dataSource.pageSize=10] - Page size for server pagination.
 * @param {Array}  [config.dataSource.data] - Array of objects for local data (e.g. [{ id, text }, ...]).
 * @param {Function} [config.dataSource.processResults] - Custom function to transform remote response.
 *
 * // Events
 * @param {Function} [config.onSelect] - Callback when an item is selected, receives { id, text }.
 * @param {Function} [config.onClear] - Callback when selection is cleared.
 * @param {Function} [config.onOpen] - Callback when dropdown is opened.
 * @param {Function} [config.onClose] - Callback when dropdown is closed.
 * @param {Function} [config.onChange] - Callback on any value change.
 *
 * // Other
 * @param {Element} [config.dropdownParentOverride] - If provided, forcibly append dropdown here.
 *
 * @returns {Object} - Public API with methods { setValue, getValue, destroy }
 */
export function select2Component(selector, config = {}) {
    // 1. Extract config with defaults
    const {
        allowClear = true,
        multiple = false,
        tags = false,
        placeholder = 'انتخاب کنید',
        minimumInputLength = 0,
        onSelect,
        onClear,
        onOpen,
        onClose,
        onChange,
        dropdownParentOverride,
        // Data source config
        dataSource = {
            type: 'remote',
            url: '',
            pageSize: 10,
            data: [],
            processResults: null,
        },
    } = config;

    // 2. Check existence of the element
    const element = document.querySelector(selector);
    if (!element) {
        console.error(`Select2 component error: Element not found for selector: ${selector}`);
        return null;
    }

    // 3. Determine the dropdown parent
    //    - If a manual override is provided, use that
    //    - Else detect if inside a modal (closest with .modal)
    //    - Otherwise use <body>
    let dropdownParent;
    if (dropdownParentOverride) {
        dropdownParent = $(dropdownParentOverride);
    } else {
        const parentModal = $(element).closest('.modal');
        dropdownParent = parentModal.length > 0 ? parentModal : $('body');
    }

    // 4. Construct the final Select2 config
    const select2Config = {
        placeholder,
        allowClear,
        multiple,
        tags,
        minimumInputLength,
        dropdownParent,
    };

    // 5. Configure data source (remote or local)
    if (dataSource.type === 'local') {
        // Local data
        select2Config.data = dataSource.data || [];
    } else {
        // Remote data
        const pageSize = dataSource.pageSize ?? 10;
        select2Config.ajax = {
            url: dataSource.url,
            dataType: 'json',
            delay: 250,
            data: (params) => {
                // page is 1-based in params, so compute offset:
                const page = params.page || 1;
                const offset = (page - 1) * pageSize;

                // Example filters or search
                const query = {
                    offset,
                    limit: pageSize,
                };
                if (params.term) {
                    // Provide a default filter or let the server handle `params.term`
                    query.filters = `Title|eq|${params.term}`;
                }
                return query;
            },
            processResults: (data, params) => {
                // page is in params
                const page = params.page || 1;

                // If the user provided a custom `processResults`, use it.
                if (typeof dataSource.processResults === 'function') {
                    return dataSource.processResults(data, page);
                }

                // Otherwise default to expecting { data: [], total: number }
                if (!data.data) {
                    console.warn('Remote data format unexpected. Expected { data: [], total: number }.');
                    data.data = [];
                    data.total = 0;
                }

                const results = data.data.map((item) => ({
                    id: item.id,
                    text: item.title ?? item.name ?? item.text, // default to "title" or "text"
                }));

                return {
                    results,
                    pagination: {
                        more: page * pageSize < data.total,
                    },
                };
            },
        };
    }

    // 6. Initialize Select2
    $(element).select2(select2Config);

    // 7. Attach event listeners
    //    - onSelect
    $(element).on('select2:select', (event) => {
        if (typeof onSelect === 'function') {
            onSelect(event.params.data); // pass the selected data
        }
        if (typeof onChange === 'function') {
            onChange($(element).val());
        }
    });

    //    - onClear
    $(element).on('select2:clear', () => {
        if (typeof onClear === 'function') {
            onClear();
        }
        if (typeof onChange === 'function') {
            onChange($(element).val());
        }
    });

    //    - onOpen
    $(element).on('select2:open', () => {
        if (typeof onOpen === 'function') {
            onOpen();
        }
    });

    //    - onClose
    $(element).on('select2:close', () => {
        if (typeof onClose === 'function') {
            onClose();
        }
    });

    //    - onChange (any selection changed, either select or unselect)
    $(element).on('change', () => {
        if (typeof onChange === 'function') {
            onChange($(element).val());
        }
    });

    // 8. Public methods
    return {
        /**
         * Programmatically set the value(s) of the Select2.
         * For multi-select, pass an array.
         * @param {string|Array} value - The ID (or array of IDs) to select.
         */
        /**
         * Programmatically set the value(s) of the Select2.
         * For multi-select, pass an array.
         * @param {string|Array} value - The ID (or array of IDs) to select.
         * @param {Object} [optionData] - Optional data to add if the option is not loaded (e.g., { id, text }).
         */
        setValue(value, optionData = null) {
            const selectElement = $(element);

            // Check if the option exists
            const existingOption = selectElement.find(`option[value="${value}"]`).length > 0;

            if (!existingOption && optionData) {
                // Add the missing option dynamically
                const newOption = new Option(optionData.text, optionData.id, true, true);
                selectElement.append(newOption);
            }

            // Set the value and trigger change
            selectElement.val(value).trigger('change');
        },


        /**
         * Get the current value of the Select2.
         * @returns {string|Array} - The current selected value(s).
         */
        getValue() {
            return $(element).val();
        },

        /**
         * Destroy the Select2 instance (if needed).
         */
        destroy() {
            $(element).select2('destroy');
        },
    };
}
