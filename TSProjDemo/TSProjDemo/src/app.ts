// Bridge output file needs to be loaded explicitly
// to initialize all types and data structures.
require("../dist/bridge/bridge.js");
require("../dist/bridge/TSProjDemo.Bridge.js");

import { MathDemo } from "./mathDemo";

// Run demo:
const demo = new MathDemo();
demo.Run();
