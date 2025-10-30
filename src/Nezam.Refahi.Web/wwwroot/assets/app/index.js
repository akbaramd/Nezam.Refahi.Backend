import {Counter} from "./core/Counter.js";
import {RazorX} from "./core/RazorX.js";


const razorX = new RazorX();
const counterComponent = new Counter({
    url: "/Counter/RenderCounter"
});

razorX.registerComponent(counterComponent, "#counterRoot");