using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SudokuApp
{
    public partial class Form1 : Form
    {
        const int SIZE = 9; //Defines a constant size for the 9x9 sudoku grid
        Label[] numberLabels = new Label[SIZE]; //Creates an array used to create the number buttons below the grid
        Random random = new Random(); //Object of type Random used to generate random numbers
        TextBox[,] gridLayout = new TextBox[SIZE, SIZE]; //Creates an array used for creating the 9x9 sudoku grid, and takes user input
        Label mistakeLabel; //Creates a label for the mistake counter
        int[,] solution = new int[SIZE, SIZE]; //Creates an array to hold the solution of each generated sudoku puzzle
        int mistakeCount = 0; 
        public Form1()
        {
            InitializeComponent(); //Initializes the appearance of the window from Form1.Designer.cs
            InitializeGrid();
            InitializeNumberLabels();
            InitializeMistakeCounter();
            InitializeSolveButton();
            InitializeRestartButton();          //Initializes all the labels and buttons
            InitializeHowToPlayButton();
            InitializeSudokuLabel();
            InitializeNewGameButton();
            GenerateSudoku(); //Generates a new Sudoku puzzle

            foreach (TextBox cell in gridLayout)
            {
                cell.KeyPress += TextBox_KeyPress; //Calls for a validation check for each cell in the grid
            }

        }
        

        private void InitializeGrid()
        {

            TableLayoutPanel gridPanel = new TableLayoutPanel //Creates a new table layout panel
            {
                RowCount = SIZE, //Creating a grid with 9 rows
                ColumnCount = SIZE, //Creating a 9 columns
                AutoSize = true, //Panel will automatically adjust its size to fit the TextBoxes
                AutoSizeMode = AutoSizeMode.GrowAndShrink, //Panel will grow or shrink based on its contents
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single, //Adds single line borders around each cell in the panel
            };

            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    TextBox cell = new TextBox //Creates a new TextBox 
                    {
                        Width = 105,
                        Height = 200,
                        TextAlign = HorizontalAlignment.Center,
                        MaxLength = 1, //Number of characters the user can enter
                        Font = new Font("Arial", 30),
                        BorderStyle = BorderStyle.Fixed3D, //Gives a 3D border to the cells
                        BackColor = Color.SkyBlue, //Sets background color
                        Margin = new Padding(0), //Removes any additional padding around the cells
                        
                    };

                    if (row % 3 == 2){ //Identifies the last 3 row blocks in a 3x3 grid
                        cell.BorderStyle = BorderStyle.Fixed3D;
                        cell.Margin = new Padding(0, 0, 0, 13); 
                        cell.BackColor = Color.SkyBlue;
                    }
                    if (col % 3 == 2){ //Identifies the last 3 columns blocks in a 3x3 grid
                        cell.BorderStyle = BorderStyle.Fixed3D; 
                        cell.Margin = new Padding(0, 0, 13, 0);
                        cell.BackColor = Color.SkyBlue;
                    }

                    cell.TextChanged += TextBox_TextChanged; //Call to event when user enters a value
                    gridLayout[row, col] = cell; //Stores the created TextBox cells in the grid
                    gridPanel.Controls.Add(cell, col, row); //Adds the TextBoxes to their correct position
                }
            }
            
            gridPanel.Location = new Point(415, 230); //Sets the location for the grid
            this.Controls.Add(gridPanel); //Adds the component to the form
        }

        private void InitializeNumberLabels()
        {
            int gridY = 840; //Y-coordinate for the grid location
            int gridHeight = (SIZE * 40 + 10); //Estimate for the grids height (40 for each textbox and 10 for border)
            int startY = gridY + gridHeight; //Combines the two heights to locate the labels below and in line with the grid
            for(int num = 0; num < SIZE; num++){

                Label numberLabel = new Label //Creates one Label for each number 1-9
                {
                    Width = 100,
                    Height = 100,
                    Text = (num + 1).ToString(), //Turns each number to a string
                    Font = new Font("Arial", 30),
                    AutoSize = false, //Prevents the labels from resizing based on the content they hold
                    TextAlign = ContentAlignment.MiddleCenter,
                    BorderStyle = BorderStyle.Fixed3D,
                    BackColor = Color.SkyBlue,
                    ForeColor = Color.White,
                    Location = new Point(num * 115 + 405, startY),
                };

                numberLabel.Click += NumberLabel_Click; //Call to event when one of the labels is clicked
                numberLabels[num] = numberLabel; //Stores the labels in the numberLabels array
                this.Controls.Add(numberLabel); //Adds the component to the form
            }
        }

        private void InitializeMistakeCounter()
        {

            mistakeLabel = new Label //Creates a new label for the mistake counter
            {
                Width = 500,
                Height = 100,
                Text = "Mistakes: 0",
                Font = new Font("Arial", 20),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(660, 135),
            };

            this.Controls.Add(mistakeLabel); //Adds the component to the form
        }

        private void InitializeSolveButton()
        {
            Button solveButton = new Button //Creates a new button
            {
                Text = "Solve",
                Font = new Font("Arial", 16),
                Size = new Size(250, 80), //Sets the size of the button
                Location = new Point(500, 1350), 
                BackColor = Color.SkyBlue,
                ForeColor = Color.White
            };

            solveButton.Click += SolveButton_Click; //Call to event when the button is clicked
            this.Controls.Add(solveButton); //Adds button to form
            solveButton.BringToFront(); //Brings the button to front
        }

        private void InitializeRestartButton()
        {
            Button restartButton = new Button 
            {
                Text = "Restart",
                Font = new Font("Arial", 16),
                Size = new Size(250, 80),
                Location = new Point(1080, 1350), 
                BackColor = Color.SkyBlue,
                ForeColor = Color.White
            };

            restartButton.Click += RestartButton_Click; //Call to event when button is clicked
            this.Controls.Add(restartButton);
            restartButton.BringToFront();
        }

        private void InitializeHowToPlayButton()
        {
            Button howToPlayButton = new Button
            {
                Text = "?",
                Font = new Font("Arial", 16),
                Size = new Size(90, 80),
                Location = new Point(1338, 1350), 
                BackColor = Color.SkyBlue,
                ForeColor = Color.White
            };

            howToPlayButton.Click += HowToPlayButton_Click; //Call to event when button is clicked
            this.Controls.Add(howToPlayButton);
            howToPlayButton.BringToFront();
        }

        private void InitializeSudokuLabel()
        {
            Label sudokuLabel = new Label
            {
                Text = "Sudoku",
                Font = new Font("Arial", 40),
                Size = new Size(1000, 140),
                Location = new Point(700, 35),
                ForeColor = Color.FromArgb(90,185,222),
            };

            this.Controls.Add(sudokuLabel);
        }   

        private void InitializeNewGameButton()
        {
            Button newGameButton = new Button
            {
                Text = "New Game",
                Font = new Font("Arial", 16),
                Size = new Size(270, 80),
                Location = new Point(780, 1350), 
                BackColor = Color.SkyBlue,
                ForeColor = Color.White 
            };

            newGameButton.Click += NewGameButton_Click; //Call to event when button is clicked
            this.Controls.Add(newGameButton);
            newGameButton.BringToFront();
        }

        private void FillDiagonalBlocks(int[,] board)
        {
            int blockSize = (int)Math.Sqrt(SIZE); //Determines the size of each block (3x3)

            for (int i = 0; i < SIZE; i += blockSize) //Iterates over the diagonal blocks (top left, middle, bottom right)
            {
                FillBlock(board, i, i); //Calls FillBlock to fill the diagonal 3x3 blocks
            }
        }

        private void FillBlock(int[,] board, int a, int b)
        {
            int[] numbers = ShuffleNumbers(); //Recieves an array with shuffled numbers
            int index = 0;

            for (int row = 0; row < Math.Sqrt(SIZE); row++)
            {
                for (int col = 0; col < Math.Sqrt(SIZE); col++)
                {
                    board[a + row, b + col] = numbers[index++]; //Locates the cells from the diagonal blocks and fills them with shuffled numbers
                }
            }
        }

        private int[] ShuffleNumbers()
        {
            int[] numbers = Enumerable.Range(1, SIZE).ToArray(); //Creates a sequence of numbers 1-9 and makes it into an array
            for (int i = 0; i < SIZE; i++)
            {
                int swapIndex = random.Next(i, SIZE); //Selects a random index from 0-8
                int temp = numbers[i]; //Temporarily stores the number at index i
                numbers[i] = numbers[swapIndex]; //Replaces the value at i with the value at swapIndex
                numbers[swapIndex] = temp; //Places the temporarily stored number into swapIndex
            }
            return numbers;
        }

        private void DisplayBoard(int[,] board)
        {
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    var cell = gridLayout[row, col]; //Gets the TextBox at a specific position from the grid

                    cell.Text = board[row, col] == 0 ? "" : board[row, col].ToString(); //Checks if the cell is empty (if yes, display empty cell, if no, display the number as a string)

                    if (board[row, col] != 0)  //Checks if the number is pre set
                    {
                        cell.ForeColor = Color.White;
                        cell.ReadOnly = true; //Cannot be edited
                        cell.BackColor = Color.FromArgb(90,185,222);
                    }
                    else  //Else the number is a user input value
                    {
                        cell.ForeColor = Color.White;
                        cell.ReadOnly = false; //Can be edited
                        cell.BackColor = Color.SkyBlue; 
                    }
                }
            }
        }

        private bool FillSudoku(int[,] board)
        {
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    if (board[row, col] == 0) //Identifies empty cells which need to be filled
                    {
                        for (int num = 1; num <= SIZE; num++) //Loop through possible number entries
                        {
                            if (IsSafe(board, row, col, num)) //Determines if the number can be used based on Sudoku rules
                            {
                                board[row, col] = num; //Stores the number

                                if (FillSudoku(board)) //Attempts to continue filling the board with numbers
                                    return true;

                                board[row, col] = 0; //Resets the current cell to be empty if the number does not pass by Sudoku rules
                            }
                        }

                        return false; //No valid number can be entered in the cell
                    }
                }
            }
            return true; //Confirms that the board has been successfully filled
        }

        private bool IsSafe(int[,] board, int row, int col, int num)
        {
            for (int x = 0; x < SIZE; x++)
            {
                if (board[row, x] == num || board[x, col] == num) //Checks if the number already exists in the row and column
                    return false;
            }

            int blockStartRow = row - row % (int)Math.Sqrt(SIZE); //Calculates the starting row index
            int blockStartCol = col - col % (int)Math.Sqrt(SIZE); //Calculates the starting column index

            for (int i = 0; i < Math.Sqrt(SIZE); i++) 
            {
                for (int j = 0; j < Math.Sqrt(SIZE); j++)
                {
                    if (board[i + blockStartRow, j + blockStartCol] == num) //Checks if the number already exists in the 3x3 block
                        return false;
                }
            }
            return true; //It is safe to place the number in the cell
        }

        private void RemoveNumbers(int[,] board, int count)
        {
            while (count > 0) 
            {
                int row = random.Next(SIZE); //Gives a random number
                int col = random.Next(SIZE); //Gives a random number

                if (board[row, col] != 0) //Checks if the cell is not already empty
                {
                    board[row, col] = 0; //Sets the cell to be empty, erasing the number
                    count--;
                }
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox; //Casts the sender object to a TextBox type to allow interraction

            if (string.IsNullOrEmpty(textBox.Text)) //Checks if the TextBox is empty
            {
                textBox.BackColor = textBox.ReadOnly ? Color.FromArgb(90,185,222) : Color.SkyBlue; //Resets to default colors
                return;
            }

            int enteredValue = int.Parse(textBox.Text); //Converts the value to an integer and validates it

            int row = -1, col = -1; //Initializes the variables so they can store the position of the current TextBox 

            for (int r = 0; r < SIZE; r++)
            {
                for (int c = 0; c < SIZE; c++)
                {       
                    if (gridLayout[r, c] == textBox) //Compares the current cell with the TextBox that triggered the event
                    {
                        row = r; 
                        col = c;
                        break;
                    }
                }
                if (row != -1)
                {
                    break; //Exits the loop for efficiency  
                } 
            }

            if (row != -1 && col != -1) //Ensure the TextBox was found in the grid
            {
                if (solution[row, col] == enteredValue) //Compares the entered value with the correct solution of the puzzle
                {
                    textBox.BackColor = Color.LightGreen; //If its correct, change background color to green
                    CheckNumberCompletion(enteredValue); //Checks if one number has been placed in all right positions
                    CheckForCompletion(); //Check if the puzzle has been solved completely
                }       
                else
                {
                    textBox.BackColor = Color.Red; //Change background color to red if the number doesnt match the solution
                    IncrementMistakeCounter(); //Increment number of mistakes
                }
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsDigit(e.KeyChar) || e.KeyChar == '0') //Check if entry is non numeric or zero
            {
                if(e.KeyChar != (char)Keys.Back) //Allows backspace to be used for deleting entry
                {
                    e.Handled = true; //Prevents the entry from being entered
                }
            }
        }

        private void CheckNumberCompletion(int number)
        {
            bool allCorrect = true; //Tracks if all number entries are correct
            int correctGuesses = 0; //Tracks how many cells contain the correct number

            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    if (gridLayout[row, col].Text == number.ToString()) //Checks if the cell contains the specific number
                    {
                        if (solution[row, col] == number) //Check if the user-entered numbers are correct
                        {
                            correctGuesses++; //Increment the number or correct guesses
                        }
                        else
                        {
                            allCorrect = false; //It means that the number was not entered correctly in all places
                        }
                    }
                }
            }

            if (allCorrect && correctGuesses == SIZE) //Checks if all instances of the number are correctly entered
            {
                Label numberLabel = numberLabels[number - 1]; //Disables corresponding number label 
                numberLabel.BackColor = Color.LightGray;  //Changes the color to indicate completion
                numberLabel.Enabled = false;  //Disables the label

                foreach (TextBox cell in gridLayout)
                {
                    if (int.TryParse(cell.Text, out int cellValue) && cellValue == number) //Validates if the number is correct again
                    {
                        cell.BackColor = Color.LightGreen;  //Keeps the correct numbers highlighted in green
                    }
                }
            }
        }       

        private void CheckForCompletion()
        {
            bool isComplete = true;

            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    if (string.IsNullOrEmpty(gridLayout[row, col].Text) || int.Parse(gridLayout[row, col].Text) != solution[row, col]) //Checks if the cell is empty or if the entered values do not match the solution of the puzzle
                    {
                        isComplete = false; //Incomplete Sudoku puzzle
                        break;
                    }
            	}

                if (!isComplete) break; //Stops checking if the puzzle is completed
            }

            if (isComplete)
            {
                this.Controls.Clear(); //Removes all components from the form 

                PictureBox youWin = new PictureBox //Inserts an image
                {
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Size = new Size(1000, 860),
                    Location = new Point(400, 400),
                    Image = Image.FromFile(@"C:\Users\User\Desktop\Sudoku\SudokuApp\gameWon.jpg")
                };

                this.Controls.Add(youWin); //Add the image to the form
                
                Label youWinLabel = new Label //Creates a text label
                {
                    Text = "You Win!",
                    Font = new Font("Courier New", 50),
                    Size = new Size(1000, 300),
                    Location = new Point(600, 160),
                    ForeColor = Color.FromArgb(228, 155, 15),
                };

                this.Controls.Add(youWinLabel);
                youWinLabel.BringToFront();

                Button playMoreButton = new Button //Creates a button for new game
                {
                    Text = "Play More?",
                    Font = new Font("Courier New", 20),
                    Size = new Size(400, 100),
                    BackColor = Color.Yellow,
                    ForeColor = Color.FromArgb(139, 128, 0),
                    Location = new Point(680, 1300)
                };

                playMoreButton.Click += (s, e) => Application.Restart(); //Restarts the app, generating a new puzzle
                this.Controls.Add(playMoreButton);
                playMoreButton.BringToFront();

                foreach (Control control in this.Controls.OfType<Control>().ToList()) //Loops through all controls on the form
                {
                    if (!(control is PictureBox || (control is Label && control.Text == "You Win!") || (control is Button && control.Text == "Play More?")))  //Removes all the controls except the picture, label and button
                    {
                        this.Controls.Remove(control);
                    }
                }
            }
        }

        private void IncrementMistakeCounter()
        {
            mistakeCount++;
            mistakeLabel.Text = $"Mistakes: {mistakeCount}"; //Set to change the text as the counter is incremented

            if (mistakeCount >= 3)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            this.Controls.Clear(); //Removes all components from the form

            PictureBox gameOverGif = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(1000, 600),
                Location = new Point(400, SIZE * 40 + 65),
                Image = Image.FromFile(@"C:\Users\User\Desktop\Sudoku\SudokuApp\gameOverGif.gif")
            }; 

            this.Controls.Add(gameOverGif);

            Button tryAgainButton = new Button
            {
                Text = "Try Again",
                Font = new Font("Courier New", 20),
                Size = new Size(400, 100),
                BackColor = Color.Yellow,
                ForeColor = Color.FromArgb(139, 128, 0),
                Location = new Point(660, 1000)
            };
            
            tryAgainButton.Click += (s, e) => Application.Restart();
            this.Controls.Add(tryAgainButton);
            tryAgainButton.BringToFront();

            foreach (Control control in this.Controls.OfType<Control>().ToList())
            {
                if (!(control is PictureBox || control is Button && control.Text == "Try Again")) //Remove every control except the gif and button
                {
                    this.Controls.Remove(control);
                }
            }

            
        }

        private void SolveButton_Click(object sender, EventArgs e)
        {
            int[,] board = new int[SIZE, SIZE]; //Represents the Sudoku 9x9 grid
    
            // Copy the current board state into the board variable
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    int.TryParse(gridLayout[row, col].Text, out board[row, col]); //Converts the text from the grid into an integer, if invalid or empty, value will be 0
                }
            }

            foreach (var cell in gridLayout)
            {
                cell.TextChanged -= TextBox_TextChanged; //Unsubscribes from the event temporarily to prevent triggering the mistake counter
            }

            if (FillSudoku(board)) //Fills the board, returns true if correctly filled
            {
                DisplayBoard(board); //Displays the solved board on the grid
            }
            else
            {
                MessageBox.Show("This puzzle cannot be solved!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //In case an unsolvable board is generated
            }

            foreach (var cell in gridLayout)
            {
                cell.TextChanged += TextBox_TextChanged; //Resubscribes to the event after solving the puzzle
            }
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    var cell = gridLayout[row, col]; //Takes the current cell from the grid

                    if (cell.ReadOnly == false) //If it is a user-entered number
                    {
                        cell.Text = string.Empty; //Deletes the input
                        cell.BackColor = Color.SkyBlue; //Resets the background color
                    }
                }
            }

            mistakeCount = 0; //Resets the mistake counter
            mistakeLabel.Text = $"Mistakes: {mistakeCount}"; //Changes the label

            foreach (Label numberLabel in numberLabels) //Resets the completed number labels
            {
                numberLabel.Enabled = true;
                numberLabel.BackColor = Color.SkyBlue;
                UnhighlightAllCells();
            }
        }    

        private int? currentlyHighlightedNumber = null; //Variable to track the currently highlighted number

        private void NumberLabel_Click(object sender, EventArgs e)
        {       
            Label clickedLabel = sender as Label; //Sender is set to a label to identify the clicked label
            int clickedNumber = int.Parse(clickedLabel.Text); //To find which number was clicked

            if (currentlyHighlightedNumber == clickedNumber) //Checks if the same number is clicked again
            {
                UnhighlightAllCells(); //Unhighlights all cells
                currentlyHighlightedNumber = null; //Resets the highlighted number
            }
            else
            {
                UnhighlightAllCells(); //If a different number is clicked, unhighlight the previous number label
                currentlyHighlightedNumber = clickedNumber; //Sets the currently highlighted number to the specified number
                HighlightMatchingCells(clickedNumber); //Highlights all the specified number
            }
        }       

        private void HighlightMatchingCells(int number)
        {
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    var cell = gridLayout[row, col]; //Takes the current cell from the grid

                    if (int.TryParse(cell.Text, out int cellValue) && cellValue == number) //Converts the cell value into an integer and checks if it matched the clickedNumber
                    {
                        cell.BackColor = Color.FromArgb(53,106,106); //If it matches, change the background color
                    }
                }
            }
        }

        private void UnhighlightAllCells()
        {
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    var cell = gridLayout[row, col]; //Takes the current cell from the grid
            
                    if (cell.ReadOnly) //Checks whether the cell was a preset one or a user-entered one
                    {
                        cell.BackColor = Color.FromArgb(90,185,222);
                    }
                    else if (string.IsNullOrEmpty(cell.Text)) //Checks if the cell is empty
                    {
                        cell.BackColor = Color.SkyBlue;
                    }
                    else if (int.TryParse(cell.Text, out int cellValue) && cellValue != solution[row, col]) //Checks if the value is incorrectly placed
                    {
                        cell.BackColor = Color.Red;
                    }
                    else //Else the cell contains the correct number
                    {
                        cell.BackColor = Color.LightGreen;
                    }
                }
            }
        }


        private void HowToPlayButton_Click(object sender, EventArgs e)
        {
            //Displays a popup window with how to play instructions
            MessageBox.Show("Here's a quick guide on how to play Sudoku: \n1. Objective: \n- Fill a 9x9 grid with numbers from 1 to 9. \n- Each row, column, and 3x3 subgrid must contain every number from 1 to 9 without repetition. \n2. Initial Setup:\n- Some numbers are pre-filled in the grid. These are clues to help you solve the puzzle. \n3. Rules: \n- Each number (1-9) can appear only once in each row, column, and 3x3 box. \n- Use logic to deduce the correct placement of numbers. \n4. How to Play: \n- Look for rows, columns, or boxes that are nearly filled in to help you narrow down possible numbers. \n- Try filling in a number that fits according to the Sudoku rules. \n- If you make a mistake, it will highlight the wrong cell, and you can correct it. \n- Be careful, three mistakes and you will lose. \n- If you get stuck, you can use the \"Solve\" button to see the completed puzzle or restart with the \"Clear\" button. \n5. Winning: \n- You win when all cells are correctly filled, and every row, column, and 3x3 box contains the numbers 1 to 9 without repetition. ", "How To Play?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }  

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            GenerateSudoku(); //Generates a new puzzle with each click
            mistakeCount = 0;
            mistakeLabel.Text = $"Mistakes: {mistakeCount}"; 
        }             

        private void GenerateSudoku()
        {
            int[,] board = new int[SIZE, SIZE]; //Declares a new array to be used for the Sudoku puzzle
            FillDiagonalBlocks(board); //Fills the diagonal blocks
            FillSudoku(board); //Fills the remaining part of the grid
            Array.Copy(board, solution, board.Length); //Makes a copy of the grid to store as a solution to the puzzle for validation and comparison
            RemoveNumbers(board, 40); //Removes 40 numbers from the grid to create a valid Sudoku puzzle
            DisplayBoard(board); //Displays all of the cells in the grid
        }
    }
}
