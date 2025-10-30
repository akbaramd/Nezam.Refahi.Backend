document.addEventListener('DOMContentLoaded', () => {
    let filterString = ""; // For storing the applied filter string

    // Initialize Select2 for dropdowns
    function initializeSelect2(selectElement, apiUrl) {
        const parentModal = $(selectElement).closest('.modal'); // Find the closest modal
        const dropdownParent = parentModal.length > 0 ? parentModal : $('body'); // Use modal if found, otherwise default to body

        $(selectElement).select2({
            ajax: {
                url: apiUrl,
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        offset: (params.page || 1 - 1) * 10, // Pagination offset
                        limit: 10, // Items per page
                        filters: params.term ? `Title|eq|${params.term}` : null // Search filter
                    };
                },
                processResults: function (data, params) {
                    params.page = params.page || 1;
                    return {
                        results: data.data.map(item => ({
                            id: item.id,
                            text: item.title
                        })),
                        pagination: {
                            more: (params.page * 10) < data.total // Check if more data is available
                        }
                    };
                }
            },
            placeholder: 'انتخاب کنید',
            allowClear: false,
            dropdownParent: dropdownParent // Dynamically set dropdown parent
        });
    }

    // Apply Select2 for the filter and modal dropdowns
    initializeSelect2('#filterDevice', '/DressRoomDevice/GetData');
    initializeSelect2('#roomDevice', '/DressRoomDevice/GetData');

    // Initialize GridJS for the table
    const grid = new gridjs.Grid({
        columns: [
            { name: 'شناسه', hidden: true },
            { name: 'عنوان اتاق', width: '200px' },
            { name: 'دستگاه', width: '200px' },
            {
                name: 'عملیات',
                formatter: (_, row) => gridjs.html(`
                    <button class='btn btn-sm btn-info btn-label waves-effect waves-light editButton' data-id='${row.cells[0].data}'>
                        <i class='ri-edit-line label-icon align-middle fs-16 me-2'></i> ویرایش
                    </button>
                    <button class='btn btn-sm btn-danger btn-label waves-effect waves-light deleteButton' data-id='${row.cells[0].data}'>
                        <i class='ri-delete-bin-line label-icon align-middle fs-16 me-2'></i> حذف
                    </button>
                `),
                width: '200px'
            }
        ],
        pagination: {
            limit: 5,
            server: {
                url: (prev, page, limit) => {
                    const url = new URL(prev, window.location.origin);
                    url.searchParams.set('offset', page * limit);
                    url.searchParams.set('limit', limit);
                    if (filterString) url.searchParams.set('filters', filterString);
                    return url.toString();
                }
            }
        },
        server: {
            url: '/DressRoom/GetData',
            then: data => data.data.map(room => [
                room.id,
                room.title,
                room.device ? room.device.title : "بدون دستگاه"
            ]),
            total: data => data.total
        },
        sort: {
            multiColumn: false,
            server: {
                url: (prev, columns) => {
                    if (!columns.length) return prev;
                    const url = new URL(prev, window.location.origin);
                    const col = columns[0];
                    const dir = col.direction === 1 ? 'asc' : 'desc';
                    const colName = ['id', 'title', 'device.title'][col.index];
                    url.searchParams.set('sortColumn', colName);
                    url.searchParams.set('sortDirection', dir);
                    return url.toString();
                }
            }
        },
        language: {
            search: { placeholder: "جستجو..." },
            pagination: {
                previous: "قبلی",
                next: "بعدی",
                showing: "نمایش",
                results: () => "نتیجه",
            },
            loading: "در حال بارگذاری...",
            noRecordsFound: "رکوردی یافت نشد",
            error: "خطا در بارگذاری داده‌ها",
        }
    }).render(document.getElementById('roomTable'));

    // Apply filters
    document.getElementById('applyFilterButton').addEventListener('click', () => {
        const formData = new FormData(document.getElementById('filterFormFields'));
        const filtersArray = [];

        formData.forEach((value, key) => {
            const inputElement = document.querySelector(`[name="${key}"]`);
            const operator = inputElement?.getAttribute('data-operator') || '='; // Default operator is '='
            if (value.trim()) {
                filtersArray.push(`${key}|${operator}|${value}`);
            }
        });

        filterString = filtersArray.join(',');
        grid.forceRender();
        toggleResetButton(filtersArray.length > 0);
    });

    // Reset filters
    document.getElementById('resetFilterButton').addEventListener('click', () => {
        document.getElementById('filterFormFields').reset();
        filterString = "";
        grid.forceRender();
        toggleResetButton(false);
    });

    function toggleResetButton(show) {
        document.getElementById('resetFilterButton').classList.toggle('d-none', !show);
    }

    // Handle Add/Edit modal
    document.getElementById('addRoomButton').addEventListener('click', () => openModal());

    document.getElementById('roomTable').addEventListener('click', (event) => {
        const editButton = event.target.closest('.editButton');
        const deleteButton = event.target.closest('.deleteButton');

        if (editButton) {
            const id = editButton.dataset.id;
            openModal(id);
        }

        if (deleteButton) {
            const id = deleteButton.dataset.id;
            deleteRoom(id);
        }
    });

    document.getElementById('saveRoomButton').addEventListener('click', saveRoom);

    function openModal(id = null) {
        const modal = new bootstrap.Modal(document.getElementById('roomModal'));

        if (id) {
            fetch(`/DressRoom/GetDressRoom?id=${id}`)
                .then(response => response.json())
                .then(data => {
                    document.getElementById('roomId').value = data.id;
                    document.getElementById('roomTitle').value = data.title;
                    $('#roomDevice').val(data.deviceId).trigger('change'); // Set and update Select2 UI
                    modal.show();
                });
        } else {
            document.getElementById('roomForm').reset();
            $('#roomDevice').val(null).trigger('change'); // Reset Select2 UI
            modal.show();
        }
    }

    function saveRoom() {
        const id = document.getElementById('roomId').value;
        const formData = {
            id: id ? parseInt(id) : 0,
            title: document.getElementById('roomTitle').value,
            deviceId: $('#roomDevice').val()
        };

        fetch('/DressRoom/SaveDressRoom', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(formData)
        }).then(response => {
            if (response.ok) {
                grid.forceRender();
                const modal = bootstrap.Modal.getInstance(document.getElementById('roomModal'));
                modal.hide();
            }
        });
    }

    function deleteRoom(id) {
        if (confirm('آیا مطمئن هستید که می‌خواهید این اتاق را حذف کنید؟')) {
            fetch(`/DressRoom/DeleteDressRoom?id=${id}`, { method: 'DELETE' })
                .then(response => {
                    if (response.ok) {
                        grid.forceRender();
                    }
                });
        }
    }
});
