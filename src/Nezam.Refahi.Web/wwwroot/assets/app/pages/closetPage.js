import { select2Component } from "../components/select2Component.js";
import { createGridComponent } from "../components/gridComponent.js";
import { createOffCanvas } from "../components/offCanvasComponent.js";
import { createModal } from "../components/modalComponent.js";
import { getCloset, saveCloset, deleteCloset } from "../services/closetService.js";

document.addEventListener("DOMContentLoaded", () => {
    let filterString = "";

    // Initialize Select2 for the filter dropdown
    const filterDressRoomSelect = select2Component("#filterDressRoom", {
        dataSource: {
            type: "remote",
            url: "/LockerRoom/GetData",
        },
        placeholder: "اتاق مورد نظر را انتخاب کنید",
    });

    // Initialize Select2 for the modal dropdown
    const closetDressRoomSelect = select2Component("#closetDressRoom", {
        dataSource: {
            type: "remote",
            url: "/LockerRoom/GetData",
        },
        placeholder: "اتاق مربوطه را انتخاب کنید",
    });

    // Initialize the grid for Closet
    const grid = createGridComponent({
        selector: "#closetTable",
        dataSource: {
            type: "remote",
            url: "/Closet/GetData",
            method: "GET",
        },
        columns: [
            { name: "شناسه", field: "id", hidden: true, sortField: "id" },
            { name: "عنوان کمد", field: "title", sortField: "title", width: "200px" },
            { name: "شماره کمد", field: "number", sortField: "number", width: "100px" },
            { name: "سطر", field: "row", sortField: "row", width: "100px" },
            { name: "ستون", field: "column", sortField: "column", width: "100px" },
            {
                name: "اتاق",
                field: "lockerRoom.title",
                sortField: "dressRoomId",
                width: "200px",
            },
            {
                name: "عملیات",
                formatter: (_, row) =>
                    gridjs.html(`
                        <button class='btn btn-sm btn-info editButton' data-id='${row.cells[0].data}'>ویرایش</button>
                        <button class='btn btn-sm btn-danger deleteButton' data-id='${row.cells[0].data}'>حذف</button>
                    `),
                width: "200px",
            },
        ],
    });

    // Fetch initial data
    grid.render();

    // Initialize the modal for editing/adding closets
    const closetModal = createModal("#closetModal", {
        onOpen: () => console.log("Modal opened"),
        onClose: () => console.log("Modal closed"),
    });

    // Initialize the off-canvas for filtering
    const filterOffCanvas = createOffCanvas("#filterOffCanvas", {
        onOpen: () => console.log("Filter Off-Canvas opened"),
        onClose: () => console.log("Filter Off-Canvas closed"),
    });

    // Open filter off-canvas
    document.getElementById("filterToggleButton").addEventListener("click", () => {
        filterOffCanvas.open();
    });

    // Apply Filters
    document.getElementById("applyFilterButton").addEventListener("click", applyFilters);
    document.getElementById("resetFilterButton").addEventListener("click", resetFilters);

    function applyFilters() {
        const formData = new FormData(document.getElementById("filterFormFields"));
        const filtersArray = [];

        formData.forEach((value, key) => {
            const inputElement = document.querySelector(`[name="${key}"]`);
            const operator = inputElement?.getAttribute("data-operator") || "eq";
            if (value.trim()) {
                filtersArray.push(`${key}|${operator}|${value}`);
            }
        });

        filterString = filtersArray.join(",");
        grid.setFilters(filterString);
        grid.render();
        filterOffCanvas.close(); // Close the off-canvas after applying filters
    }

    function resetFilters() {
        document.getElementById("filterFormFields").reset();
        filterDressRoomSelect.setValue(null);
        filterString = "";
        grid.render();
        filterOffCanvas.close(); // Close the off-canvas after resetting
    }

    // Add/Edit Closet Modal Handlers
    document.getElementById("addClosetButton").addEventListener("click", () => {
        openModal();
    });

    document.getElementById("closetTable").addEventListener("click", async (event) => {
        const editButton = event.target.closest(".editButton");
        const deleteButton = event.target.closest(".deleteButton");

        if (editButton) {
            const id = editButton.dataset.id;
            openModal(id);
        }

        if (deleteButton) {
            const id = deleteButton.dataset.id;
            await deleteClosetHandler(id);
        }
    });

    document.getElementById("saveClosetButton").addEventListener("click", async () => {
        await saveClosetHandler();
    });

    function openModal(id = null) {
        if (id) {
            getCloset(id).then((data) => {
                document.getElementById("closetId").value = data.id;
                document.getElementById("closetTitle").value = data.title;
                document.getElementById("closetNumber").value = data.number;
                document.getElementById("closetRow").value = data.row;
                document.getElementById("closetColumn").value = data.column;
                closetDressRoomSelect.setValue(data.dressRoomId, {
                    id: data.dressRoom.id,
                    text: data.dressRoom.title,
                });
                closetModal.open();
            });
        } else {
            document.getElementById("closetForm").reset();
            closetDressRoomSelect.setValue(null);
            closetModal.open();
        }
    }

    async function saveClosetHandler() {
        const id = document.getElementById("closetId").value;
        const formData = {
            id: id ? parseInt(id) : 0,
            title: document.getElementById("closetTitle").value,
            lockerNumber: parseInt(document.getElementById("closetNumber").value),
            lockerRoomId: closetDressRoomSelect.getValue(),
            row: parseInt(document.getElementById("closetRow").value),
            column: parseInt(document.getElementById("closetColumn").value),
        };

        try {
            const res = await saveCloset(formData);
            if (res.ok) {
                grid.render();
                closetModal.hide();
                showToast("کمد با موفقیت ذخیره شد.", "#4fbe87");
            } else {
                showToast("خطا در ذخیره سازی کمد. کمدی با این شماره و رختکن موجود میباشد.", "#f3616d");
            }
        } catch (error) {
            console.error("Error saving closet:", error);
            showToast("خطای شبکه. لطفاً دوباره تلاش کنید.", "#f3616d");
        }
    }

    async function deleteClosetHandler(id) {
        if (confirm("آیا مطمئن هستید که می‌خواهید این کمد را حذف کنید؟")) {
            await deleteCloset(id);
            grid.render();
        }
    }

    function showToast(message, bgColor) {
        Toastify({
            text: message,
            duration: 5000,
            close: true,
            gravity: "top",
            position: "right",
            backgroundColor: bgColor,
        }).showToast();
    }
});
