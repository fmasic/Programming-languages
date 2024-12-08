var selected_num = null; //no selected numbers initially
var selected_tile = null; //no selected tiles initially
var errors = 0; //setting the error count
var solution = []; // creating an empty solution matrix which will store the solved board
var board = []; // creating an empty board matrix which will store randomized numbers

window.onload = function () {
    generateSudoku(); 
    setGame(); 
    createClearButton();
    createNewGameButton();
    createSudokuSolver();
}; //Creating a window that calls a function to generate a sudoku matrix, create a clear, new game and sudoku solver button and creates the actual sudoku grid

function generateSudoku() {
    solution = Array.from({ length: 9 }, () => Array(9).fill(0)); //creating a grid filled with zeros
    fillBoard(solution); //fill the board with valid solutions

    board = JSON.parse(JSON.stringify(solution)); //cloning the solution so some of the numbers can be taken out to create the game aspect
    removeNumbers(board); //remove some numbers from the solved board
}

function fillBoard(grid) {
    const isSafe = (row, col, num) => { //checking if the placed number is valid
        for (let i = 0; i < 9; i++) {
            if (grid[row][i] === num || grid[i][col] === num) return false;
        } //checking rows and columns to see if the placed number is valid
        const startRow = row - (row % 3); 
        const startCol = col - (col % 3); 
        for (let i = startRow; i < startRow + 3; i++) {
            for (let j = startCol; j < startCol + 3; j++) {
                if (grid[i][j] === num) return false;
            }
        }
        return true; // checking the subgrid for the placed number aka checking if we didnt place a number we already have
    };

    const solve = () => {
        for (let row = 0; row < 9; row++) {
            for (let col = 0; col < 9; col++) {
                if (grid[row][col] === 0) { //checking for an empty cell
                    const nums = shuffle([1, 2, 3, 4, 5, 6, 7, 8, 9]); // calling shuffle function to shuffle the numbers 
                    for (let num of nums) {
                        if (isSafe(row, col, num)) { //checking if the number is safe
                            grid[row][col] = num; //place the number
                            if (solve()) return true;
                            grid[row][col] = 0;
                        }
                    }
                    return false;
                }
            }
        }
        return true; //grid is solved
    };

    solve();
}

function removeNumbers(grid) {
    let attempts = 30; //more attempts makes the game harder
    while (attempts > 0) { 
        const row = Math.floor(Math.random() * 9); //random row
        const col = Math.floor(Math.random() * 9); //random column
        if (grid[row][col] !== 0) { //if cell is not empty 
            grid[row][col] = 0; //remove number
            attempts--;
        }
    }
}

function shuffle(array) {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]]; //swap elements
    }
    return array;
}

function setGame() {
    for (let i = 1; i <= 9; i++) {
        let num = document.createElement("div");
        num.id = i;
        num.innerText = i;
        num.addEventListener("click", selectNum);
        num.classList.add("number");
        document.getElementById("digits").appendChild(num);
    }

    for (let row = 0; row < 9; row++) {
        for (let col = 0; col < 9; col++) {
            let tile = document.createElement("div");
            tile.id = row.toString() + "-" + col.toString();
            if (board[row][col] !== 0) {
                tile.innerText = board[row][col];
                tile.classList.add("tile-start");
            }
            if (row === 2 || row === 5) {
                tile.classList.add("horizontal-line");
            }
            if (col === 2 || col === 5) {
                tile.classList.add("vertical-line");
            }
            tile.addEventListener("click", selectTile);
            tile.classList.add("tile");
            document.getElementById("board").append(tile);
        }
    }
}

function selectNum() {
    if (selected_num != null) {
        selected_num.classList.remove("number-selected");
    }
    selected_num = this;
    selected_num.classList.add("number-selected");
}

