import tkinter as tk #imports the tkinter module, which is used for creating GUI applications in Python;alias tk used to simplify access to its classes and functions
import random #imports the random module, which provides functions to generate random numbers
from tkinter import messagebox #imports the messagebox module from tkinter, which is used to display message boxes

class SudokuGrid:
    """Constructor of the SudokuGrid class, and it's responsible for initializing the object when it is created"""
    def __init__(self, root): #2 arguments: root - main window (Tk object) and self - instance of the SudokuGrid class being created
        self.root = root
        self.root.title("Sudoku")
        self.board, self.solution = self.generate_sudoku()  #calls the generate_sudoku method (which generates a Sudoku puzzle and its solution) and stores the resulting puzzle and solution
        self.original_board = [row[:] for row in self.board]  # Original puzzle 
        self.entries = {} # empty dictionary which will store references to the tk.Entry widgets (used for inputting the numbers into the grid)
        self.selected_number = None # used for highlighting
        self.mistakes = 0 # used for tracking mistakes

        self.create_widgets() #used to build the GUI components and display them in the root window

    def create_widgets(self): #creating GUI components
        # Title
        self.title_label = tk.Label(self.root, text="Sudoku", font=("Courier", 20, "bold"), fg="blue")
        self.title_label.pack(pady=(10, 5))  # Add some padding for spacing

        # Mistake Counter
        self.mistake_label = tk.Label(self.root, text="Mistakes: 0", font=("Courier", 15), fg="red")
        self.mistake_label.pack() #pack is used to add it to the window

        # Main puzzle board is generated and displayed
        self.create_grid()

        # Controls (number selection buttons and action buttons)
        self.create_controls()

    def create_grid(self):
        self.grid_frame = tk.Frame(self.root)  # container for the 9x9 grid of Entry widgets
        self.grid_frame.pack(pady=10)

    # Define a validation function
        def validate_input(P):
            return P.isdigit() and 1 <= int(P) <= 9 or P == ""

    # Register the validation function
        vcmd = (self.root.register(validate_input), "%P")

        for i in range(9):
            for j in range(9):
            # Set subgrid colors based on (i, j) coordinates
                bg_color = "#80C1FF" if (i // 3 + j // 3) % 2 == 0 else "#BAD7F2"

                entry = tk.Entry(
                    self.grid_frame,
                    width=2,
                    font=("Courier", 18, "bold"),
                    justify="center",
                    bg=bg_color,
                    readonlybackground=bg_color,  # Match the read-only background color
                    validate="key",
                    validatecommand=vcmd,  # Attach the validation command
            )
                entry.grid(
                    row=i,
                    column=j,
                    padx=(2 if j % 3 == 0 else 0, 2 if j % 3 == 2 else 0),
                    pady=(2 if i % 3 == 0 else 0, 2 if i % 3 == 2 else 0),
            )

                if self.board[i][j] != 0:  # Preset numbers
                    entry.insert(0, self.board[i][j])
                    entry.config(state="readonly", fg="black")  # Preset cells are black
                else:  # Allow editing for user inputs
                    entry.config(state="normal", fg="black")  # Editable cells are black
                    entry.bind("<FocusOut>", lambda e, x=i, y=j: self.check_user_input(x, y))  # Bind focus-out event

                self.entries[(i, j)] = entry

    def create_controls(self):
        # Control Frame
        self.control_frame = tk.Frame(self.root) #creates container for buttons
        self.control_frame.pack(pady=10)

        # Number selection buttons (will be at the top)
        self.number_frame = tk.Frame(self.control_frame)
        self.number_frame.pack()

        for num in range(1, 10):
            btn = tk.Button(self.number_frame, text=str(num), font=("Courier", 12, "bold"), width=3, fg="#005EB5", bg="#BAF2E9",
                        command=lambda n=num: self.highlight_number(n))
            btn.pack(side="left", padx=2)

        # Action Buttons (below the number buttons)
        self.action_frame = tk.Frame(self.control_frame)
        self.action_frame.pack()

        # Solve Button
        solve_btn = tk.Button(self.action_frame, text="Solve", font=("Courier", 12, "bold"), width=8, fg="#00274B", bg="#BAF2E9", command=self.solve_board)
        solve_btn.pack(side="left", padx=10)

        # New Game Button
        new_game_btn = tk.Button(self.action_frame, text="New Game", font=("Courier", 12, "bold"), width=8, fg="#00274B", bg="#BAF2E9", command=self.new_game)
        new_game_btn.pack(side="left", padx=10)

        # Restart Button
        restart_btn = tk.Button(self.action_frame, text="Restart", font=("Courier", 12, "bold"), width=8, fg="#00274B", bg="#BAF2E9", command=self.restart_game)
        restart_btn.pack(side="left", padx=10)

        # How to Play Button
        how_to_play_btn = tk.Button(self.action_frame, text="?", font=("Courier", 12, "bold"), width=8, fg="#00274B", bg="#BAF2E9", command=self.show_how_to_play)
        how_to_play_btn.pack(side="left", padx=10)

    def highlight_number(self, number):
        if self.selected_number == number:
        # If the same number is selected again, unhighlight all cells
            self.selected_number = None
            for (i, j), entry in self.entries.items():
            # Reset the background color for all cells
                bg_color = "#80C1FF" if (i // 3 + j // 3) % 2 == 0 else "#BAD7F2"
                entry.config(bg=bg_color)  # Reset background color
                entry.config(highlightbackground="lightgray")  # Reset border color
                if entry.cget("state") == "readonly":  # Reset readonly background color as well
                    entry.config(readonlybackground=bg_color)
        else:
        # Highlight the selected number
            self.selected_number = number
            for (i, j), entry in self.entries.items():
                value = entry.get()
                if value == str(number):
                    if entry.cget("state") == "normal":  # Editable cells
                        entry.config(bg="yellow")  # Highlight editable cells
                        entry.config(highlightbackground="yellow")  # Highlight border for editable cells
                    else:  # Readonly cells (preset numbers)
                        entry.config(bg="yellow")  # Highlight background for preset cells
                        entry.config(highlightbackground="yellow")  # Highlight border for preset cells
                        entry.config(readonlybackground="yellow")  # Also change the readonly background color
                else:
                # Reset the background color for other cells
                    bg_color = "#80C1FF" if (i // 3 + j // 3) % 2 == 0 else "#BAD7F2"
                    entry.config(bg=bg_color)  # Reset background color for other cells
                    entry.config(highlightbackground="lightgray")  # Reset border color for other cells
                    if entry.cget("state") == "readonly":
                        entry.config(readonlybackground=bg_color)  # Reset readonly background color


    def check_user_input(self, row, col):
        entry = self.entries[(row, col)]
        user_input = entry.get()

        if user_input.isdigit():
            user_input = int(user_input)

        # Check if user input is correct
            if user_input != self.solution[row][col]:
                if entry.cget("fg") != "red":  # Prevent unnecessary color updates
                    self.mistakes += 1
                    self.mistake_label.config(text=f"Mistakes: {self.mistakes}")
                    entry.config(fg="red")
                    if self.mistakes == 3:  # Game Over on 3 mistakes
                        self.game_over()
            else:
                entry.config(fg="green")

        elif user_input == "":  # Reset color when cleared
            entry.config(fg="black")

    # Check if the game is won (all cells are correct)
        if self.is_board_solved():
            self.show_play_again_popup()


    def solve_board(self):
        # Solve the puzzle by filling with the correct values from the solution
        for i in range(9):
            for j in range(9):
                entry = self.entries[(i, j)]
                entry.delete(0, tk.END)
                entry.insert(0, self.solution[i][j])  # Fill with the solved values
                entry.config(state="readonly")  # Make it read-only

    def new_game(self):
    # Generate a new Sudoku puzzle and its solution
        self.board, self.solution = self.generate_sudoku()  # Ensure both puzzle and solution are updated
        self.original_board = [row[:] for row in self.board]  # Update the original board
        self.mistakes = 0
        self.mistake_label.config(text=f"Mistakes: {self.mistakes}")
        self.update_grid()  # Reset and update the grid


    def restart_game(self):
        # Restart the current game
        self.board = [row[:] for row in self.original_board]
        self.mistakes = 0
        self.mistake_label.config(text=f"Mistakes: {self.mistakes}")
        self.update_grid()

    def update_grid(self):
        for (i, j), entry in self.entries.items():
            entry.config(state="normal")  # Allow editing for all cells initially
            entry.delete(0, tk.END)  # Clear the cell's content

        # Reset background colors
            bg_color = "#80C1FF" if (i // 3 + j // 3) % 2 == 0 else "#BAD7F2"
            entry.config(bg=bg_color, readonlybackground=bg_color, fg="black")  # Reset colors

        # Clear all bindings to avoid leftover behavior
            entry.unbind("<KeyPress>")
            entry.unbind("<FocusOut>")

            if self.board[i][j] != 0:  # Preset numbers
                entry.insert(0, self.board[i][j])  # Insert the new preset value
                entry.config(state="readonly", fg="black")  # Make the cell readonly
            else:  # If the cell is empty in the new game
                entry.config(state="normal")  # Ensure it's editable
            # Rebind focus-out event
                entry.bind("<FocusOut>", lambda e, x=i, y=j: self.check_user_input(x, y))

    # Reapply highlighting for the selected number
        if self.selected_number is not None:
            self.highlight_number(self.selected_number)



    def game_over(self):
    # Create a custom pop-up window
        popup = tk.Toplevel(self.root)
        popup.title("Game Over")
        popup.config(bg="#80C1FF")  # Dark background for the pop-up window

    # Custom message label
        label = tk.Label(popup, text="You made 3 mistakes. Game Over!", font=("Courier", 14, "bold"), fg="white", bg="#80C1FF")
        label.pack(pady=20)

    # Button to quit the game
        quit_btn = tk.Button(popup, text="Quit", font=("Courier", 12), command=self.root.quit, bg="lightcoral", fg="black")
        quit_btn.pack(side="right", padx=20, pady=10)

    # Button to try again (new game)
        try_again_btn = tk.Button(popup, text="Try Again", font=("Courier", 12), command=self.restart_game, bg="lightblue", fg="black")
        try_again_btn.pack(side="left", padx=20, pady=10)


    def show_how_to_play(self):
        how_to_play_text = (
            "Sudoku Rules:\n\n"
            "1. The grid is a 9x9 puzzle, divided into 9 3x3 subgrids.\n"
            "2. Each row, column, and 3x3 subgrid must contain the numbers 1-9.\n"
            "3. Some numbers are pre-filled; others are left blank.\n"
            "4. Fill in the blanks by selecting numbers from 1 to 9.\n"
            "5. You can use the 'Solve' button to fill the grid.\n"
        )

        popup = tk.Toplevel(self.root)
        popup.title("How to Play")
        popup.config(bg="#80C1FF")  

    # Custom message label
        label = tk.Label(popup, text=how_to_play_text, font=("Courier", 12), fg="black", bg="#80C1FF", justify="left")
        label.pack(pady=20, padx=20)

    # Button to close the help window
        close_btn = tk.Button(popup, text="Close", font=("Courier", 12), command=popup.destroy, bg="lightcoral", fg="black")
        close_btn.pack(pady=10)

    def show_play_again_popup(self):
        popup = tk.Toplevel(self.root)
        popup.title("Congratulations!")
        popup.config(bg="#80C1FF")  

    # Custom message label
        label = tk.Label(popup, text="You solved the puzzle!", font=("Courier", 14, "bold"), fg="green", bg="#80C1FF")
        label.pack(pady=20)

    # Button to play again (new game)
        play_again_btn = tk.Button(popup, text="Play Again", font=("Courier", 12), command=self.new_game, bg="lightblue", fg="black")
        play_again_btn.pack(side="left", padx=20, pady=10)

    # Button to quit
        quit_btn = tk.Button(popup, text="Quit", font=("Courier", 12), command=self.root.quit, bg="lightcoral", fg="black")
        quit_btn.pack(side="right", padx=20, pady=10)

    def generate_sudoku(self):
        base = 3
        side = base * base

        # Define the pattern for the Sudoku grid; (r % base): Ensures numbers repeat every 3 rows.(r // base): Shifts blocks of numbers down for each 3 rows.c: Adds the column index.% side: Wraps the pattern within the grid size.
        def pattern(r, c): return (base * (r % base) + r // base + c) % side
        def shuffle(s): return random.sample(s, len(s))


        #shuffle: Randomizes the order of elements in a list.rows and cols:Randomizes the order of rows and columns within each 3x3 block.Ensures variation while preserving the overall Sudoku structure.nums:Randomizes the order of numbers (1-9) to fill the grid differently each time.
        rBase = range(base)
        rows = [g * base + r for g in shuffle(rBase) for r in shuffle(rBase)]
        cols = [g * base + c for g in shuffle(rBase) for c in shuffle(rBase)]
        nums = shuffle(range(1, side + 1))

        # Create a fully solved Sudoku board (solution board)
        solution_board = [[nums[pattern(r, c)] for c in cols] for r in rows]

        # Now create a puzzle board by removing numbers
        puzzle_board = [row[:] for row in solution_board]  # Copy of the solution
        squares = side * side
        empties = squares * 2 // 4  # Adjusted for difficulty
        for p in random.sample(range(squares), empties):
            puzzle_board[p // side][p % side] = 0

        return puzzle_board, solution_board

    def is_board_solved(self):
        for i in range(9):
            for j in range(9):
                entry = self.entries[(i, j)]
                if not entry.get().isdigit() or int(entry.get()) != self.solution[i][j]:
                    return False
        return True

if __name__ == "__main__":
    root = tk.Tk()
    app = SudokuGrid(root)
    root.mainloop()
