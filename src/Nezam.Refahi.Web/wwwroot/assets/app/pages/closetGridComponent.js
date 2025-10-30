import { createGridComponent } from '../components/gridComponent.js';
import BaseComponent from '../core/BaseComponent.js';

export default class ClosetGridComponent extends BaseComponent {
    constructor(selector, onEdit, onDelete) {
        super(selector, "/Closet/_Grid"); // فراخوانی کانستراکتور والد

        this.dataSourceUrl = "/Closet/GetData";
        this.onEdit = onEdit; // کال‌بک برای ویرایش
        this.onDelete = onDelete; // کال‌بک برای حذف
        this.grid = null; // نمونه گرید
    }

    /**
     * رندر اولیه و ساخت گرید.
     */
    async onRender(isFirstRender) {
        console.log('ClosetGridComponent rendered');
        this.grid = createGridComponent({
            selector: ".grid",
            dataSource: {
                type: 'remote',
                url: this.dataSourceUrl,
                method: 'GET',
            },
            columns: [
                { name: 'شناسه', field: 'id', hidden: true },
                { name: 'عنوان کمد', field: 'title', width: '150px' },
                { name: 'شماره', field: 'number', width: '100px' },
                { name: 'ردیف', field: 'row', width: '100px' },
                { name: 'ستون', field: 'column', width: '100px' },
                {
                    name: 'اتاق رختکن',
                    field: 'dressRoom.title',
                    sortField: 'dressRoomId',
                    width: '200px',
                },
                {
                    name: 'عملیات',
                    formatter: (_, row) =>
                        gridjs.html(`
                            <button class='btn btn-sm btn-info editButton' data-id='${row.cells[0].data}'>ویرایش</button>
                            <button class='btn btn-sm btn-danger deleteButton' data-id='${row.cells[0].data}'>حذف</button>
                        `),
                    width: '200px',
                },
            ],
        });

        this.grid.render();

        // مدیریت ایونت‌ها برای دکمه‌های ویرایش و حذف
        this.registerEvent(`${this.selector}`, 'click', (event) => {
            const editButton = event.target.closest('.editButton');
            const deleteButton = event.target.closest('.deleteButton');

            if (editButton && this.onEdit) {
                const id = editButton.dataset.id;
                this.onEdit(id);
            }

            if (deleteButton && this.onDelete) {
                const id = deleteButton.dataset.id;
                this.onDelete(id);
            }
        });
    }

    /**
     * اعمال فیلترها و بازسازی گرید.
     * @param {string} filterString - رشته فیلتر.
     */
    setFilters(filterString) {
        if (this.grid) {
            this.grid.updateConfig({
                dataSource: {
                    type: 'remote',
                    url: `${this.dataSourceUrl}?filters=${encodeURIComponent(filterString)}`,
                },
            }).forceRender();
        }
    }
}
