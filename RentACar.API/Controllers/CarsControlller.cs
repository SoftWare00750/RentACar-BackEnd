function fibonacciGenerator(n) {
    var output = [];

    if (n === 1) {
        output = [0];
    } 
    else if (n === 2) {
        output = [0, 1];
    } 
    else {
        output = [0, 1];
        
        // Start the loop from index 2 since we already have 0 and 1
        for (var i = 2; i < n; i++) {
            // Add the last two numbers in the current array
            var nextValue = output[output.length - 2] + output[output.length - 1];
            output.push(nextValue);
        }
    }

    return output;
}