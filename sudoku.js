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
    createHowtoPlay();
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
    } // making the digits below the board, adding them to the document and making them clickable/ adding an event listener

    //this for loop is responsible for creating the sudoku grid 
    for (let row = 0; row < 9; row++) {
        for (let col = 0; col < 9; col++) {
            let tile = document.createElement("div"); // creating the tiles- div is basically a container used to group together elements and here we're grouping tiles
            tile.id = row.toString() + "-" + col.toString();
            if (board[row][col] !== 0) {
                tile.innerText = board[row][col];
                tile.classList.add("tile-start");
            } //once a board is randomized it will have 0's but since there are no 0's in sudoku we dont want them on the board so our condition is if
            //a certain number from the solutions is not 0 put the text on the tiles, 0's will be empty and thats what user has to find
            if (row === 2 || row === 5) {
                tile.classList.add("horizontal-line");
            }
            if (col === 2 || col === 5) {
                tile.classList.add("vertical-line"); //creating the subgrids by adding css styles vertical and horizontal line at certain points
            }
            tile.addEventListener("click", selectTile);
            tile.classList.add("tile");
            document.getElementById("board").append(tile);  //making the tiles clickable so once we click the digit we want to place we can click on the tile 
        }
    }
}

function selectNum() {
    if (selected_num != null) {
        selected_num.classList.remove("number-selected");
        unhighlightCells();
    } // with this we are toggling number selected so you can only select one digit below the board at a time and if user clicks on a new one number selected shading is removed
    selected_num = this; //refers to the div itself
    selected_num.classList.add("number-selected"); // adding our style class so basically shading a digit once user selects it to be placed

    const selectedNum=selected_num.id;
    highlightCells(selectedNum);
}

function highlightCells(number){
    const tiles=document.querySelectorAll(".tile");
    tiles.forEach((tile)=> {
        if(tile.innerText.trim()=== number){ //trim function deletes any whitespace that might cause a mismatch in the string comparison
            tile.classList.add("tile-highlight");
        }
    });
}

function unhighlightCells(){
    const tiles=document.querySelectorAll(".tile");
    tiles.forEach((tile)=> {
            tile.classList.remove("tile-highlight");
    });
}

function selectTile() {
    if (selected_num) {
        if (this.innerText !== "") {
            return;
        } //basically places selected digit on the board, but if the tile already has a number in it it returns (it doesnt change the number underneath)

        let coords = this.id.split("-");  
        let row = parseInt(coords[0]);
        let col = parseInt(coords[1]); //our ids look something like "0-0" "0-1"... we get the ids and split them between the dash and we get two individual numbers,
        //which creates an array of two individual numbers  ["0","1"] so we parseInt(function converts strings to integers) these digits since theyre strings.

        if (solution[row][col] == selected_num.id) {
            this.innerText = selected_num.id;
             checkCompletedDigit(selected_num.id); //checking if users number placed is correct using the solution array we've created 
        } else {
            errors += 1;
            document.getElementById("errors").innerText = errors;
            if (errors === 3) {
                quitGame(); //if not correct error count increases and when it hits 3 the game ends and a new board is generated and set
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
            }//looping thorugh the grid we are checking if all of our digits are placed (if 9 9's are placed count==9 shade highlight and remove event listener)
        }
    }

    if (count === 9) {
        let numButton = document.getElementById(digit);
        numButton.classList.add("digit-highlight"); 
        numButton.removeEventListener("click", selectNum); 
    } //adds highligh and removes event listener to any digit that is fully placed on the board thats why we use count to check if each digit appears 9 times meaning its done

    if(isBoardCompleted()){
        endCelebration();
        newGame();
        for(let i=1;i<=9;i++){
            const digit=document.getElementById(i);
            if(digit){
                digit.classList.remove("digit-highlight","number-selected");
                digit.addEventListener("click",selectNum);
            }
        }
    }
}

function isBoardCompleted(){
    for (let row = 0; row < 9; row++) {
        for (let col = 0; col < 9; col++) {
            let tile = document.getElementById(`${row}-${col}`);
            if(tile.innerText==="" || parseInt(tile.innerText) !== solution[row][col]){ //we string compare the users input to the solution board if everything is complete return true
                return false;
            }
        }}
        return true; //else true
}

