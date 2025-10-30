/**
 * Create a reusable off-canvas component that manages off-canvas behavior and events.
 *
 * @param {string} canvasSelector - The CSS selector for the off-canvas element.
 * @param {Object} [options] - Configuration object for off-canvas behavior.
 * @param {Function} [options.onOpen] - Callback when the off-canvas is opened.
 * @param {Function} [options.onClose] - Callback when the off-canvas is closed.
 * @returns {Object} - Public API to manage the off-canvas.
 */
export function createOffCanvas(canvasSelector, { onOpen = null, onClose = null } = {}) {
    const canvasElement = document.querySelector(canvasSelector);
    if (!canvasElement) {
        console.error(`Off-canvas element not found: ${canvasSelector}`);
        return null;
    }

    // Bootstrap Off-Canvas instance
    const canvasInstance = new bootstrap.Offcanvas(canvasElement);

    // Attach Bootstrap Off-Canvas events
    canvasElement.addEventListener('shown.bs.offcanvas', () => {
        if (typeof onOpen === 'function') {
            onOpen();
        }
    });

    canvasElement.addEventListener('hidden.bs.offcanvas', () => {
        if (typeof onClose === 'function') {
            onClose();
        }
    });

    // Public API
    return {
        /**
         * Open the off-canvas.
         */
        open() {
            canvasInstance.show();
        },

        /**
         * Close the off-canvas.
         */
        close() {
            canvasInstance.hide();
        },

        /**
         * Attach an event listener to the off-canvas.
         * @param {string} eventName - The name of the Bootstrap off-canvas event (e.g., 'shown.bs.offcanvas').
         * @param {Function} callback - The function to execute when the event is triggered.
         */
        on(eventName, callback) {
            canvasElement.addEventListener(eventName, callback);
        },

        /**
         * Detach an event listener from the off-canvas.
         * @param {string} eventName - The name of the Bootstrap off-canvas event.
         * @param {Function} callback - The function to remove.
         */
        off(eventName, callback) {
            canvasElement.removeEventListener(eventName, callback);
        },

        /**
         * Set the position of the off-canvas dynamically (e.g., start, end, top, bottom).
         * @param {string} position - The position class to apply (e.g., 'offcanvas-start', 'offcanvas-end').
         */
        setPosition(position) {
            const validPositions = ['offcanvas-start', 'offcanvas-end', 'offcanvas-top', 'offcanvas-bottom'];
            if (!validPositions.includes(position)) {
                console.error(`Invalid off-canvas position: ${position}`);
                return;
            }

            // Remove existing position classes
            validPositions.forEach((pos) => canvasElement.classList.remove(pos));

            // Add the new position class
            canvasElement.classList.add(position);
        },
    };
}
