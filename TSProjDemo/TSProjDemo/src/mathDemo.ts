///<reference path="../dist/bridge/TSProjDemo.Bridge.d.ts" />

import * as rl from "readline-sync";

export class MathDemo {
    Run(): void {
        console.log("*** Math Expression Evaluaton Demo ***");
        console.log("   (enter an empty string to exit)");
        console.log("");
        console.log("Sample: (1 + 2) * 3");
        console.log("Result: 9");
        console.log("");

        // Create an instance of Evaluator:
        const evaluator = new TSProjDemo.Bridge.Evaluator();

        let expr: string;
        while ((expr = rl.question("Math Expression: ")).length > 0) {
            try {
                // Evaluate:
                var res = evaluator.Evaluate(expr);
                console.log("Result: " + res);
            } catch (err) {
                console.log("[Error]: " + err.message);
            }

            console.log("");
        }
    }
}