function endCelebration(){
    const modal=document.createElement("div");
    modal.id="celebration-modal";
    modal.style.position="fixed";
    modal.style.top="0";
    modal.style.left="0";
    modal.style.width="100%";
    modal.style.height="100%";
    modal.style.padding="20px";
    modal.style.backgroundColor="rgba(0,0,0,0.8)";
    modal.style.display="flex";
    modal.style.justifyContent="center";
    modal.style.alignItems="center";
    modal.style.zIndex="1000"; 

    const container=document.createElement("div");
    container.style.position="relative";
    container.style.padding="30px";
    container.style.backgroundColor="#B8860B";
    container.style.textAlign="center";
    container.style.maxWidth="500px";
    container.style.boxShadow="0 4px 8px rgba(0,0,0,0.2)";

    const playMore=document.createElement("button");
    playMore.innerText="Play More?";
    playMore.style.fontSize="25px";
    playMore.style.padding="10px 30px";
    playMore.style.backgroundColor="#FFFF00";
    playMore.style.color="#B8860B";
    playMore.style.border="none";
    playMore.style.borderRadius="5px";
    playMore.style.cursor="pointer";


    playMore.addEventListener("click",()=>{
        modal.remove();
    });

    const celeImage=document.createElement("img");
    celeImage.src="https://i.postimg.cc/qv1xd4fv/uwin.jpg";
    celeImage.style.width="100%";
    celeImage.style.borderRadius="10px";
    celeImage.style.width="400px";
    celeImage.style.height="400px";

    container.appendChild(celeImage);
    container.appendChild(playMore);
    modal.appendChild(container);
    document.body.appendChild(modal);
}

function clearBoard() {
    for (let row = 0; row < 9; row++) {
        for (let col = 0; col < 9; col++) {
            let tile = document.getElementById(`${row}-${col}`);
            if (!tile.classList.contains("tile-start")) {
                tile.innerText = "";
            } //similar logic used in the new game function: looping thorugh rows and columns and erasing all of the previously filled spots 
        }
    }
    errors = 0;
    document.getElementById("errors").innerText = errors; //reseting errors
    for (let i = 1; i <= 9; i++) {
        const digit = document.getElementById(i);
        if (digit) {
            digit.classList.remove("digit-highlight"); 
            digit.addEventListener("click", selectNum); 
        }
    } //this relates to the digits below the board so we are looping through the digits removing the higlight which appears once a number is all cleared (all 9's are
    //placed on the board for example.) once this happens the number is unclickable and has a highlight so we're removing that and adding the event listener
}

