export default class BasePage {
    constructor(selector, viewUrl) {
        if (!selector) {
            throw new Error('Page selector is required for BasePage.');
        }
        if (!viewUrl) {
            throw new Error('View URL is required for BasePage.');
        }

        this.pageArea = document.querySelector(selector); // The target container
        if (!this.pageArea) {
            throw new Error(`No page area found for selector: ${selector}`);
        }

        this.viewUrl = viewUrl; // URL for fetching partial views
        this.state = {}; // State management
        this.eventRegistry = []; // Event management
    }

    /**
     * Initialize the page and render the content.
     */
    init() {
        console.log('BasePage initialized');
        document.addEventListener('DOMContentLoaded', async () => {
            await this._renderPartialView();
        });
    }

    /**
     * Called when the page content is rendered.
     * To be overridden in child classes if needed.
     */
    async onRender() {
        console.log('onRender called after rendering the partial view');
       
    }


    /**
     * Fetch the partial view from the server via POST and render it inside the page area.
     */
    async _renderPartialView() {
        console.log('Rendering partial view');

        try {
            const response = await fetch(this.viewUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(this.state),
            });

            if (!response.ok) {
                throw new Error(`Failed to fetch partial view: ${response.status}`);
            }

            const html = await response.text();
            this.pageArea.innerHTML = html;

            // Reinitialize registered events after rendering
            this.reInit();
            
            await this.onRender();
   
        } catch (error) {
            console.error('Error rendering partial view:', error);
        }
    }

    /**
     * Reinitialize registered events after DOM changes.
     */
    reInit() {
        console.log('Reinitializing events');
        // Loop through registered events and re-bind them
        this.eventRegistry.forEach(({ elementSelector, event, handler }) => {
            const element = document.querySelector(elementSelector);
            if (element) {
                element.addEventListener(event, handler);
                console.log(`Reinitialized event '${event}' for`, element);
            } else {
                console.warn(`Element not found for reinitializing event '${event}':`, elementSelector);
            }
        });
    }

    /**
     * Clear all registered events.
     */
    clearEvents() {
        this.eventRegistry.forEach(({ elementSelector, event, handler }) => {
            const element = document.querySelector(elementSelector);
            if (element) {
                element.removeEventListener(event, handler);
                console.log(`Cleared event '${event}' for`, element);
            }
        });
        this.eventRegistry = [];
    }

    /**
     * Register an event for better management and reinitialization.
     * @param {string} elementSelector - The CSS selector for the target element.
     * @param {string} event - The event type (e.g., 'click').
     * @param {Function} handler - The event handler function.
     */
    registerEvent(elementSelector, event, handler) {
        const element = document.querySelector(elementSelector);
        if (element) {
            element.addEventListener(event, handler);
            this.eventRegistry.push({ elementSelector, event, handler });
            console.log(`Registered event '${event}' for`, element);
        } else {
            console.warn(`Element not found for event '${event}':`, elementSelector);
        }
    }
}
