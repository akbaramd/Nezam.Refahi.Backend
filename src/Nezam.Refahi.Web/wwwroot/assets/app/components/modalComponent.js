/**
 * Create a reusable modal component that manages modal behavior and events.
 *
 * @param {string} modalSelector - The CSS selector for the modal element.
 * @param {Object} [options] - Configuration object for modal behavior.
 * @param {Function} [options.onOpen] - Callback when the modal is opened.
 * @param {Function} [options.onClose] - Callback when the modal is closed.
 * @returns {Object} - Public API to manage the modal.
 */
export function createModal(modalSelector, { onOpen = null, onClose = null } = {}) {
    const modalElement = document.querySelector(modalSelector);
    if (!modalElement) {
        console.error(`Modal element not found: ${modalSelector}`);
        return null;
    }

    // Bootstrap modal instance
    const modalInstance = new bootstrap.Modal(modalElement);

    // Attach Bootstrap modal events
    modalElement.addEventListener('shown.bs.modal', () => {
        if (typeof onOpen === 'function') {
            onOpen();
        }
    });

    modalElement.addEventListener('hidden.bs.modal', () => {
        if (typeof onClose === 'function') {
            onClose();
        }
    });

    // Public API
    return {
        /**
         * Open the modal.
         */
        open() {
            modalInstance.show();
        },

        /**
         * Hide the modal.
         */
        hide() {
            modalInstance.hide();
        },

        /**
         * Attach an event listener to the modal.
         * @param {string} eventName - The name of the Bootstrap modal event (e.g., 'shown.bs.modal').
         * @param {Function} callback - The function to execute when the event is triggered.
         */
        on(eventName, callback) {
            modalElement.addEventListener(eventName, callback);
        },

        /**
         * Detach an event listener from the modal.
         * @param {string} eventName - The name of the Bootstrap modal event.
         * @param {Function} callback - The function to remove.
         */
        off(eventName, callback) {
            modalElement.removeEventListener(eventName, callback);
        },
    };
}