// creating the four buttons: clear, new game, how to play and solver:
function createClearButton() {
    const clearButton = document.createElement("button");
    clearButton.id = "clear-board";
    clearButton.innerText = "RESTART";
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

function createSudokuSolver() {
    const sudokuSolver = document.createElement("button");
    sudokuSolver.id = "sudoku-solver";
    sudokuSolver.innerText = "SOLVE BOARD";
    sudokuSolver.addEventListener("click", solveSudoku);
    document.getElementById("controls").appendChild(sudokuSolver);
} 

function createHowtoPlay() {
    const howToPlay = document.createElement("button");
    howToPlay.id = "how-to-play";
    howToPlay.innerText = "?";
    howToPlay.addEventListener("click", showPlayModal);
    document.getElementById("controls").appendChild(howToPlay);

    const modal=document.createElement("div");
    modal.id="how-to-play-modal";
    modal.style.display="none";
    modal.style.position="fixed";
    modal.style.top="50%";
    modal.style.left="50%";
    modal.style.transform="translate(-50%,-50%)";
    modal.style.padding="20px";
    modal.style.backgroundColor="white";
    modal.style.border="1px solid #000";
    modal.style.zIndex="1000"; //modal is like a ui which we are creating to store game instructions and this is the logic of creating such

    const modalContent=  `
    <div style="color: black; font-family: Arial, sans-serif; font-size: 16px; text-align: center;">
        <h2>Here's a quick guide on how to play Sudoku:</h2>
        <p>Objective: </p>
        <p>Fill a 9x9 grid with numbers from 1 to 9.</p>
        <p>Each row, column, and 3x3 subgrid must contain every number from 1 to 9 without repetition.</p>
        <p>Initial Setup:</p>
        <p>Some numbers are pre-filled in the grid. These are clues to help you solve the puzzle.</p>
        <p>Rules:</p>
        <p>Each number (1-9) can appear only once in each row, column, and 3x3 box.</p>
        <p>Use logic to deduce the correct placement of numbers.</p>
        <p>How to Play:</p>
        <p> Look for rows, columns, or boxes that are nearly filled in to help you narrow down possible numbers.</p>
        <p>Try filling in a number that fits according to the Sudoku rules. </p>
        <p>If you make a mistake, it will highlight the wrong cell, and you can correct it.</p>
        <p> Be careful, three mistakes and you will lose. </p>
        <p>If you get stuck, you can use the "Solve" button to see the completed puzzle or restart with the "Clear" button.</p>
        <p>Winning:</p>
        <p>You win when all cells are correctly filled, and every row, column, and 3x3 box contains the numbers 1 to 9 without repetition.</p>
        <button id="close-modal">Close</button>
    </div>
`; //inner text of the modal, we are making a div to be the container of our text

    modal.innerHTML=modalContent;
    document.body.appendChild(modal); //adding it to the document
    
    document.getElementById("close-modal").addEventListener("click", ()=> {
        modal.style.display="none";
    }); //adding an event listener for when we finish reading the instructions

} //all buttons function in the same way (except how to play since clicking it shows a modal): we create a css style that creates the look of our buttons they 
//are all the same style repeated, then we create an element in the document made possible by the controils div in the html file, we get its id and equate it 
//to the css function we made previously, we title the button add an event listener(making it able to perform the action we want aka make the button 
//clickable and when clicked to do something) and we connect it to the html file using append

function showPlayModal(){
    const modal=document.getElementById("how-to-play-modal");
    modal.style.display="block";
}

//a function that just resets the board/ creates a new game once the new game button is clicked, similar logic to the clear button function at the start but
//since we"re reseting everything and generating a whole new board we dont need to remove or add event listeners
function newGame(){
    errors=0; 
    document.getElementById("errors").innerText=errors; //errors reset to zero
    selected_num = null; 
    selected_tile = null; //reset all tiles
    generateSudoku(); //generate new board
    document.getElementById("board").innerHTML=""; 
    setGame(); 
}

//since previously we made a board number randomizer, meaning the numbers in each subgrid will always be different so you will never play the same game twice and we 
//removed some numbers to create the blanks that user fills and stored them in the solution array, this function just takes the solutions that we already need to have
//to be able to keep count of errors etc. and just print them in their spots by 1. adding the inner text in to each tile from the solution array and 2. marking each tile as solved
function solveSudoku(){
    for (let row = 0; row < 9; row++) {
        for (let col = 0; col < 9; col++) {
            const tile = document.getElementById(`${row}-${col}`);
            tile.innerText=solution[row][col];
            tile.classList.add("tile-solved");
        } 
    }
}

//a function whose main purpose is dealing with the issue of user reaching 3 errors, once user makes three errors the game is done and it resets
function quitGame() {
    const modal=document.createElement("div");
    modal.id="game-over-modal";
    modal.style.position="fixed";
    modal.style.top="0";
    modal.style.left="0";
    modal.style.width="100%";
    modal.style.height="100%";
    modal.style.padding="20px";
    modal.style.backgroundColor="rgba(0,0,0,0.8)";
    modal.style.display="flex";
    modal.style.justifyContent="center";
    modal.style.alignItems="center";
    modal.style.zIndex="1000"; 

    const container=document.createElement("div");
    container.style.position="relative";
    container.style.padding="30px";
    container.style.backgroundColor="#B8860B";
    container.style.textAlign="center";
    container.style.maxWidth="500px";
    container.style.boxShadow="0 4px 8px rgba(0,0,0,0.2)";

    const tryAgain=document.createElement("button");
    tryAgain.innerText="Try again?";
    tryAgain.style.fontSize="25px";
    tryAgain.style.padding="15px 30px";
    tryAgain.style.backgroundColor="#FFFF00";
    tryAgain.style.color="#B8860B";
    tryAgain.style.border="none";
    tryAgain.style.borderRadius="5px";
    tryAgain.style.cursor="pointer";


    tryAgain.addEventListener("click",()=>{
        modal.remove();
    });

    const gif=document.createElement("img");
    gif.src="https://media.giphy.com/media/1hMbkOaFfYmZvvEBq9/giphy.gif";
    gif.style.width="100%";
    gif.style.borderRadius="10px";
    gif.style.width="400px";
    gif.style.height="400px";

    container.appendChild(gif);
    container.appendChild(tryAgain);
    modal.appendChild(container);
    document.body.appendChild(modal);
    //display game over gif if user reaches 3 errors

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
