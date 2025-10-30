export class BlazorishComponent {
    constructor(props = {}) {
        this.props = props;
        this.state = {};
        this.root = null;                // DOM element for mounting
        this._isInitialized = false;     // Track if onInitialized has run
        this._hasRendered = false;       // Track if the component has ever rendered

        this.onInitialized();            // Blazor-like lifecycle
        this._isInitialized = true;
    }

    /**
     * Lifecycle: Called once after construction.
     * In Blazor: OnInitialized / OnInitializedAsync
     */
    onInitialized() {
        // Subclasses can override to perform setup or fetch initial data.
    }

    /**
     * Public method to update the component's state.
     * After updating, it triggers a re-render.
     * In Blazor: StateHasChanged()
     */
    setState(nextState) {
        this.state = { ...this.state, ...nextState };
        this.render();
    }

    /**
     * Main render method: fetch partial HTML from ASP.NET Core and inject into DOM.
     */
    async render() {
        if (!this.root) return;  // If there's no DOM element, skip.

        const requestData = { ...this.props, ...this.state };
        const html = await this.fetchPartialView(requestData);

        if (html) {
            this.root.innerHTML = html;

            // If this is the first successful render, mark it
            const wasFirstRender = !this._hasRendered;
            this._hasRendered = true;

            // Blazor-like post-render hook
            // Pass wasFirstRender as an argument if you want your child classes 
            // to know if it's the first render or not
            this.onAfterRender(wasFirstRender);
        }
    }

    /**
     * AJAX call to retrieve server-rendered HTML from an MVC partial view.
     */
    async fetchPartialView(data) {
        try {
            const response = await fetch(this.props.url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "X-RazorX": "true"
                },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                return await response.text();
            }
            console.error("Failed to fetch partial view:", response.statusText);
        } catch (e) {
            console.error("Error fetching partial view:", e);
        }
        return "";
    }

    /**
     * Lifecycle: Called after each successful render.
     * In Blazor: OnAfterRender / OnAfterRenderAsync
     */
    onAfterRender(isFirstRender) {
        // Subclasses can override to run logic after the DOM is updated.
        // `isFirstRender` indicates if this was the *very first* render.
    }
}
