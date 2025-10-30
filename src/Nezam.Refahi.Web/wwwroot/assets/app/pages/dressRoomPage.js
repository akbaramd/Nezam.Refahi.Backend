import { select2Component } from "../components/select2Component.js";
import { createGridComponent } from "../components/gridComponent.js";
import { createOffCanvas } from "../components/offCanvasComponent.js";
import { deleteRoom, getRoom, saveRoom } from "../services/dressRoomService.js";
import {createModal} from "../components/modalComponent.js";

document.addEventListener("DOMContentLoaded", () => {
    let filterString = "";

    // Initialize Select2 for the filter dropdown
    const filterDeviceSelect = select2Component("#filterDevice", {
        dataSource: {
            type: "remote",
            url: "/LockerDevice/GetData",
        },
        placeholder: "دستگاه مورد نظر را انتخاب کنید",
    });

    // Initialize Select2 for the modal dropdown
    const roomDeviceSelect = select2Component("#roomDevice", {
        dataSource: {
            type: "remote",
            url: "/LockerDevice/GetData",
        },
        placeholder: "دستگاه مربوطه را انتخاب کنید",
    });

    // Initialize the grid for DressRoom
    const grid = createGridComponent({
        selector: "#roomTable",
        dataSource: {
            type: "remote",
            url: "/LockerRoom/GetData",
            method: "GET",
        },
        columns: [
            { name: "شناسه", field: "id", hidden: true, sortField: "id" },
            {
                name: "عنوان اتاق",
                field: "title",
                sortField: "title",
                width: "200px",
            },
            {
                name: "دستگاه",
                field: "lockerDevice.title",
                sortField: "lockerDeviceId",
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

    // Initialize the modal for editing/adding rooms
    const roomModal = createModal("#roomModal", {
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
        filterDeviceSelect.setValue(null);
        filterString = "";
        grid.render();
        filterOffCanvas.close(); // Close the off-canvas after resetting
    }

    // Add/Edit Room Modal Handlers
    document.getElementById("addRoomButton").addEventListener("click", () => {
        openModal();
    });

    document.getElementById("roomTable").addEventListener("click", async (event) => {
        const editButton = event.target.closest(".editButton");
        const deleteButton = event.target.closest(".deleteButton");

        if (editButton) {
            const id = editButton.dataset.id;
            openModal(id);
        }

        if (deleteButton) {
            const id = deleteButton.dataset.id;
            await deleteRoomHandler(id);
        }
    });

    document.getElementById("saveRoomButton").addEventListener("click", async () => {
        await saveRoomHandler();
    });

    function openModal(id = null) {
        if (id) {
            getRoom(id).then((data) => {
                document.getElementById("roomId").value = data.id;
                document.getElementById("roomTitle").value = data.title;
                roomDeviceSelect.setValue(data.lockerDeviceId, {
                    id: data.lockerDevice.id,
                    text: data.lockerDevice.title,
                });
                roomModal.open();
            });
        } else {
            document.getElementById("roomId").value = null;
            document.getElementById("roomTitle").value = null;
            roomDeviceSelect.setValue(null);
            roomModal.open();
        }
    }

    async function saveRoomHandler() {
        const id = document.getElementById("roomId").value;
        const formData = {
            id: id ? parseInt(id) : 0,
            title: document.getElementById("roomTitle").value,
            location:"موقیت فعلی",
            lockerDeviceId: roomDeviceSelect.getValue(),
        };

        await saveRoom(formData);
        grid.render();
        roomModal.hide();
    }

    async function deleteRoomHandler(id) {
        if (confirm("آیا مطمئن هستید که می‌خواهید این اتاق را حذف کنید؟")) {
            await deleteRoom(id);
            grid.render();
        }
    }
});