function selectTile() {
    if (selected_num) {
        if (this.innerText !== "") {
            return;
        }

        let coords = this.id.split("-");
        let row = parseInt(coords[0]);
        let col = parseInt(coords[1]);

        if (solution[row][col] == selected_num.id) {
            this.innerText = selected_num.id;
             checkCompletedDigit(selected_num.id);
        } else {
            errors += 1;
            document.getElementById("errors").innerText = errors;
            if (errors === 3) {
                quitGame();
            }
        }
    }
}

function checkCompletedDigit(digit) {
    let count = 0;
    for (let row = 0; row < 9; row++) {
        for (let col = 0; col < 9; col++) {
            if (document.getElementById(`${row}-${col}`).innerText === digit) {
                count++;
            }
        }
    }

    if (count === 9) {
        let numButton = document.getElementById(digit);
        numButton.classList.add("digit-highlight"); 
        numButton.removeEventListener("click", selectNum); 
    }

    if(isBoardCompleted()){
        endCelebration();
    }
}

function isBoardCompleted(){
    for (let row = 0; row < 9; row++) {
        for (let col = 0; col < 9; col++) {
            let tile = document.getElementById(`${row}-${col}`);
            if(tile.innerText==="" || parseInt(tile.innerText) !== solution[row][col]){
                return false;
            }
        }}
        return true;
}

function endCelebration(){
    alert("Congrats! Board is complete.");
    newGame();
  
}

function clearBoard() {
    for (let row = 0; row < 9; row++) {
        for (let col = 0; col < 9; col++) {
            let tile = document.getElementById(`${row}-${col}`);
            if (!tile.classList.contains("tile-start")) {
                tile.innerText = "";
            }
        }
    }
    errors = 0;
    document.getElementById("errors").innerText = errors;
    for (let i = 1; i <= 9; i++) {
        const digit = document.getElementById(i);
        if (digit) {
            digit.classList.remove("digit-highlight"); 
            digit.addEventListener("click", selectNum); 
        }
    }
}

function createClearButton() {
    const clearButton = document.createElement("button");
    clearButton.id = "clear-board";
    clearButton.innerText = "CLEAR BOARD";
    clearButton.addEventListener("click", clearBoard);
    document.getElementById("controls").appendChild(clearButton);
}

function createNewGameButton(){
    const newGameButton = document.createElement("button");
    newGameButton.id = "new-game";
    newGameButton.innerText = "NEW GAME";
    newGameButton.addEventListener("click", newGame);
    document.getElementById("controls").appendChild(newGameButton);
}

function newGame(){
    errors=0; 
    document.getElementById("errors").innerText=errors;
    selected_num = null; 
    selected_tile = null;
    generateSudoku();
    document.getElementById("board").innerHTML=""; 
    setGame();
}

function createSudokuSolver() {
    const sudokuSolver = document.createElement("button");
    sudokuSolver.id = "sudoku-solver";
    sudokuSolver.innerText = "SOLVE BOARD";
    sudokuSolver.addEventListener("click", solveSudoku);
    document.getElementById("controls").appendChild(sudokuSolver);
}

function solveSudoku(){
    for (let row = 0; row < 9; row++) {
        for (let col = 0; col < 9; col++) {
            const tile = document.getElementById(`${row}-${col}`);
            tile.innerText=solution[row][col];
            tile.classList.add("tile-solved");
        } 
    }
}

function quitGame() {
    alert("GAME OVER! No more moves."); //display alert ui if user reaches 3 errors
    const tiles = document.querySelectorAll(".tile"); 
    tiles.forEach((tile) => (tile.onclick = null)); //disable tile clicks
    for (let i = 1; i <= 9; i++) { //reset digits
        const digit = document.getElementById(i);
        if (digit) {
            digit.classList.remove("digit-highlight", "number-selected"); 
            digit.removeEventListener("click", selectNum);
            digit.addEventListener("click", selectNum); 
        } //as digits reset disable event listener and all specific functions
    }
    errors=0; //reset errors
    document.getElementById("errors").innerText=errors; //update error display
    generateSudoku(); //generate new sudoku
    document.getElementById("board").innerHTML=""; //update sudoku display
    setGame(); //reinitialize the game
}


