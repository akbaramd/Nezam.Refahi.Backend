export class RazorX {
    constructor() {
        this.components = [];
    }

    registerComponent(component, selector) {
        const root = document.querySelector(selector);
        if (!root) {
            console.warn(`RazorX: No element found for selector ${selector}`);
            return;
        }
        component.root = root;
        this.components.push(component);
        component.render();
    }
}
