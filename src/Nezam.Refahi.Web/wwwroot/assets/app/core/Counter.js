import { BlazorishComponent } from "./RazorXComponent.js";

export class Counter extends BlazorishComponent {
    constructor(props) {
        super(props);
        // Initialize local state
        this.state = { count: 0 };
    }

    /**
     * Lifecycle: Called once during component construction.
     * Useful for initial setup or data loading.
     */
    onInitialized() {
        console.log("Counter: onInitialized");
        // E.g., you could fetch initial data or set up logging here.
    }

    /**
     * Lifecycle: Called after each render.
     * isFirstRender = true if it's the very first time the component has rendered.
     */
    onAfterRender(isFirstRender) {
        console.log("Counter: onAfterRender | firstRender =", isFirstRender);

        // Attach event listeners to the newly injected HTML
        const incrementBtn = this.root.querySelector("#increment");
        const decrementBtn = this.root.querySelector("#decrement");

        incrementBtn?.addEventListener("click", () => {
            this.setState({ count: this.state.count + 1 });
        });

        decrementBtn?.addEventListener("click", () => {
            this.setState({ count: this.state.count - 1 });
        });

        // If needed, you can do something special on the very first render:
        if (isFirstRender) {
            console.log("Counter: First render occurred. Doing setup...");
        }
    }
}
