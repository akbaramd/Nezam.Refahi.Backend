export default class BaseComponent {
    constructor(selector, viewUrl) {
        if (!selector) {
            throw new Error('Component selector is required for BaseComponent.');
        }
        if (!viewUrl) {
            throw new Error('View URL is required for BaseComponent.');
        }

        this.componentArea = null;
        this.selector = selector;
        this.viewUrl = viewUrl; // URL for fetching partial views
        this.state = {}; // Component's state management
        this.eventRegistry = []; // Registered events for reinitialization
    }

    /**
     * Initialize the component and render the content once the DOM is fully loaded.
     */
    async init() {
        this.componentArea = document.querySelector(this.selector); // Container for the component
        if (!this.componentArea) {
            throw new Error(`No component area found for selector: ${selector}`);
        }
        
        
        console.log('Component initialization started');

        if (document.readyState === 'complete' || document.readyState === 'interactive') {
            // Document is already loaded
            console.log('Document is already loaded');
            await this.render(); // Initial render
        } else {
            // Wait for the DOMContentLoaded event
            document.addEventListener('DOMContentLoaded', async () => {
                console.log('DOMContentLoaded event triggered');
                await this.render(true); // Initial render
            });
        }
    }

    /**
     * Fetch the partial view and state from the server and render the component.
     */
    async render(isFirstRender= false) {
        console.log('Fetching partial view and state from the server');
        try {
            const response = await fetch(this.viewUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(this.state), // Send the current state to the server
            });

            if (!response.ok) {
                throw new Error(`Failed to fetch partial view: ${response.status}`);
            }

            const jsonResponse = await response.json();

            // Validate JSON response
            if (!jsonResponse.content || !jsonResponse.state) {
                throw new Error('Invalid response format. Expected { content, state }.');
            }

            // Update the state
            this.state = { ...this.state, ...jsonResponse.state };

            // Update the component content
            this.componentArea.innerHTML = jsonResponse.content;

            // Reinitialize registered events after rendering
            this.reInit();

            // Optional hook for child classes
            this.onRender(isFirstRender);
        } catch (error) {
            console.error('Error rendering component:', error);
        }
    }

    /**
     * Hook for additional logic after rendering.
     * To be overridden in child classes.
     */
    onRender(isFirstRender = fa) {
        console.log('onRender called after rendering the partial view');
    }

    /**
     * Update the state and re-render the component.
     * @param {object} newState - New state to merge with the current state.
     */
    async setState(newState) {
        this.state = { ...this.state, ...newState }; // Merge new state with the existing state
        await this.render(); // Re-render the component with the updated state
    }

    /**
     * Reinitialize events after DOM changes.
     */
    reInit() {
        console.log('Reinitializing events');
        this.eventRegistry.forEach(({ selector, event, handler }) => {
            const element = document.querySelector(selector);
            if (element) {
                element.addEventListener(event, handler);
            } else {
                console.warn(`Element not found for reinitializing event '${event}':`, selector);
            }
        });
    }

    /**
     * Register an event for automatic reinitialization.
     * @param {string} selector - CSS selector for the target element.
     * @param {string} event - The event type (e.g., 'click').
     * @param {Function} handler - The event handler function.
     */
    registerEvent(selector, event, handler) {
        const element = document.querySelector(selector);
        if (element) {
            element.addEventListener(event, handler);
            this.eventRegistry.push({ selector, event, handler });
            console.log(`Registered event '${event}' for`, selector);
        } else {
            console.warn(`Element not found for event '${event}':`, selector);
        }
    }
}
